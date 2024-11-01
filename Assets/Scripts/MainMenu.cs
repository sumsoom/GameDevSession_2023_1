using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public InputField inputField;
    public Button button;

    private void Awake()
    {
        button.onClick.AddListener(StartServer);
    }

    private void StartServer()
    {
        PhotonNetwork.NickName = inputField.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");

        RoomOptions roomOptions = new RoomOptions();     
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("InGame");
    }
}
