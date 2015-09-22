using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/ItemBase")]
public class ItemBase : MonoBehaviour {

    public enum ItemType {
        gun,
        flashlight,
        compass,
        note,
		none,
    }

    public Animator animator;

    public ItemType thisItemType;

    public KeyCode itemKey;

	public InventoryController inv;

    public bool interuptedAction = false;

    protected virtual void Awake() {
		//inv = GameObject.FindGameObjectWithTag ("Player").GetComponent<InventoryController> ();

        SaveAnimBools();
    }

    protected virtual void OnEnable() {
        LoadAnimBools();
    }

    protected virtual void OnDisable() {
        SaveAnimBools();
    }

    public virtual void Equip() {
		//inv.EquipItem (thisItemType);
        //animator.SetTrigger("EquipItem_Trig");
    }

    public virtual void Unequip() {
		//inv.EquipItem (ItemType.none);
        //animator.SetTrigger("UnequipItem_Trig");
    }

    public virtual void EnableItem() {
        gameObject.SetActive(true);
    }

    public virtual void DisableItem() {
        gameObject.SetActive(false);
    }

    public virtual void Use() {

    }
    /*
    public virtual void Interrupt() {
        interuptedAction = true;
    }
    */
    public virtual void SaveAnimBools() {
        //Debug.Log("saving bools");
    }

    public virtual void LoadAnimBools() {
        //Debug.Log("loading bools");
    }
}
