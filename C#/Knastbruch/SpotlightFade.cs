using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightFade : MonoBehaviour
{
    public GameObject group;
    float distance;
    [SerializeField] private GameObject[] spotlights;

    void Update()
    {
        var charPos = this.transform.position;

        int index = GetClosestSpotlightIndex(charPos);

        Vector2 spotlight2d = new Vector2 (spotlights[index].transform.position.x,spotlights[index].transform.position.y);
        Vector2 player2d = new Vector2 (charPos.x,charPos.y);
        float spotLightDistance = (spotlight2d- player2d).magnitude;
        
        if (spotLightDistance < spotlights[index].GetComponent<Light>().range)
        {
            Color tmp = this.GetComponent<SpriteRenderer>().color;
            tmp.a = 1 - ((spotLightDistance / spotlights[index].GetComponent<CircleCollider2D>().radius));
            this.GetComponent<SpriteRenderer>().color = tmp;
        }
    }

    private int GetClosestSpotlightIndex(Vector3 charPos)
    {
        float lowestDistance = float.MaxValue;
        int saveIndex = 0;
        for (int i = 0; i < spotlights.Length; i++)
        {
            distance = (spotlights[i].transform.position - charPos).magnitude;
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                saveIndex = i;
            }
        }
        return saveIndex;
    }
}
