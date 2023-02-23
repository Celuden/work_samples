using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CharacterControllerPrison : MonoBehaviour
{
    private Rigidbody2D body;
    private float horizontal;
    private float vertical;
    private float moveLimiter = 0.7f;
    private bool isRunning = false;
    private bool facingRight = true;
    private float frameLimiter = 0.13f;
    private bool isDead = false;
    public bool inWater = false;
    public List<string> deathSounds;
    public List<GameObject> spotlights;
    public GameObject audioManager;
    public GameObject zoomCamera;
    public float runSpeed = 20.0f;
    public GameObject respawnPoint;
    public Sprite[] idleSequence;
    public Sprite[] runSequence;
    public float FramesPerSecond = 30;
    public GameObject tomb;
    public GameObject finish;

    void Start ()
    {
        transform.position = respawnPoint.transform.position;
        body = GetComponent<Rigidbody2D>();
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 1);
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        
        if (horizontal > 0 && !facingRight)
        {
            FlipHorizontal();
        }
        if (horizontal < 0 && facingRight)
        {
            FlipHorizontal();
        }
        
        Animate();
    }

    void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0)
        {
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
            isRunning = true;
            StartWalkSound();
        }
        else if (horizontal != 0 || vertical != 0)
        {
            isRunning = true;
        }
        else
        {
            StopWalkSound();
            isRunning = false;
        }

        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Spotlight"))
        {
            Initiate.Fade("Level", Color.black, 1.0f);
            Time.timeScale = 1.0f;
            runSpeed = 0;
            Death();
        }
        else if (other.gameObject.CompareTag("Finish"))
        {
            audioManager.GetComponent<AudioManager>().Stop("Theme");
            audioManager.GetComponent<AudioManager>().Play("Victory");
            gameObject.GetComponent<CircleCollider2D>().enabled = !gameObject.GetComponent<CircleCollider2D>().enabled;
            finish.gameObject.SetActive(true);
            Initiate.Fade("Menu", Color.black, 0.2f);
            runSpeed = 0;
        }
    }
    private void Animate()
    {
        int frame = (int)(Time.time * FramesPerSecond * frameLimiter);

        if (isRunning)
        {
            frame = frame % runSequence.Length;
            var renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = runSequence[frame];
        }
        else
        {
            frame = frame % idleSequence.Length;
            var renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = idleSequence[frame];
        }
    }

    private void FlipHorizontal()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }

    public void Death(string death = "Standard")
    {
        if (!isDead)
        {
            audioManager.GetComponent<AudioManager>().StopAllSounds();
            Time.timeScale = 0.6f;
            tomb.GetComponent<AnimateTombstone>().StartAnimation(gameObject.transform.position);
            zoomCamera.gameObject.SetActive(true);
            foreach (var s in spotlights)
            {
                s.gameObject.GetComponent<MoveSpotlight>().enabled =
                    !s.gameObject.GetComponent<MoveSpotlight>().enabled;
            }

            switch (death)
            {
                case "Standard":
                    audioManager.GetComponent<AudioManager>().Play("Gun");
                    break;
                case "Croco":
                    audioManager.GetComponent<AudioManager>().Play("Croco");
                    break;
                case "Dog":
                    audioManager.GetComponent<AudioManager>().Play("Bark");
                    break;
                default:
                    Debug.Log("No correct death sound.");
                    break;
            }
            
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
            
            ParticleSystem[] pSystems = gameObject.transform.GetComponentsInChildren<ParticleSystem>();
            foreach (var p in pSystems)
            {
                p.Play();
            }

            int indexDeath = Random.Range(0, deathSounds.Count - 1);
            string deathSound = deathSounds[indexDeath];
            audioManager.GetComponent<AudioManager>().Play(deathSound);
            isDead = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            audioManager.GetComponent<AudioManager>().Play("Lose");
        }
    }
    private void StartWalkSound()
    {

        if (!inWater)
        {
            if (audioManager.GetComponent<AudioManager>().IsPlaying("WaterWalk"))
            {
                audioManager.GetComponent<AudioManager>().Stop("WaterWalk");
            }
            
            if (!audioManager.GetComponent<AudioManager>().IsPlaying("GrassWalk"))
            {
                audioManager.GetComponent<AudioManager>().Play("GrassWalk");
            }
        }
        else
        {
            if (audioManager.GetComponent<AudioManager>().IsPlaying("GrassWalk"))
            {
                audioManager.GetComponent<AudioManager>().Stop("GrassWalk");
            }
            
            if (!audioManager.GetComponent<AudioManager>().IsPlaying("WaterWalk"))
            {
                audioManager.GetComponent<AudioManager>().Play("WaterWalk");
            }
        }
    }

    private void StopWalkSound()
    {
        audioManager.GetComponent<AudioManager>().Stop("GrassWalk");
        audioManager.GetComponent<AudioManager>().Stop("WaterWalk");
    }
}
