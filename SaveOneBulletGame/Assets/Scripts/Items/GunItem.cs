using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/GunItem")]
[RequireComponent(typeof(AudioSource))]
public class GunItem : ItemBase {

    public LayerMask hitLayers;

    private AudioSource _audio;
    public AudioClip gunshotSound;
    public AudioClip openBreachSound;
    public AudioClip closeBreachSound;

    public int ammo = 8;

    public float range = 40.0f;

    public float aimDownSightsSpeed = 3.0f;
    private int _aimDownSightLayerIndex = 1;
    private bool _aimingDownSights = false;

    public int reloadTime = 3;
    public bool canFire = true;
    public bool reloading = false; //bool to keep track on if its reloading or not
    public bool doneReloading = false;
    public bool interruptReload = false;

	private float _reloadTimer;
	//private float _reloadTimerStart;

    public Transform bulletSpawnPoint;
    private ParticleSystem _particleEmitter;
    private Light _shootLight;
    public int particlesEmittedCount = 10;
    public float shootLightTime = 0.2f;

    private PhysicsMeshSpawner _physMeshSpawner;


    //IEnumerator reloadCoroutine;
    /*
    protected override void OnEnable() {
        base.OnEnable();
    }
    */

	//set itemType
    protected override void Awake() {
        base.Awake();

        _audio = GetComponent<AudioSource>();

        thisItemType = ItemType.gun;

        _particleEmitter = bulletSpawnPoint.GetComponent<ParticleSystem>();
        _shootLight = bulletSpawnPoint.GetComponent<Light>();

        _physMeshSpawner = GetComponentInChildren<PhysicsMeshSpawner>();

        _shootLight.enabled = false;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(doneReloading);

		if (reloading) {


            //animator.SetBool("OpenBreach_Bool", true);


            if (doneReloading == false && _reloadTimer < reloadTime) {//increment timer
                _reloadTimer += Time.deltaTime;
                //Debug.Log("timer");
            }

            if (_reloadTimer >= reloadTime) {
                SetHasFinishedReloading();
                _reloadTimer = 0.0f;
            }

		}

        //handle aiming down sights
        AimDownSightProtocol();//POSSIBLY OPTIMIZE?
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

    //AIM DOWN SIGHTS
    public override void UseAlternate() {
        _aimingDownSights = !_aimingDownSights;//set up as toggle button

    }


    //will fire bullet
    public void FireBullet() {
        if (ammo > 0) {
            animator.SetTrigger("Shoot_Trig");
            --ammo;


            //audio.clip = gunshot;
            _audio.PlayOneShot(gunshotSound);

            _particleEmitter.Emit(particlesEmittedCount);
            StartCoroutine(ShootLightTime());

            //***NEED TO RAYCAST OUT INTO THE ENVIRONMENT AND TRY TO DAMAGE SOMETHING
        
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position,
                                Camera.main.transform.forward,
                                range,
                                hitLayers);

            foreach (RaycastHit hit in hits) {
                if (hit.collider.gameObject.layer == 9) {//hit a wall
                    break;
                }
                else {
                    WraithAI wAI = hit.collider.GetComponentInParent<WraithAI>();
                    if (wAI != null) {
						wAI.ReactToItem(ItemType.gun);
                    }
                }
            }

        }

    }


	//will start the clock for reloading time
    public void Reload() {
		reloading = true;
        animator.SetBool("Reload", true);

        DisallowCanFire();
        SetHasNotFinishedReloading();

        _reloadTimer = 0.0f;

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

    private void AimDownSightProtocol() {

        float aimTargetValue = 0.0f;//temp value for what we want to move towards this frame

        //figure out if we're aiming down or not
        if (_aimingDownSights) {
            aimTargetValue = 1.0f;
        }
        else {
            aimTargetValue = 0.0f;
        }

        //perform aim down sights action
        animator.SetLayerWeight(_aimDownSightLayerIndex,
                                Mathf.MoveTowards(animator.GetLayerWeight(_aimDownSightLayerIndex), //gradually move into aiming down sights
                                                  aimTargetValue,
                                                  aimDownSightsSpeed * Time.deltaTime));

    }

    private IEnumerator ShootLightTime() {

        _shootLight.enabled = true;

        yield return new WaitForSeconds(shootLightTime);

        _shootLight.enabled = false;

        yield break;
    }

    public void PlayOpenBreachSound() {
        _audio.PlayOneShot(openBreachSound);
    }

    public void PlayCloseBreachSound() {
        _audio.PlayOneShot(closeBreachSound);
    }

    public void TriggerBulletEjection() {
        _physMeshSpawner.SpawnObject();
    }
}
