using UnityEngine;
using System.Collections;

public class GameCrashCollider : MonoBehaviour {
    
    void Awake () {
        
    }
    
	void Start () {
        
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.collider.gameObject.layer == 10) {
			Application.Quit();
		}
	}
}
