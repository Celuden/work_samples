// Impressum
// Das Projekt "Maro's Mayhem" ist im Studiengang MultiMediaTechnology / FHS im Rahmen des MultiMediaProjekt 1 von Alija Suljic erstellt worden.
// The project "Maro's Mayhem" has been developed within the MultiMediaTechnology Bachelor Studies at the Fachhochschule Salzburg as part of the MultiMediaProject 1 by Alija Suljic in the year 2022.

using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

internal class Game
{
    private List<GameObject> _gameObjects;
    private bool reachedStage02;
    private bool reachedStage03;
    private bool reachedStage04;
    private bool reachedStage05;
    private int touchDamageModifier;
    private int spearDamageMultiplier;
    private VideoMode mode;
    private RenderWindow window;
    private View view;
    private Clock clock;
    private GameStateManager gameStateManager;
    private MainMenu mainMenu;
    private PauseMenu pauseMenu;
    private Player player;
    private HUD hud;
    private EnemyManager enemyManager;
    private TransitionManager transitionManager;
    private CutsceneManager cutsceneManager;
    private Endscreen endScreen;
    public Game()
    {
        mode = new VideoMode(600, 900);
        window = new RenderWindow(mode, "MARO'S MAYHEM – ALIJA SULJIC");
        clock = new Clock();

        // Event Callback
        window.Closed += CloseGame;
        window.Resized += (sender, args) =>
        {
            view.Size = (Vector2f)window.Size;
        };
        window.SetMouseCursorVisible(false);
    }

    private void CloseGame(object sender, EventArgs e)
    {
        window?.Close();
    }

    public void Run()
    {

        Initialize();

        while (window.IsOpen)
        {
            var deltaTime = clock.Restart().AsSeconds();

            HandleEvents();

            Update(deltaTime);

            Draw();
        }
    }

