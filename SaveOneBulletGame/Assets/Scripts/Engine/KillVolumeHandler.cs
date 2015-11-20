using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class KillVolumeHandler : MonoBehaviour {

    Collider killVolume;

    void OnTriggerEnter(Collider other) {
        //if (other.GetComponent<PlayerController>() != null) {
        if (other.transform.root.tag == "Player") {

            Application.LoadLevel(0);
        }
        
    }

    void Awake() {
        killVolume = GetComponent<BoxCollider>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
