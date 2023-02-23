// Impressum
// Das Projekt "Maro's Mayhem" ist im Studiengang MultiMediaTechnology / FHS im Rahmen des MultiMediaProjekt 1 von Alija Suljic erstellt worden.
// The project "Maro's Mayhem" has been developed within the MultiMediaTechnology Bachelor Studies at the Fachhochschule Salzburg as part of the MultiMediaProject 1 by Alija Suljic in the year 2022.

using SFML.System;
using SFML.Graphics;

internal class EnemyProjectile : GameObject
{
    private Sprite projectileSprite;
    private Vector2f _moveVector;
    private IntRect collisionRect;
    private float animationTime;
    private float animationSpeed;
    private int animationPosX;
    private int animationFrame;
    private int animationLength;
    private float projectileSpeed;
    private bool isTornado;
    public EnemyProjectile(Sprite projectileTexture, int _animationLength, float _projectileSpeed = 200f, bool _isTornado = false)
    {
        projectileSprite = projectileTexture;
        animationLength = _animationLength;
        projectileSpeed = _projectileSpeed;
        isTornado = _isTornado;
    }
    public override void Initialize()
    {
        // Set variables
        _moveVector = new Vector2f(1, 0);
        animationSpeed = 10f;
        animationPosX = 0;
                
        // Set Sprite
        projectileSprite.Position = new Vector2f(0, 0);
        projectileSprite.TextureRect = new IntRect(0, 0, projectileSprite.TextureRect.Width / animationLength, projectileSprite.TextureRect.Height);
        projectileSprite.Origin = new Vector2f(projectileSprite.TextureRect.Width / 2, projectileSprite.TextureRect.Height / 2);
        projectileSprite.Scale = new Vector2f(0.5f, 0.5f);
    }
    public override void Update(float deltaTime)
    {
        AnimateProjectile(deltaTime);
        MoveProjectile(deltaTime);
        UpdateCollision();
    }
    public override void Draw(RenderWindow window)
    {
        window.Draw(projectileSprite);
        DrawObjects.DrawRectOutline(projectileSprite.Position, (int)projectileSprite.GetGlobalBounds().Width, (int)projectileSprite.GetGlobalBounds().Height, Color.Red, window);
        DrawObjects.DrawCollisionRect(projectileSprite.Position, collisionRect, Color.Green, window);
    }
    private void AnimateProjectile(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;

        // Frame Positioning
        animationFrame = (int)animationTime % animationLength;
        animationPosX = animationFrame * projectileSprite.TextureRect.Width;

        projectileSprite.TextureRect = new IntRect(animationPosX, 0, projectileSprite.TextureRect.Width, projectileSprite.TextureRect.Height);
    }
    private void MoveProjectile(float deltaTime)
    {
        projectileSprite.Position += _moveVector * deltaTime * projectileSpeed;
    }
    public void SetSettings(Vector2f pos, float rotation, Vector2f moveVector)
    {
        projectileSprite.Position = pos;
        projectileSprite.Rotation = rotation;
        _moveVector = moveVector;
    }
    public Sprite GetSprite()
    {
        return projectileSprite;
    }
    private void UpdateCollision()
    {
        if (isTornado)
        {
            collisionRect = new IntRect((int)projectileSprite.Position.X - (int)projectileSprite.GetGlobalBounds().Width / 2, 
            (int)projectileSprite.Position.Y - (int)projectileSprite.GetGlobalBounds().Height / 2,
            (int)projectileSprite.GetGlobalBounds().Width / 2,
            (int)projectileSprite.GetGlobalBounds().Height / 2);
        }
        else
        {
            collisionRect = new IntRect((int)projectileSprite.Position.X - (int)projectileSprite.GetGlobalBounds().Width / 2, 
            (int)projectileSprite.Position.Y - (int)projectileSprite.GetGlobalBounds().Height / 2,
            (int)projectileSprite.GetGlobalBounds().Width / 3,
            (int)projectileSprite.GetGlobalBounds().Height / 3);
        }
    }
    public IntRect GetCollisionRect()
    {
        return collisionRect;
    }
}