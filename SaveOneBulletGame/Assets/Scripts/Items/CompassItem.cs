using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/CompassItem")]
public class CompassItem : ItemBase {

	public Transform needle;

	protected override void Awake() {
		base.Awake();
		
		thisItemType = ItemType.compass;

	}
	
	// Update is called once per frame
	void Update () {
        pointToEscape();
	}

	//using the compass will bring the compass in front of you and will point to the way out
	public override void Use() {
		base.Use ();
		//pointToEscape ();
	}

	//function that will update the needle on the compass to point to escape
	private void pointToEscape(){
        Vector3 exitDirec = (MazeController.singleton.exit.transform.position - needle.position).normalized;
        //needlePlane.SetNormalAndPosition(needle.up, needle.position);

        Vector3 needleDirec = (Vector3.ProjectOnPlane(exitDirec, needle.up)).normalized;

        needle.rotation = Quaternion.LookRotation(needleDirec, needle.up);
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
