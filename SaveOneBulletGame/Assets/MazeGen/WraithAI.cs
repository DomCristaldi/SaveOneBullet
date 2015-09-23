using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/AI/Wraith")]
[RequireComponent(typeof(NodeTracker))]
[RequireComponent(typeof(Motor))]
public class WraithAI : MonoBehaviour {

	public enum AIState {
		idle,
		chasing,
		searching,
	}

	private bool _isReal;
	public bool isReal;
	public Motor motor;
	public NodeTracker nodeTracker;
	public NodeTracker player;
	public GameObject model;
	public Collider col;
	public float drawDistance;
	[Header("Behavior Variables:")]
	public AIState currentBehavior;
	public bool hasLineOfSight;
	public bool enraged;
	public Transform target;
	public MazeNode lastKnownPlayerLocation;
	public MazeNode guessedNextPlayerLocation;
	public float provokeRange;
	public float idleRandom;
	public float idleRange;
	public float enragedIdleRange;
	public float searchRandom;
	public float searchTime;
	public float enragedSearchTime;
	float currentSearchTime;
	bool decrementSearchTimer;
	MazeNode currentSearchNode;
	MazeNode nextSearchNode;
	public float despawnTime;
	public bool freezeAI;
	[Header("Movement Variables:")]
	public float idleSpeed;
	public float provokedSpeed;
	public float enragedSpeed;
	public float currentSpeed;
	Coroutine fadeRoutine;
	AgentMaterialController matController;
    
    void Awake () {
		nodeTracker = GetComponent<NodeTracker>();
		motor = GetComponent<Motor>();
		currentBehavior = AIState.idle;
		hasLineOfSight = false;
		lastKnownPlayerLocation = null;
		freezeAI = false;
		matController = GetComponent<AgentMaterialController>();
    }
    
	void Start () {
		target = nodeTracker.closestNode.transform;
		Calm();
	}
	
	void FixedUpdate () {
		isReal = _isReal;
		if (!freezeAI) {
			UpdateAIState();
		}
		if (motor.trueDirec.magnitude > 0) {
			model.transform.forward = motor.trueDirec.normalized;
		}
		if (Vector3.Distance(player.transform.position, transform.position) > drawDistance) {
			model.SetActive(false);
		}
		else {
			model.SetActive(true);
		}
	}

	public void SetReal (bool reality) {
		isReal = _isReal = reality;
	}

	void UpdateAIState () {
		if (currentBehavior == AIState.idle) {
			if (PlayerInsideProvokeRange() && HasLineOfSight()) {
				Provoke();
			}
			else {
				Idle();
			}
		}
		if (currentBehavior == AIState.chasing) {
			if (PlayerOutsideIdleRange()) {
				Calm();
			}
			if (!HasLineOfSight()) {
				StartSearching();
			}
			else {
				Chase();
			}
		}
		if (currentBehavior == AIState.searching) {
			if (PlayerOutsideIdleRange()) {
				Calm();
			}
			if (HasLineOfSight()) {
				StartChasing();
			}
			else {
				Search();
			}
		}
	}

	bool PlayerInsideProvokeRange () {
		return (Vector3.Distance(player.transform.position, transform.position) < provokeRange);
	}

	bool HasLineOfSight () {
		float dist = Vector3.Distance(player.transform.position, transform.position);
		RaycastHit[] hits;
		Debug.DrawLine(transform.position + Vector3.up, player.transform.position + Vector3.up, Color.green);
		hits = Physics.RaycastAll(transform.position + Vector3.up, player.transform.position - transform.position, dist, MazeController.singleton.layerMask);
		foreach (RaycastHit hit in hits) {
			if (hit.collider != null) {
				if (hit.collider.gameObject == gameObject) {
					continue;
				}
				if (!hit.collider.enabled) {
					continue;
				}
				if (hit.collider.gameObject.GetComponentInParent<NodeTracker>() == player) {
					hasLineOfSight = true;
					return hasLineOfSight;
				}
				else {
					hit.collider.gameObject.SetActive(true);
					Debug.DrawRay(hit.point, hit.normal, Color.black);
					Debug.DrawRay(hit.point, Vector3.up, Color.white);
					hasLineOfSight = false;
					return hasLineOfSight;
				}
			}
		}
		hasLineOfSight = false;
		return hasLineOfSight;
	}

	bool PlayerOutsideIdleRange () {
		if (enraged) {
			return (Vector3.Distance(player.transform.position, transform.position) > enragedIdleRange);
		}
		else {
			return (Vector3.Distance(player.transform.position, transform.position) > idleRange);
		}
	}

