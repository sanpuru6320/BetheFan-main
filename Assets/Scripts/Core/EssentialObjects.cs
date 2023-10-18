using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    bool hasSpawn = false;
    private void Awake()
    {
        if (!hasSpawn)
        {
            DontDestroyOnLoad(gameObject);
            hasSpawn = true;
        }
    }
}
