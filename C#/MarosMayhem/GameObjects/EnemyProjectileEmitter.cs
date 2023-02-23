// Impressum
// Das Projekt "Maro's Mayhem" ist im Studiengang MultiMediaTechnology / FHS im Rahmen des MultiMediaProjekt 1 von Alija Suljic erstellt worden.
// The project "Maro's Mayhem" has been developed within the MultiMediaTechnology Bachelor Studies at the Fachhochschule Salzburg as part of the MultiMediaProject 1 by Alija Suljic in the year 2022.

using System.Collections.Generic;
using System.Diagnostics;
using SFML.System;
using SFML.Graphics;

internal class EnemyProjectileEmitter : GameObject
{
    protected Vector2f position;
    private Vector2f placePosition;
    protected Vector2f _target;
    private Texture _projectileTexture;
    protected List<EnemyProjectile> _projectileList;
    private float projectileSpeed;
    private Stopwatch sw;
    private int _projectileInterval;
    private int _projectileAnimationLength;
    private bool stopBulletSpawn;
    protected bool stopBoss;
    public EnemyProjectileEmitter(Vector2f playerTarget, Texture projectileTexture, int projectileAnimationLength, int projectileInterval, float _projectileSpeed = 200f)
    {
        _target = playerTarget;
        _projectileTexture = projectileTexture;
        _projectileAnimationLength = projectileAnimationLength;
        _projectileInterval = projectileInterval;
        projectileSpeed = _projectileSpeed;
    }
    public override void Initialize()
    {
        // Set variables
        stopBulletSpawn = false;
        position = new Vector2f(0, 0);
        placePosition = new Vector2f(0, 0);
        _projectileList = new();
    
        // Stopwatch
        sw = new();
        sw.Start();
    }
    public override void Update(float deltaTime)
    {
        PlaceEmitter(placePosition);
        ShootProjectiles();
        foreach (EnemyProjectile p in _projectileList)
        {
            p.Update(deltaTime);
        }
    }
    public override void Draw(RenderWindow window)
    {
        foreach (EnemyProjectile p in _projectileList)
        {
            p.Draw(window);
            if (p.GetSprite().Position.X > window.Size.X / 2 || p.GetSprite().Position.Y > window.Size.Y / 2 
            || p.GetSprite().Position.X < -(window.Size.X / 2) || p.GetSprite().Position.Y < -(window.Size.Y / 2))
            {
                _projectileList.Remove(p);
                break;
            }
        }
    }
    public virtual void ShootProjectiles()
    {
        if (sw.ElapsedMilliseconds > _projectileInterval && !stopBulletSpawn)
        {
            EnemyProjectile projectile = new(new Sprite(_projectileTexture), _projectileAnimationLength, projectileSpeed);
            projectile.Initialize();
            projectile.SetSettings(position, Utils.ToDegrees(Utils.AngleBetween(position, _target)), Utils.Normalize(_target - position));
            _projectileList.Add(projectile);
            sw.Restart();
        }  
    }
    public void SetPosition(Vector2f newPosition)
    {
        placePosition = newPosition;
    }

    private void PlaceEmitter(Vector2f _placePosition)
    {
        position = _placePosition;
    }
    public void SetTarget(Vector2f target)
    {
        _target = target;
    }
    public void SetStopBulletSpawn(bool value)
    {
        stopBulletSpawn = value;
    }
    public bool GetStopBoss()
    {
        return stopBoss;
    }
    public Stopwatch GetStopwatch()
    {
        return sw;
    }
    public List<EnemyProjectile> GetProjectileList()
    {
        return _projectileList;
    }

    // Boss Code | This is only code to be overwritten by /// DO NOT SCROLL FURTHER, CONSIDER THIS A WARNING /// WEE OO WEE OO /// MAYBE I AM THE PROBLEM
    public virtual void SetBossEyePosition(Vector2f leftEyePos, Vector2f rightEyePos) {}
    public virtual void SetAttackMove(int _attackMove) {}
    public virtual bool GetAttackFinished() {return true;}
    public virtual bool GetNearlyAttackFinished() {return true;}
    public virtual void SetNearlyAttackFinished() {}
}