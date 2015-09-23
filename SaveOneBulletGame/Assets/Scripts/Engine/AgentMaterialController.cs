using UnityEngine;
using System.Collections.Generic;

public class AgentMaterialController : MonoBehaviour {

    public List<Renderer> renderList;
    /*
    [Range(0.0f, 1.0f)]
    public float dissolveAmount = 0.0f;
    */
    void Awake() {
        
    }

	// Use this for initialization
	void Start () {
        //SetDissolveAmount(0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetDissolveAmount(float amount) {
        foreach (Renderer ren in renderList) {
            ren.material.SetFloat("_SliceAmount", amount);
        }
    }
}
