using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AdvancedMotor))]
[RequireComponent(typeof(ViewController))]
public class PlayerController : MonoBehaviour {

    public enum MovementMode {
        walking,
        running,
        sneaking,
    }

    //GET COMPONENT
    Transform tf;

    AdvancedMotor motor;
    ViewController viewControl;

    public bool canMove = true;
    public bool canLook = true;
    public bool usingMouse = true;

    //USER-ASSIGNED
    public Collider bodyCollider;

    //DEFAULT SETTINGS
    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode moveLeft = KeyCode.A;

    public KeyCode sneakKey = KeyCode.LeftShift;

    public float aimSensitivity = 1.0f;

    public Vector3 moveDirec = Vector3.zero;
    public Vector3 aimDirec = Vector2.zero;

    public MovementMode curMovementMode;

    void Awake() {
        tf = GetComponent<Transform>();

        motor = GetComponent<AdvancedMotor>();
        viewControl = GetComponent<ViewController>();
    }

	// Use this for initialization
	void Start () {
        //Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
        RecieveInput();

        SendMotorInput();

        //Debug.Log(Input.GetAxis("Mouse X"));
        //Debug.Log(Input.mousePosition);
	}

    private void SendMotorInput() {
        if (curMovementMode == MovementMode.walking) {//WALK
            motor.InputDirec(moveDirec * motor.walkSpeed);
        }
        else if (curMovementMode == MovementMode.running) {//RUN
            motor.InputDirec(moveDirec * motor.runSpeed);
        }
        else if (curMovementMode == MovementMode.sneaking) {//SNEAK
            motor.InputDirec(moveDirec * motor.sneakSpeed);
        }
    }
    
    private void RecieveInput() {
        if (canMove) {
            RecieveMoveInput();
        }
        if (canLook) {
            RecieveViewInput();
        }
    }

    private void RecieveMoveInput() {
        //NOTE: put jumping in here if we want it (y value for inputDirec)

        //allow for additions to 0 so movement can cancel itself out
        float vertMovement = 0.0f;
        float horMovement = 0.0f;

        if (Input.GetKey(moveForward)) {
            vertMovement += 1.0f;
        }
        else if (Input.GetKey(moveBackward)) {
            vertMovement += -1.0f;
        }

        if (Input.GetKey(moveRight)) {
            horMovement += 1.0f;
        }
        if (Input.GetKey(moveLeft)) {
            horMovement += -1.0f;
        }

        moveDirec = tf.rotation * (new Vector3(horMovement, 0.0f, vertMovement)).normalized;
    }

    private void RecieveViewInput() {
        if (usingMouse) {
            viewControl.InputDeltaView(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

            //viewControl.InputDeltaView(new Vector2(Input.GetAxis("Mouse X"), 0.0f));
            //viewControl.InputDeltaView(new Vector2(0.0f, Input.GetAxis("Mouse Y")));
        }
    }
}
