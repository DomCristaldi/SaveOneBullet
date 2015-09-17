﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/GunItem")]
public class GunItem : ItemBase {

    public int ammo = 8;
	public int reloadTime = 3;
    public bool canFire = true;
	public bool reloading = false; //bool to keep track on if its reloading or not
	private float timer;
	private float start;

	//set itemType
    protected override void Awake() {
        base.Awake();

        thisItemType = ItemType.gun;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (reloading) {
            animator.SetBool("OpenBreach_Bool", true);
			timer += Time.deltaTime;
			if(timer - start >= reloadTime)
				reloading = false;
		}
        else {
            animator.SetBool("OpenBreach_Bool", false);
        }
	}

    public override void Equip() {
        base.Equip();
        animator.SetTrigger("EquipItem_Trig");
    }

	//will fire the gun if its not still reloading
    public override void Use() {
		if (!reloading) {
			base.Use ();
			FireBullet ();
			Debug.Log ("fire bullet");
		} else {
			Debug.Log ("reloading");
		}
    }

	//will fire bullet
    public void FireBullet() {
        --ammo;
		Reload ();
    }

	//will start the clock for reloading time
    public void Reload() {
		reloading = true;
		start = Time.deltaTime;
		timer = start;
    }
}
