using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Movimiento))]
public class Cerebro : MonoBehaviour
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
    [SerializeField]
    private float slideTime = 2;
    [SerializeField]
    private float wallRunTime = 1;


    PlayerInput playerInput;
    Movimiento movimiento;
    public Camera playerCamera;
    public CharacterController controller;
    CameraMovement cameraScript;
    public GameObject selectedGraffiti;
    
    private bool isFlyng { get { return !isGrounded; } }
    private bool isGrounded;
    private bool isMoving;
    private bool isRuning;
    private bool timerSlideIsRunning = false;
    private bool timerWallRunIsRunning = false;
    private float velocity = 5f;
    private bool isSideWalled;
    private bool isFrontWalled;
    private bool isWallRunJumping;
    private bool haveFrontJump;

    RaycastHit leftHit;
    RaycastHit rightHit;

    Vector3 wallRunDirection = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector3 jumpDirection = Vector3.zero;

    StatusEnum status;

    private Texture2D t_static_tx = null;
    private Texture2D t_dynamic_tx = null;
    private WWW t_load = null;

    void OnGUI()
    {
        
        if (t_load == null)
        {
            Debug.Log("Application data path: " + Application.dataPath);
            string targetFile = "file://" + Application.dataPath + "/Resources/graffitiTexture.png";
            Debug.Log("Beginning load at time: " + Time.time);
            t_load = new WWW(targetFile);
        }
        else if (t_load.isDone && t_dynamic_tx == null)
        {
            Debug.Log("File has finished being loaded at " + Time.time);
            t_dynamic_tx = new Texture2D(64, 64);
            Debug.Log("Preparing to load PNG into Texture");
            t_load.LoadImageIntoTexture(t_dynamic_tx);
            Debug.Log("Loaded image into texture");
        }
        Material graffitiMaterial = selectedGraffiti.GetComponent<Renderer>().sharedMaterial;
        graffitiMaterial.SetTexture("_EmissionMap", t_dynamic_tx);
    }
    private void Start()
    {
        status = StatusEnum.Unmoved;
        playerInput = GetComponent<PlayerInput>();
        movimiento = GetComponent<Movimiento>();
        cameraScript = playerCamera.GetComponent<CameraMovement>();

        OnGUI();
    }
    private void Update()
    {
        if (playerInput.changeSceneToDraw)
        {
            SceneManager.LoadScene("MakeGraffiti");
        }
        if (playerInput.graffiting)
        {
            makeGraffiti();
        }
        Vector2 inputMovement = playerInput.input;
        bool isJumping = playerInput.Jump;
        bool isCrouching = playerInput.crouching;

        checkBooleanVariables(inputMovement);
        checkForWall();
        applyGravity();
        checkVelocity();

        //Debug.Log(inputMovement);
        if (inputMovement != Vector2.zero || isGrounded)
        {
            move(inputMovement);
        }
        if (isJumping)
        {
            checkJump();
        }
        else if (isCrouching && isGrounded)
        {
            if (isRuning && status != StatusEnum.Slide && status != StatusEnum.Crouch && !timerSlideIsRunning)
            {
                Debug.Log("empezare a deslizarme");
                slide();
            }
            else if(!isRuning)
            {
                crouch();
            }
        }
        else if (!isCrouching && status == StatusEnum.Crouch)
        {
            unCrouch();
        }
        movimiento.Move(moveDirection,controller);
    }

    void move(Vector2 input)
    {
        input *= velocity;
        if (status != StatusEnum.Slide && status != StatusEnum.WallRun)
        {
            moveDirection = new Vector3(input.x,moveDirection.y,input.y);
            moveDirection = transform.TransformDirection(moveDirection);
        }
        else if (status == StatusEnum.Slide)
        {
            moveDirection = new Vector3(input.x / 5, moveDirection.y, input.y);
            moveDirection = transform.TransformDirection(moveDirection);
        }
        else if (status == StatusEnum.WallRun)
        {
            moveDirection = new Vector3(wallRunDirection.z * velocity, moveDirection.y, wallRunDirection.x * velocity * -1);
        }
    }
    void checkJump()
    {
        if(status == StatusEnum.WallRun)
        {
            isWallRunJumping = true;
            jump(new Vector3(0, 1, 0));
        }
        else if(isSideWalled && !isGrounded && isMoving)
        {
            wallRun();
        }
        else if(isFrontWalled && !isGrounded && !isSideWalled && haveFrontJump)
        {
            haveFrontJump = false;
            moveDirection.y = 0;
            jump(new Vector3(0, 1, 0));
        }
        else if(status == StatusEnum.Slide)
        {
            status = StatusEnum.Unmoved;
            jump(new Vector3(0, 1, 0));
        }
        else if (isGrounded)
        {
            jump(new Vector3(0,1,0));
        }
        else
        {
            //jetpack
        }
    }   
    void crouch()
    {
        status = StatusEnum.Crouch;
        controller.height = 1f;
        isGrounded = true;
        cameraScript.cameraCrouch();
    }
    void unCrouch()
    {
        status = StatusEnum.Unmoved;
        controller.height = 2f;
        cameraScript.cameraUnCrouch();
    }
    void slide()
    {
        status = StatusEnum.Slide;
        StartCoroutine("timerSlide");
        controller.height = 1f;
        isGrounded = true;
        cameraScript.cameraCrouch();
    }
    void unSlide()
    {
        status = StatusEnum.Unmoved;
        controller.height = 2f;
        isGrounded = true;
        cameraScript.cameraUnCrouch();
    }
    private IEnumerator timerSlide()
    {
        Debug.Log("Entre al timer Slide");
        timerSlideIsRunning = true;
        float normalizedTime = 0;
        while (normalizedTime <= 2f)
        {
            if ((status != StatusEnum.Slide || !isRuning) && normalizedTime <= 1f)
            {
                unSlide();
                normalizedTime = 1;
                Debug.Log("Deje de slidear por nueva accion");
            }
            if (normalizedTime >= 1f && status == StatusEnum.Slide)
            {
                unSlide();
                Debug.Log("Deje de slidear");
            }
            normalizedTime += Time.deltaTime / slideTime;
            yield return null;
        }
        timerSlideIsRunning = false;
        Debug.Log("Me fui del timer");
    }
    void wallRun()
    {
        status = StatusEnum.WallRun;
        StartCoroutine("timerWallRun");
        if(rightHit.normal != Vector3.zero)
        {
            wallRunDirection = rightHit.normal;
        }
        else
        {
            wallRunDirection = leftHit.normal * -1;
        }
    }
    void unWallRun()
    {
        status = StatusEnum.Unmoved;
        isWallRunJumping = false;
    }
    private IEnumerator timerWallRun()
    {
        Debug.Log("Entre al timer WallRun");
        timerWallRunIsRunning = true;
        float normalizedTime = 0;
        while (normalizedTime <= 1f && isSideWalled && isMoving && !isWallRunJumping)
        {
            denyGravity();
            normalizedTime += Time.deltaTime / wallRunTime;
            yield return null;
        }
        timerWallRunIsRunning = false;
        unWallRun();
        Debug.Log("Me fui del timer WallRun");
    }
    void jump(Vector3 direction)
    {
        jumpDirection = direction * jumpSpeed;
        if (jumpDirection.x != 0) moveDirection.x += jumpDirection.x;
        if (jumpDirection.y != 0) moveDirection.y += jumpDirection.y;
        if (jumpDirection.z != 0) moveDirection.z += jumpDirection.z;
        jumpDirection = Vector3.zero;
    }
    void jetPack()
    {

    }
    void addVelocity()
    {
        if(velocity < momentumLimit)
        {
            velocity *= 1.001f;
        }
    }
    void substractVelocity()
    {
        if(velocity > 5)
        {
            velocity *= 0.995f;
        }
    }
    void checkVelocity()
    {
        if(isMoving && velocity < 7)
        {
            addVelocity();
        }
        else if (!isGrounded)
        {
            addVelocity();
        }
        else if (status == StatusEnum.Slide && isMoving)
        {
            addVelocity();
        }
        else
        {
            substractVelocity();
        }
    }
    void checkBooleanVariables(Vector2 inputMovement)
    {
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            haveFrontJump = true;
        }
        if (inputMovement != Vector2.zero)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        if(velocity >= 6.8f)
        {
            isRuning = true;
        }
        else
        {
            isRuning = false;
        }
    }
    void applyGravity()
    {
        if (isGrounded)
        {
            moveDirection.y = -1;
        }

        moveDirection.y -= gravity * Time.deltaTime;
    }
    void denyGravity()
    {
        //como deniego la gravedad cuando salta, no esta pudiendo saltar en un wallrun
        moveDirection.y = 0;
    }
    void checkForWall()
    {
        var frontWalledRay = transform.position;
        frontWalledRay.y -= 0.5f;
        bool rightWalled = Physics.Raycast(transform.position, transform.TransformVector(Vector3.right), out rightHit, 1f);
        bool leftWalled = Physics.Raycast(transform.position, transform.TransformVector(Vector3.left),out leftHit, 1f);
        bool frontWalled = Physics.Raycast(frontWalledRay, transform.TransformVector(Vector3.forward), 1f);
        if (rightWalled || leftWalled)
        {
            isSideWalled = true;
        }
        else
        {
            isSideWalled = false;
        }
        if (frontWalled)
        {
            isFrontWalled = true;
        }
        else
        {
            isFrontWalled = false;
        }
    }

    private void makeGraffiti()
    {
        //Este codigo es una poronga, pero no hay tiempo para mejorarlo, dios esos ifff malisimos
        RaycastHit hit;
        bool isHitting = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f);
        if (isHitting && hit.transform.gameObject.layer == LayerMask.NameToLayer("GraffitiWallBig"))
        {
            Vector3 asd = new Vector3(0,0,1);
            var finalPosition = hit.transform.position + asd;
            var rotation = hit.transform.rotation * Quaternion.Euler(0,-90,0);
            GameObject graffiti = Instantiate(selectedGraffiti, hit.transform.position, rotation);//Quaternion.Slerp(hit.transform.rotation,rotation,Time.deltaTime));
            graffiti.transform.localScale += new Vector3(4,2.8f,0);
        }else if(isHitting && hit.transform.gameObject.layer == LayerMask.NameToLayer("GraffitiWallMedium"))
        {
            Vector3 asd = new Vector3(0, 0, 1);
            var finalPosition = hit.transform.position + asd;
            var rotation = hit.transform.rotation * Quaternion.Euler(0, -90, 0);
            GameObject graffiti = Instantiate(selectedGraffiti, hit.transform.position, rotation);//Quaternion.Slerp(hit.transform.rotation,rotation,Time.deltaTime));
            graffiti.transform.localScale += new Vector3(2.2f, 1f, 0);
        }
        else if (isHitting && hit.transform.gameObject.layer == LayerMask.NameToLayer("GraffitiWallSmall"))
        {
            Vector3 asd = new Vector3(0, 0, 1);
            var finalPosition = hit.transform.position + asd;
            var rotation = hit.transform.rotation * Quaternion.Euler(0, -90, 0);
            GameObject graffiti = Instantiate(selectedGraffiti, hit.transform.position, rotation);//Quaternion.Slerp(hit.transform.rotation,rotation,Time.deltaTime));
            //graffiti.transform.localScale += new Vector3(1, 1, 0);
        }
    }
}
