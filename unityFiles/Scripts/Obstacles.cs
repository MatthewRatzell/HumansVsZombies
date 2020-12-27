using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    //giving a radius so we can set its radius
    public float radius = 1f;
    //place to hold our gamemanager referance
    GameManager1 gameManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager1>();
    }
}
