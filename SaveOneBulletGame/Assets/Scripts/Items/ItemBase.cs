using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/ItemBase")]
public class ItemBase : MonoBehaviour {

    public enum ItemType {
        gun,
        flashlight,
        compass,
		none,
    }

    public Animator animator;

    public ItemType thisItemType;

    public KeyCode itemKey;

	public InventoryController inv;

    protected virtual void Awake() {
		inv = GameObject.FindGameObjectWithTag ("Player").GetComponent<InventoryController> ();
    }

    public virtual void Equip() {
		inv.EquipItem (thisItemType);
    }

    public virtual void Unequip() {
		inv.EquipItem (ItemType.none);
    }

    public virtual void Use() {

    }
}
