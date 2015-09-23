using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Player/PlayerController")]
[RequireComponent(typeof(AdvancedMotor))]
[RequireComponent(typeof(ViewController))]
public class PlayerController : MonoBehaviour {

    [System.Serializable]
    public class ItemKeyMapping {
        public ItemBase.ItemType itemType;
        public KeyCode inputKey;
    }

    public enum MovementMode {
        walking,
        running,
        sneaking,
    }

    //GET COMPONENT
    Transform tf;

    AdvancedMotor motor;
    ViewController viewControl;
    InventoryController invControl;

    public bool canMove = true;
    public bool canLook = true;
    public bool canUseItem = true;
    public bool usingMouse = true;

    //USER-ASSIGNED
    public Collider bodyCollider;

    //DEFAULT SETTINGS
    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode moveLeft = KeyCode.A;

    public KeyCode sneakKey = KeyCode.LeftShift;
	public KeyCode runKey = KeyCode.LeftControl;

    public KeyCode useItem = KeyCode.Mouse0;

    public ItemKeyMapping[] itemKeyList;
    public Dictionary<KeyCode, ItemKeyMapping> itemKeyMap;

    public float aimSensitivity = 1.0f;

    public Vector3 moveDirec = Vector3.zero;
    public Vector3 aimDirec = Vector2.zero;

    public MovementMode curMovementMode;

    void Awake() {
        tf = GetComponent<Transform>();

        motor = GetComponent<AdvancedMotor>();
        viewControl = GetComponent<ViewController>();
        invControl = GetComponent<InventoryController>();

        PopulateItemKeyMap();
    }

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SendInput();

        SendMotorInput();
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            invControl.EquipItem(ItemBase.ItemType.gun);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            invControl.EquipItem(ItemBase.ItemType.none);
        }
        */
        //Debug.Log(Input.GetAxis("Mouse X"));
        //Debug.Log(Input.mousePosition);
	}

    private void SendMotorInput() {

        if (Input.GetKey(sneakKey)) {
            motor.curMovementMode = AdvancedMotor.MovementMode.sneaking;
        }
		else if (Input.GetKey(runKey)) {
			motor.curMovementMode = AdvancedMotor.MovementMode.running;
		}
        else {
            motor.curMovementMode = AdvancedMotor.MovementMode.walking;
        }

        motor.InputDirec(moveDirec);

        /*
        if (curMovementMode == MovementMode.walking) {//WALK
            motor.InputDirec(moveDirec * motor.walkSpeed);
        }
        else if (curMovementMode == MovementMode.running) {//RUN
            motor.InputDirec(moveDirec * motor.runSpeed);
        }
        else if (curMovementMode == MovementMode.sneaking) {//SNEAK
            motor.InputDirec(moveDirec * motor.sneakSpeed);
        }
        */
    }
    
    private void SendInput() {
        if (canMove) {
            SendMoveInput();
        }
        if (canLook) {
            SendViewInput();
        }
        if (canUseItem) {
            SendItemInput();
        }
    }

    private void SendMoveInput() {
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

    private void SendViewInput() {
        if (usingMouse) {
            viewControl.InputDeltaView(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

            //viewControl.InputDeltaView(new Vector2(Input.GetAxis("Mouse X"), 0.0f));
            //viewControl.InputDeltaView(new Vector2(0.0f, Input.GetAxis("Mouse Y")));
        }
    }

    private void SendItemInput() {
        
        foreach (ItemKeyMapping ikm in itemKeyList) {
            if (Input.GetKeyDown(ikm.inputKey)) {

                invControl.EquipItem(ikm.itemType);

                break;
            }
        }

        /*
        foreach (KeyValuePair kvp in itemKeyMap) {

        }
        */

        if (Input.GetKeyDown(useItem)) {
            invControl.UseItem();
        }
    }

    private void PopulateItemKeyMap() {
        itemKeyMap = new Dictionary<KeyCode, ItemKeyMapping>();

        foreach (ItemKeyMapping ikm in itemKeyList) {
            itemKeyMap.Add(ikm.inputKey, ikm);
        }
    }
}
