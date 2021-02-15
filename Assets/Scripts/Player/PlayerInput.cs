using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 input
    {
        get
        {
            Vector2 moveInput = Vector2.zero;
            moveInput.y = Input.GetAxis("Vertical");
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput = Vector2.ClampMagnitude(moveInput, 1f);
            return moveInput;
        }
    }

    public bool run
    {
        get { return false; }
        //get { return Input.GetKey(KeyCode.LeftShift); }
    }

    public bool changeSceneToDraw
    {
        get { return Input.GetKeyDown(KeyCode.Tab); }
    }

    public bool graffiting
    {
        get { return Input.GetKeyDown(KeyCode.T); }
    }

    public bool crouch
    {
        get { return Input.GetKeyDown(KeyCode.LeftControl); }
    }

    public bool crouching
    {
        get { return Input.GetKey(KeyCode.LeftControl); }
    }

    public KeyCode interactKey
    {
        get { return KeyCode.E; }
    }

    public bool interact
    {
        get { return Input.GetKeyDown(interactKey); }
    }

    public bool Jump
    {
        get { return Input.GetButtonDown("Jump"); }
    }

}
