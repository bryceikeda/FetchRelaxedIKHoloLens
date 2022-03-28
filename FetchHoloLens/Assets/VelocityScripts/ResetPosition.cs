using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    public Transform EELink;
    private bool Lock = true; 

    void Update()
    {
        if (Lock)
        {
            transform.position = EELink.position;
            transform.rotation = EELink.rotation;
            ToggleLock();
        }
    }
    public void ToggleLock()
    {
        Lock = !Lock; 
    }
}
