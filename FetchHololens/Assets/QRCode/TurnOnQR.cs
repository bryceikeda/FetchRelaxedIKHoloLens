using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnQR : MonoBehaviour
{
    [SerializeField] GameObject qrManager;

    private void Start()
    {
        StartCoroutine(TurnOn()); 
    }

    IEnumerator TurnOn()
    {
        yield return new WaitForSeconds(5);
        qrManager.SetActive(true);
    }
}
