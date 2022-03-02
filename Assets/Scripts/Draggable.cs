using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector2 diff = Vector2.zero;
    private void OnMouseDown() {
        diff = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
    }
    private void OnMouseDrag() {
        if(GameManager.editMode){
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - diff;
        }
    }
}
