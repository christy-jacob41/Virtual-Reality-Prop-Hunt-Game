using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    public GameObject targetObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Vector3 newPosition = targetObject.transform.position + new Vector3(0, targetObject.transform.lossyScale.y + 10f, 0)
        //                         + targetObject.transform.GetComponent<Renderer>().bounds.center;

        Vector3 newPosition = new Vector3(0, (targetObject.transform.GetComponent<Renderer>().bounds.center.y - 588) + 3, 0)
                                + targetObject.transform.GetComponent<Renderer>().bounds.center;
        transform.position = newPosition;

    }
}
