using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Vector3 movement = Vector3.zero;
    public bool leftMouseButtonDown = false;
    public Vector3 mousePos = new Vector3(0, 0, 0);
    public float scrollWheel = 0;

    void Start()
    {
        
    }

    void Update()
    {
        leftMouseButtonDown = Input.GetMouseButtonDown(0);
        mousePos = Input.mousePosition;
        scrollWheel = Input.GetAxis("Mouse ScrollWheel") * (-1);

        if (Input.GetKeyDown("a"))
        {
            movement.x = -1;
        }
        if (Input.GetKeyUp("a"))
        {
            movement.x = 0;
        }

        if (Input.GetKeyDown("d"))
        {
            movement.x = 1;
        }
        if (Input.GetKeyUp("d"))
        {
            movement.x = 0;
        }

        if (Input.GetKeyDown("w"))
        {
            movement.z = 1;
        }
        if (Input.GetKeyUp("w"))
        {
            movement.z = 0;
        }

        if (Input.GetKeyDown("s"))
        {
            movement.z = -1;
        }
        if (Input.GetKeyUp("s"))
        {
            movement.z = 0;
        }
    }
}
