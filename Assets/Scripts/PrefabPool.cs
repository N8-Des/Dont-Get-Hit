using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool : MonoBehaviour
{
    public List<NewPool> pools = new List<NewPool>();
    public PhotonView view;    
    [System.Serializable]
    public class NewPool
    {
        public string prefabName;
        public List<GameObject> prefabs = new List<GameObject>();
        public int maxPrefabs; 
    }
    [PunRPC]
    public void turnOnObject(int viewObject)
    {
        PhotonView.Find(viewObject).gameObject.SetActive(true);
    }
    [PunRPC]
    public void setObjectPosition(int viewObject, Vector3 viewPosition, Quaternion viewRotation)
    {
        GameObject go = PhotonView.Find(viewObject).gameObject;
        if (PhotonNetwork.isMasterClient && go.tag == "Dodgeball")
        {
            go.GetComponent<PhotonView>().RPC("RPC_SetRealPosition", PhotonTargets.All, viewPosition);
        }
        go.transform.position = viewPosition;
        go.transform.rotation = viewRotation;
    }
    public GameObject spawnObject(Vector3 position, Quaternion rotation, int index)
    {
        if (pools[index].prefabs.Count == 0)
        {
            GameObject newSpawn = PhotonNetwork.Instantiate(pools[index].prefabName, position, rotation, 0);
            pools[index].prefabs.Add(newSpawn);
            return newSpawn;
        }
        else
        {
            foreach (GameObject go in pools[index].prefabs)
            {
                if (!go.activeSelf)
                {
                    view.RPC("turnOnObject", PhotonTargets.All, go.GetPhotonView().viewID);
                    view.RPC("setObjectPosition", PhotonTargets.All, go.GetPhotonView().viewID, position, rotation);
                    return go;
                }
            }               
            GameObject newSpawn = PhotonNetwork.Instantiate(pools[index].prefabName, position, rotation, 0);
            pools[index].prefabs.Add(newSpawn);
            return newSpawn;
        }
    }
}
