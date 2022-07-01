using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineIt : MonoBehaviour
{
    // Start is called before the first frame update
    public bool selected;
    GameObject currentSelected;
    void Start()
    {
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(selected && Input.GetButtonDown("Submit"))
        {
            // GameObject followScript = GameObject.Find("FollowObject");
            var fscript = this.GetComponent<FollowCharacter>();
            Debug.Log("Y");
            selected = false;
            
            GameObject oldObject = fscript.targetObject;
            GameObject newAvatar = GameObject.Instantiate (currentSelected);
            fscript.targetObject =  newAvatar;
            Debug.Log("Old:" + oldObject.transform.position.x + ", " + oldObject.transform.position.y 
                        + ", " + oldObject.transform.position.z);
            var oldPosition = new Vector3 (oldObject.transform.position.x, 
                currentSelected.transform.position.y + 5  , oldObject.transform.position.z);
            if(oldObject.name.Equals("Cube"))
            {
                oldObject.SetActive(false);
            }
            else
            {
                Destroy(oldObject);
            }
            newAvatar.transform.position = oldPosition;
            var newLocalScale = currentSelected.transform.localScale;
            if(currentSelected.transform.parent != null)
            {
                GameObject parentOriginal = currentSelected.transform.parent.gameObject;
                newLocalScale = parentOriginal.transform.localScale;
            }
            
            
            // if(parentOriginal != null)
            // {
            //     newLocalScale = parentOriginal.transform.localScale;
            // }
            newAvatar.transform.localScale = newLocalScale;
            Debug.Log("New: " + newAvatar.transform.position.x + ", " + newAvatar.transform.position.y 
                        + ", " + newAvatar.transform.position.z);

            // Destroy(newAvatar.GetComponent<Outline>());
            // Destroy(newAvatar.GetComponent<OutlineIt>());
            Destroy(newAvatar.GetComponent("EventTrigger"));
            
            // var newController = newAvatar.AddComponent<CharacterController>();
            // newController.enabled = true;
            var movementScript = newAvatar.AddComponent<MovementScript>();
            movementScript.enabled = true;
            // movementScript.controller = newController;

            var rigidBody = newAvatar.AddComponent<Rigidbody>();
            rigidBody.isKinematic = false;
            rigidBody.constraints =  RigidbodyConstraints.FreezeRotationX
                                        | RigidbodyConstraints.FreezeRotationZ;
            rigidBody.collisionDetectionMode =  CollisionDetectionMode.ContinuousSpeculative;
            // rigidBody.enabled = true;
            movementScript.cam = gameObject;

        }
    }

    public void Set()
    {
        // var outline = GetComponent<Outline>();
        // outline.enabled = true;
        selected = true;
        Debug.Log("Set");
        currentSelected = GvrPointerInputModule.CurrentRaycastResult.gameObject;
    }

    public void Reset()
    {
        selected = false;
    }
}
