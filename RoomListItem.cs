using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;



public class RoomListItem : MonoBehaviour
{
	[SerializeField] Text text;

	public RoomInfo info;

	public void SetUp(RoomInfo _info)
	{
		info = _info;
		text.text = _info.Name;
		text.enabled = true;
	}

	public void OnClick()
	{
		Launcher.Instance.JoinRoom(info);
	}
}
