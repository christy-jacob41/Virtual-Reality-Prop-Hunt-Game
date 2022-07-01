using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 500f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -98.1f;
    public GameObject cam;

    private Vector3 moveDirection = Vector3.zero;

    public bool groundedPlayer;

    private bool isRotating = false;

    public Vector3 player;
    // Start is called before the first frame update

    Rigidbody m_Rigidbody;
    void Start()
    {
        // controller = GetComponent<CharacterController>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.AddForce(transform.up * -9.8f);
    }

    void checkBounds()
    {
        if (transform.position.y < 587)
        {
            transform.position = new Vector3(transform.position.x, 590, transform.position.z);
        }
        if(transform.position.z > 576)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 573);
        }
        if (transform.position.z < 188)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 191);
        }
        if (transform.position.x > -1899)
        {
            transform.position = new Vector3(-1902, transform.position.y, transform.position.z);
        }
       
    }
    // Update is called once per frame
    void Update()
    {
        // groundedPlayer = controller.isGrounded;

        
        //if (groundedPlayer && player.y < -20f)
        if (player.y < -20f)
        {

            player.y = -20f;
        }
        

        // if(transform.position.y > 50)
        // {

        //     transform.position = new Vector3(transform.position.x, 100.0f, transform.position.z);
        //     Debug.Log(transform.position.y);
        // }

        // float x = Input.GetAxis("Horizontal");
        // float z = Input.GetAxis("Vertical");
        /*
        Debug.Log(controller.transform.position.y + " : " + cam.transform.position.y);

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move.y = 0;
        move = cam.transform.TransformDirection(move);
        move.y = 0;
        controller.Move(move);
        */
        
        Vector3 v = transform.localPosition;
        
        moveDirection=new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0,Input.GetAxis("Vertical") * Time.deltaTime * speed);
        var rotationV = cam.transform.rotation;
        // rotationV.x = 0;
        Vector3 rotatedDirection = rotationV * moveDirection *1f;

        v.x = v.x + (Input.GetAxis("Horizontal") * Time.deltaTime * 0.01f);

        v.z = v.z + (Input.GetAxis ("Vertical") * Time.deltaTime * 0.01f);

        rotatedDirection.y = 0;
        transform.position = transform.position + rotatedDirection;
        // move = cam.transform.TransformDirection(transform.position);
        // if (isRotating) //Check if your game object is currently rotating
        //     SetRotate(this.gameObject, cam);

        // if (transform.rotation.eulerAngles.y != cam.transform.rotation.eulerAngles.y)
        // {
        //     isRotating = !isRotating;
        // }
        m_Rigidbody.AddForce(transform.up * -9.8f);

        if(Input.GetButtonDown("Fire2") ){
                
            m_Rigidbody.AddForce(Vector3.up * 15, ForceMode.Impulse);

        }
        
        // moveDirection=new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        // moveDirection = cam.transform.TransformDirection(moveDirection);
        // moveDirection *= speed;
        // moveDirection.y = 0.0f;
        // controller.Move(moveDirection * Time.deltaTime);
        //controller.Move(move);

        // // if (move == Vector3.zero)
        // // {
        // //     transform.forward = move;
        // // }

        // player.y += gravityValue * Time.deltaTime;
        // controller.Move(player * Time.deltaTime);

        // if(transform.position.y < -10)
        // {
        //     // transform.position = new Vector3(0, 0, 15);
        // }

        if(Input.GetButton("Fire3") ){
                
            transform.Rotate(0, 25 *Time.deltaTime, 0);

        }

        checkBounds();
    }

    void SetRotate(GameObject toRotate, GameObject camera)
    {
        //You can call this function for any game object and any camera, just change the parameters when you call this function
        transform.rotation = Quaternion.Lerp(toRotate.transform.rotation, camera.transform.rotation, speed * Time.deltaTime);
    }
}

