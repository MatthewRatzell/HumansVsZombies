using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTreasure : MonoBehaviour
{
    public void GrabTreasure()
    {
        Camera cam = Camera.main;
        float max = 9.5f;
        float min = 0f;

        transform.position = new Vector3(Random.Range(min, max), 1.1f ,Random.Range(min, max));
    }
}