    private void Initialize()
    {        
        // Load Assets
        AssetManager.Instance.LoadTexture("Player", "assets/Maro_Spritesheet.png");

        AssetManager.Instance.LoadTexture("PlayerProjectile", "assets/PlayerProjectile_SpriteSheet.png");
        AssetManager.Instance.LoadTexture("PlayerSpear", "assets/Maro_Spear_SpriteSheet.png");
        AssetManager.Instance.LoadTexture("OrangeProjectile", "assets/Orange_Projectile.png");
        AssetManager.Instance.LoadTexture("GreenProjectile", "assets/Green_Projectile.png");
        AssetManager.Instance.LoadTexture("PurpleProjectile", "assets/Purple_Projectile.png");
        AssetManager.Instance.LoadTexture("WhiteProjectile", "assets/White_Projectile.png");
        AssetManager.Instance.LoadTexture("SharkProjectile", "assets/Shark03_Projectile.png");
        AssetManager.Instance.LoadTexture("SharknadoProjectile", "assets/Sharknado_Projectile.png");
        AssetManager.Instance.LoadTexture("Bomb", "assets/Maro_Bomb.png");
        AssetManager.Instance.LoadTexture("Logo", "assets/MarosMayhemUnderwater.png");
        AssetManager.Instance.LoadTexture("SpacebarIsolated", "assets/Space_Isolated.png");
        AssetManager.Instance.LoadTexture("PointyArrow", "assets/PointyArrow_SpriteSheet.png");
        AssetManager.Instance.LoadTexture("BGLV01", "assets/Background_Level01.png");
        AssetManager.Instance.LoadTexture("BGLV02", "assets/Background_Level02.png");
        AssetManager.Instance.LoadTexture("BGLV03", "assets/Background_Level03.png");

        AssetManager.Instance.LoadMusic("MainMenuTheme", "assets/MainMenuTheme.ogg");
        AssetManager.Instance.LoadMusic("LevelBGM", "assets/NormalLevelTheme.ogg");
        AssetManager.Instance.LoadMusic("BossTheme", "assets/Boss_Theme.ogg");

        AssetManager.Instance.LoadSound("Fire", "assets/fire.wav");
        AssetManager.Instance.LoadSound("Alarm", "assets/alarm.wav");
        AssetManager.Instance.LoadSound("EnemyDeath", "assets/enemyDeath.wav");
        AssetManager.Instance.LoadSound("Bomb", "assets/bomb.wav");
        AssetManager.Instance.LoadSound("LevelUp", "assets/levelUp.wav");
        AssetManager.Instance.LoadSound("PlayerHit", "assets/playerHit.wav");
        AssetManager.Instance.LoadSound("GameOver", "assets/GameOver.wav");
        AssetManager.Instance.LoadSound("LevelCleared", "assets/levelCleared.wav");
        AssetManager.Instance.LoadSound("ButtonPress", "assets/buttonPress.wav");
        AssetManager.Instance.LoadSound("MenuSelect", "assets/menuSelect.wav");
        AssetManager.Instance.LoadSound("TextSFX", "assets/textSFX.wav");
        AssetManager.Instance.LoadSound("NewHighscore", "assets/newHighscore.wav");
        AssetManager.Instance.LoadSound("AddingScore", "assets/addingScoreSFX.wav");

        AssetManager.Instance.LoadFont("Font", "assets/PIXY.ttf");

        // Faces and Drawings
        AssetManager.Instance.LoadTexture("MainMenuBG", "assets/MainMenuBackground.png");
        AssetManager.Instance.LoadTexture("MainMenuV2", "assets/MainMenuBackgroundV2.png");
        AssetManager.Instance.LoadTexture("WASD", "assets/Keyboard_WASD.png");
        AssetManager.Instance.LoadTexture("E", "assets/Keyboard_E.png");
        AssetManager.Instance.LoadTexture("R", "assets/Keyboard_R.png");
        AssetManager.Instance.LoadTexture("Space", "assets/Keyboard_Space.png");
        AssetManager.Instance.LoadTexture("MaroFace", "assets/Maro_Viribus.png");
        AssetManager.Instance.LoadTexture("VeniaFace", "assets/Venia_Viribus.png");
        AssetManager.Instance.LoadTexture("VolvoFace", "assets/Volvo_Viribus.png");

        // Enemies
        AssetManager.Instance.LoadTexture("Eel01", "assets/Eel01_SpriteSheet.png");
        AssetManager.Instance.LoadTexture("Eel02", "assets/Eel02_SpriteSheet.png");
        AssetManager.Instance.LoadTexture("Shark03", "assets/Shark03_Spritesheet.png");
        AssetManager.Instance.LoadTexture("Jellyfish02", "assets/Jellyfish02_SpriteSheet.png");
        AssetManager.Instance.LoadTexture("Jellyfish03", "assets/Jellyfish03_SpriteSheet.png");
        AssetManager.Instance.LoadTexture("Abysswyrm01", "assets/Abysswyrm01_Spritesheet.png");
        AssetManager.Instance.LoadTexture("Cycrab03", "assets/Cycrab03_Spritesheet.png");
        AssetManager.Instance.LoadTexture("Anglerfish03", "assets/Anglerfish03_Spritesheet.png");

        // Bosses
        AssetManager.Instance.LoadTexture("HammerheadShark", "assets/HammerheadShark_SpriteSheet.png");
        AssetManager.Instance.LoadTexture("Sharknado", "assets/Sharknado_SpriteSheet.png");
        AssetManager.Instance.LoadTexture("Kraken", "assets/Kraken_SpriteSheet.png");

        // Settings for music and sounds
        AssetManager.Instance.music["LevelBGM"].Volume = 40;
        AssetManager.Instance.music["LevelBGM"].Loop = true;
        AssetManager.Instance.music["BossTheme"].Volume = 45;
        AssetManager.Instance.music["BossTheme"].Loop = true;
        AssetManager.Instance.music["MainMenuTheme"].Volume = 40;
        AssetManager.Instance.music["MainMenuTheme"].Loop = true;
        AssetManager.Instance.sounds["Fire"].Volume = 1;
        AssetManager.Instance.sounds["LevelUp"].Volume = 75;
        AssetManager.Instance.sounds["LevelCleared"].Volume = 40;
        AssetManager.Instance.sounds["EnemyDeath"].Volume = 60;
        AssetManager.Instance.sounds["PlayerHit"].Volume = 30;
        AssetManager.Instance.sounds["GameOver"].Volume = 60;
        AssetManager.Instance.sounds["ButtonPress"].Volume = 35;
        AssetManager.Instance.sounds["MenuSelect"].Volume = 35;
        AssetManager.Instance.sounds["TextSFX"].Volume = 35;
        
        // Load Inputs
        InputManager.Instance.Init(window);

        // Initialize variables and lists
        _gameObjects = new();
        touchDamageModifier = 3;
        spearDamageMultiplier = 15;

        // Initialize GameStateManager
        gameStateManager = new();
        gameStateManager.AddState(GameState.MainMenu);

        // Initialize Cutscene Manager
        cutsceneManager = new();
        cutsceneManager.Initialize();

        // Initialize Transition Manager
        transitionManager = new();
        transitionManager.Initialize();

        // Initialize MainMenu
        mainMenu = new(window);
        mainMenu.Initialize();
        mainMenu.GetMenuTheme().Play();

        // Initialize PauseMenu
        pauseMenu = new(window);
        pauseMenu.Initialize();

        // Create Endscreen
        endScreen = new(window);

        // Initialize Player
        player = new();

        // Initialize HUD
        hud = new(gameStateManager.GetCurrentState());

        // Enemy Manager
        enemyManager = new(Level.Level01);
        
        // Add GameObjects to list
        _gameObjects.Add(enemyManager);
        _gameObjects.Add(player);
        _gameObjects.Add(hud);
        _gameObjects.Add(transitionManager);
        

        foreach (GameObject obj in _gameObjects)
        {
            obj.Initialize();
        }
        hud.InitializePlayerInformation(player);

        // Set bools for level-ups
        ResetStageBools();

        // View
        view = new View(player.Position, (Vector2f)window.Size);
        window.SetView(view);
    }

