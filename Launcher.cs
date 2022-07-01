using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    // Start is called before the first frame update
    [SerializeField] InputField roomNameInputField;
    [SerializeField] Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    private static Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;

    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("TitleMenu");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");

    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("LoadingMenu");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("RoomList");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        
        foreach (Transform child in playerListContent)
        {
            Debug.Log("Destroyed");

            Destroy(child.gameObject);
        }

        Debug.Log(players.Length);

        for (int i = 0; i < players.Length; i++)
        {

            if (players[i].IsMasterClient)
                players[i].NickName = players[i].NickName + " (Seeker)";
            GameObject playerClone = Instantiate(PlayerListItemPrefab, playerListContent);

            playerClone.GetComponent<Text>().enabled = true;

            playerClone.GetComponent<PlayerListItem>().SetUp(players[i]);

        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Debug.Log("Destroyed");

            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {

            if (players[i].IsMasterClient)
                players[i].NickName = players[i].NickName + " (Seeker)";
            GameObject playerClone = Instantiate(PlayerListItemPrefab, playerListContent);

            playerClone.GetComponent<Text>().enabled = true;

            playerClone.GetComponent<PlayerListItem>().SetUp(players[i]);

        }
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.OpenMenu("ErrorMenu");
    }

    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("LoadingMenu");
    }

    public void LeaveRoom2(){
        MenuManager.Instance.OpenMenu("TitleMenu");
    }


    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("LoadingMenu");

    }

    public override void OnLeftRoom(){
        MenuManager.Instance.OpenMenu("TitleMenu");
        cachedRoomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);

        }
        Debug.Log("all rooms: " + roomList.Count);
        // Debug.Log(string.Concat(GetRoomList().Select(x => x.name + "\n\r").ToArray()));
        // for (int i = 0; i < roomList.Count; i++)
        // {
        //     if (roomList[i].RemovedFromList)
        //         continue;
        //     GameObject listClone = Instantiate(roomListItemPrefab, roomListContent);
        //     listClone.GetComponent<Image>().enabled = true;
        //     listClone.GetComponent<Button>().enabled = true;

        //     listClone.GetComponent<RoomListItem>().SetUp(roomList[i]);
        // }
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }

        foreach (KeyValuePair<string, RoomInfo> entry in cachedRoomList)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(cachedRoomList[entry.Key]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void exit()
    {
        Application.Quit();
    }
}
