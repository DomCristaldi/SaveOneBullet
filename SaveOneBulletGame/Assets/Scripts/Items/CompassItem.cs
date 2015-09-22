using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/CompassItem")]
public class CompassItem : ItemBase {

	protected override void Awake() {
		base.Awake();
		
		thisItemType = ItemType.compass;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//using the compass will bring the compass in front of you and will point to the way out
	public override void Use() {
		base.Use ();
		pointToEscape ();
	}

	//function that will update the arrow on the compass to point to escape
	private void pointToEscape(){

	}
}