    private void HandleEvents()
    {
        window.DispatchEvents();
    }

    private void Update(float deltaTime)
    {
        // Check for the end of a cutscene
        CheckCutsceneEnd();

        // Checks for which level should be loaded next
        CheckForLevelTransition();

        // Check if GameState has been changed and update game accordingly
        CheckForGamestateChange();
        
        // Check if Level is completed
        CheckForLevelClear(deltaTime);

        // Check for Level-Restart
        CheckForLevelRestart();

        // Check for Pause-Menu-Call from Player
        CheckForPauseMenu();

        // Checks for current Game State and only updates that one
        CurrentGamestateUpdate(deltaTime);
        
        // Check for Boss Alarm
        if (enemyManager.GetInitiateAlarm())
        {
            hud.SetAlarmDone(false);
            enemyManager.SetInitiateAlarm(false);
        }
        
        // Clear all Projectiles (uses a bomb)
        ClearProjectilesCheck();

        // All CollisionDetection
        CollisionDetection();

        // Check for PowerUps - should always be last
        PowerUpCheck();
    }
    private void Draw()
    {
        window.Clear(Color.Blue);
        if (gameStateManager.GetCurrentState() == GameState.MainMenu)
        {
            mainMenu.Draw(window);
            transitionManager.Draw(window);  
        }
        else if (gameStateManager.GetCurrentState() == GameState.Cutscene)
        {
            cutsceneManager.Draw(window);
        }
        else if (gameStateManager.GetCurrentState() == GameState.PauseMenu)
        {
            pauseMenu.Draw(window);
        }
        else
        {
            foreach (GameObject obj in _gameObjects)
            {
                obj.Draw(window);
            }
        }
        window.Display();
    }
    private void GameOver()
    {
        AssetManager.Instance.sounds["GameOver"].Play();
        Transition();
        gameStateManager.RemoveState();
        ResetPlayer();
    }
    private void LevelComplete(float deltaTime)
    {
        foreach (KeyValuePair<string, SFML.Audio.Music> kvp in AssetManager.Instance.music)
        {
            kvp.Value.Stop();
        }
        AssetManager.Instance.sounds["LevelCleared"].Play();

        // New function: Show points at the end of level
        PlayEndscreen(deltaTime);
        gameStateManager.RemoveState();
        gameStateManager.RemoveState();
        hud.WriteHighscore();
        mainMenu.UpdateHighscore();
        ResetPlayer();
        enemyManager.SetLevelCleared();
        enemyManager.SetBossSpawn(false);
    }
    private void ChangeState()
    {
        if (gameStateManager.GetCurrentState() == GameState.Level01)
        {
            _gameObjects.Remove(enemyManager);
            enemyManager = new(Level.Level01);
            enemyManager.Initialize();
            _gameObjects.Insert(0, enemyManager);
        }
        else if (gameStateManager.GetCurrentState() == GameState.Level02)
        {
            _gameObjects.Remove(enemyManager);
            enemyManager = new(Level.Level02);
            enemyManager.Initialize();
            _gameObjects.Insert(0, enemyManager);
        }
        else if (gameStateManager.GetCurrentState() == GameState.Level03)
        {
            _gameObjects.Remove(enemyManager);
            enemyManager = new(Level.Level03);
            enemyManager.Initialize();
            _gameObjects.Insert(0, enemyManager);
        }
    }
    private void ResetStageBools()
    {
        reachedStage02 = false;
        reachedStage03 = false;
        reachedStage04 = false;
        reachedStage05 = false;
    }

