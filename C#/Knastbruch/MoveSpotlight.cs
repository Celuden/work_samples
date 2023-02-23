using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveSpotlight : MonoBehaviour
{
    Transform[] positions;
    Vector2 currectMoveVector = new(0, 0);
    public GameObject group;
    public float moveSpeed;
    public float closeThreshold;
    private int index = 0;
    void Start()
    {
        positions = group.GetComponentsInChildren<Transform>();
        CalculateMoveVector();
    }
    void Update()
    {
        float distance = (new Vector2(positions[index].transform.position.x, positions[index].transform.position.y) -
                         new Vector2(this.transform.position.x, this.transform.position.y)).magnitude;
        if (distance < closeThreshold)
            CalculateMoveVector();
        UpdateSpotlightMovement();
    }

    private void UpdateSpotlightMovement()
    {
        this.transform.position += new Vector3(currectMoveVector.x, currectMoveVector.y, 0) *
                    moveSpeed * Time.timeScale;
    }
    private void CalculateMoveVector()
    {
        index++;
        if (index >= positions.Length)
        {
            index = 0;
        }
        currectMoveVector = (new Vector2(positions[index].transform.position.x, positions[index].transform.position.y) 
            - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;
    }
}
