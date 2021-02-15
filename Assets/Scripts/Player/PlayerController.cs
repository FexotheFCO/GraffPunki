using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    
    PlayerMovement movement;
    PlayerInput playerInput;

    Vector2 movementDirection;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
    }
    //este update era fixedupdate, el problema es que como consigue los input desde aca, empieza a tener problemas si es fixed
    private void Update()
    {
        movementDirection = playerInput.input;

        DefaultMovement();
    }

    void DefaultMovement()
    {
        if (movement.isGrounded && playerInput.Jump)
        {
            movement.Jump(new Vector3(0,1,1), 1f);
            //playerInput.ResetJump();
        }
        movement.Move(playerInput.input, playerInput.run, playerInput.crouching, playerInput.crouch);
    }

}
