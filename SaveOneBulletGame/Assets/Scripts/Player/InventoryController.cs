using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

    [System.Serializable]
    public class EquipmentSlot {
        public ItemBase item;
        public bool currentlyEquipped = false;
    }

    public Transform handsTf;

    public ItemBase equippedItem;
    public List<EquipmentSlot> equipmentSlots;

	// Use this for initialization
	void Start () {
        //ChangeEquippedItem();
        EquipItem(ItemBase.ItemType.gun);
        PutEquippedItemInHand();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void EquipItem(ItemBase.ItemType type) {
        foreach (EquipmentSlot slot in equipmentSlots) {
            if (slot.item.thisItemType == type) {
                slot.currentlyEquipped = true;
                equippedItem = slot.item;
            }
            else {
                slot.currentlyEquipped = false;
                equippedItem.transform.parent = null;
            }
        }
    }

    private void HandleEuippedItem() {

    }

    private void PutEquippedItemInHand() {
        equippedItem.transform.position = handsTf.position;
        equippedItem.transform.rotation = handsTf.rotation;
        equippedItem.transform.parent = handsTf;
    }

    /*
    public void ChangeEquippedItem() {
        foreach (EquipmentSlot slot in equipmentSlots) {
            if (slot.currentlyEquipped) {

                equippedItem = slot.item;

                //break;
            }
        }
    }
    */

    public void UseItem() {
        equippedItem.Use();
        Debug.Log("UseItem()");
    }
}
