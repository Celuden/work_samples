// Impressum
// Das Projekt "Maro's Mayhem" ist im Studiengang MultiMediaTechnology / FHS im Rahmen des MultiMediaProjekt 1 von Alija Suljic erstellt worden.
// The project "Maro's Mayhem" has been developed within the MultiMediaTechnology Bachelor Studies at the Fachhochschule Salzburg as part of the MultiMediaProject 1 by Alija Suljic in the year 2022.

using System.Diagnostics;
using SFML.System;
using SFML.Graphics;

internal class Enemy : GameObject
{
    private IntRect collisionRect;
    private Sprite sprite;
    private Vector2f moveVector;
    private int health;
    private int damage;
    private float moveSpeed;
    private float animationTime;
    private float animationSpeed;
    private int animationPosX;
    private int animationPosY;
    private int animationFrame;
    private int windowSizeY;
    private int tilingX;
    private int tilingY;
    private int score;
    private float spriteScale;
    private float colSizeX;
    private float colSizeY;
    private bool enemyDamaged;
    private Stopwatch sw;
    private Stopwatch deathTimer;
    private Animationtype m_currentAnimation;
    private EnemyProjectileEmitter projectileEmitter;
    private Vector2f _target;
    private Texture projectileTexture;
    private int projectileAnimationLength;
    private int projectileInterval;
    private bool stopEnemy;
    private bool stopDrawingEnemy;
    private bool hasProjectileEmitter;
    private float projectileSpeed;
    public Enemy(int _health, int _damage, int _score, Sprite _sprite, Texture _projectileTexture, 
        int _projectileAnimationLength, int _projectileInterval, int _tilingX, int _tilingY, 
        float scale, float _colSizeX, float _colSizeY, float _projectileSpeed = 200f,bool _hasProjectileEmitter = true)
    {
        health = _health;
        damage = _damage;
        sprite = _sprite;
        tilingX = _tilingX;
        tilingY = _tilingY;
        score = _score;
        spriteScale = scale;
        colSizeX = _colSizeX;
        colSizeY = _colSizeY;
        projectileTexture = _projectileTexture;
        projectileAnimationLength = _projectileAnimationLength;
        projectileInterval = _projectileInterval;
        hasProjectileEmitter = _hasProjectileEmitter;
        projectileSpeed = _projectileSpeed;
    }
    public override void Initialize()
    {
        // Set fixed variables
        windowSizeY = 900;
        moveVector = new Vector2f(0, 1);
        moveSpeed = 150f;
        animationSpeed = 6f;
        animationPosX = 0;
        animationPosY = 0;
        m_currentAnimation = Animationtype.Idle;
        enemyDamaged = false;
        projectileEmitter = new(_target, projectileTexture, projectileAnimationLength, projectileInterval, projectileSpeed);
        projectileEmitter.Initialize();
        sw = new();
        deathTimer = new();
        stopEnemy = false;
        stopDrawingEnemy = false;

        // Set Sprite
        sprite.Position = new Vector2f(0, -windowSizeY/2);
        sprite.Scale = new Vector2f(spriteScale, spriteScale);
        sprite.TextureRect = new IntRect(0, 0, sprite.TextureRect.Width / tilingX, sprite.TextureRect.Height / tilingY);
        sprite.Origin = new Vector2f(sprite.TextureRect.Width / 2, sprite.TextureRect.Height / 2);
    }
    public override void Update(float deltaTime)
    {
        if (!stopEnemy)
        {
            if (hasProjectileEmitter)
            {
                projectileEmitter.SetPosition(sprite.Position);
                projectileEmitter.SetTarget(_target);
            }
            Move(deltaTime);
            EnemyDamaged();
            UpdateCollision();
        }
        if (hasProjectileEmitter)
        {
            projectileEmitter.Update(deltaTime);
        }
        Animate(deltaTime);
    }
    public override void Draw(RenderWindow window)
    {
        if (!stopDrawingEnemy)
        {
            window.Draw(sprite);
        }
        if (hasProjectileEmitter)
        {
            projectileEmitter.Draw(window);
        }
        DrawObjects.DrawCollisionRect(sprite.Position, collisionRect, Color.Green, window);
    }
    private void UpdateCollision()
    {
        collisionRect = new IntRect((int)sprite.Position.X - (int)(sprite.GetGlobalBounds().Width / 2 / colSizeX), 
            (int)sprite.Position.Y - (int)(sprite.GetGlobalBounds().Height / 2 / colSizeY),
            (int)(sprite.GetGlobalBounds().Width / colSizeX),
            (int)(sprite.GetGlobalBounds().Height / colSizeY));
    }
    private void Animate(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;

        animationFrame = (int)animationTime % tilingX;
        animationPosX = animationFrame * sprite.TextureRect.Width;
        animationPosY = (int)m_currentAnimation * sprite.TextureRect.Height;

        sprite.TextureRect = new IntRect(animationPosX, animationPosY, sprite.TextureRect.Width, sprite.TextureRect.Height);
    }
    private void Move(float deltaTime)
    {
        sprite.Position += moveVector * deltaTime * moveSpeed;
    }
    public void DamageEnemy(int playerDamage)
    {
        health -= playerDamage;
        sw.Restart();
        m_currentAnimation = Animationtype.Damaged;
        enemyDamaged = true;
    }
    private void EnemyDamaged()
    {
        if (sw.ElapsedMilliseconds > 200 && enemyDamaged)
        {
            enemyDamaged = false;
            m_currentAnimation = Animationtype.Idle;
            sw.Reset();
        }
    }
    public void SetMovement(Vector2f position, Vector2f direction, float rotation, bool flipCollision)
    {
        sprite.Position = position;
        moveVector = direction;
        sprite.Rotation = rotation;
        if (flipCollision == true)
        {
            Vector2f swap = new Vector2f(colSizeX, colSizeY);
            colSizeX = swap.Y;
            colSizeY = swap.X;
        }
    }
    public Sprite GetSprite()
    {
        return sprite;
    }
    public int GetScore()
    {
        return score;
    }
    public void SetTarget(Vector2f target)
    {
        _target = target;
    }
    public void Stop()
    {
        moveVector = new Vector2f(0, 0);
        m_currentAnimation = Animationtype.Defeated;
        collisionRect = new IntRect(0, 0, 0, 0);
        if (hasProjectileEmitter)
        {
            projectileEmitter.SetStopBulletSpawn(true);
        }
        stopEnemy = true;
    }
    public Stopwatch GetDeathTimer()
    {
        return deathTimer;
    }
    public bool GetStopDrawingEnemy()
    {
        return stopDrawingEnemy;
    }
    public void SetStopDrawingEnemy(bool value)
    {
        stopDrawingEnemy = value;
    }
    public void SetAnimationTime(float value)
    {
        animationTime = value;
    }
    public void SetAnimationSpeed(float value)
    {
        animationSpeed = value;
    }
    public EnemyProjectileEmitter GetProjectileEmitter()
    {
        return projectileEmitter;
    }
    public int GetDamage()
    {
        return damage;
    }
    public int GetHealth()
    {
        return health;
    }
    public IntRect GetCollisionRect()
    {
        return collisionRect;
    }
}