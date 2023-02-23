using UnityEngine;
using System.Collections;

public class enemy_movement : MonoBehaviour 
{
    public float walkSpeed = 1.0f;      
    public float wallLeft = 0.0f;       
    public float wallRight = 5.0f;      
    float walkingDirection = 1.0f;
    Vector2 walkAmount;
    float originalX;
	private bool movingRight = true;

    void Start () 
    {
        this.originalX = this.transform.position.x;
		wallLeft = transform.position.x - 4f;
		wallRight = transform.position.x + 4f;
    }

    void Update () 
    {
        walkAmount.x = walkingDirection * walkSpeed * Time.deltaTime;
        if (walkingDirection > 0.0f && transform.position.x >= wallRight) 
		{
            walkingDirection = -1.0f;
			transform.localScale= new Vector2(2,2);
			movingRight = false;
        } 
		else if (walkingDirection < 0.0f && transform.position.x <= wallLeft) 
		{
            walkingDirection = 1.0f;
			transform.localScale= new Vector2(-2,2);
			movingRight = true;
        }
        transform.Translate(walkAmount);
    }
}