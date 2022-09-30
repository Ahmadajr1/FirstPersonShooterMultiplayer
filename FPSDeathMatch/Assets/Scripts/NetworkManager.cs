using System.Collections;
using System.Collections.Generic;
using System.Text;
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
    [SerializeField] private GameObject LoadingCanvas;
    [SerializeField] private GameObject LoginCanvas;
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
    [SerializeField] private TMP_InputField nameInputField;

    [Header("References")]
    [SerializeField] private RoomButton RoomReference;

    List<RoomButton> roomsList = new List<RoomButton>();
    public string roomName;
    #endregion

    #region Unity Methods
 
    #endregion

    #region PUN Callbacks
    public override void OnConnectedToMaster()
    {
        JoinTheLobby();        
    }

    public override void OnJoinedLobby()
    {
        MainMenuCanvas.SetActive(true);
    }
    public override void OnCreatedRoom()
    {
        OpenRoomPanel(roomName);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        OpenErrorPanel("returnCode = " + returnCode.ToString() + ", error message =" + message);
    }
    public override void OnJoinedRoom()
    {
        OpenRoomPanel(roomName);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        OpenErrorPanel("returnCode = " + returnCode.ToString() + ", error message =" + message);
    }
    public override void OnLeftRoom()
    {
        OpenMainMenuPanel();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        List<RoomButton> buttonsToBeRemoved = new List<RoomButton>();

        foreach (RoomInfo roomInfo in roomList)
        {
            foreach (RoomButton roomButton in roomsList)
            {
                if (roomButton.roomInfo.Name == roomInfo.Name)
                {
                    Destroy(roomButton.gameObject);
                    buttonsToBeRemoved.Add(roomButton);
                }
            }

            if (roomInfo.PlayerCount < roomInfo.MaxPlayers && !roomInfo.RemovedFromList)
            {
                RoomButton tempButton = Instantiate(RoomReference, RoomReference.transform.parent);
                tempButton.SetRoomInfo(roomInfo);
                tempButton.gameObject.SetActive(true);
                roomsList.Add(tempButton);
            }
        }

        foreach (RoomButton roomButton in buttonsToBeRemoved)
            roomsList.Remove(roomButton);
    }
    #endregion

    #region Custom Methods
    #region Buttons Methods
    public void CloseAllMenus()
    {
        LoginCanvas.SetActive(false);
        LoadingCanvas.SetActive(false);
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
        LoadingCanvas.SetActive(true);
        loadingText.text = message;
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
        errorText.text = message;
    }

    public void AssignRandomName()
    {
        nameInputField.text = GenerateRandomName();
    }

    public void Login()
    {
        if (nameInputField.text != "")
            ConnectToServer();
    }
    #endregion

    public string GenerateRandomName()
    {
        int randIndex1 = Random.Range(0, RandomNameGenerator.names.Length);
        int randIndex2 = Random.Range(0, RandomNameGenerator.names.Length);
        int randIndex3 = Random.Range(0, RandomNameGenerator.names.Length);
        string firstName = RandomNameGenerator.names[randIndex1];
        string middleName = RandomNameGenerator.names[randIndex2];
        string lastName = RandomNameGenerator.names[randIndex3];

        StringBuilder name = new StringBuilder();
        name.Append(firstName + " ");
        name.Append(middleName + " ");
        name.Append(lastName);
        return name.ToString();
    }
    private void ConnectToServer()
    {
        OpenLoadingPanel("Connecting to Server");
        PhotonNetwork.ConnectUsingSettings();
    }

    private void JoinTheLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        OpenLoadingPanel("Creating room...");
        roomName = createRoomInputField.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = isOpen;
        roomOptions.IsVisible = isVisible;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom()
    {
        OpenLoadingPanel("Joining room...");
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion
}
