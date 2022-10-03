using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPunCallbacks
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

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
            SpawnPlayer();
        else
            SceneManager.LoadScene("Main Menu");
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    public void Death()
    {
        PhotonNetwork.Destroy(player);
        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(3);
        Transform spawnPoint = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
