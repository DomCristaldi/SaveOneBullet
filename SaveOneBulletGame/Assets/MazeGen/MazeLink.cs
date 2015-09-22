using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/MazeGen/MazeLink")]
public class MazeLink : MonoBehaviour {
    
	public GameObject wall;
	public bool onMainPath;
	public float goalDistanceDifferential;
	public MazeNode adjacentNode1;
	public MazeNode adjacentNode2;
	public MeshRenderer floorRenderer;
	public MeshRenderer cubeRenderer;
	public Material floorMat;
	public Material cubeMat;
	public BoxCollider wallCollider;

    void Awake () {
		onMainPath = false;
		goalDistanceDifferential = 0f;
		floorRenderer.material = floorMat;
		cubeRenderer.material = cubeMat;
		wallCollider = wall.GetComponent<BoxCollider>();
    }
    
	void Start () {
        
	}
	
	void Update () {
        
	}
}
