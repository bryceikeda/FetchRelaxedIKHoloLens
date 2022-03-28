using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class TargetLabel : MonoBehaviour
{
    public TextMeshPro tmp;

    // Update is called once per frame
    void Update()
    {
        tmp.text = "x      y     z\n" + transform.position.x + ", " + transform.position.z + ", " + transform.position.y; 
    }
}
