using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Menu[] menus;

    void Awake(){
        Instance = this;
    }

    public void OpenMenu(string menuName){
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName){
                menus[i].Open();
            }
            else if(menus[i].open){
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu){
        for(int i = 0; i < menus.Length; i++){
            if(menus[i].open){
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu){
        menu.Close();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
