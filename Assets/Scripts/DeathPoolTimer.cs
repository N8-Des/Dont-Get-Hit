using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPoolTimer : MonoBehaviour
{
    public float lifetime;
    void OnEnable()
    {
        StartCoroutine(turnOff());   
    }
    IEnumerator turnOff()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}
