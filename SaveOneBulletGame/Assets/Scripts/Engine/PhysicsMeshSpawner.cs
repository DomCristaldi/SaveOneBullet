using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PhysicsMeshSpawner : MonoBehaviour {

    public GameObject physicalObjectToSpawn;

    public float forceAmount;
    public Vector3 relativeDirectionOfForce_Unit;
    public ForceMode forceType = ForceMode.Force;

    public void SpawnObject() {
        GameObject spawnedObject = (GameObject)Instantiate(physicalObjectToSpawn, transform.position, transform.rotation);


        Rigidbody spawnedRigBod = spawnedObject.GetComponent<Rigidbody>();
        if (spawnedRigBod == null) {
            spawnedObject.AddComponent<Rigidbody>();
        }

        spawnedRigBod.AddForce(transform.rotation * relativeDirectionOfForce_Unit.normalized * forceAmount,
                               forceType);

    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(PhysicsMeshSpawner))]
public class PhysicsMeshSpawner_Editor : Editor {
    PhysicsMeshSpawner selfScript;

    void OnEnable() {
        selfScript = (PhysicsMeshSpawner)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("test spawn mesh")) {
            selfScript.SpawnObject();
        }
    }
}
#endif
