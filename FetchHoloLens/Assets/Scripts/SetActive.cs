using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    public GameObject hand;
    int count; 
    // Start is called before the first frame update
    void Start()
    {
        count = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        count++;
        if (count == 6000)
        {
            hand.SetActive(true); 
        }
    }
}
