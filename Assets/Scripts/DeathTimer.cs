using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTimer : MonoBehaviour
{
    public float lifetime;
    void Start()
    {
        StartCoroutine(inevitable());
    }

    IEnumerator inevitable()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
