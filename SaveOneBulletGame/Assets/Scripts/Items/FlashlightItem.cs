using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Items/FlashlightItem")]
public class FlashlightItem : ItemBase {

    public LayerMask hitLayers;

	//bool to keep track of whether the light in on or off
	public bool lightEnabled;

    public List<Light> spotLights;

	protected override void Awake() {
		base.Awake();
		
		thisItemType = ItemType.flashlight;
		
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
		foreach (Light light in spotLights) {
			light.gameObject.SetActive (true);
		}
	}

	public void Off(){
		lightEnabled = false;
		foreach (Light light in spotLights) {
			light.gameObject.SetActive (false);
		}
	}

    private void BurnWraiths() {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position,
                              Camera.main.transform.forward,
                              spotLights[0].range,
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
