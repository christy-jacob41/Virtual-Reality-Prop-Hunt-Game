    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
   public float speed =100000.0f;
   private Vector3 moveDirection = Vector3.zero;
   private CharacterController controller;

    void Start()
    {
        controller=GetComponent<CharacterController>();  
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection=new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection *= speed;
        moveDirection.y = 0.0f;
        controller.Move(moveDirection * Time.deltaTime);
    }
}