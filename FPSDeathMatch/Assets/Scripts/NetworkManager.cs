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
    [Header("Panels")]
    [SerializeField] private GameObject LoadingPanel;
    [SerializeField] private GameObject MainMenuCanvas;
    [SerializeField] private GameObject FindRoomCanvas;
    [SerializeField] private GameObject CreateRoomCanvas;
    [SerializeField] private GameObject RoomCanvas;
    [SerializeField] private GameObject ErrorCanvas;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TMP_InputField createRoomInputField;

    [Header("References")]
    [SerializeField] private RoomButton RoomReference;

    #endregion

    List<RoomButton> RoomsList = new List<RoomButton>();

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
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomButton roomButton in RoomsList)
        {
            Destroy(roomButton.gameObject);
        }

        RoomsList.Clear();

        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.PlayerCount < roomInfo.MaxPlayers && !roomInfo.RemovedFromList)
            {
                RoomButton tempButton = Instantiate(RoomReference, RoomReference.transform.parent);
                tempButton.SetRoomInfo(roomInfo);
                tempButton.gameObject.SetActive(true);
                RoomsList.Add(tempButton);
            }
        }
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
        RoomCanvas.SetActive(false);
        ErrorCanvas.SetActive(false);
    }

    public void OpenMainMenuPanel()
    {
        CloseAllMenus();
        MainMenuCanvas.SetActive(true);
    }

    public void OpenLoadingPanel(string message)
    {
        CloseAllMenus();
        LoadingPanel.SetActive(true);
    }

    public void OpenFindRoomPanel()
    {
        CloseAllMenus();
        FindRoomCanvas.SetActive(true);
    }
    public void OpenCreateRoomPanel()
    {
        CloseAllMenus();
        CreateRoomCanvas.SetActive(true);
    }

    public void OpenRoomPanel(string roomName)
    {
        CloseAllMenus();
        RoomCanvas.SetActive(true);
        roomNameText.text = roomName;
    }

    public void OpenErrorPanel(string message)
    {
        CloseAllMenus();
        ErrorCanvas.SetActive(true);
    }
    #endregion

    private void JoinTheLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        string roomName = createRoomInputField.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = isOpen;
        roomOptions.IsVisible = isVisible;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        OpenRoomPanel(roomName);
    }
    #endregion
}
