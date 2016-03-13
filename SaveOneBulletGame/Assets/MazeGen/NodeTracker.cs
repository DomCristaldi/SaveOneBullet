using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/MazeGen/NodeTracker")]
public class NodeTracker : MonoBehaviour {
    
	public MazeNode closestNode;
	bool tracking;
	public bool isEnemy;

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
			UpdateClosestNode();
			//Debug.DrawLine(transform.position, closestNode.transform.position, Color.red); //***DEBUG***
		}
	}

	void UpdateClosestNode () {
		MazeNode newClosestNode = MazeController.singleton.ClosestNodeToPositon(
            transform.position);
		if (newClosestNode == null) {
			Debug.LogError("NodeTracker cannot find closest node!");
			return;
		}
		if (newClosestNode == closestNode) {
			return;
		}
		if (isEnemy) {
			if (closestNode != null) {
				closestNode.enemyOccupied = false;
				if (MazeController.singleton.debugColors) {
					closestNode.floorRenderer.material = closestNode.floorMat;
				}
			}
			newClosestNode.enemyOccupied = true;
			if (MazeController.singleton.debugColors) {
				newClosestNode.floorRenderer.material = MazeController.singleton.debugRed;
			}
		}
		else {
			if (MazeController.singleton.debugColors) {
				if (closestNode != null) {
					closestNode.floorRenderer.material = closestNode.floorMat;
				}
				newClosestNode.floorRenderer.material = MazeController.singleton.debugBlue;
			}
		}
		closestNode = newClosestNode;
	}
}