    // Resets the Player and HUD-Elements for a fresh start
    private void ResetPlayer()
    {
        Transition();
        foreach (KeyValuePair<string, Music> kvp in AssetManager.Instance.music)
        {
            kvp.Value.Stop();
        }
        // Reset Player
        _gameObjects.Remove(player);
        if (player.GetRestartLevel())
        {
            player = new();
            player.Initialize();
            player.SetRestartLevel(true);
        }
        else
        {
            player = new();
            player.Initialize();
        }
        
        _gameObjects.Insert(1, player);

        // Reset Hud, write Highscore
        _gameObjects.Remove(hud);
        hud = new(gameStateManager.GetCurrentState());
        hud.Initialize();
        hud.InitializePlayerInformation(player);
        _gameObjects.Insert(_gameObjects.Count-1, hud);

        // Reset Power-Up Bools
        ResetStageBools();

        // Start Playing MainMenu-Theme again if level is not getting restarted
        if (!(player.GetRestartLevel()))
        {
            mainMenu.GetMenuTheme().Play();
        }
    }

    private void Transition()
    {
        transitionManager.SetFadeOut(true);

        while (transitionManager.GetFadeOut())
        {
            transitionManager.Update(1f);
            window.Clear(Color.Blue);
            foreach (GameObject obj in _gameObjects)
            {
                if (obj != transitionManager)
                {
                    obj.Draw(window);
                }
            }
            if (enemyManager.IsLevelCleared())
            {
                endScreen.Draw(window);
            }
            transitionManager.Draw(window);
            window.Display();
        }
    }
    private void TransitionFromMainMenu()
    {
        transitionManager.SetFadeOut(true);
        transitionManager.SetPauseFadeIn(true);

        while (transitionManager.GetFadeOut())
        {
            transitionManager.Update(1f);
            window.Clear(Color.Blue);
            mainMenu.Draw(window);
            transitionManager.Draw(window);
            window.Display();
        }
    }
    private void CreditsToMainMenu()
    {
        transitionManager.SetColorAlpha(byte.MaxValue);
        transitionManager.SetFadeOut(true);
        transitionManager.SetFadeIn(true);

        while (transitionManager.GetFadeOut())
        {
            transitionManager.Update(1f);
            window.Clear(Color.Blue);
            mainMenu.Draw(window);
            transitionManager.Draw(window);
            window.Display();
        }
    }
    private void CheckForLevelTransition()
    {
        // Start Level 01 from Main Menu
        if (mainMenu.GetLevel(1))
        {
            TransitionFromMainMenu();
            gameStateManager.AddState(GameState.Level01);
            gameStateManager.AddState(GameState.Cutscene);
            cutsceneManager.LoadGamestate(GameState.Level01);
            mainMenu.SetLevel(1, false);
        }
        // Start Level 02 from Main Menu
        else if (mainMenu.GetLevel(2))
        {
            TransitionFromMainMenu();
            gameStateManager.AddState(GameState.Level02);
            gameStateManager.AddState(GameState.Cutscene);
            cutsceneManager.LoadGamestate(GameState.Level02);
            mainMenu.SetLevel(2, false);
        }

        // Start Level 03 from Main Menu
        else if (mainMenu.GetLevel(3))
        {
            TransitionFromMainMenu();
            gameStateManager.AddState(GameState.Level03);
            gameStateManager.AddState(GameState.Cutscene);
            cutsceneManager.LoadGamestate(GameState.Level03);
            mainMenu.SetLevel(3, false);
        }
    }
    private void PowerUpCheck()
    {
        if (hud.GetScore() >= (int)PlayerLevels.Evolutions.Stage02 && reachedStage02 == false)
        {
            player.PowerUp(hud.GetScore());
            reachedStage02 = true;
            hud.PlayLevelUpAnimation();
        }
        else if (hud.GetScore() >= (int)PlayerLevels.Evolutions.Stage03 && reachedStage03 == false)
        {
            player.PowerUp(hud.GetScore());
            reachedStage03 = true;
            hud.PlayLevelUpAnimation();
        }
        else if (hud.GetScore() >= (int)PlayerLevels.Evolutions.Stage04 && reachedStage04 == false)
        {
            player.PowerUp(hud.GetScore());
            reachedStage04 = true;
            hud.PlayLevelUpAnimation();
        }
        else if (hud.GetScore() >= (int)PlayerLevels.Evolutions.Stage05 && reachedStage05 == false)
        {
            player.PowerUp(hud.GetScore());
            reachedStage05 = true;
            hud.PlayLevelUpAnimation();
        }
    }
    private void CheckForLevelRestart()
    {
        if (player.GetRestartLevel())
        {
            ResetPlayer();
            GameState tmp = gameStateManager.GetCurrentState();
            gameStateManager.RemoveState();
            gameStateManager.AddState(tmp);
            player.SetRestartLevel(false);
        }
    }
    private void CheckCutsceneEnd()
    {
        if (cutsceneManager.GetEndCutscene())
        {
            if (!cutsceneManager.GetCreditsOn())
            {
                mainMenu.GetMenuTheme().Stop();
            }
            else
            {
                CreditsToMainMenu();
                cutsceneManager.SetCreditsOn(false);
            }
            gameStateManager.RemoveState();
            transitionManager.SetPauseFadeIn(false);
            transitionManager.SetFadeIn(true);
            cutsceneManager.SetEndCutscene(false);
        }
    }
    private void CheckForGamestateChange()
    {
        if (gameStateManager.GetIsStateChanged())
        {
            // Initialize EnemyManager according to level
            ChangeState();
            hud.SetCurrentLevel(gameStateManager.GetCurrentState());
            mainMenu.UpdateHighscore();
            gameStateManager.SetIsStateChanged(false);
        }
    }
    private void CheckForLevelClear(float deltaTime)
    {
        if (enemyManager.IsLevelCleared())
        {
            LevelComplete(deltaTime);

            // Check if credits should be rolled
            if (enemyManager.GetRollCredits())
            {
                gameStateManager.AddState(GameState.Cutscene);
                cutsceneManager.LoadGamestate(GameState.Credits);
                enemyManager.SetRollCredits(false);
            }
        }
    }
    private void CheckForPauseMenu()
    {
        if (player.GetCallPauseMenu() && !enemyManager.GetBossSpawn())
        {
            pauseMenu.UpdateCurrentScore(hud.GetScore());
            gameStateManager.AddState(GameState.PauseMenu);
            gameStateManager.SetIsStateChanged(false);

            // Pause Stopwatches in the game
            enemyManager.GetStopwatch().Stop();
            foreach (Enemy e in enemyManager.GetEnemyList())
            {
                e.GetProjectileEmitter().GetStopwatch().Stop();
                if (e.GetDeathTimer().ElapsedMilliseconds > 0)
                {
                    e.GetDeathTimer().Stop();
                }
            }
            player.SetCallPauseMenu(false);
        }
        else if (player.GetCallPauseMenu() && InputManager.Instance.GetKeyDown(Keyboard.Key.Escape))
        {
            AssetManager.Instance.sounds["ButtonPress"].Play();
        }
    }
    private void ClearProjectilesCheck()
    {
        if (player.GetClearProjectiles())
        {
            foreach (Enemy e in enemyManager.GetEnemyList())
            {
                e.GetProjectileEmitter().GetProjectileList().Clear();
            }
            foreach (Boss b in enemyManager.GetBossList())
            {
                b.GetProjectileEmitter().GetProjectileList().Clear();
            }
            player.SetClearProjectiles(false);
            hud.GetPlayerBombsList().RemoveAt(0);
        }
    }

