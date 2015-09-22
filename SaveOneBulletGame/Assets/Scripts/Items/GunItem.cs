﻿using UnityEngine;
using System.Collections;

public class GunItem : ItemBase {

    public int ammo = 8;

	public int reloadTime = 3;
    public bool canFire = true;
    public bool reloading = false; //bool to keep track on if its reloading or not
    public bool doneReloading = false;
    public bool interruptReload = false;

    

	private float timer;
	private float start;

    public Transform bulletSpawnPoint;
    private ParticleSystem partEmitter;
    private Light shootLight;
    public int particlesEmittedCount = 10;
    public float shootLightTime = 0.2f;


    //IEnumerator reloadCoroutine;
    /*
    protected override void OnEnable() {
        base.OnEnable();
    }
    */

	//set itemType
    protected override void Awake() {
        base.Awake();

        thisItemType = ItemType.gun;

        partEmitter = bulletSpawnPoint.GetComponent<ParticleSystem>();
        shootLight = bulletSpawnPoint.GetComponent<Light>();

        shootLight.enabled = false;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log(doneReloading);

		if (reloading) {


            //animator.SetBool("OpenBreach_Bool", true);


            if (doneReloading == false && timer < reloadTime) {//increment timer
                timer += Time.deltaTime;
                Debug.Log("timer");
            }

            if (timer >= reloadTime) {
                SetHasFinishedReloading();
                timer = 0.0f;
            }

            /*
			timer += Time.deltaTime;
			if(timer - start >= reloadTime) {
				//reloading = false;

                //animator.SetBool("Reloading_Bool", false);

                SetHasFinishedReloading();
            }
            */
		}
        /*
        else {
            animator.SetBool("Reloading_Bool", false);
            //AllowCanFire();
            //AllowCanFire();
            //reloading = false;
            //interruptReload = false;
        }
        */
        
	}

    public override void Equip() {
        base.Equip();
        //if (!interuptedAction) {
            animator.SetTrigger("EquipItem_Trig");
        //}
    }

    public override void Unequip() {
        base.Unequip();
        animator.SetTrigger("UnequipItem_Trig");
    }

	//will fire the gun if its not still reloading
    public override void Use() {
        if (animator.GetBool("Reload") == false && animator.GetBool("CanShoot_Bool") == true) {
            FireBullet();
        }

    }

	//will fire bullet
    public void FireBullet() {
        if (ammo > 0) {
            animator.SetTrigger("Shoot_Trig");
            --ammo;

            partEmitter.Emit(particlesEmittedCount);
            StartCoroutine(ShootLightTime());

            //***NEED TO RAYCAST OUT INTO THE ENVIRONMENT AND TRY TO DAMAGE SOMETHING
        }

    }


	//will start the clock for reloading time
    public void Reload() {
		reloading = true;
        animator.SetBool("Reload", true);

        DisallowCanFire();
        SetHasNotFinishedReloading();

        timer = 0.0f;

		//start = Time.deltaTime;
		//timer = start;
        //animator.SetBool("CanShoot_Bool", false);

    }

    public void ExitReload() {
        reloading = false;
        animator.SetBool("Reload", false);
    }

    public void ForeceCanFire() {
        //reloading = false;
        //animator.SetBool("Reload", false);
        //canFire = true;
        ExitReload();
        SetHasFinishedReloading();
        AllowCanFire();
        
    }

    public void AllowCanFire() {
        canFire = true;
        animator.SetBool("CanShoot_Bool", true);
    }

    public void DisallowCanFire() {
        canFire = false;
        animator.SetBool("CanShoot_Bool", false);
    }

    public void SetHasFinishedReloading() {
        doneReloading = true;
        animator.SetBool("FinishReload_Bool", true);
    }

    public void SetHasNotFinishedReloading() {
        doneReloading = false;
        animator.SetBool("FinishReload_Bool", false);
    }

    public override void SaveAnimBools() {
        base.SaveAnimBools();

        reloading = animator.GetBool("Reload");
        canFire = animator.GetBool("CanShoot_Bool");
        doneReloading = animator.GetBool("FinishReload_Bool");
    }

    public override void LoadAnimBools() {
        base.LoadAnimBools();

        animator.SetBool("Reload", reloading);
        animator.SetBool("CanShoot_Bool", canFire);
        animator.SetBool("FinishReload_Bool", doneReloading);
    }

    private IEnumerator ShootLightTime() {

        shootLight.enabled = true;

        yield return new WaitForSeconds(shootLightTime);

        shootLight.enabled = false;

        yield break;
    }
}
