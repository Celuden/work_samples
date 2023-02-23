// Impressum
// Das Projekt "Maro's Mayhem" ist im Studiengang MultiMediaTechnology / FHS im Rahmen des MultiMediaProjekt 1 von Alija Suljic erstellt worden.
// The project "Maro's Mayhem" has been developed within the MultiMediaTechnology Bachelor Studies at the Fachhochschule Salzburg as part of the MultiMediaProject 1 by Alija Suljic in the year 2022.

using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using SFML.System;
using SFML.Graphics;

internal class EnemyManager : GameObject
{
    private Dictionary<string, List<string[]>> waveManager;
    private List<Enemy> _enemyList;
    private List<Boss> _bossList;
    private Background _background;
    private Sprite deadBoss;
    private Stopwatch sw;
    private int spawnInterval;
    private int windowSizeX;
    private int windowSizeY;
    private int outerRemove;
    private Dictionary<string, bool> spawnPool;
    private bool bossSpawn;
    private bool levelMusicIsPlaying;
    private bool levelCleared;
    private Level _level;
    private string levelTextPath;
    private Vector2f playerTarget;
    private int startBossMusic;
    private bool spawnJellyswarm;
    private bool bossDisplay;
    private bool initiateAlarm; 
    private bool rollCredits;
    public EnemyManager(Level level)
    {
        _level = level;
    }
    public override void Initialize()
    {
        // Set fixed variables
        spawnInterval = 10000;
        windowSizeX = 600;
        windowSizeY = 900;
        outerRemove = 300;
        startBossMusic = 1000;
        initiateAlarm = false;
        rollCredits = false;
        spawnJellyswarm = false;
        bossDisplay = false;
        waveManager = new();
        _bossList = new();
        _enemyList = new();
        sw = new();
        bossSpawn = false;
        levelMusicIsPlaying = false;
        levelCleared = false;

        // Spawn Pool
        spawnPool = new();
        spawnPool.Add("spawn01", false); spawnPool.Add("spawn02", false); spawnPool.Add("spawn03", false); spawnPool.Add("spawn04", false); spawnPool.Add("spawn05", false);
        spawnPool.Add("spawn06", false); spawnPool.Add("spawn07", false); spawnPool.Add("spawn08", false); spawnPool.Add("spawn09", false); spawnPool.Add("spawn10", false);
        spawnPool.Add("spawn11", false); spawnPool.Add("spawn12", false); spawnPool.Add("spawn13", false); spawnPool.Add("spawn14", false); spawnPool.Add("spawn15", false);
        spawnPool.Add("spawn16", false); spawnPool.Add("spawn17", false); spawnPool.Add("spawn18", false); spawnPool.Add("spawn19", false); spawnPool.Add("spawn20", false);
        spawnPool.Add("spawn21", false); spawnPool.Add("spawn22", false); spawnPool.Add("spawn23", false); spawnPool.Add("spawn24", false);

        // Set Background
        if (_level == Level.Level03)
        {
            _background = new(AssetManager.Instance.textures["BGLV03"], 7200, 600, 40);
            _background.Initialize();
        }
        else if (_level == Level.Level02)
        {
            _background = new(AssetManager.Instance.textures["BGLV02"], 3600, 600, 40);
            _background.Initialize();
        }
        else
        {
            _background = new(AssetManager.Instance.textures["BGLV01"], 2250, 600, 35);
            _background.Initialize();
        }

        // Read Wave-File
        switch (_level)
        {
            case Level.Level02:
                levelTextPath = "SpawnData/level02.txt";
                break;
            case Level.Level03:
                levelTextPath = "SpawnData/level03.txt";
                break;
            default:
                levelTextPath = "SpawnData/level01.txt";
                break;
        }

        string currentSpawn = "";
        List<string[]> wave = new();
        using(StreamReader sr = new StreamReader(levelTextPath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Length == 7 || line.Length == 8)
                {
                    waveManager.Add(line, new());
                    currentSpawn = line;
                    wave = new();
                }
                else
                {
                    string[] splitLine = line.Split('\\');
                    wave.Add(splitLine);
                    waveManager[currentSpawn] = wave;
                }
            }
        }
        
    }
    public override void Update(float deltaTime)
    {
        _background.Update(deltaTime);
        CreateLevel(_level);
        CheckForJellyswarm();
        foreach (Enemy e in _enemyList)
        {
            e.SetTarget(playerTarget);
            e.Update(deltaTime);

            // Garbage Collect Enemies manually whilst keeping projectiles
            if (e.GetDeathTimer().ElapsedMilliseconds > 1000 && !e.GetStopDrawingEnemy())
            {
                e.SetStopDrawingEnemy(true);
            }
            if (e.GetDeathTimer().ElapsedMilliseconds > 2500)
            {
                _enemyList.Remove(e);
                e.GetDeathTimer().Reset();
                break;
            }
        }
        if (bossSpawn)
        {
            foreach (Boss b in _bossList)
            {
                b.SetTarget(playerTarget);
                b.Update(deltaTime);
            }
        }
    }
    public override void Draw(RenderWindow window)
    {
        _background.Draw(window);
        foreach (Enemy e in _enemyList)
        {
            e.Draw(window);
            if (e.GetSprite().Position.X > window.Size.X / 2 + outerRemove || e.GetSprite().Position.Y > window.Size.Y / 2 + outerRemove
            || e.GetSprite().Position.X < -(window.Size.X / 2) - outerRemove || e.GetSprite().Position.Y < -window.Size.Y / 2 - outerRemove * 3)
            {
                _enemyList.Remove(e);
                break;
            }
        }

        if (bossSpawn)
        {
            foreach (Boss b in _bossList)
            {
                b.Draw(window);
            }
        }

        if (bossDisplay)
        {
            window.Draw(deadBoss);
        }
    }
    private void SpawnEnemy(EnemyTypeList.EnemyTypes enemyType, Vector2f spawnPoint, Vector2f moveDirection, float rotation, bool flipCollision = false)
    {
        Enemy enemy = EnemyTypeList.GetEnemyData(enemyType);
        enemy.Initialize();
        enemy.SetMovement(spawnPoint, moveDirection, rotation, flipCollision);
        _enemyList.Add(enemy);
    }
    private void SpawnBoss(EnemyTypeList.EnemyTypes bossType)
    {
        Boss hammerHeadShark = EnemyTypeList.GetBossData(bossType);
        hammerHeadShark.Initialize();
        _bossList.Add(hammerHeadShark);
    }
    private void SpawnWave(string currentWave)
    {
        foreach (string[] s in waveManager[currentWave])
        {
            SpawnEnemy(GetEnemyType(s[0]), new Vector2f(float.Parse(s[1]), float.Parse(s[2])), 
                    new Vector2f(float.Parse(s[3]), float.Parse(s[4])), float.Parse(s[5]), bool.Parse(s[6]));
        }
        spawnPool[currentWave] = true;
    }
    private void CreateLevel(Level level)
    {
        // ----------------- LEVEL 01 -----------------------------
        if (level == Level.Level01)
        {
            if (!levelMusicIsPlaying)
            {
                AssetManager.Instance.music["LevelBGM"].Play();
                levelMusicIsPlaying = true;
            }
            
            sw.Start();
            if (sw.ElapsedMilliseconds > spawnInterval*0.5 && !spawnPool["spawn01"]) {SpawnWave("spawn01");}
            if (sw.ElapsedMilliseconds > spawnInterval && !spawnPool["spawn02"]) {SpawnWave("spawn02");}
            if (sw.ElapsedMilliseconds > spawnInterval*1.5 && !spawnPool["spawn03"]) {SpawnWave("spawn03");}
            if (sw.ElapsedMilliseconds > spawnInterval*2 && !spawnPool["spawn04"]) {SpawnWave("spawn04");}
            if (sw.ElapsedMilliseconds > spawnInterval*2.3 && !spawnPool["spawn05"]) {SpawnWave("spawn05");}
            if (sw.ElapsedMilliseconds > spawnInterval*2.6 && !spawnPool["spawn06"]) {SpawnWave("spawn06");}
            if (sw.ElapsedMilliseconds > spawnInterval*3 && !spawnPool["spawn07"]) {SpawnWave("spawn07");}

            if (sw.ElapsedMilliseconds > spawnInterval*4 - startBossMusic && !spawnPool["spawn08"])
            {
                AssetManager.Instance.sounds["Alarm"].Play();
                AssetManager.Instance.music["LevelBGM"].Stop();
                initiateAlarm = true;
                spawnPool["spawn08"] = true;
            }
            
            // Initiate Boss-Fight here
            if (sw.ElapsedMilliseconds > spawnInterval*4 && bossSpawn == false)
            {
                AssetManager.Instance.music["BossTheme"].Play();
                SpawnBoss(EnemyTypeList.EnemyTypes.HammerHeadShark01);
                bossSpawn = true;
            }

            // End Level, save Highscore (outside in Game-Class)
            if (bossSpawn && _bossList.Count == 0)
            {
                levelCleared = true;
            }
        }
        if (level == Level.Level02)
        {
            if (!levelMusicIsPlaying)
            {
                AssetManager.Instance.music["LevelBGM"].Play();
                levelMusicIsPlaying = true;
            }

            sw.Start();
            if (sw.ElapsedMilliseconds > spawnInterval*0.3f && !spawnPool["spawn01"]) {SpawnWave("spawn01");}
            if (sw.ElapsedMilliseconds > spawnInterval*0.5f && !spawnPool["spawn02"]) {SpawnWave("spawn02");}
            if (sw.ElapsedMilliseconds > spawnInterval && !spawnPool["spawn03"]) {SpawnWave("spawn03");}
            if (sw.ElapsedMilliseconds > spawnInterval*1.2f && !spawnPool["spawn04"]) {SpawnWave("spawn04");}
            if (sw.ElapsedMilliseconds > spawnInterval*1.5f && !spawnPool["spawn05"]) {SpawnWave("spawn05");}
            if (sw.ElapsedMilliseconds > spawnInterval*2f && !spawnPool["spawn06"]) {SpawnWave("spawn06");}
            if (sw.ElapsedMilliseconds > spawnInterval*2.3f && !spawnPool["spawn07"]) {SpawnWave("spawn07");}
            if (sw.ElapsedMilliseconds > spawnInterval*2.7f && !spawnPool["spawn08"]) {SpawnWave("spawn08");}
            if (sw.ElapsedMilliseconds > spawnInterval*3.3f && !spawnPool["spawn09"]) {SpawnWave("spawn09");}
            if (sw.ElapsedMilliseconds > spawnInterval*3.6f && !spawnPool["spawn10"]) {SpawnWave("spawn10");}
            if (sw.ElapsedMilliseconds > spawnInterval*4f && !spawnPool["spawn11"]) {SpawnWave("spawn11");}
            if (sw.ElapsedMilliseconds > spawnInterval*4.3f && !spawnPool["spawn12"]) {SpawnWave("spawn12");}
            if (sw.ElapsedMilliseconds > spawnInterval*4.6f && !spawnPool["spawn13"]) {SpawnWave("spawn13");}
            
            if (sw.ElapsedMilliseconds > spawnInterval*5 - startBossMusic && !spawnPool["spawn14"])
            {
                AssetManager.Instance.sounds["Alarm"].Play();
                AssetManager.Instance.music["LevelBGM"].Stop();
                initiateAlarm = true;
                SpawnWave("spawn14");
            }
            // Initiate Boss-Fight here
            if (sw.ElapsedMilliseconds > spawnInterval*5 && bossSpawn == false)
            {
                AssetManager.Instance.music["BossTheme"].Play();
                SpawnBoss(EnemyTypeList.EnemyTypes.Sharknado02);
                sw.Restart();
                bossSpawn = true;
            }

            // Endless Sharks
            if (sw.ElapsedMilliseconds > spawnInterval && bossSpawn)
            {
                SpawnWave("spawn14");
                sw.Restart();
            }

            // End Level, save Highscore (outside in Game-Class)
            if (bossSpawn && _bossList.Count == 0)
            {
                levelCleared = true;
            }
        }
        if (level == Level.Level03)
        {
            if (!levelMusicIsPlaying)
            {
                AssetManager.Instance.music["LevelBGM"].Play();
                levelMusicIsPlaying = true;
            }

            sw.Start();

            if (sw.ElapsedMilliseconds > spawnInterval*0.3f && !spawnPool["spawn01"]) {SpawnWave("spawn01");}
            if (sw.ElapsedMilliseconds > spawnInterval*0.5f && !spawnPool["spawn02"]) {SpawnWave("spawn02");}
            if (sw.ElapsedMilliseconds > spawnInterval && !spawnPool["spawn03"]) {SpawnWave("spawn03");}
            if (sw.ElapsedMilliseconds > spawnInterval*1.3f && !spawnPool["spawn04"]) {SpawnWave("spawn04");}
            if (sw.ElapsedMilliseconds > spawnInterval*1.6f && !spawnPool["spawn05"]) {SpawnWave("spawn05");}
            if (sw.ElapsedMilliseconds > spawnInterval*2f && !spawnPool["spawn06"]) {SpawnWave("spawn06");}
            if (sw.ElapsedMilliseconds > spawnInterval*2.3f && !spawnPool["spawn07"]) {SpawnWave("spawn07");}
            if (sw.ElapsedMilliseconds > spawnInterval*2.6f && !spawnPool["spawn08"]) {SpawnWave("spawn08");}
            if (sw.ElapsedMilliseconds > spawnInterval*3f && !spawnPool["spawn09"]) {SpawnWave("spawn09");}
            if (sw.ElapsedMilliseconds > spawnInterval*3.5f && !spawnPool["spawn10"]) {SpawnWave("spawn10");}
            if (sw.ElapsedMilliseconds > spawnInterval*3.8f && !spawnPool["spawn11"]) {SpawnWave("spawn11");}
            if (sw.ElapsedMilliseconds > spawnInterval*4.4f && !spawnPool["spawn12"]) {SpawnWave("spawn12");}
            if (sw.ElapsedMilliseconds > spawnInterval*5f && !spawnPool["spawn13"]) {SpawnWave("spawn13");}
            if (sw.ElapsedMilliseconds > spawnInterval*5.5f && !spawnPool["spawn14"]) {SpawnWave("spawn14");}
            if (sw.ElapsedMilliseconds > spawnInterval*5.8f && !spawnPool["spawn15"]) {SpawnWave("spawn15");}
            if (sw.ElapsedMilliseconds > spawnInterval*6.1f && !spawnPool["spawn16"]) {SpawnWave("spawn16");}
            if (sw.ElapsedMilliseconds > spawnInterval*6.5f && !spawnPool["spawn17"]) {SpawnWave("spawn17");}
            if (sw.ElapsedMilliseconds > spawnInterval*6.8f && !spawnPool["spawn18"]) {SpawnWave("spawn18");}
            if (sw.ElapsedMilliseconds > spawnInterval*7.2f && !spawnPool["spawn19"]) {SpawnWave("spawn19");}
            if (sw.ElapsedMilliseconds > spawnInterval*7.7f && !spawnPool["spawn20"]) {SpawnWave("spawn20");}
            if (sw.ElapsedMilliseconds > spawnInterval*8f && !spawnPool["spawn21"]) {SpawnWave("spawn21");}
            if (sw.ElapsedMilliseconds > spawnInterval*8.3f && !spawnPool["spawn22"]) {SpawnWave("spawn22");}
            if (sw.ElapsedMilliseconds > spawnInterval*8.7f && !spawnPool["spawn23"]) {SpawnWave("spawn23");}
            if (sw.ElapsedMilliseconds > spawnInterval*9 - startBossMusic && !spawnPool["spawn24"])
            {
                AssetManager.Instance.sounds["Alarm"].Play();
                AssetManager.Instance.music["LevelBGM"].Stop();
                initiateAlarm = true;
                SpawnWave("spawn24");
            }

            // Initiate Boss-Fight here
            if (sw.ElapsedMilliseconds > spawnInterval*9 && bossSpawn == false)
            {
                AssetManager.Instance.music["BossTheme"].Play();
                SpawnBoss(EnemyTypeList.EnemyTypes.Kraken03);
                sw.Restart();
                bossSpawn = true;
            }

            // End Level, save Highscore (outside in Game-Class)
            if (bossSpawn && _bossList.Count == 0)
            {
                levelCleared = true;
                rollCredits = true;
            }

            if (spawnJellyswarm)
            {
                SpawnEnemy(EnemyTypeList.EnemyTypes.Jellyfish03, new Vector2f(-windowSizeX/2 - outerRemove / 2, windowSizeY/4), new Vector2f(2f, 0), 90f, true);
                SpawnEnemy(EnemyTypeList.EnemyTypes.Jellyfish03, new Vector2f(-windowSizeX/2 - outerRemove / 2, windowSizeY/4*3), new Vector2f(1.8f, 0), 90f, true);
                SpawnEnemy(EnemyTypeList.EnemyTypes.Jellyfish03, new Vector2f(windowSizeX/2 + outerRemove / 2, windowSizeY/5), new Vector2f(-1.9f, 0), -90f, true);
                SpawnEnemy(EnemyTypeList.EnemyTypes.Jellyfish03, new Vector2f(0, -windowSizeY/2 - 100), new Vector2f(0, 2f), 180f);
                SpawnEnemy(EnemyTypeList.EnemyTypes.Jellyfish03, new Vector2f(215, windowSizeY/2 + 100), new Vector2f(0, -1.9f), 0);
                SpawnEnemy(EnemyTypeList.EnemyTypes.Jellyfish03, new Vector2f(-244, windowSizeY/2 + 100), new Vector2f(0, -1.2f), 0);
                spawnJellyswarm = false;
            }
        }
    }
    private void CheckForJellyswarm()
    {
        foreach (Boss b in _bossList)
        {
            if (b.GetSpawnJellyswarm())
            {
                spawnJellyswarm = true;
            }
        }
    }
    public void EnemyDeath(Enemy e)
    {
        AssetManager.Instance.sounds["EnemyDeath"].Play();
        e.Stop();
        e.SetAnimationTime(0);
        e.SetAnimationSpeed(9f);
        e.GetDeathTimer().Start();
    }
    public void BossDeath(Boss b)
    {
        deadBoss = _bossList[0].GetSprite();
        bossDisplay = true;
        _bossList.Remove(b);
    }
    public void SetPlayerPosition(Vector2f playerPosition)
    {
        playerTarget = playerPosition;
    }
    public bool DidBossSpawn()
    {
        return bossSpawn;
    }
    public bool IsLevelCleared()
    {
        return levelCleared;
    }
    public void SetLevelCleared()
    {
        levelCleared = false;
    }
    private EnemyTypeList.EnemyTypes GetEnemyType(string enemyName)
    {
        switch (enemyName)
        {
            case "eel01":
                return EnemyTypeList.EnemyTypes.Eel01;
            case "abysswyrm01":
                return EnemyTypeList.EnemyTypes.Abysswyrm01;
            case "eel02":
                return EnemyTypeList.EnemyTypes.Eel02;
            case "jellyfish02":
                return EnemyTypeList.EnemyTypes.Jellyfish02;
            case "shark03":
                return EnemyTypeList.EnemyTypes.Shark03;
            case "cycrab03":
                return EnemyTypeList.EnemyTypes.Cycrab03;
            case "anglerfish03":
                return EnemyTypeList.EnemyTypes.Anglerfish03;
            case "jellyfish03":
                return EnemyTypeList.EnemyTypes.Jellyfish03;
            default:
                return EnemyTypeList.EnemyTypes.Shark03;
        }
    }
    public bool GetInitiateAlarm()
    {
        return initiateAlarm;
    }
    public void SetInitiateAlarm(bool value)
    {
        initiateAlarm = value;
    }
    public bool GetBossSpawn()
    {
        return bossSpawn;
    }
    public void SetBossSpawn(bool value)
    {
        bossSpawn = value;
    }
    public bool GetRollCredits()
    {
        return rollCredits;
    }
    public void SetRollCredits(bool value)
    {
        rollCredits = value;
    }
    public List<Enemy> GetEnemyList()
    {
        return _enemyList;
    }
    public List<Boss> GetBossList()
    {
        return _bossList;
    }
    public Stopwatch GetStopwatch()
    {
        return sw;
    }
}