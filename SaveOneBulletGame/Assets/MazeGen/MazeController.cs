using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/MazeGen/MazeController")]
public class MazeController : MonoBehaviour {

	public enum EvadeUseMode {
		display,
		wallPlacement,
	}

	public enum SearchUseMode {
		display,
		enemyPlacement,
	}

	public static MazeController singleton;
	public LayerMask layerMask;
	[Header("Prefabs:")]
	public GameObject wall;
	public GameObject floorLink;
	public GameObject floorNode;
	public GameObject playerPrefab;
	public GameObject wraithPrefab;
	[Header("Check to spawn player:")]
	public bool spawnPlayer;
	[Header("Wraith spawning stuff:")]
	public bool spawnWraiths;
	public int realWraiths;
	public int fakeWraiths;
	public int minSpawnDistance;
	public int maxSpawnDistance;
	public float wraithWrapDistance;
	public float wraithRandom;
	[Header("Dimensions (# of nodes):")]
	public int nodeWidth;
	public int nodeHeight;
	[Header("Scaling:")]
	public float mazeScale;
	[Header("Actual dimensions (calculated at runtime):")]
	public int mazeWidth;
	public int mazeHeight;
	public List<List<GameObject>> mazePieces;
	public List<List<MazeNode>> mazeNodes;
	[Header("Node graph stuff (DO NOT CHANGE):")]
	public MazeNode exitNode;
	public MazeLink exit;
	public MazeNode startNode;
	public NodeTracker player;
	public MazeNode playerNode;
	public List<MazeNode> path;
	public List<MazeNode> unconnected;
	List<MazeNode> distanceChecked;
	[Header("Keycodes:")]
	public bool useDebugKeys;
	public KeyCode showPathKey;
	public KeyCode showLoopsKey;
	public KeyCode aStarKey;
	public KeyCode searchKey;
	public KeyCode evadeKey;
	public KeyCode clearKey;
	[Header("Turn debug coloring on/off:")]
	public bool debugColors;
	bool colorsOff;
	[Header("Debug Materials:")]
	public Material debugRed;
	public Material debugGreen;
	public Material debugBlue;
	public Material debugCyan;
	public Material debugMagenta;
	public Material debugYellow;
	public Material debugBlack;
	public Material debugWhite;
	[Header("Maze Generation Parms (FOR INFORMED DESIGNERS ONLY):")]
	public float goldenPathRandom;
	public int offshootStopChance;
	public int maxAllowedStragglers;
	public int wallRemovalsGoal;
	public int wallRemovalsStart;
	public int aStarPasses;
	public AStarMode aStarMode;
	public float aStarRandom;
	public int aStarSpeed;
	public float searchRandom;
	public int evadeDistance;

	void Awake () {
		if (singleton == null) {
			singleton = this;
		}
		mazeWidth = nodeWidth * 2 + 1;
		mazeHeight = nodeHeight * 2 + 1;
		colorsOff = false;
	}

	void Start () {
		CreateMaze();
	}

	void Update () {
		if (useDebugKeys) {
			if (Input.GetKeyDown(aStarKey)) {
				StartCoroutine(AStarAgent());
			}
			if (Input.GetKeyDown(searchKey)) {
				ClearNodesSearched();
				AgentSearch(playerNode, 0);
			}
			if (Input.GetKeyDown(evadeKey)) {
				ClearNodesSearched();
				AgentEvade(playerNode, exitNode, 0, evadeDistance);
			}
			if (Input.GetKeyDown(clearKey)) {
				ClearDebugColors();
			}
			if (Input.GetKeyDown(showPathKey)) {
				ShowGoldenPath();
			}
			if (Input.GetKeyDown(showLoopsKey)) {
				DetectLoops();
			}
		}
		if (debugColors) {
			if (colorsOff) {
				ShowGoldenPath();
				colorsOff = false;
			}
		}
		if (!debugColors) {
			if (!colorsOff) {
				ClearDebugColors();
				colorsOff = true;
			}
		}
		if (player != null) {
			playerNode = player.closestNode;
		}
	}

