using UnityEngine;
using System.Collections;

public class MazeLink : MonoBehaviour {
    
	public GameObject wall;
	public bool onMainPath;
	public float goalDistanceDifferential;
	public MazeNode adjacentNode1;
	public MazeNode adjacentNode2;

    void Awake () {
		onMainPath = false;
		goalDistanceDifferential = 0f;
    }
    
	void Start () {
        
	}
	
	void Update () {
        
	}
}
