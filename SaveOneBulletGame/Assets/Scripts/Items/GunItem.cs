using UnityEngine;
using System.Collections;

public class GunItem : ItemBase {

    public int ammo = 8;

    protected override void Awake() {
        base.Awake();

        thisItemType = ItemType.gun;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void Use() {
        base.Use();
        FireBullet();

        Debug.Log("using");
    }

    public void FireBullet() {
        --ammo;
        
    }

    public void Reload() {

    }
}
