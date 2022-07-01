using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable; 
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerController : MonoBehaviourPunCallbacks
{
	// [SerializeField] Image healthbarImage;
	// [SerializeField] GameObject ui;

	// [SerializeField] GameObject cameraHolder;

	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

	// [SerializeField] Item[] items;

	int itemIndex;

	int parentOffset;
	int previousItemIndex = -1;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	Rigidbody rb;

	PhotonView PV;

	const float maxHealth = 100f;
	float currentHealth = maxHealth;

	PlayerManager playerManager;

	public GameObject targetObject;

	GameObject reticle;

	double coolDownTime;

	public int numKills = 0;

	bool activeGame;

	void Awake()
	{
		// rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();

		// playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	}


	[PunRPC]
	void setTag(int objectID, string tag)
	{
		// objectToTag.gameObject.tag = tag;
		PhotonView.Find(objectID).gameObject.tag = tag;
		GameObject temp = PhotonView.Find(objectID).gameObject;
		
	}

	[PunRPC]
	void setMeshRenderer(int objectID)
	{
		// objectToTag.gameObject.tag = tag;
		
		GameObject temp = PhotonView.Find(objectID).gameObject;
		if(temp.GetComponent<MeshRenderer>() == null)
		{
			temp.AddComponent<MeshRenderer>();
			Destroy(temp.GetComponent<MeshCollider>());
		}
		Debug.Log(objectID);
	}

	[PunRPC]
	void openLockedRoom(int objectID)
	{
		// objectToTag.gameObject.tag = tag;
		
		GameObject temp = PhotonView.Find(objectID).gameObject;
		temp.SetActive(false);
		Debug.Log(objectID);
	}


	[PunRPC]
	void changeColor(int objectID, bool master)
	{
		// objectToTag.gameObject.tag = tag;
		
		GameObject temp = PhotonView.Find(objectID).gameObject;
		// if(master)
		// {
		// 	temp.GetComponent<Renderer>().material.color = Color.black;
		// }
		// else
		// {
		// 	temp.GetComponent<Renderer>().material.color = new Color((182f/255f), (96.0f/255f), (23.0f/255f));
		// }
		// temp.AddComponent<MeshRenderer>();
		// var col = temp.AddComponent<MeshCollider>();
		// col.convex = true;
		// Destroy(temp.GetComponent<MeshCollider>());
		var rigidBody = temp.AddComponent<Rigidbody>();
            rigidBody.isKinematic = false;
            rigidBody.constraints =  RigidbodyConstraints.FreezeRotationX
                                        | RigidbodyConstraints.FreezeRotationZ;
            rigidBody.collisionDetectionMode =  CollisionDetectionMode.ContinuousSpeculative;
		
	}


	[PunRPC]
	void killPlayer(int objectID)
	{
		// objectToTag.gameObject.tag = tag;
		numKills +=1;
		GameObject temp = PhotonView.Find(objectID).gameObject;
		temp.transform.localScale = new Vector3(0.00001f, 0.00001f, 0.00001f);
		temp.gameObject.tag = "Untagged";
		GameObject tempController = PhotonView.Find((objectID/1000) *1000 + 2).gameObject;
		tempController.gameObject.tag = "Dead";
		var rigidBody = temp.GetComponent<Rigidbody>();
		if(rigidBody == null)
		{
			return;
		}
		rigidBody.constraints =  RigidbodyConstraints.FreezeRotationX
                                        | RigidbodyConstraints.FreezeRotationZ
										| RigidbodyConstraints.FreezePositionY;
		// GameObject temp2 = PhotonView.Find((objectID/1000) *1000 + 2).gameObject;
		// var countDownDead = temp2.GetComponent<CountDownTimer>();
		// Canvas PlayerDisplay = countDownDead.PlayerUI;
		// PlayerDisplay.transform.GetChild(1).gameObject.SetActive(true);
		// Debug.Log("Player Died");
		// Text endScreenTxt = countDownDead.endscreenText;
		// endScreenTxt.text = "You are Dead";
			
			GameObject[] alive =  GameObject.FindGameObjectsWithTag("Controller");
			GameObject[] dead =  GameObject.FindGameObjectsWithTag("Dead");
			GameObject[] respawns = alive.Concat(dead).ToArray();
			var countDown = GetComponent<CountDownTimer>();
			foreach(GameObject o in respawns)
			{
				var t = o.GetComponent<CountDownTimer>();
				if(t != null && t.enabled == true)
				{
					countDown = o.GetComponent<CountDownTimer>();
					continue;
				}
				Debug.Log("Player pos: " + o.transform.position);

			}
			
			GameObject[] playersLeft =  GameObject.FindGameObjectsWithTag("Player");
			if(countDown.hiderText != null)
			{
				Text hidersRemaining = countDown.hiderText;
				hidersRemaining.text = playersLeft.Length  + " hiders left";
			}
		
		if(playersLeft.Length>0)
        {			
			// Canvas PlayerDisplay = countDown.PlayerUI;
			GameObject temp2 = PhotonView.Find((objectID/1000) *1000 + 2).gameObject;
			var canvasComponent = temp2.GetComponentInChildren<Canvas>();
			if(canvasComponent != null)
			{
				GameObject PlayerDisplay = canvasComponent.gameObject;
				GameObject EndScreen = PlayerDisplay.transform.GetChild(1).gameObject;
				EndScreen.SetActive(true);

				Debug.Log("You are Dead Called");

				Text endScreenTxt = EndScreen.transform.GetChild(1).gameObject.GetComponent<Text>();
				endScreenTxt.text = "You are Dead";
				StartCoroutine(waiter(endScreenTxt));
				// endScreenTxt.text = "";
			}
			// PlayerDisplay.GetC("EndScreen").GameObject.SetActive(true);
			

		}
		
		rigidBody.isKinematic = true;
		GameObject fire = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Particle System"), temp.transform.position, Quaternion.identity);
		
	}


	[PunRPC]
	void morphPlayer(int objectID)
	{
		// objectToTag.gameObject.tag = tag;
		targetObject = PhotonView.Find(objectID).gameObject;;
		GameObject temp = PhotonView.Find(objectID).gameObject;
		if(temp.GetComponent<Rigidbody>() != null)
		{
			return;
		}
		var rigidBody = temp.AddComponent<Rigidbody>();
        rigidBody.isKinematic = false;
        rigidBody.constraints =  RigidbodyConstraints.FreezeRotationX
                                        | RigidbodyConstraints.FreezeRotationZ;
        rigidBody.collisionDetectionMode =  CollisionDetectionMode.ContinuousSpeculative;
		rigidBody.mass = 10;
		
		
	}

	[PunRPC]
	void morphBack(int objectID)
	{
		// objectToTag.gameObject.tag = tag;

		targetObject = PhotonView.Find(objectID).gameObject;
		GameObject temp = PhotonView.Find(objectID).gameObject;
		if(temp.GetComponent<Rigidbody>() != null)
		{
			return;
		}
		var rigidBody = temp.AddComponent<Rigidbody>();
        rigidBody.isKinematic = false;
        rigidBody.constraints =  RigidbodyConstraints.FreezeRotationX
                                        | RigidbodyConstraints.FreezeRotationZ;
        rigidBody.collisionDetectionMode =  CollisionDetectionMode.ContinuousSpeculative;
		rigidBody.mass = 10;
		
		
	}

	[PunRPC]
	void Setting (int someValue)
	{
		if(PhotonNetwork.IsMasterClient)
        {
			numKills = someValue;
		}
		Debug.Log("howwzzzyyy: " + someValue + "np " + numKills);
	}
	[PunRPC]
	void hiderDub (int playersLeft)
	{
		
		GameObject[] alive =  GameObject.FindGameObjectsWithTag("Controller");
		GameObject[] dead =  GameObject.FindGameObjectsWithTag("Dead");
		GameObject[] respawns = alive.Concat(dead).ToArray();
		var countDown = GetComponent<CountDownTimer>();
		foreach(GameObject o in respawns)
		{
			var t = o.GetComponent<CountDownTimer>();
			if(t != null && t.enabled == true)
			{
				countDown = o.GetComponent<CountDownTimer>();
				break;
			}
		}
			// var countDown = GetComponent<CountDownTimer>();
			// Image endScreen = countDown.EndScreen;
			// endScreen.enabled = true;
		Canvas PlayerDisplay = countDown.PlayerUI;
		// PlayerDisplay.GetC("EndScreen").GameObject.SetActive(true);
		PlayerDisplay.transform.GetChild(1).gameObject.SetActive(true);

		// StartCoroutine(DelayAction(3f));

		Debug.Log("Seeker Dub Called");

		Text endScreenTxt = countDown.endscreenText;
		endScreenTxt.text = " Hiders Won";
	}

	[PunRPC]
	void seekerDub (int playersLeft)
	{
		
		if(playersLeft == 0)
        {
			GameObject[] alive =  GameObject.FindGameObjectsWithTag("Controller");
			GameObject[] dead =  GameObject.FindGameObjectsWithTag("Dead");
			GameObject[] respawns = alive.Concat(dead).ToArray();
			var countDown = GetComponent<CountDownTimer>();
			foreach(GameObject o in respawns)
			{
				var t = o.GetComponent<CountDownTimer>();
				if(t != null && t.enabled == true)
				{
					countDown = o.GetComponent<CountDownTimer>();
					break;
				}
			}
			// var countDown = GetComponent<CountDownTimer>();
			// Image endScreen = countDown.EndScreen;
			// endScreen.enabled = true;
			Canvas PlayerDisplay = countDown.PlayerUI;
			// PlayerDisplay.GetC("EndScreen").GameObject.SetActive(true);
			PlayerDisplay.transform.GetChild(1).gameObject.SetActive(true);

			

			Debug.Log("Seeker Dub Called");

			Text endScreenTxt = countDown.endscreenText;
			endScreenTxt.text = " Seeker Won";
		}
	}


	[PunRPC]
	void DisconnectFromGame()
	{
		StartCoroutine(DisconnectAndLoad());
	}

	IEnumerator DelayAction(float delayTime)
	{
		//Wait for the specified delay time before continuing.
		yield return new WaitForSeconds(delayTime);

		//Do the action after the delay time has finished.
		// LeaveGame();
		DisconnectAndLoad();
	}

	IEnumerator DisconnectAndLoad()
	{
		yield return new WaitForSeconds(5f);
		PhotonNetwork.LeaveRoom();
		while(PhotonNetwork.InRoom)
		{
			yield return null;
		}
		
		
		// PhotonNetwork.LoadLevel(0);
		SceneManager.LoadScene(0);
		base.OnLeftRoom();
		Destroy(GameObject.Find("VoiceManager"));
		
		
		// SceneManager.LoadScene("Lobby");
	}

	IEnumerator waiter(Text endScreenTxt)
	{
		//Wait for 4 seconds
		yield return new WaitForSeconds(5f);
		endScreenTxt.text = "";
	}




	void Start()
	{
		activeGame = true;

		if(PV.IsMine)
		{
			// EquipItem(0);
			// GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			
			// Debug.Log("all paths: " + scenePaths);
			
			var spawnPosition = new Vector3(-2267, 592, 365);
			if(PhotonNetwork.IsMasterClient)
			{
				spawnPosition = new Vector3(-2076, 592, 191);
				var timer = PhotonView.Find(193).gameObject.GetComponent<Timer>();
				timer.enabled = true;
				GameObject voiceManager = GameObject.Find("VoiceManager");
				// var voiceNet = voiceManager.GetComponent("Photon Voice Network");
				// voiceNet.enabled = false;
				voiceManager.SetActive(false);
				gameObject.AddComponent<FindClosestPlayer>();
			}

			GameObject cube;
			if(PhotonNetwork.IsMasterClient)
            {
				cube = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Avatar3"), spawnPosition, Quaternion.identity);
				cube.transform.localScale = new Vector3(5, 5, 5);

			}
			else
            {
				cube = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Avatar4"), spawnPosition, Quaternion.identity);
				cube.transform.localScale = new Vector3(3, 3, 3);

			}

			reticle = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
			
			
			targetObject = cube;
			
			PV.RPC("morphPlayer", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID);

			// bool isMaster = false;

			// if(PhotonNetwork.IsMasterClient)
			// {
			// 	PV.RPC("changeColor", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID, true);
			// }
			// else
			// {
			// 	PV.RPC("changeColor", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID, false);
				
			// }

			// numKills = 0;
			var movementScript = cube.AddComponent<MovementScript>();
			movementScript.enabled = true;
			
			movementScript.cam = GetComponentInChildren<Camera>().gameObject;

			if(PhotonNetwork.IsMasterClient)
			{
				PV.RPC("setTag", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID, "Seeker");
				movementScript.speed = 50f;
			}
			else
			{
				PV.RPC("setTag", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID, "Player");
				movementScript.speed = 20f;
			}
			

			int parentOffset = 0;
			
		}
		else
		{
			var t = GetComponent<CountDownTimer>();
			t.enabled = false;
			Destroy(GetComponentInChildren<Camera>().gameObject);
			
			// Destroy(rb);
			// Destroy(ui);
		}
	}



void Update()
	{
		if(!PV.IsMine)
			return;

		if(GetComponent<CountDownTimer>().timeRemaining <= 0 && activeGame){

			GameObject[] playersLeft =  GameObject.FindGameObjectsWithTag("Player");
			PV.RPC("hiderDub", RpcTarget.All, playersLeft.Length);
			if(PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(RoomManager.Instance.gameObject);
			}
			
							
							// PhotonNetwork.Destroy(GameObject.Find("VoiceManager"));
			// Destroy(RoomManager.Instance.gameObject);
			PV.RPC("DisconnectFromGame", RpcTarget.All);
			activeGame = false;
		}
		if(Input.GetButtonDown("Fire1") && targetObject.tag.Equals("Player"))
		{
			GameObject selected = GvrPointerInputModule.CurrentRaycastResult.gameObject;
			var oldPosition = new Vector3 (targetObject.transform.position.x, 
        	selected.transform.position.y, targetObject.transform.position.z);
			PhotonNetwork.Destroy(targetObject);

			GameObject newAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Avatar4"), oldPosition, selected.transform.rotation);
			targetObject = newAvatar;
			targetObject.transform.localScale = new Vector3(3, 3, 3);

			PV.RPC("morphBack", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID);
			var movementScript = targetObject.AddComponent<MovementScript>();
				movementScript.enabled = true;
				movementScript.speed = 20f;
				movementScript.cam = GetComponentInChildren<Camera>().gameObject;
			PV.RPC("setTag", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID, "Player");
		}


		if(Input.GetButtonDown("Submit") && targetObject.tag.Equals("Player"))
		{
			GameObject selected = GvrPointerInputModule.CurrentRaycastResult.gameObject;
			Debug.Log("Child height: " + (selected.transform.GetComponent<Renderer>().bounds.center.y - 588)*2);
			
			if(selected.transform.parent != null)
            {
               	selected = selected.transform.parent.gameObject;
				Debug.Log("Parent name: " + selected.name);
           		selected.AddComponent<MeshRenderer>();
				Debug.Log("Parent height1: " + (selected.transform.GetComponent<Renderer>().bounds.center.y - 588)*2);
				parentOffset = 3;
			}
			else
			{
				parentOffset = 0;
			}


			if(selected.tag.Equals("Prop"))
			{
				var oldPosition = new Vector3 (targetObject.transform.position.x, 
                selected.transform.position.y + 5  , targetObject.transform.position.z);


				PhotonNetwork.Destroy(targetObject);

				

				GameObject newAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", selected.name), oldPosition, selected.transform.rotation);
				targetObject = newAvatar;
				

				PV.RPC("setTag", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID, "Player");
				PV.RPC("setMeshRenderer", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID);
				PV.RPC("morphPlayer", RpcTarget.All, targetObject.GetComponent<PhotonView>().ViewID);
				
				var movementScript = targetObject.AddComponent<MovementScript>();
				movementScript.enabled = true;
				movementScript.speed = 20f;
				movementScript.cam = GetComponentInChildren<Camera>().gameObject;

				
				
			}
		}

		if(Input.GetButtonDown("Submit") && targetObject.tag.Equals("Seeker"))
		{
			GameObject selected = GvrPointerInputModule.CurrentRaycastResult.gameObject;
			

			if(selected != null)
			{
				Debug.Log("Killing: " + GvrPointerInputModule.CurrentRaycastResult.gameObject.name);
				if(selected.transform.parent != null)
				{
					selected = selected.transform.parent.gameObject;
					Debug.Log("Parent name: " + selected.name);
				}
				if (selected.tag.Equals("Player"))
				{
				// int playerID = selected.GetComponent<PhotonView>().ViewID;
					PV.RPC("killPlayer", RpcTarget.All, selected.GetComponent<PhotonView>().ViewID);
					PV.RPC("Setting", RpcTarget.All, numKills);

					GameObject[] playersLeft =  GameObject.FindGameObjectsWithTag("Player");
					PV.RPC("seekerDub", RpcTarget.All, playersLeft.Length);
					if(playersLeft.Length == 0)
					{
						PhotonNetwork.Destroy(RoomManager.Instance.gameObject);
							
							// PhotonNetwork.Destroy(GameObject.Find("VoiceManager"));

							PV.RPC("DisconnectFromGame", RpcTarget.All);
					}
				}
			
				coolDownTime = PhotonNetwork.Time;
				reticle.SetActive(false);
				
					print(PhotonNetwork.PlayerList[0].ToString());
					
				
			}
			
		}
		Vector3 newPosition = new Vector3(0, (targetObject.transform.GetComponent<Renderer>().bounds.center.y - 588) + parentOffset, 0)
                                + targetObject.transform.GetComponent<Renderer>().bounds.center;
        transform.position = newPosition;
		
		if(PhotonNetwork.Time > coolDownTime + 2)
		{
			reticle.SetActive(true);
		}
		

	}

	

	void Move()
	{
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * walkSpeed, ref smoothMoveVelocity, smoothTime);
		// moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
	}

	

	void FixedUpdate()
	{
		if(!PV.IsMine)
			return;

		// rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	void LateUpdate()
	{
		if(!PV.IsMine)
			return;

		
		// rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	public GameObject getTargetObject()
	{
		return targetObject;
	}

	public void LeaveGame()
	{
		PhotonNetwork.LeaveRoom();
		// while(PhotonNetwork.InRoom)
		// 	yield return null;
		
		// SceneManager.LoadScene("Lobby");
	}

	

	public void OnLeftRoom()
	{
		PhotonNetwork.LoadLevel(0);
	}


	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		
		GameObject[] respawns =  GameObject.FindGameObjectsWithTag("Player");
		int actorNr = otherPlayer.ActorNumber;
		int viewId = actorNr * PhotonNetwork.MAX_VIEW_IDS + 2;
		Debug.Log("View ID: " + viewId);
		GameObject deadPlayer = PhotonView.Find(viewId).gameObject;
		

		var playerScript = deadPlayer.GetComponent<PlayerController>();
		deadPlayer = playerScript.getTargetObject();
		Debug.Log("deadplayer name: " + deadPlayer.name);
		if(deadPlayer.gameObject.tag.Equals("Seeker"))
		{
			Debug.Log("Seeker left");
			if(PV.IsMine)
			{
				DisconnectFromGame();
			}
			
				return;
		}

		if(!deadPlayer.gameObject.tag.Equals("Player"))
		{
			Debug.Log("Dead player left");
			
			return;
		}
		var timerScript = gameObject.GetComponent<CountDownTimer>();
		
				if(timerScript.enabled == true)
				{
					Text hiderCount = timerScript.hiderText;
					if(respawns.Length == 0)
					{
						hiderCount.text = 0 + " hiders left";
						PV.RPC("seekerDub", RpcTarget.All, 0);
						PhotonNetwork.Destroy(RoomManager.Instance.gameObject);
						
						// PhotonNetwork.Destroy(GameObject.Find("VoiceManager"));
						PV.RPC("DisconnectFromGame", RpcTarget.All);
						
						
					}
					else
					{
						hiderCount.text = respawns.Length-1 + " hiders left";
						PV.RPC("seekerDub", RpcTarget.All, respawns.Length-1);
						if(respawns.Length-1 == 0)
						{
							PhotonNetwork.Destroy(RoomManager.Instance.gameObject);
							
							// PhotonNetwork.Destroy(GameObject.Find("VoiceManager"));

							PV.RPC("DisconnectFromGame", RpcTarget.All);
						}
					
					}
					
				}
			

		
		
	}
}