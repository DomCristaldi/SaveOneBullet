using UnityEngine;
using System.Collections;

public class ItemBase : MonoBehaviour {

    public Animator animator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void Equip() {

    }

    public virtual void Unequip() {

    }

    public virtual void Use() {

    }
}
