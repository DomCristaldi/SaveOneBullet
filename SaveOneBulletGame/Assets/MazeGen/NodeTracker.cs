using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/MazeGen/NodeTracker")]
public class NodeTracker : MonoBehaviour {
    
	public MazeNode closestNode;
	bool tracking;

    void Awake () {
		closestNode = null;
		tracking = true;
    }
    
	void Start () {
        if (MazeController.singleton == null) {
			tracking = false;
			Debug.LogError("No MazeController can be found!");
		}
	}
	
	void Update () {
        if (tracking) {
			closestNode = MazeController.singleton.ClosestNodeToPositon(transform.position);
			Debug.DrawLine(transform.position, closestNode.transform.position, Color.red);
		}
	}
}
