using UnityEngine;
using System.Collections;

public class ViewController : MonoBehaviour {

    Transform tf;
    public Camera cam;
    Transform camTF;

    public float aimSensitivity = 3.0f;

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

            Debug.Log(value);
            return value;
        }
    }

    void Awake() {
        tf = GetComponent<Transform>();
        camTF = cam.transform;//index the camera transform for readability and faster access
    }

	// Use this for initialsization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        HandleCam();
	}

    private void HandleCam() {
        //Debug.Log(deltaAimAngle_X);
        //Debug.Log(deltaAimAngle_Y);

        //Debug.DrawRay(tf.position, deltaAimPos_X, Color.red);
        Debug.DrawRay(tf.position, deltaAimPos_Y, Color.green);
        //Debug.DrawRay(tf.position, Vector3.back * 10.0f);

        //HORIZONTAL
        Quaternion newHorRot = GetNewHorCamRot(deltaAimAngle_X);
        tf.rotation = Quaternion.Slerp(tf.rotation, newHorRot, Time.deltaTime * aimSensitivity);

        //VERTICAL
        //if (Mathf.Sign(deltaAimDirec.y) > 0.0f && Vector3.Angle(camTF.forward, Vector3.forward) < 90.0f)
        Quaternion newVertRot = GetNewVertCamRot(deltaAimAngle_Y);
        camTF.rotation = Quaternion.Slerp(camTF.rotation, newVertRot, Time.deltaTime * aimSensitivity);

    }

    
    private Quaternion GetNewHorCamRot(float angle) {
        //tf.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
        return tf.rotation * Quaternion.AngleAxis(angle, Vector3.up);
    }


    private Quaternion GetNewVertCamRot(float angle) {
        /*
        Quaternion deltaRot = Quaternion.AngleAxis(angle, camTF.right);
        
        if ((camTF.rotation * deltaRot).eulerAngles.y > 90.0f
            || (camTF.rotation * deltaRot).eulerAngles.y < -90.0f) 
        {
            camTF.rotation *= deltaRot;
        }
        */

        //Debug.Log("angle: " + angle);
        //camTF.rotation *= Quaternion.AngleAxis(angle, Vector3.right);
        return camTF.rotation * Quaternion.AngleAxis(angle, Vector3.right);
    }

    public void InputDeltaView(Vector2 deltaView) {
        deltaAimDirec = deltaView;
    }
}
