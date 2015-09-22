using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Player/ViewController")]
public class ViewController : MonoBehaviour {

    Transform tf;
    public Transform camNeckTf;
    public Camera cam;
    Transform camTf;

    public float aimSensitivity = 5.0f;

    public Vector2 deltaAimDirec;//unit vector representing how far mouse moved this frame

    //used to get isolated vectors for quaternion calculation
    Vector3 deltaAimPos_X {
        get {return new Vector3(deltaAimDirec.x, 0.0f, 0.0f); }
    }
    Vector3 deltaAimPos_Y {
        get {return new Vector3(0.0f, deltaAimDirec.y, 0.0f); }
    }

    private float deltaAimAngleDampening = 1.0f;//internal to help dampen the deltaAimAngle

    float deltaAimAngle_X {
        get {
            float value = Mathf.Sign(deltaAimDirec.x) * Vector3.Angle(deltaAimPos_X - Vector3.forward, Vector3.zero - Vector3.forward * deltaAimAngleDampening);
            /*
            if (Mathf.Approximately(value, 0.0f) || value < 0.0001f) {
                return 0.0f;
            }
            */
            return value;
        }
        //get { return Mathf.Sign(deltaAimDirec.x) * Vector3.Angle(delta)
    }
    float deltaAimAngle_Y {
        get {
            float value = -Mathf.Sign(deltaAimDirec.y) * Vector3.Angle(deltaAimPos_Y - Vector3.forward, Vector3.zero - Vector3.forward * deltaAimAngleDampening);
            /*
            if (Mathf.Approximately(value, 0.0f) || value < 0.0001f) {
                Debug.Log("maybe here");

                return 0.0f;
            }
            */

            //Debug.Log(value);
            return value;
        }
    }

    void Awake() {
        tf = GetComponent<Transform>();
        camTf = cam.transform;//index the camera transform for readability and faster access
    }

	// Use this for initialsization
	void Start () {
        //Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
        HandleCam();

        //Debug.DrawRay(GetRayFromCamera().origin, GetRayFromCamera().direction * 10.0f);
	}

    private void HandleCam() {
        //Debug.Log(deltaAimAngle_X);
        //Debug.Log(deltaAimAngle_Y);

        //Debug.DrawRay(tf.position, deltaAimPos_X, Color.red);
        //Debug.DrawRay(tf.position, deltaAimPos_Y, Color.green);
        //Debug.DrawRay(tf.position, Vector3.back * 10.0f);

    //HORIZONTAL
        Quaternion newHorRot = GetNewHorCamRot(deltaAimAngle_X);
        tf.rotation = Quaternion.Slerp(tf.rotation, newHorRot, Time.deltaTime * aimSensitivity);


    //VERTICAL
        //if (Mathf.Sign(deltaAimDirec.y) > 0.0f && Vector3.Angle(camTF.forward, Vector3.forward) < 90.0f)
        Quaternion newVertRot = GetNewVertCamRot(deltaAimAngle_Y);
        Quaternion finalVertRot = Quaternion.Slerp(camNeckTf.rotation, newVertRot, Time.deltaTime * aimSensitivity);


        //Debug.DrawRay(camNeckTf.position, finalVertRot * Vector3.forward, Color.red);

        //planes for limiting player's view rotation (currently generated every frame, this is only ideal if we change the player's orientation, like if gravity changed)
        Plane forwardPlane = new Plane(tf.forward, camNeckTf.position);
        Plane upPlane = new Plane(tf.up, camNeckTf.position);

        if (!forwardPlane.GetSide((finalVertRot * Vector3.forward) + camNeckTf.position)) {
        //LIMIT UP
            if (upPlane.GetSide((finalVertRot * Vector3.forward) + camNeckTf.position)) {
                //Debug.Log("up");
                finalVertRot = tf.rotation;
                finalVertRot *= Quaternion.LookRotation(tf.up, tf.up);

            }
        //LIMIT DOWN
            else {
                //Debug.Log("down");
                finalVertRot = tf.rotation;
                finalVertRot *= Quaternion.LookRotation(-tf.up, tf.up);
            }
        }

        //Debug.DrawRay(camNeckTf.position, upPlane.normal * 2.0f, Color.blue);


        camNeckTf.rotation = finalVertRot;
    }

    
    private Quaternion GetNewHorCamRot(float angle) {
        //tf.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
        return tf.rotation * Quaternion.AngleAxis(angle, Vector3.up);
    }


    private Quaternion GetNewVertCamRot(float angle) {

        return camNeckTf.rotation * Quaternion.AngleAxis(angle, Vector3.right);

    }

    public void InputDeltaView(Vector2 deltaView) {
        deltaAimDirec = deltaView;
    }

    public Ray GetRayFromCamera() {
        return new Ray(camTf.position, camTf.forward);

        /*
        RaycastHit hit;
        
        if (hit == null) {
            Debug.Log("try this");
        }

        if (Physics.Raycast(camTf.position, camTf.forward, out hit)) {
            return hit.collider.gameObject;
        }
        else {
            return null;
        }
        */
    }
}
