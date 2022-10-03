using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region singleton
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int killsToWin = 3;
    [SerializeField] private TextMeshProUGUI numberOfKills;
    [SerializeField] private TextMeshProUGUI numberOfDeaths;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private TextMeshProUGUI winText;

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStats
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(0);
        }
        else
            NewPlayerSend();
    }

    public void OnEvent(EventData photonEvent)
    {
        //200+ are reserved for Photon, so we want to listen to any code below 200
        if (photonEvent.Code < 200)
        {
            //get the event code and cast it into our enum
            EventCodes theEventCode = (EventCodes)photonEvent.Code;
            //get the custom data and put it into an array
            object[] customData = (object[])photonEvent.CustomData;
            switch (theEventCode)
            {
                case EventCodes.NewPlayer:
                    NewPlayerRecieve(customData);
                    break;
                case EventCodes.ListPlayers:
                    ListPlayerReceive(customData);
                    break;
                case EventCodes.UpdateStats:
                    UpdateStatReceive(customData);
                    break;
            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    #region Sending and Receiving Updates Methods
    public void NewPlayerSend()
    {
        object[] package = new object[]
        {
            PhotonNetwork.LocalPlayer.NickName,
            PhotonNetwork.LocalPlayer.ActorNumber,
            0,
            0
        };

        PhotonNetwork.RaiseEvent(
                (byte)EventCodes.NewPlayer,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                new SendOptions { Reliability = true }
        );
    }

    public void NewPlayerRecieve(object[] dataReceived)
    {
        PlayerInfo player = new PlayerInfo(
            (string)dataReceived[0],
            (int)dataReceived[1],
            (int)dataReceived[2],
            (int)dataReceived[3]
        );
        allPlayers.Add(player);
        if (allPlayers.Count == PhotonNetwork.CurrentRoom.MaxPlayers)
            ListPlayerSend();

    }

    public void ListPlayerSend()
    {
        object[] package = new object[allPlayers.Count];
        for (int i = 0; i < allPlayers.Count; i++)
        {
            object[] playerPackage = new object[]
            {
                allPlayers[i].PlayerName,
                allPlayers[i].PhotonID,
                allPlayers[i].PlayerKills,
                allPlayers[i].PlayerDeaths
            };
            package[i] = playerPackage;
        }
        PhotonNetwork.RaiseEvent(
                (byte)EventCodes.ListPlayers,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
        );
    }

    public void ListPlayerReceive(object[] dataReceived)
    {
        allPlayers = new List<PlayerInfo>(new PlayerInfo[dataReceived.Length]);
        for (int i = 0; i < dataReceived.Length; i++)
        {
            object[] objArr = dataReceived[i] as object[];
            PlayerInfo playerInfo = new PlayerInfo((string)objArr[0], (int)objArr[1], (int)objArr[2], (int)objArr[3]);
            allPlayers[i] = playerInfo;
        }
    }

    public void UpdateStatSend(int SenderID , int statToUpdate, int amountToChange)
    {
        object[] package = new object[]
        {
            SenderID,
            statToUpdate,
            amountToChange
        };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdateStats,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void UpdateStatReceive(object[] dataReceived)
    {
        int senderID = (int)dataReceived[0];
        int statToUpdate = (int)dataReceived[1];
        int amountToChange = (int)dataReceived[2];
        
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].PhotonID == senderID)
            {
                switch (statToUpdate)
                {
                    //kills
                    case 0:
                        allPlayers[i].PlayerKills += amountToChange;
                        Debug.Log($"Player {allPlayers[i].PhotonID} Kills: {allPlayers[i].PlayerKills}");
                        WinCheck();
                        break;

                    //deaths
                    case 1:
                        allPlayers[i].PlayerDeaths += amountToChange;
                        Debug.Log($"Player {allPlayers[i].PhotonID} Deaths: {allPlayers[i].PlayerDeaths}");
                        break;
                }

                if (PhotonNetwork.LocalPlayer.ActorNumber == senderID)
                {
                    numberOfKills.text = "Number of kills : " + allPlayers[i].PlayerKills.ToString();
                    numberOfDeaths.text = "Number of Deaths : " + allPlayers[i].PlayerDeaths.ToString();
                }
            } 
        }
        WinCheck();
    }
    #endregion

    private void WinCheck()
    {
        bool gameOver = false;
        string winner = "";

        foreach (PlayerInfo playerInfo in allPlayers)
        {
            if (playerInfo.PlayerKills >= killsToWin)
            {
                winner = playerInfo.PlayerName;
                gameOver = true;
            }
        }

        if (gameOver)
        {
            winText.text = winner + " won!";
            winCanvas.SetActive(true);

            StartCoroutine(ReturnToMainMenu());
        }
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(3);
        PhotonNetwork.LoadLevel(0);
    }
}

[System.Serializable]
public class PlayerInfo
{
    private string playerName;
    private int photonID;
    private int playerKills;
    private int playerDeaths;
    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value; }
    }
    public int PhotonID
    {
        get { return photonID; }
        set { photonID = value; }
    }
    public int PlayerKills
    {
        get { return playerKills; }
        set { playerKills = value; }
    }
    public int PlayerDeaths
    {
        get { return playerDeaths; }
        set { playerDeaths = value; }
    }
    public PlayerInfo(string name, int id, int kills, int deaths)
    {
        playerName = name;
        photonID = id;
        playerKills = kills;
        playerDeaths = deaths;
    }
}
