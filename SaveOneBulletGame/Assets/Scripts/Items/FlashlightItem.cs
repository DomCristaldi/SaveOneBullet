using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/FlashlightItem")]
public class FlashlightItem : ItemBase {

    public LayerMask hitLayers;

	//bool to keep track of whether the light in on or off
	public bool lightEnabled;

	public GameObject lightObject;
    Light spotLight;

	protected override void Awake() {
		base.Awake();
		
		thisItemType = ItemType.flashlight;

        if (lightObject != null) {
            spotLight = lightObject.GetComponent<Light>();
        }
	}

	// Update is called once per frame
	void Update () {
        BurnWraiths();

	}

	//using the flashlight will turn on/off the light accosiated with it
	public override void Use() {
		base.Use ();
		if (lightEnabled)
			Off ();
		else
			On ();
	}
	

	public void On(){
		lightEnabled = true;
		lightObject.SetActive (true);
	}

	public void Off(){
		lightEnabled = false;
		lightObject.SetActive (false);
	}

    private void BurnWraiths() {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position,
                              Camera.main.transform.forward,
                              spotLight.range,
                              hitLayers);

        foreach (RaycastHit hit in hits) {
            if (hit.collider.gameObject.layer == 9) {//hit a wall
                break;
            }
            else {
                WraithAI wAI = hit.collider.GetComponent<WraithAI>();
                if (wAI != null) {
                    wAI.ReactToItem(thisItemType);
                }
            }
        }
    }

    public override void Equip() {
        base.Equip();

        gameObject.SetActive(true);
    }

    public override void Unequip() {
        base.Unequip();

        gameObject.SetActive(false);
    }
}
