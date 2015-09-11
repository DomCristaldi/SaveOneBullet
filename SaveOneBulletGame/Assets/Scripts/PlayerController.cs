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
    AdvancedMotor motor;

    //USER-ASSIGNED
    public Collider bodyCollider;

    //DEFAULT SETTINGS
    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode moveLeft = KeyCode.A;

    public KeyCode sneakKey = KeyCode.LeftShift;

    public float aimSensitivity = 1.0f;

    public Vector3 inputDirec = Vector3.zero;

    public MovementMode curMovementMode;

    void Awake() {
        
        motor = GetComponent<AdvancedMotor>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        RecieveInput();

        SendMotorInput();
	}

    private void SendMotorInput() {
        if (curMovementMode == MovementMode.walking) {//WALK
            motor.InputDirec(inputDirec * motor.walkSpeed);
        }
        else if (curMovementMode == MovementMode.running) {//RUN
            motor.InputDirec(inputDirec * motor.runSpeed);
        }
        else if (curMovementMode == MovementMode.sneaking) {//SNEAK
            motor.InputDirec(inputDirec * motor.sneakSpeed);
        }
    }

    private void RecieveInput() {
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

        inputDirec = new Vector3(horMovement, 0.0f, vertMovement);
    }
}
