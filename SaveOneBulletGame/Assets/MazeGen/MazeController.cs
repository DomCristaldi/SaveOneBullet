using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeController : MonoBehaviour {

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

	void Awake () {
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
		//***CHANGE CURRENT CONNECTIONS***
		SolidifyWalls();
	}

	void ConnectNodes () {
		int i = 0;
		for (int x = 1; x < mazeWidth; x += 2) {
			int k = 0;
			for (int z = 1; z < mazeHeight; z += 2) {
				MazeNode thisNode = mazePieces[x][z].GetComponent<MazeNode>();
				mazeNodes[i].Add(thisNode);
				if (i > 0) {
					thisNode.AddConnection(MazeNode.Direction.left, mazeNodes[i-1][k]);
				}
				if (k > 0) {
					thisNode.AddConnection(MazeNode.Direction.up, mazeNodes[i][k-1]);
				}
				thisNode.AddLink(MazeNode.Direction.up, mazePieces[x][z-1].GetComponent<MazeLink>());
				thisNode.AddLink(MazeNode.Direction.down, mazePieces[x][z+1].GetComponent<MazeLink>());
				thisNode.AddLink(MazeNode.Direction.left, mazePieces[x-1][z].GetComponent<MazeLink>());
				thisNode.AddLink(MazeNode.Direction.right, mazePieces[x+1][z].GetComponent<MazeLink>());
				k++;
			}
			i++;
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
		GameObject newPiece = Instantiate(piecePrefab, new Vector3((float)x - (float)(mazeWidth / 2), 0f, (float)z - (float)(mazeHeight / 2)) + transform.position, Quaternion.identity) as GameObject;
		newPiece.transform.parent = gameObject.transform;
		return newPiece;
	}
}
