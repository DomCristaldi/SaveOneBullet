using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/MazeGen/MazeNode")]
public class MazeNode : MonoBehaviour {
    
	public enum Direction {
		center = 0,
		up = 1,
		right = 2,
		down = -1,
		left = -2
	}

	public static int DirectionToIndex (Direction direction) {
		return (int)direction + 2;
	}

	public static Direction InverseDirection (Direction direction) {
		return (Direction)(-(int)direction);
	}

	public static Direction IndexToDirection (int index) {
		return (Direction)index - 2;
	}

	public List<MazeNode> connections;
	public List<MazeNode> currentConnections;
	public List<MazeLink> links;
	public MazeNode cameFrom;
	public float g;
	public float h;
	public float f;
	public bool closed;
	public bool connectedToMain;
	public float distanceToGoal;
	public bool searched;
	public MeshRenderer floorRenderer;
	public Material floorMat;
	public bool enemyOccupied;

    void Awake () {
		connections = new List<MazeNode>();
		for (int i = 0; i < 5; i++) {
			connections.Add(null);
		}
		currentConnections = new List<MazeNode>();
		for (int i = 0; i < 5; i++) {
			currentConnections.Add(null);
		}
		links = new List<MazeLink>();
		for (int i = 0; i < 5; i++) {
			links.Add(null);
		}
		ResetAStarVariables();
		distanceToGoal = Mathf.Infinity;
		floorRenderer.material = floorMat;
    }
    
	void Start () {
        
	}
	
	void Update () {
        
	}

	public void AddConnection (Direction direction, MazeNode node) {
		connections[DirectionToIndex(direction)] = node;
		node.connections[DirectionToIndex(InverseDirection(direction))] = this;
		links[DirectionToIndex(direction)].adjacentNode1 = this;
		links[DirectionToIndex(direction)].adjacentNode2 = node;
		//***DEBUG***
		//AddCurrentConnections(direction, node);
	}

	public void AddCurrentConnections (Direction direction, MazeNode node, bool isMainPath = false) {
		currentConnections[DirectionToIndex(direction)] = node;
		node.currentConnections[DirectionToIndex(InverseDirection(direction))] = this;
		links[DirectionToIndex(direction)].wall.SetActive(false);
		links[DirectionToIndex(direction)].wallCollider.gameObject.layer = 2;
		//links[DirectionToIndex(direction)].wall.transform.localPosition = Vector3.up * 3f;
		if (isMainPath) {
			links[DirectionToIndex(direction)].onMainPath = true;
			if (MazeController.singleton.debugColors) {
				links[DirectionToIndex(direction)].floorRenderer.material = MazeController.singleton.debugYellow;
			}
		}
	}

	public void RemoveCurrentConnections (Direction direction, MazeNode node) {
		currentConnections[DirectionToIndex(direction)] = null;
		node.currentConnections[DirectionToIndex(InverseDirection(direction))] = null;
		links[DirectionToIndex(direction)].wall.SetActive(true);
		links[DirectionToIndex(direction)].wallCollider.gameObject.layer = 9;
		//links[DirectionToIndex(direction)].wall.transform.localPosition = Vector3.up;
	}

	public void ConnectToNode (MazeNode node, bool isMainPath = false) {
		int connectionIndex = -1;
		for (int i = 0; i < 5; i++) {
			if (connections[i] == node) {
				connectionIndex = i;
				break;
			}
		}
		if (connectionIndex == -1) {
			Debug.LogError("Unable to connect nodes!");
				return;
		}
		AddCurrentConnections(IndexToDirection(connectionIndex), node, isMainPath);
		if (isMainPath) {
			connectedToMain = true;
			node.connectedToMain = true;
		}
	}

	public void DisconnectFromNode (MazeNode node) {
		int connectionIndex = -1;
		for (int i = 0; i < 5; i++) {
			if (connections[i] == node) {
				connectionIndex = i;
				break;
			}
		}
		if (connectionIndex == -1) {
			Debug.LogError("Unable to disconnect nodes!");
			return;
		}
		RemoveCurrentConnections(IndexToDirection(connectionIndex), node);
	}

	public void AddLink (Direction direction, MazeLink link) {
		links[DirectionToIndex(direction)] = link;
	}

	public void SolidifyWalls () {
		for (int i = 0; i < 5; i++) {
			if (i == 2) {
				continue;
			}
			if (currentConnections[i] == null) {
				links[i].wall.SetActive(true);
				links[i].wallCollider.gameObject.layer = 9;
				//links[i].wall.transform.localPosition = Vector3.up;
			}
			else {
				links[i].wall.SetActive(false);
				links[i].wallCollider.gameObject.layer = 2;
				//links[i].wall.transform.localPosition = Vector3.up * 3f;
			}
		}
	}

	public void ResetAStarVariables () {
		g = Mathf.Infinity;
		h = Mathf.Infinity;
		f = Mathf.Infinity;
		closed = false;
		cameFrom = null;
	}

	public MazeLink DetermineGoalDistanceDifferential (Direction direction) {
		links[DirectionToIndex(direction)].goalDistanceDifferential = Mathf.Abs(distanceToGoal - connections[DirectionToIndex(direction)].distanceToGoal);
		return links[DirectionToIndex(direction)];
	}
}
