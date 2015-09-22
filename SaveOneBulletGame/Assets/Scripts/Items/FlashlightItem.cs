using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/FlashlightItem")]
public class FlashlightItem : ItemBase {

	//bool to keep track of whether the light in on or off
	public bool lightEnabled;

	public GameObject lightObject;

	protected override void Awake() {
		base.Awake();
		
		thisItemType = ItemType.flashlight;
	}

	// Update is called once per frame
	void Update () {
	}

	//using the flashlight will turn on/off the light accosiated with it
	public override void Use() {
		base.Use ();
		lightEnabled = !lightEnabled;
		lightObject.SetActive(lightEnabled);
	}
}
