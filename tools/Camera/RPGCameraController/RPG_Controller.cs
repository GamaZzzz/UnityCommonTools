using UnityEngine;
using System.Collections;

public class RPG_Controller : MonoBehaviour {

    public static RPG_Controller instance;

    public CharacterController characterController;
    public float walkSpeed = 10f;
    public float turnSpeed = 2.5f;
    public float jumpHeight = 10f;
    public float gravity = 20f;
    public float fallingThreshold = -6f; // -6f gets the character beeing almost always grounded

    private Vector3 playerDir;
    private Vector3 playerDirWorld;
    private Vector3 rotation = Vector3.zero;


    void Awake() {
        instance = this;
        characterController = GetComponent("CharacterController") as CharacterController;

        RPG_Camera.CameraSetup();
    }

	
	void Update () {
        if (Camera.main == null)
            return;

        if (characterController == null) {
            Debug.Log("Error: No Character Controller component found! Please add one to the GameObject which has this script attached.");
            return;
        }

        GetInput();

        StartMotor();
	}


    void GetInput() {
        
        //MovementKeys():
        
        float horizontalStrafe = 0f;
        float vertical = 0f;

        if (Input.GetButton("Horizontal"))
            horizontalStrafe = Input.GetAxis("Horizontal") < 0 ? -1f : Input.GetAxis("Horizontal") > 0 ? 1f : 0f;

        if (Input.GetButton("Vertical"))
            vertical = Input.GetAxis("Vertical") < 0 ? -1f : Input.GetAxis("Vertical") > 0 ? 1f : 0f;

        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
            vertical = 1f;
            
        playerDir = horizontalStrafe * Vector3.right + vertical * Vector3.forward;
        if (RPG_Animation.instance != null)
            RPG_Animation.instance.SetCurrentMoveDir(playerDir);

        if (characterController.isGrounded) {    
            playerDirWorld = transform.TransformDirection(playerDir);
            
            if (Mathf.Abs(playerDir.x) + Mathf.Abs(playerDir.z) > 1)
                playerDirWorld.Normalize();
            
            playerDirWorld *= walkSpeed;
            playerDirWorld.y = fallingThreshold;
            
            if (Input.GetButtonDown("Jump")) {
                playerDirWorld.y = jumpHeight;
                if (RPG_Animation.instance != null)
                    RPG_Animation.instance.Jump(); // the pattern for calling animations is always the same: just add some lines under line 77 and write an if statement which
            }                                      // checks for an arbitrary key if it is pressed and, if true, calls "RPG_Animation.instance.YourAnimation()". After that you add
        }                                          // this method to the other animation clip methods in "RPG_Animation" (do not forget to make it public) 

        rotation.y = Input.GetAxis("Horizontal") * turnSpeed;
    }


    void StartMotor() {
        playerDirWorld.y -= gravity * Time.deltaTime;
        characterController.Move(playerDirWorld * Time.deltaTime);

        transform.Rotate(rotation);
        if (!Input.GetMouseButton(0))
            RPG_Camera.instance.RotateWithCharacter();
    }
}
