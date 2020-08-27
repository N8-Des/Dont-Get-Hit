using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampagoLightning : MonoBehaviour
{
    public GameObject[] trails; 
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(trailLoop());
    }
    IEnumerator trailLoop()
    {
        while (true)
        {
            foreach(GameObject go in trails)
            {
                float r = Random.Range(-3.0f, 3.0f);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, r, 0);
            }
            yield return new WaitForSeconds(0.01f);

        }
    }
}