	void ClearNodesSearched () {
		for (int x = 0; x < nodeWidth; x++) {
			for (int z = 0; z < nodeHeight; z++) {
				mazeNodes[x][z].searched = false;
			}
		}
	}

	void ClearDebugColors () {
		for (int x = 0; x < nodeWidth; x++) {
			for (int z = 0; z < nodeHeight; z++) {
				if (debugColors && mazeNodes[x][z].enemyOccupied) {
					continue;
				}
				mazeNodes[x][z].floorRenderer.material = mazeNodes[x][z].floorMat;
				foreach(MazeLink link in mazeNodes[x][z].links) {
					if (link == null) {
						continue;
					}
					link.floorRenderer.material = link.floorMat;
					link.cubeRenderer.material = link.cubeMat;
				}
			}
		}
	}

	void ShowGoldenPath () {
		foreach (MazeNode node in path) {
			node.floorRenderer.material = debugYellow;
			foreach (MazeLink link in node.links) {
				if (link == null) {
					continue;
				}
				if (link.onMainPath) {
					link.floorRenderer.material = debugYellow;
				}
			}
		}
		exitNode.floorRenderer.material = debugGreen;
		exit.cubeRenderer.material = debugBlack;
		startNode.floorRenderer.material = debugWhite;
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

	void SpawnPlayer () {
		if (playerPrefab == null) {
			Debug.LogWarning("No player prefab assigned to MazeController!");
			return;
		}
		if (spawnPlayer) {
			GameObject spawnedPlayer = Instantiate(playerPrefab, startNode.transform.position, Quaternion.identity) as GameObject;
			player = spawnedPlayer.GetComponent<NodeTracker>();
			player.closestNode = startNode;
			playerNode = startNode;
		}
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
				mazePieces[x][z].transform.localScale = Vector3.one * mazeScale;
			}
		}
		ConnectNodes();
		SetExit();
		FindGoldenPath();
		CarveOtherPaths(offshootStopChance);
		ConnectStragglers(maxAllowedStragglers);
		CreateLoops(wallRemovalsGoal, wallRemovalsStart);
		SolidifyWalls();
		SpawnPlayer();
		SpawnWraiths();
	}

	void FindGoldenPath () {
		path = AStar(startNode, exitNode, AStarMode.randomManhattan, goldenPathRandom, true);
		foreach (MazeNode node in path) {
			if (node != exitNode && node != startNode) {
				if (debugColors) {
					node.floorRenderer.material = debugYellow;
				}
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
		exit = exitNode.links[MazeNode.DirectionToIndex(d)];
		startNode = mazeNodes[nodeWidth - 1 - x][nodeHeight - 1 - z];
		playerNode = startNode;
		if (debugColors) {
			exitNode.floorRenderer.material = debugGreen;
			exit.cubeRenderer.material = debugBlack;
			startNode.floorRenderer.material = debugWhite;
		}
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
		return ((float)x - (float)(mazeWidth / 2)) * mazeScale;
	}

	public float IndexToZ (int z) {
		return ((float)z - (float)(mazeHeight / 2)) * mazeScale;
	}

	public MazeNode ClosestNodeToPositon (Vector3 position) {
		List<int> indices = PositionToIndex (position);
		return mazeNodes[indices[0]][indices[1]];
	}

	public List<int> PositionToIndex (Vector3 position) {
		List<int> indices = new List<int>();
		indices.Add(XToIndex(position.x));
		indices.Add(ZToIndex(position.z));
		return indices;
	}

	public int XToIndex (float x) {
		x = x - transform.position.x;
		x = x / 2f;
		x = x / mazeScale;
		x = x + nodeWidth / 2;
		return Mathf.RoundToInt(x);
	}

	public int ZToIndex (float z) {
		z = z - transform.position.z;
		z = z / 2f;
		z = z / mazeScale;
		z = z + nodeHeight / 2;
		return Mathf.RoundToInt(z);
	}

	public enum AStarMode {
		manhattan,
		euclidian,
		randomManhattan,
		randomEuclidian,
	}

	bool AgentEvade (MazeNode node, MazeNode evadeNode, int depth, int distance = 50, EvadeUseMode mode = EvadeUseMode.display) {
		if (depth >= distance) {
			ClearNodesSearched();
			return true;
		}
		List<MazeNode> nodes = new List<MazeNode>(node.currentConnections);
		List<MazeNode> inOrder = new List<MazeNode>();
		while (nodes.Count > 0) {
			MazeNode bestNode = nodes[0];
			float largestDist;
			if (nodes[0] != null) {
				largestDist = GetHeuristic(nodes[0], evadeNode, AStarMode.randomEuclidian, searchRandom);
			}
			else {
				largestDist = 0f;
			}
			for (int i = 1; i < nodes.Count; i++) {
				if (nodes[i] == null) {
					continue;
				}
				if (nodes[i].searched) {
					continue;
				}
				float dist = GetHeuristic(nodes[i], evadeNode, AStarMode.randomEuclidian, searchRandom);
				if (dist > largestDist) {
					largestDist = dist;
					bestNode = nodes[i];
				}
			}
			if (bestNode != null) {
				inOrder.Add(bestNode);
				nodes.Remove(bestNode);
			}
			else {
				break;
			}
		}
		foreach (MazeNode n in inOrder) {
			n.searched = true;
			if (debugColors) {
				n.floorRenderer.material = debugGreen;
			}
			bool endFound = AgentEvade(n, evadeNode, depth + 1, distance, mode);
			if (endFound) {
				if (depth + 1 >= distance) {
					if (mode == EvadeUseMode.wallPlacement) {
						PlaceWall(node, n);
					}
				}
				return true;
			}
		}
		return false;
	}

	void PlaceWall (MazeNode node1, MazeNode node2) {

	}

	bool AgentSearch (MazeNode node, int depth, int distance = int.MaxValue, SearchUseMode mode = SearchUseMode.display, bool realWraith = false) {
		float srchRand = 0f;
		if (mode == SearchUseMode.display) {
			srchRand = searchRandom;
		}
		if (mode == SearchUseMode.enemyPlacement) {
			srchRand = wraithRandom;
		}
		if (node == exitNode) {
			ClearNodesSearched();
			return true;
		}
		if (depth >= distance) {
			if (mode == SearchUseMode.enemyPlacement && !node.enemyOccupied) {
				SpawnWraith(node, realWraith);
				ClearNodesSearched();
				return true;
			}
			return false;
		}
		List<MazeNode> nodes = new List<MazeNode>(node.currentConnections);
		List<MazeNode> inOrder = new List<MazeNode>();
		while (nodes.Count > 0) {
			MazeNode bestNode = nodes[0];
			float smallestDist;
			if (nodes[0] != null) {
				smallestDist = GetHeuristic(nodes[0], exitNode, AStarMode.randomEuclidian, srchRand);
			}
			else {
				smallestDist = Mathf.Infinity;
			}
			for (int i = 1; i < nodes.Count; i++) {
				if (nodes[i] == null) {
					continue;
				}
				if (nodes[i].searched) {
					continue;
				}
				float dist = GetHeuristic(nodes[i], exitNode, AStarMode.randomEuclidian, srchRand);
				if (dist < smallestDist) {
					smallestDist = dist;
					bestNode = nodes[i];
				}
			}
			if (bestNode != null) {
				inOrder.Add(bestNode);
				nodes.Remove(bestNode);
			}
			else {
				break;
			}
		}
		foreach (MazeNode n in inOrder) {
			n.searched = true;
			if (debugColors && mode == SearchUseMode.display) {
				n.floorRenderer.material = debugMagenta;
			}
			bool endFound = AgentSearch(n, depth + 1, distance, mode, realWraith);
			if (endFound) {
				return true;
			}
		}
		return false;
	}

	IEnumerator AStarAgent (AStarMode mode = AStarMode.manhattan, float rand = 5f) {
		int timer = 0;
		List<MazeNode> closed = new List<MazeNode>();
		List<MazeNode> open = new List<MazeNode>();
		open.Add(startNode);
		startNode.g = 0f;
		while (open.Count > 0) {
			MazeNode bestNode = open[0];
			foreach (MazeNode node in open) {
				if (node.f < bestNode.f) {
					bestNode = node;
				}
			}
			if (bestNode == exitNode) {
				break;
			}
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
					node.h = GetHeuristic(node, exitNode, mode, rand);
					node.f = node.g + node.h;
					node.cameFrom = bestNode;
				}
			}
			closed.Add(bestNode);
			bestNode.closed = true;
			if (debugColors) {
				bestNode.floorRenderer.material = debugCyan;
			}
			open.Remove(bestNode);
			timer++;
			if (timer >= aStarSpeed) {
				timer = 0;
				yield return new WaitForSeconds(0.01f);
			}
		}
		//Reset came from
		foreach(MazeNode node in closed) {
			node.ResetAStarVariables();
		}
		foreach(MazeNode node in open) {
			node.ResetAStarVariables();
		}
		yield break;
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

	void DetectLoops () {
		for (int j = 0; j < aStarPasses; j++) {
			List<MazeNode> newPath = AStar(startNode, exitNode, aStarMode, aStarRandom);
			foreach (MazeNode node in newPath) {
				if (node != exitNode && node != startNode) {
					if (debugColors) {
						node.floorRenderer.material = debugYellow;
					}
				}
			}
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
			DetectLoops();
		}
		for (int i = 0; i < goalPasses; i++) {
			DetermineDistancesFromGoal(exitNode);
			MazeLink highestDiffLink = DetermineGoalDistanceDifferentials();
			//Debug.DrawLine(highestDiffLink.transform.position, new Vector3(0f, 20f, 0f), Color.black);
			highestDiffLink.adjacentNode1.ConnectToNode(highestDiffLink.adjacentNode2);
			ResetGoalDistances();
			//Debug.Break();
			DetectLoops();
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
		/*
		if (debugColors) {
			startNode.floorRenderer.material.color = new Color(0f, distance / 200f, 0f);
		}
		*/
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

	void SpawnWraith (MazeNode node, bool realWraith) {
		if (debugColors) {
			node.floorRenderer.material = debugRed;
		}
		if (wraithPrefab == null) {
			Debug.LogError("Maze Controller wraith prefab not set!");
			return;
		}
		GameObject newWraith = Instantiate(wraithPrefab, node.transform.position, Quaternion.identity) as GameObject;
		NodeTracker wraithTracker = newWraith.GetComponent<NodeTracker>();
		if (wraithTracker == null) {
			Debug.LogError("Wraith prefab needs a NodeTracker script!");
			return;
		}
		wraithTracker.closestNode = node;
		node.enemyOccupied = true;
		WraithAI ai = newWraith.GetComponent<WraithAI>();
		if (ai == null) {
			Debug.LogError("Wraith prefab needs an AI script!");
			return;
		}
		ai.SetReal(realWraith);
		ai.player = player;
	}

	void SpawnWraiths () {
		if (!spawnWraiths) {
			return;
		}
		int realSpawned = 0;
		int fakeSpawned = 0;
		int totalWraithsToSpawn = fakeWraiths + realWraiths;
		if (totalWraithsToSpawn > (maxSpawnDistance - minSpawnDistance)) {
			maxSpawnDistance = minSpawnDistance + totalWraithsToSpawn;
		}
		if (totalWraithsToSpawn <= 1 || fakeWraiths < 1 || realWraiths < 1) {
			Debug.LogWarning("Not spawning wraiths because the spawn number isn't high enough.");
			return;
		}
		float distanceStep = (float)(maxSpawnDistance - minSpawnDistance) / (float)(totalWraithsToSpawn - 1);
		int placeDistance = minSpawnDistance;
		float actualPlaceDistance = (float)placeDistance;
		while ((realSpawned + fakeSpawned) < totalWraithsToSpawn) {
			if (Random.value > (float)fakeSpawned / (float)(fakeWraiths)) {
				AgentSearch(startNode, 0, placeDistance, SearchUseMode.enemyPlacement, false);
				fakeSpawned++;
			}
			else {
				AgentSearch(startNode, 0, placeDistance, SearchUseMode.enemyPlacement, true);
				realSpawned++;
			}
			actualPlaceDistance += distanceStep;
			while (actualPlaceDistance - (float)placeDistance >= 1f) {
				placeDistance++;
			}
		}
	}
}
