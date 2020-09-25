using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
   public float timer = 0.0001f;
    void Start()
    {
        Destroy(gameObject, timer);
        Debug.Log("Destroying stuff");
        
    }

}
