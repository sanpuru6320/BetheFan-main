using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpriteOrder : MonoBehaviour
{

    SpriteRenderer mySR;
    [SerializeField] Transform playerTrans;
    [SerializeField] float boundValue;
    float boundY;

    void Awake()
    {
        mySR = GetComponent<SpriteRenderer>();
        boundY = transform.position.y - boundValue;
    }

    void Update()
    {
        if (playerTrans.position.y < boundY)
        {  //player‚ª‘O
            mySR.sortingLayerName = "CharacterBack";
        }
        else
        {
            mySR.sortingLayerName = "CharacterFront";
        }
    }
}