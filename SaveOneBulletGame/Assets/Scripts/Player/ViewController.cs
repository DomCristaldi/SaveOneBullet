using UnityEngine;
using System.Collections;

public class ViewController : MonoBehaviour {

    Transform tf;
    public Transform camNeckTf;
    public Camera cam;
    Transform camTf;

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


        Debug.DrawRay(camNeckTf.position, finalVertRot * Vector3.forward, Color.red);

        Plane forwardPlane = new Plane(tf.forward, camNeckTf.position);
        Plane upPlane = new Plane(tf.up, camNeckTf.position);
        if (!forwardPlane.GetSide((finalVertRot * Vector3.forward) + camNeckTf.position)) {
            Debug.Log("backward");

            if (upPlane.GetSide((finalVertRot * Vector3.forward) + camNeckTf.position)) {
                Debug.Log("up");

                //Quaternion tempQuat = Quaternion.Euler(0.0f, 0.0f, camNeckTf.localRotation.y);

                //Vector3 tempVec = Vector3.ProjectOnPlane(finalVertRot * Vector3.forward, upPlane.normal);
                //Quaternion tempQuat = Quaternion.FromToRotation(tf.forward, tempVec);

                //finalVertRot
                //Quaternion tempQuat = finalVertRot;
                //tempQuat.SetEulerAngles(0.0f, -tempQuat.eulerAngles.y, 0.0f);
                finalVertRot.SetLookRotation(tf.up, tf.up);
                //finalVertRot *= tempQuat;
                //finalVertRot *= newHorRot;
            }
            else {
                Debug.Log("down");
                finalVertRot.SetLookRotation(-tf.up, tf.up);
                //finalVertRot *= newHorRot;
            }
        }

        Debug.DrawRay(camNeckTf.position, upPlane.normal * 2.0f, Color.blue);



        camNeckTf.rotation = finalVertRot;



        /*
        if (!(camNeckTf.localRotation.eulerAngles.x > 0.0f && camNeckTf.localRotation.eulerAngles.x < 90.0f)) {
            camNeckTf.localRotation.SetEulerAngles(0.0f,
                                                   camNeckTf.localRotation.eulerAngles.y,
                                                   camNeckTf.localRotation.eulerAngles.z);
        }
        */
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

        //Debug.Log(angle);

        //Debug.Log(camNeckTf.localRotation.eulerAngles.x);
        //Debug.Log(angle + camNeckTf.localRotation.eulerAngles.x);
        if (angle + camNeckTf.localRotation.eulerAngles.x < 0.0f || angle + camNeckTf.localRotation.eulerAngles.x > 90.0f) {
            //Debug.Log("asdf");
        }

        /*
        if (camNeckTf.rotation.eulerAngles.x >= 0.0f && camNeckTf.rotation.eulerAngles.x < 90.0f) {
            //Debug.Log("down");
            if (angle + camNeckTf.rotation.eulerAngles.x <= 0.0f) {
                Debug.Log("adjust");
            }
        }

        if (camNeckTf.rotation.eulerAngles.x > 270.0f && camNeckTf.rotation.eulerAngles.x <= 360.0f) {
            Debug.Log("up");
        }
        */
        //if (camNeckTf.rotation.eulerAngles.x 
                

        Quaternion newRot = camNeckTf.rotation * Quaternion.AngleAxis(angle, Vector3.right);

        //Debug.Log(newRot.eulerAngles.x);

        //Vector3 newRotVec = newRot * tf.forward;

        
        //float clampedX = Mathf.Clamp(newRot.eulerAngles.x, -90.0f, 90.0f);
        //Debug.Log(clampedX);
        //Debug.Log(newRot.eulerAngles.x);
        //newRot.SetEulerAngles(clampedX, newRot.eulerAngles.y, newRot.eulerAngles.z);

        //Debug.Log(newRot.eulerAngles);

        /*
        if (Vector3.Angle(Vector3.forward, newRot * Vector3.forward) > 90.0f) {
            if (Vector3.Angle(Vector3.up, newRot * Vector3.forward) < Vector3.Angle(-Vector3.up, newRot * Vector3.forward)) {
                //Debug.Log("up");
                //newRot = new Quaternion();
                //newRot.SetLookRotation(Vector3.up);
                return camNeckTf.rotation;
            }
            else {
                //Debug.Log("down");
                //newRot = new Quaternion();
                //newRot.SetLookRotation(Vector3.up);
                return camNeckTf.rotation;
            }
        }
        */
        return newRot;


        //Debug.Log("angle: " + angle);
        //camTF.rotation *= Quaternion.AngleAxis(angle, Vector3.right);
        //return camTF.rotation * Quaternion.AngleAxis(angle, Vector3.right);
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
