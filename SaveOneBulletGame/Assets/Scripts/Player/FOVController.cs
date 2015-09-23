using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Player/FOVController")]
[RequireComponent(typeof(AdvancedMotor))]
[RequireComponent(typeof(ViewController))]
public class FOVController : MonoBehaviour {
    
	AdvancedMotor motor;
	ViewController viewController;

	public float standingFOV;
	public float runningFOV;

	public float lerpSpeed;

	float currentFOV;
	float targetFOV;

    void Awake () {
		motor = GetComponent<AdvancedMotor>();
		viewController = GetComponent<ViewController>();
    }
    
	void Start () {
        
	}
	
	void Update () {
		SetFOV();
	}

	void SetFOV () {
		targetFOV = standingFOV + (runningFOV - standingFOV) * (motor.trueDirec.magnitude / motor.runSpeed);
		currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * lerpSpeed);
		viewController.cam.fieldOfView = currentFOV;
	}
}
