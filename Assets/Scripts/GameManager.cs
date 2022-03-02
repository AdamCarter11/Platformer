using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool editMode;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && GameManager.editMode == false){
            GameManager.editMode = true;
            //print(GameManager.editMode);
        }
        else if(Input.GetKeyDown(KeyCode.E) && GameManager.editMode == true){
            GameManager.editMode = false;
        }
    }
}
