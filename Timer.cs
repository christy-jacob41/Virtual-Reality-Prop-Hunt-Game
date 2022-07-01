using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class Timer : MonoBehaviour
{
    public float timeRemaining = 10;
    public bool timerIsRunning = false;
    public Text timeText;

    PhotonView PV;


    [PunRPC]
	void openLockedRoom(int objectID)
	{
		// objectToTag.gameObject.tag = tag;
		
		GameObject temp = PhotonView.Find(objectID).gameObject;
		temp.SetActive(false);
		Debug.Log(objectID);
	}
    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        PV = GetComponent<PhotonView>();
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                PV.RPC("openLockedRoom", RpcTarget.All, 193);
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}