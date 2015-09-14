using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeController : MonoBehaviour {

	public static MazeController singleton;
	[Header("Prefabs:")]
	public GameObject wall;
	public GameObject floorLink;
	public GameObject floorNode;
	[Header("Dimensions (# of nodes):")]
	public int nodeWidth;
	public int nodeHeight;
	[Header("Actual Dimensions (calculated at runtime):")]
	public int mazeWidth;
	public int mazeHeight;
	public List<List<GameObject>> mazePieces;
	public List<List<MazeNode>> mazeNodes;
	public MazeNode exitNode;
	public MazeLink exit;
	public MazeNode startNode;
	public List<MazeNode> path;
	public List<MazeNode> unconnected;
	List<MazeNode> distanceChecked;
	[Header("Maze Generation Parms (for informed designers only):")]
	public int offshootStopChance;
	public int wallRemovalsGoal;
	public int wallRemovalsStart;
	public int aStarPasses;
	public AStarMode aStarMode;
	public float aStarRandom;

	void Awake () {
		if (singleton == null) {
			singleton = this;
		}
		mazeWidth = nodeWidth * 2 + 1;
		mazeHeight = nodeHeight * 2 + 1;
	}

	void Start () {
		CreateMaze();
	}

	void Update () {
		
	}

	void InitializeMazePieces () {
		mazePieces = new List<List<GameObject>>();
		for (int x = 0; x < mazeWidth; x++) {
			mazePieces.Add(new List<GameObject>());
		}
		mazeNodes = new List<List<MazeNode>>();
		for (int x = 0; x < nodeWidth; x++) {
			mazeNodes.Add(new List<MazeNode>());
		}
		path = new List<MazeNode>();
		unconnected = new List<MazeNode>();
	}

	void CreateMaze () {
		InitializeMazePieces();
		for (int x = 0; x < mazeWidth; x++) {
			for (int z = 0; z < mazeHeight; z++) {
				if (x % 2 == 0 && z % 2 == 0) {
					mazePieces[x].Add(CreatePiece(x, z, wall));
				}
				else if (x % 2 == 1 && z % 2 == 1) {
					mazePieces[x].Add(CreatePiece(x, z, floorNode));
				}
				else {
					mazePieces[x].Add(CreatePiece(x, z, floorLink));
				}
			}
		}
		ConnectNodes();
		SetExit();
		FindGoldenPath();
		CarveOtherPaths(offshootStopChance);
		ConnectStragglers();
		CreateLoops(wallRemovalsGoal, wallRemovalsStart);
		SolidifyWalls();
	}

	void FindGoldenPath () {
		path = AStar(startNode, exitNode, AStarMode.randomManhattan, 5000f, true);
		foreach (MazeNode node in path) {
			if (node != exitNode && node != startNode) {
				node.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
			}
		}
		unconnected.Remove(path[0]);
		for (int i = 1; i < path.Count; i++) {
			path[i].ConnectToNode(path[i-1], true);
			unconnected.Remove(path[i]);
		}
	}

	int GetRandomNonCenter (int tries) {
		int rand = 0;
		for (int i = 0; i < tries; i++) {
			rand = Random.Range(0,4);
			if (rand != 2) {
				break;
			}
		}
		return rand;
	}

	void CarveOtherPaths (int stopChance) {
		foreach (MazeNode mainNode in path) {
			CarvePath(mainNode, stopChance);
		}
	}

	void CarvePath (MazeNode start, int stopChance, bool connectStragglers = false) {
		if (start.connectedToMain && connectStragglers) {
			return;
		}
		List<MazeNode> nodes = new List<MazeNode>(start.connections);
		List<MazeNode> randNodes = new List<MazeNode>();
		while (nodes.Count > 0) {
			int rand = Random.Range(0, nodes.Count - 1);
			randNodes.Add(nodes[rand]);
			nodes.RemoveAt(rand);
		}
		foreach (MazeNode node in randNodes) {
			if (node == null) {
				continue;
			}
			if (node.connectedToMain && !connectStragglers) {
				continue;
			}
			if (Random.Range(0, stopChance) == 0 && !connectStragglers) {
				return;
			}
			node.ConnectToNode(start);
			if (!connectStragglers) {
				node.connectedToMain = true;
				unconnected.Remove(node);
			}
			CarvePath(node, stopChance, connectStragglers);
			if (connectStragglers) {
				start.connectedToMain = true;
				unconnected.Remove(start);
				return;
			}
		}
	}

	void ConnectStragglers (int maxStragglers = 100) {
		int stragglers = 0;
		while (unconnected.Count > 0 && stragglers < maxStragglers) {
			int rand = Random.Range(0, unconnected.Count - 1);
			if (unconnected[rand].connectedToMain) {
				unconnected.RemoveAt(rand);
				continue;
			}
			CarvePath(unconnected[rand], 0, true);
			stragglers++;
		}
	}

	void ConnectNodes () {
		int i = 0;
		for (int x = 1; x < mazeWidth; x += 2) {
			int k = 0;
			for (int z = 1; z < mazeHeight; z += 2) {
				MazeNode thisNode = mazePieces[x][z].GetComponent<MazeNode>();
				mazeNodes[i].Add(thisNode);
				unconnected.Add(thisNode);
				thisNode.AddLink(MazeNode.Direction.up, mazePieces[x][z-1].GetComponent<MazeLink>());
				thisNode.AddLink(MazeNode.Direction.down, mazePieces[x][z+1].GetComponent<MazeLink>());
				thisNode.AddLink(MazeNode.Direction.left, mazePieces[x-1][z].GetComponent<MazeLink>());
				thisNode.AddLink(MazeNode.Direction.right, mazePieces[x+1][z].GetComponent<MazeLink>());
				if (i > 0) {
					thisNode.AddConnection(MazeNode.Direction.left, mazeNodes[i-1][k]);
				}
				if (k > 0) {
					thisNode.AddConnection(MazeNode.Direction.up, mazeNodes[i][k-1]);
				}
				k++;
			}
			i++;
		}
	}

	void SetExit () {
		int x;
		int z;
		MazeNode.Direction d;
		if (Random.value >= .5f) {
			//Use Z Axis
			z = Random.Range(0, nodeHeight - 1);
			if (Random.value >= .5f) {
				//Left
				x = 0;
				d = MazeNode.Direction.left;
			}
			else {
				//Right
				x = nodeWidth - 1;
				d = MazeNode.Direction.right;
			}
		}
		else {
			//Use X Axis
			x = Random.Range(0, nodeWidth - 1);
			if (Random.value >= .5f) {
				//Top
				z = 0;
				d = MazeNode.Direction.up;
			}
			else {
				//Bottom
				z = nodeHeight - 1;
				d = MazeNode.Direction.down;
			}
		}
		exitNode = mazeNodes[x][z];
		exitNode.GetComponentInChildren<MeshRenderer>().material.color = Color.green; //***DEBUG***
		exit = exitNode.links[MazeNode.DirectionToIndex(d)];
		exit.wall.GetComponent<MeshRenderer>().material.color = Color.blue; //***DEBUG***
		startNode = mazeNodes[nodeWidth - 1 - x][nodeHeight - 1 - z];
		startNode.GetComponentInChildren<MeshRenderer>().material.color = Color.red; //***DEBUG***
	}

	void SolidifyWalls () {
		for (int x = 0; x < nodeWidth; x++) {
			for (int z = 0; z < nodeHeight; z++) {
				mazeNodes[x][z].SolidifyWalls();
			}
		}
	}

	GameObject CreatePiece (int x, int z, GameObject piecePrefab) {
		GameObject newPiece = Instantiate(piecePrefab, IndexToPosition(x, z), Quaternion.identity) as GameObject;
		newPiece.transform.parent = gameObject.transform;
		return newPiece;
	}

	public Vector3 IndexToPosition (int x, int z) {
		return new Vector3(IndexToX(x), 0f, IndexToZ(z)) + transform.position;
	}

	public float IndexToX (int x) {
		return (float)x - (float)(mazeWidth / 2);
	}

	public float IndexToZ (int z) {
		return (float)z - (float)(mazeHeight / 2);
	}

	public enum AStarMode {
		manhattan,
		euclidian,
		randomManhattan,
		randomEuclidian,
	}

	public List<MazeNode> AStar (MazeNode start, MazeNode end, AStarMode mode = AStarMode.manhattan, float rand = 5f, bool ignoreWalls = false) {
		List<MazeNode> path = new List<MazeNode>();
		List<MazeNode> closed = new List<MazeNode>();
		List<MazeNode> open = new List<MazeNode>();
		open.Add(start);
		start.g = 0f;
		while (open.Count > 0) {
			MazeNode bestNode = open[0];
			foreach (MazeNode node in open) {
				if (node.f < bestNode.f) {
					bestNode = node;
				}
			}
			if (bestNode == end) {
				break;
			}
			if (ignoreWalls) {
				foreach (MazeNode node in bestNode.connections) {
					if (node == null) {
						continue;
					}
					if (node.closed) {
						continue;
					}
					open.Add(node);
					float newG = bestNode.g + GetHeuristic(bestNode, node, AStarMode.manhattan, 0f);
					if (node.g > newG) {
						node.g = newG;
						node.h = GetHeuristic(node, end, mode, rand);
						node.f = node.g + node.h;
						node.cameFrom = bestNode;
					}
				}
			}
			else {
				foreach (MazeNode node in bestNode.currentConnections) {
					if (node == null) {
						continue;
					}
					if (node.closed) {
						continue;
					}
					open.Add(node);
					float newG = bestNode.g + GetHeuristic(bestNode, node, AStarMode.manhattan, 0f);
					if (node.g > newG) {
						node.g = newG;
						node.h = GetHeuristic(node, end, mode, rand);
						node.f = node.g + node.h;
						node.cameFrom = bestNode;
					}
				}
			}
			closed.Add(bestNode);
			bestNode.closed = true;
			open.Remove(bestNode);
		}
		//Determine path
		path.Add(end);
		MazeNode next = end;
		while (next != start) {
			next = next.cameFrom;
			path.Add(next);
		}
		path.Reverse();
		//Reset came from
		foreach(MazeNode node in closed) {
			node.ResetAStarVariables();
		}
		foreach(MazeNode node in open) {
			node.ResetAStarVariables();
		}
		return path;
	}

	public float GetHeuristic (MazeNode nodeStart, MazeNode nodeEnd, AStarMode mode, float rand = 0f) {
		Vector2 start = new Vector2(nodeStart.transform.position.x, nodeStart.transform.position.z);
		Vector2 end = new Vector2(nodeEnd.transform.position.x, nodeEnd.transform.position.z);
		if (mode == AStarMode.euclidian) {
			return Vector2.Distance(start, end);
		}
		if (mode == AStarMode.manhattan) {
			return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
		}
		if (mode == AStarMode.randomEuclidian) {
			return Vector2.Distance(start, end) + Random.Range(0f, rand);
		}
		if (mode == AStarMode.randomManhattan) {
			return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y) + Random.Range(0f, rand);
		}
		else {
			Debug.LogWarning("Unspecified Heuristic!");
			return 0f;
		}
	}

	void CreateLoops (int goalPasses, int startPasses) {
		for (int i = 0; i < startPasses; i++) {
			DetermineDistancesFromGoal(startNode);
			MazeLink highestDiffLink = DetermineGoalDistanceDifferentials();
			//Debug.DrawLine(highestDiffLink.transform.position, new Vector3(0f, 20f, 0f), Color.black);
			highestDiffLink.adjacentNode1.ConnectToNode(highestDiffLink.adjacentNode2);
			ResetGoalDistances();
			//Debug.Break();
			for (int j = 0; j < aStarPasses; j++) {
				List<MazeNode> newPath = AStar(startNode, exitNode, aStarMode, aStarRandom);
				foreach (MazeNode node in newPath) {
					if (node != exitNode && node != startNode) {
						node.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
					}
				}
			}
		}
		for (int i = 0; i < goalPasses; i++) {
			DetermineDistancesFromGoal(exitNode);
			MazeLink highestDiffLink = DetermineGoalDistanceDifferentials();
			//Debug.DrawLine(highestDiffLink.transform.position, new Vector3(0f, 20f, 0f), Color.black);
			highestDiffLink.adjacentNode1.ConnectToNode(highestDiffLink.adjacentNode2);
			ResetGoalDistances();
			//Debug.Break();
			for (int j = 0; j < aStarPasses; j++) {
				List<MazeNode> newPath = AStar(startNode, exitNode, aStarMode, aStarRandom);
				foreach (MazeNode node in newPath) {
					if (node != exitNode && node != startNode) {
						node.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
					}
				}
			}
		}
	}

	void ResetGoalDistances () {
		for (int x = 0; x < nodeWidth; x++) {
			for (int z = 0; z < nodeHeight; z++) {
				mazeNodes[x][z].distanceToGoal = Mathf.Infinity;
			}
		}
	}

	MazeLink DetermineGoalDistanceDifferentials () {
		MazeLink highestDiffLink = null;
		float highestDiff = 0f;
		for (int x = 0; x < nodeWidth; x++) {
			for (int z = 0; z < nodeHeight; z++) {
				if (x > 0) {
					MazeLink thisLink = mazeNodes[x][z].DetermineGoalDistanceDifferential(MazeNode.Direction.left);
					if (thisLink != null && thisLink.goalDistanceDifferential > highestDiff) {
						highestDiff = thisLink.goalDistanceDifferential;
						highestDiffLink = thisLink;
					}
				}
				if (z > 0) {
					MazeLink thisLink = mazeNodes[x][z].DetermineGoalDistanceDifferential(MazeNode.Direction.up);
					if (thisLink != null && thisLink.goalDistanceDifferential > highestDiff) {
						highestDiff = thisLink.goalDistanceDifferential;
						highestDiffLink = thisLink;
					}
				}
			}
		}
		return highestDiffLink;
	}

	void DetermineDistancesFromGoal (MazeNode startNode, float distance = 0f) {
		if (startNode.distanceToGoal > distance) {
			startNode.distanceToGoal = distance;
		}
		//***DEBUG***
		//startNode.GetComponentInChildren<MeshRenderer>().material.color = new Color(0f, distance / 200f, 0f);
		foreach (MazeNode node in startNode.currentConnections) {
			if (node == null) {
				continue;
			}
			float newDist = distance + GetHeuristic(startNode, node, AStarMode.manhattan);
			if (node.distanceToGoal <= newDist) {
				continue;
			}
			DetermineDistancesFromGoal(node, newDist);
		}
	}
}