	void Provoke () {
		currentSpeed = provokedSpeed;
		currentBehavior = AIState.chasing;
		target = player.transform;
	}

	void Enrage () {
		enraged = true;
		currentSpeed = enragedSpeed;
		currentBehavior = AIState.chasing;
		target = player.transform;
	}

	void StartChasing () {
		if (enraged) {
			Enrage();
		}
		else {
			Provoke();
		}
	}

	void Calm () {
		enraged = false;
		currentSpeed = idleSpeed;
		currentBehavior = AIState.idle;
		lastKnownPlayerLocation = null;
		guessedNextPlayerLocation = null;
	}

	void StartSearching () {
		currentBehavior = AIState.searching;
		guessedNextPlayerLocation = null;
		currentSearchNode = lastKnownPlayerLocation;
		nextSearchNode = null;
		target = lastKnownPlayerLocation.transform;
		decrementSearchTimer = false;
		if (enraged) {
			currentSearchTime = enragedSearchTime;
		}
		else {
			currentSearchTime = searchTime;
		}
	}

	void Search () {
		if (guessedNextPlayerLocation == null) {
			if (player.closestNode != lastKnownPlayerLocation) {
				guessedNextPlayerLocation = player.closestNode;
			}
		}
		if (AtTarget()) {
			decrementSearchTimer = true;
		}
		if (decrementSearchTimer) {
			if (nextSearchNode == null) {
				nextSearchNode = guessedNextPlayerLocation;
			}
			if (HasLineOfSight()) {
				return;
			}
			if (AtTarget()) {
				FindNextSearchNode();
			}
			target = currentSearchNode.transform;
			currentSearchTime -= Time.fixedDeltaTime;
			if (currentSearchTime <= 0f) {
				Calm();
			}
		}
		MoveToTarget();
	}

	void FindNextSearchNode () {
		MazeNode bestNode = null;
		float furthestDist = 0f;
		foreach (MazeNode node in nextSearchNode.currentConnections) {
			if (node == null) {
				continue;
			}
			if (node == currentSearchNode) {
				continue;
			}
			float dist = MazeController.singleton.GetHeuristic(nextSearchNode, node, MazeController.AStarMode.randomEuclidian, searchRandom);
			if (dist > furthestDist) {
				bestNode = node;
				furthestDist = dist;
			}
		}
		if (bestNode == null) {
			Calm();
			return;
		}
		currentSearchNode = nextSearchNode;
		nextSearchNode = bestNode;
	}

	void Idle () {
		MoveToTarget();
		if (AtTarget()) {
			FindRandomTarget();
		}
	}

	void FindRandomTarget () {
		MazeNode bestNode = null;
		float bestDist = 0f;
		foreach (MazeNode node in nodeTracker.closestNode.currentConnections) {
			if (node == null) {
				continue;
			}
			float dist = MazeController.singleton.GetHeuristic(nodeTracker.closestNode, player.closestNode, MazeController.AStarMode.randomEuclidian, idleRandom);
			if (dist > bestDist) {
				bestNode = node;
				bestDist = dist;
			}
		}
		if (bestNode == null) {
			return;
		}
		target = bestNode.transform;
	}

	void Chase () {
		lastKnownPlayerLocation = player.closestNode;
		target = player.transform;
		MoveToTarget();
	}

	void MoveToTarget () {
		motor.desiredDirec = (target.position - transform.position).normalized * currentSpeed;
		motor.moveSpeedModifier = currentSpeed;
	}

	bool AtTarget () {
		return (Vector3.Distance(transform.position, target.position) < currentSpeed * Time.fixedDeltaTime);
	}

	public void ReactToItem (ItemBase.ItemType itemType) {
		if (itemType == ItemBase.ItemType.gun) {
			Despawn();
		}
		if (itemType == ItemBase.ItemType.flashlight) {
			if (_isReal) {
				Enrage();
			}
			else {
				Despawn();
			}
		}
	}

	public void Despawn () {
		motor.desiredDirec = Vector3.zero;
		motor.trueDirec = Vector3.zero;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		motor.enabled = false;
		col.enabled = false;
		StartCoroutine(DestroyAfterSeconds(despawnTime));
		fadeRoutine = StartCoroutine(FadeAway(despawnTime));
	}

	IEnumerator FadeAway (float seconds) {
		float fadeTime = 0f;
		while (true) {

			yield return null;
			fadeTime += Time.deltaTime / seconds;
		}
	}

	IEnumerator DestroyAfterSeconds (float seconds) {
		yield return new WaitForSeconds(seconds);
		StopCoroutine(fadeRoutine);
		Destroy(gameObject);
	}
	
}