    private void CollisionDetection()
    {
        // Check for PlayerProjectile-Collision with Enemys
        foreach (PlayerProjectile p in player.GetPlayerEmitter().GetProjectileList())
        {
            foreach (Enemy e in enemyManager.GetEnemyList())
            {
                if (p.GetCollisionRect().Intersects(e.GetCollisionRect()))
                {
                    if (p.GetIsNormalProjectile() == false)
                    {
                        e.DamageEnemy(player.GetPlayerDamage() * spearDamageMultiplier);
                    }
                    else
                    {
                        e.DamageEnemy(player.GetPlayerDamage());
                    }
                    if (e.GetHealth() <= 0)
                    {
                        enemyManager.EnemyDeath(e);
                        hud.AddScore(e.GetScore());
                    }
                    player.GetPlayerEmitter().ProjectileCollide(p, e.GetSprite().Position);
                    goto LoopEnd;
                }
            }
            if (enemyManager.DidBossSpawn())
            {
                foreach (Boss b in enemyManager.GetBossList())
                {
                    if (p.GetCollisionRect().Intersects(b.GetCollisionRect()))
                    {
                        if (p.GetIsNormalProjectile() == false)
                        {
                            b.DamageBoss(player.GetPlayerDamage() * spearDamageMultiplier);
                        }
                        else
                        {
                            b.DamageBoss(player.GetPlayerDamage());
                        }
                        
                        if (b.GetHealth() < 0)
                        {
                            enemyManager.BossDeath(b);
                            hud.AddScore(b.GetScore());
                        }
                        player.GetPlayerEmitter().ProjectileCollide(p, b.GetPosition());
                        goto LoopEnd;
                    }
                }
            }
        }
        LoopEnd:;

        // Check for EnemyProjectile-Collision with Player
        if (!player.GetInvulnerable())
        {
            foreach (Enemy e in enemyManager.GetEnemyList())
            {
                foreach (EnemyProjectile p in e.GetProjectileEmitter().GetProjectileList())
                {
                    if (p.GetCollisionRect().Intersects(player.GetCollisionRect()))
                    {
                        player.DamagePlayer(e.GetDamage());
                        if (player.GetPlayerHealth() <= 0)
                        {
                            GameOver();
                        }
                        e.GetProjectileEmitter().GetProjectileList().Remove(p);
                        goto EnemyEnd;
                    }
                }
            }
            if (enemyManager.DidBossSpawn())
            {
                foreach (Boss b in enemyManager.GetBossList())
                {
                    foreach (EnemyProjectile p in b.GetProjectileEmitter().GetProjectileList())
                    {
                        if (p.GetCollisionRect().Intersects(player.GetCollisionRect()))
                        {
                            player.DamagePlayer(b.GetDamage());
                            if (player.GetPlayerHealth() <= 0)
                            {
                                GameOver();
                            }
                            b.GetProjectileEmitter().GetProjectileList().Remove(p);
                            goto EnemyEnd;
                        }
                    }
                }
            }
        }
        EnemyEnd:;

        // Check for Player-Collision with Enemy
        if (!player.GetInvulnerable())
        {
            foreach (Enemy e in enemyManager.GetEnemyList())
            {
                if (player.GetCollisionRect().Intersects(e.GetCollisionRect()))
                {
                    player.DamagePlayer(e.GetDamage() * touchDamageModifier);
                    if (player.GetPlayerHealth() <= 0)
                    {
                        GameOver();
                    }
                    enemyManager.EnemyDeath(e);
                }
            }
            if (enemyManager.DidBossSpawn())
            {
                foreach (Boss b in enemyManager.GetBossList())
                {
                    if (player.GetCollisionRect().Intersects(b.GetCollisionRect()))
                    {
                        player.DamagePlayer(b.GetDamage() * touchDamageModifier);
                        if (player.GetPlayerHealth() < 0)
                        {
                            GameOver();
                        }
                    }
                }
            }
        }
    }

