using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeNode : MonoBehaviour {
    
	public enum Direction {
		center = 0,
		up = 1,
		right = 2,
		down = -1,
		left = -2
	}

	public int DirectionToIndex (Direction direction) {
		return (int)direction + 2;
	}

	public Direction InverseDirection (Direction direction) {
		return (Direction)(-(int)direction);
	}

	public List<MazeNode> connections;
	public List<MazeNode> currentConnections;
	public List<MazeLink> links;

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
    }
    
	void Start () {
        
	}
	
	void Update () {
        
	}

	public void AddConnection (Direction direction, MazeNode node) {
		connections[DirectionToIndex(direction)] = node;
		node.connections[DirectionToIndex(InverseDirection(direction))] = this;
		//***DEBUG***
		AddCurrentConnections(direction, node);
	}

	public void AddCurrentConnections (Direction direction, MazeNode node) {
		currentConnections[DirectionToIndex(direction)] = node;
		node.currentConnections[DirectionToIndex(InverseDirection(direction))] = this;
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
			}
			else {
				links[i].wall.SetActive(false);
			}
		}
	}
}
