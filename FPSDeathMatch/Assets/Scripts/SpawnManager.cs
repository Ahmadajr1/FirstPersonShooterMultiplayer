using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{

    #region singleton
    public static SpawnManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
            SpawnPlayer();
        else
            SceneManager.LoadScene("Main Menu");
    }


    public Transform GetSpawnPoint
    {
        get 
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            Transform returnedTransform = spawnPoints[randomIndex];
            spawnPoints.Remove(returnedTransform);
            return returnedTransform;
        }
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = GetSpawnPoint;
        Debug.Log(spawnPoint.position);
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
