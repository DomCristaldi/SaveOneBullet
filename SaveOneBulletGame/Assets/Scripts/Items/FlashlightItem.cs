using UnityEngine;
using System.Collections;

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
		//do the same for the actual light in game ... get light component and turn it to be on/off
		lightObject.SetActive(false);
	}
}
