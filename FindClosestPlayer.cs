using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class FindClosestPlayer : MonoBehaviour
{
    double waitTime;
    double mainTime;
    Text temperatureText;
    GameObject temperature;
    public static float Squared(float a){ 
       return (a * a); 
    }

    public string HotOrCold(float a){ 
        // temperatureText.text = HotOrCold(lowestDistance);
         if(a < 50)
        {
            temperatureText.text = "Very Hot";
            temperatureText.color = new Color(1.0f, 0, 0);
            return "Very Hot";
        }
        else if(a < 100)
        {
            temperatureText.text = "Hot";
            temperatureText.color = new Color(1.0f, 0.64f, 0);
            return "Hot";
        }
        else if(a < 200)
        {
            temperatureText.text = "Cold";
            temperatureText.color = new Color(0.53f, 0.81f, 0.92f);
            return "Cold";
        }
        else
        {
            temperatureText.text = "Very Cold";
            temperatureText.color = new Color(0, 0, 1.0f);
            return "Very Cold"; 
        }
    } 

    // Start is called before the first frame update
    void Start()
    {
        waitTime = PhotonNetwork.Time;
        mainTime = PhotonNetwork.Time;
        // GameObject temperature = transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(true);  
        temperature = transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(2).gameObject;
        temperature.SetActive(true);
        temperatureText = temperature.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.Time > mainTime + 30){
            mainTime = PhotonNetwork.Time;
            // Debug.Log("Time Reset");
        }
        if(PhotonNetwork.Time < mainTime + 20){
            temperature.SetActive(false);
            // Debug.Log("HiderHEAT Disabled");
        }else{
            temperature.SetActive(true);
            // Debug.Log("HiderHEAT Enabled");
            if(PhotonNetwork.Time > waitTime + 1){
            int count = 0;
            float masterx = 0;
            float mastery = 0;
            float masterz = 0;
            float lowestDistance = float.MaxValue;
            int highestIndex = 0;

            GameObject[] respawns =  GameObject.FindGameObjectsWithTag("Controller");
			var countDown = GetComponent<CountDownTimer>();

            masterx = gameObject.transform.position.x;
            mastery = gameObject.transform.position.y;
            masterz = gameObject.transform.position.z;
            foreach(GameObject o in respawns)
            {
                count = count + 1;
                var t = o.GetComponent<CountDownTimer>();
                if(t != null && t.enabled == true)
                {
                    countDown = o.GetComponent<CountDownTimer>();
                    continue;
                }
                Debug.Log("Player pos: " + o.transform.position);
                
                    
                
                float x = o.transform.position.x;
                float y = o.transform.position.y;
                float z = o.transform.position.z;

                // float currentDistance = Squared(x -masterx) + Squared(y -mastery) + Squared(z -masterz);
                float currentDistance =  Vector3.Distance(o.transform.position, gameObject.transform.position);
                if(currentDistance < lowestDistance){
                    lowestDistance = currentDistance;
                    highestIndex = count;
                }
            }
            // count = 0;
            // foreach(GameObject o in respawns)
            // {
            //     count = count + 1;
            //     if(count == 1){
            //         masterx = o.transform.position.x;
            //         mastery = o.transform.position.y;
            //         masterz = o.transform.position.z;
            //     }
            //     if(count == highestIndex){
            //         float x = o.transform.position.x;
            //         float y = o.transform.position.y;
            //         float z = o.transform.position.z;
            //         Debug.Log("Position of Closet Person" + o.transform.position);
            //         float currentDistance1 = Squared(x -masterx) + Squared(y -mastery) + Squared(z -masterz);
            //         Debug.Log("Distance of Closet Person" + currentDistance1);
            //     }
            // }
            // foreach(GameObject o in respawns)
            // {
            //     count = count + 1;
            //     if(count == highestIndex){
            //         float x = o.transform.position.x;
            //         float y = o.transform.position.y;
            //         float z = o.transform.position.z;
            //         Debug.Log("Position of Closet Person" + o.transform.position);
            //         float currentDistance1 = Squared(x -masterx) + Squared(y -mastery) + Squared(z -masterz);
            //         Debug.Log("Distance of Closet Person" + currentDistance1);
            //     }
            // }
            // GameObject temporaryNearest = respawns[highestIndex - 1];
            // Debug.Log("Position of Closet Person" + temporaryNearest.transform.position);
            Debug.Log("Distance of Closet Person" + lowestDistance);
            Debug.Log("Temperature of Closet Person" + HotOrCold(lowestDistance));
            
            // foreach(GameObject o in respawns)
            // {
            //     count = count + 1;
            //     if(count == highestIndex){
            //         float x = o.transform.position.x;
            //         float y = o.transform.position.y;
            //         float z = o.transform.position.z;
            //         Debug.Log("Position of Closet Person" + o.transform.position);
            //         float currentDistance1 = Squared(x -masterx) + Squared(y -mastery) + Squared(z -masterz);
            //         Debug.Log("Distance of Closet Person" + currentDistance1);
            //     }
            // }
            waitTime = PhotonNetwork.Time;
        }
        }

    }
}