    private void CurrentGamestateUpdate(float deltaTime)
    {
        if (gameStateManager.GetCurrentState() == GameState.Level01 || gameStateManager.GetCurrentState() == GameState.Level02 || gameStateManager.GetCurrentState() == GameState.Level03
            || gameStateManager.GetCurrentState() == GameState.Level04 || gameStateManager.GetCurrentState() == GameState.Level05)
        {
            // Set Player as Target for all Enemies
            enemyManager.SetPlayerPosition(player.GetSpritePosition());

            // Update all GameObjects as usual
            foreach (GameObject obj in _gameObjects)
            {
                obj.Update(deltaTime);
            }
        }
        else if (gameStateManager.GetCurrentState() == GameState.Cutscene)
        {
            cutsceneManager.Update(deltaTime);
            transitionManager.Update(deltaTime);
        }
        else if (gameStateManager.GetCurrentState() == GameState.PauseMenu)
        {
            pauseMenu.Update(deltaTime);
            if (pauseMenu.GetResumeGame())
            {
                gameStateManager.RemoveState();
                gameStateManager.SetIsStateChanged(false);
                enemyManager.GetStopwatch().Start();
                foreach (Enemy e in enemyManager.GetEnemyList())
                {
                    e.GetProjectileEmitter().GetStopwatch().Start();
                    if (e.GetDeathTimer().ElapsedMilliseconds > 0)
                    {
                        e.GetDeathTimer().Start();
                    }
                }
                pauseMenu.SetResumeGame(false);
            }
            else if (pauseMenu.GetRestartLevel())
            {
                player.SetRestartLevel(true);
                gameStateManager.RemoveState();
                gameStateManager.SetIsStateChanged(false);
                pauseMenu.SetRestartLevel(false);
            }
            else if (pauseMenu.GetToMenu())
            {
                Transition();
                gameStateManager.RemoveState();
                gameStateManager.RemoveState();
                gameStateManager.SetIsStateChanged(false);
                ResetPlayer();
                pauseMenu.SetToMenu(false);
            }
        }
        else
        {
            mainMenu.Update(deltaTime);
            if (mainMenu.GetQuitGameScene())
            {
                window.Close();
                mainMenu.SetQuitGameScene(false);
            }
            transitionManager.Update(deltaTime);
        }
    }
    private void PlayEndscreen(float deltaTime)
    {
        gameStateManager.AddState(GameState.Endscreen);
        endScreen.Initialize();
        endScreen.LoadData(hud.GetHighscore(), hud.GetScore());
        while (!endScreen.GetIsDone())
        {
            endScreen.Update(deltaTime);
            window.Clear(Color.Blue);
            foreach (GameObject obj in _gameObjects)
            {
                obj.Draw(window);
            }
            endScreen.Draw(window);
            window.Display();
        }
    }
}