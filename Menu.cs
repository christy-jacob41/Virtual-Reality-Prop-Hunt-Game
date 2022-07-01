using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;


public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;
    public GameObject startBtn;

    public void Open(){
        open = true;
        gameObject.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        var standardInput = GameObject.Find("GvrEventSystem").GetComponent<StandaloneInputModule>();
        standardInput.ActivateModule();
        EventSystem.current.SetSelectedGameObject(startBtn);
    }

    public void Close(){
        open = false;
        gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        
        if ((Input.GetButtonDown("Fire3")))
        {
           // Debug.Log(EventSystem.current.currentSelectedGameObject);

        //    var button1 = resumeOrQuit.GetComponent<Button>();
        //  button1.onClick.Invoke();

        }
        

    }
}
