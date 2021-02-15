using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float jumpSpeed = 8.0f;
    [SerializeField]
    private float walkingSpeed = 1f;
    [SerializeField]
    private float runingSpeed = 1.5f;
    [SerializeField]
    private float crouchSpeed = 0.5f;
    [SerializeField]
    private float gravity = 20.00f;
    [SerializeField]
    private float momentumLimit = 10f;
    
    public float groundDistance = 0.4f;
    public Camera playerCamera;


    public CharacterController controller;
    CameraMovement cameraScript;

    public bool isFlyng { get { return !isGrounded; } }
    public bool isGrounded;
    public bool isMoving;
    public bool isRuning;

    public float deslizTime = 300;
    public bool timerIsRunning = false;

    public float momentum = 5f;

    Vector3 moveDirection = Vector3.zero;
    Vector3 jump = Vector3.zero;
    

    void Start()
    {
        cameraScript = playerCamera.GetComponent<CameraMovement>();
        controller = GetComponent<CharacterController>();
    }

    
    /*void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Activate(1.0f);
        }
        //Long Activate
        if (Input.GetButtonDown("Fire2"))
        {
            Activate(8.0f);
        }
    }*/



    void Activate(float distancia)
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, distancia))
        {
            Debug.Log(hit);
            Activador activador = hit.transform.GetComponent<Activador>();
            if (activador != null)
            {
                Debug.Log(activador);
                activador.activar();
            }
        }
    }

    public void Move(Vector2 input, bool sprint, bool crouching, bool crouch)
    {
        isGrounded = controller.isGrounded;

       

        input = calcularVelocidad(input, sprint, crouching, isMoving, crouch);

        moveDirection = new Vector3(input.x, moveDirection.y, input.y);
        moveDirection = transform.TransformDirection(moveDirection);
        if (isGrounded)
        {
            moveDirection.y = -1;
            UpdateJump();
        }

        moveDirection.y -= gravity * Time.deltaTime;
        var flags = controller.Move(moveDirection * Time.deltaTime);
        var collideSide = (flags & CollisionFlags.CollidedSides) != 0;

        if (collideSide)
        {
            isGrounded = true;
        }

        if (moveDirection.x != 0 && moveDirection.y != 0) 
        { 
            isMoving = true; 
        } else 
        { 
            isMoving = false; 
        }
    }

    public void Jump(Vector3 direction, float mult)
    {
        jump = direction * mult;
    }

    public void UpdateJump()
    {
        if (jump != Vector3.zero)
        {
            Debug.Log("el salto es diferente a 0 asi que saltare");
            Vector3 dir = (jump * jumpSpeed);
            if (dir.x != 0) moveDirection.x = dir.x;
            if (dir.y != 0) moveDirection.y = dir.y;
            if (dir.z != 0) moveDirection.z = dir.z;
        }
        jump = Vector3.zero;
    }

    Vector2 calcularMomentum(Vector2 input,bool sprint,bool crouch)
    {
        if (isFlyng)
        {
            if (momentum <= momentumLimit)
            {
                momentum *= 1.001f;
            }
            input *= momentum;
        }
        else
        {
            if (momentum > 1)
            {
                momentum *= 0.995f;
            }
            else
            {
                momentum = 1;
            }
            if (sprint)
            {
                if (momentum < runingSpeed)
                {
                    input *= runingSpeed;
                    momentum = runingSpeed;
                }
                else
                {
                    input *= momentum;
                }
            }
            else if (crouch)
            {
                input *= crouchSpeed;
            }
            else
            {
                if (momentum < walkingSpeed)
                {
                    input *= walkingSpeed;
                    momentum = walkingSpeed;
                }
                else
                {
                    input *= momentum;
                }
            }
        }
        return input;
    }

    Vector2 calcularVelocidad(Vector2 input, bool sprint, bool crouching,bool isMoving, bool crouch)
    {
        if (momentum < 5 && !crouching)
        {
            momentum = 5f;
        }
        if (isFlyng)//no esta entrando aca porque primero entra al ismoving, modificar
        {
            //aumentar porque esta volando
            if (momentum < momentumLimit)
            {
                incrementarMomentum();
            }
        }
        else if (isMoving)
        {
            if (momentum < runingSpeed)
            {
                //incrementarMomento porque esta corriendo
                incrementarMomentum();
            }
            else
            {
                //disminuirMomento
                disminuirMomentum();
            }
        }
        else
        {
            //Disminuir porque esta quieto
            if(momentum > walkingSpeed)
            {
                disminuirMomentum();
            }
        }
        if (crouching && momentum < 6f && isGrounded)
        {
            Crouch();
            momentum = crouchSpeed;
        }
        else if(crouch && momentum >= 6f && isGrounded && !timerIsRunning)
        {
            Debug.Log("masterCAca");
            StartCoroutine("Countdown");
            //agregar un timer para cortar el desliz
        }else if (!crouching)
        {
            unCrouch();
        }
        return input *= momentum;
    }

    void incrementarMomentum()
    {
        momentum *= 1.001f;
    }

    void disminuirMomentum()
    {
        momentum *= 0.995f;
    }

    void deslizar()
    {
        timerIsRunning = true;
        do
        {
            momentum = 10f;
            if (deslizTime > 0)
            {
                Debug.Log("caca1");
                deslizTime -= Time.deltaTime;
                Crouch();
            }
            else
            {
                Debug.Log("Time has run out!");
                deslizTime = 2f;
                timerIsRunning = false;
                unCrouch();
            }
        } while (timerIsRunning);
    }

    private IEnumerator Countdown()
    {
        float duration = 1.5f; 
        float normalizedTime = 0;
        while (normalizedTime <= 2f)
        {
            if(normalizedTime <= 1){
                timerIsRunning = true;
                momentum = 10f;
                Crouch();
            }
            else
            {
                unCrouch();
            }
            normalizedTime += Time.deltaTime / deslizTime;
            yield return null;
        }
        timerIsRunning = false;
    }

    void Crouch()
    {
        controller.height = 1f;
        isGrounded = true;
        cameraScript.cameraCrouch();
    }

    void unCrouch()
    {
        controller.height = 2f;
        cameraScript.cameraUnCrouch();
    }

    /*void Movimiento()
    {
        isGrounded = controller.isGrounded;
        isRuning = Input.GetKey(KeyCode.LeftShift);
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        //Movimiento
        Vector3 movement = transform.right * horizontalInput + transform.forward * verticalInput;
        movement = Vector3.ClampMagnitude(movement, 1f);
        //Correr
        if (isRuning && isGrounded)
        {
            movement *= runingSpeed;
        }
        Vector3 movementFinal = movement + velocity;
        var flags = controller.Move(movementFinal * speed * Time.deltaTime);

        var collideSide = (flags & CollisionFlags.CollidedSides) != 0;
        //jump
        if (Input.GetButtonDown("Jump"))
        {
            if (collideSide)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
                //controller.Move(movement * speed * Time.deltaTime * -1);
                cameraScript.moveCamera180();
            }
            else if (!isGrounded)
            {
                if (jetPackUsos > 0)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity * jetPackForce);
                    jetPackUsos -= 1;
                }
            }
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            }
        }

        //Gravedad
        //Si esta en el suelo lo clavo bien fuerte al suelo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //aplico gravedad
        velocity.y += gravity * Time.deltaTime;
        //Colision con algo de arriba
        var collidedUp = (flags & CollisionFlags.CollidedAbove) != 0;
        if (collidedUp && velocity.y > 0)
        {
            velocity.y = 0;
        }
    }*/

}
