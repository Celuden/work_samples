using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
	public CharacterController2D controller;
	public Animator animator;
	public GameObject gameoverText, Restartbutton, timer, pause;
	public float runSpeed = 40f;
	
	float horizontalMove = 0f;
	bool jump = false;
	bool crouch = false;
	
    void Start()
    {
        gameoverText.SetActive(false);
		Restartbutton.SetActive(false);
    }

    void Update()
    {
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		
		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
		
		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
			FindObjectOfType<AudioManager>().Play("Jump");
			animator.SetBool("IsJumping", true);
		}
		
		if (Input.GetButtonDown("Crouch"))
		{
			crouch = true;
		} 
		else if (Input.GetButtonUp("Crouch"))
		{
			crouch = false;
		}
    }
	
	public void OnLanding ()
	{
		animator.SetBool("IsJumping", false);
	}
	
	public void OnCrouching (bool isCrouching)
	{
		animator.SetBool("IsCrouching", isCrouching);
	}
	
	void FixedUpdate ()
	{
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
		jump = false;
	}
	
	void OnCollisionEnter2D (Collision2D col)
	{
		if (col.gameObject.tag.Equals("Enemy"))
		{
			FindObjectOfType<AudioManager>().Play("OwlDeath");
			Destroy(timer);
			Destroy(pause);
			Time.timeScale = 1f;
			gameoverText.SetActive(true);
			Restartbutton.SetActive(true);
			gameObject.SetActive(false);
		}
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("Worms"))
		{
			FindObjectOfType<AudioManager>().Play("Food");
			Destroy(other.gameObject);
		}
	}
	
}
