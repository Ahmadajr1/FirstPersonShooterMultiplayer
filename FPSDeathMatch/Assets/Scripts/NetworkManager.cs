using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static NetworkManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region RoomOptions
    [SerializeField] private int maximumPlayers;
    [SerializeField] private bool isVisible = true;
    [SerializeField] private bool isOpen = true;
    #endregion

    #region Canvases and UI
    [SerializeField] private GameObject LoadingPanel;
    [SerializeField] private GameObject MainMenuCanvas;
    [SerializeField] private GameObject FindRoomCanvas;
    [SerializeField] private GameObject CreateRoomCanvas;
    [SerializeField] private GameObject ErrorCanvas;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TMP_InputField createRoomInputField;
    [SerializeField] private TextMeshProUGUI errorText;
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        loadingText.SetText("Connecting to Server");
        PhotonNetwork.ConnectUsingSettings();
    }
    #endregion

    #region PUN Callbacks
    public override void OnConnectedToMaster()
    {
        JoinTheLobby();        
    }

    public override void OnJoinedLobby()
    {
        LoadingPanel.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }
    public override void OnCreatedRoom()
    {
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"returnCode = {returnCode}, error message = {message}");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("We have joined a room");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"returnCode = {returnCode}, error message = {message}");
    }
    public override void OnLeftRoom()
    {
    }
    #endregion

    #region Custom Methods
    #region Buttons Methods
    public void CloseAllMenus()
    {
        LoadingPanel.SetActive(false);
        MainMenuCanvas.SetActive(false);
        FindRoomCanvas.SetActive(false);
        CreateRoomCanvas.SetActive(false);
        ErrorCanvas.SetActive(false);
    }

    public void OpenMainMenuButtons()
    {
        MainMenuCanvas.SetActive(true);
    }

    public void OpenLoadingPanel(string message)
    {
        LoadingPanel.SetActive(true);
    }

    public void OpenRoomPanel()
    {
        FindRoomCanvas.SetActive(true);
    }
    public void OpenCreateRoomPanel()
    {
        CreateRoomCanvas.SetActive(true);
    }

    public void OpenErrorPanel(string message)
    {
        ErrorCanvas.SetActive(true);
    }
    #endregion

    private void JoinTheLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom(string RoomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = isOpen;
        roomOptions.IsVisible = isVisible;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(RoomName, roomOptions);
    }
    #endregion
}
