using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Motor : MonoBehaviour {

    Transform tf;
    Rigidbody rigbod;

    public Vector3 desiredDirec;
    public Vector3 trueDirec;

    //properties to quickly access endpoint data of direction vectors
    public Vector3 desiredPoint {
        get {
            return tf.position + desiredDirec;
        }
    }
    public Vector3 truePoint {
        get {
            return tf.position + trueDirec;
        }
    }

    [Header("Speed")]
    public float maxDesiredDirecMag = 2.0f;

    public float moveSpeedModifier = 1.0f;



    protected virtual void Awake() {
        tf = GetComponent<Transform>();
        rigbod = GetComponent<Rigidbody>();
    }

	// Use this for initialization
	protected virtual void Start () {
	
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	    HandleMovement();
	}
    
    protected virtual void FixedUpdate() {
        ApplyMovementFixed();
    }
    
//HANDLE ANY MODIFICATIONS THE MOTOR HAS TO MAKE TO MOVEMENT
    protected void HandleMovement() {
        trueDirec = desiredDirec;

        
        //rigbod.velocity = new Vector3(trueDirec.x, rigbod.velocity.y, trueDirec.z);
    }

//APPLY THE MOVEMENT TO THE RIGID BODY
    protected void ApplyMovementFixed() {//only do in FixedUpdate()
        rigbod.velocity = new Vector3(trueDirec.x, rigbod.velocity.y, trueDirec.z);
    }

    public virtual void InputDirec(Vector3 direc) {
        desiredDirec = Vector3.ClampMagnitude(direc, maxDesiredDirecMag);
    }

    public virtual void InputPos (Vector3 pos) {
        InputDirec(pos - tf.position);
    }
}

