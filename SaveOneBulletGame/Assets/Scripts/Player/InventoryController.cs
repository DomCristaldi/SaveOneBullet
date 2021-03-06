﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Player/InventoryController")]
public class InventoryController : MonoBehaviour {

    [System.Serializable]
    public class EquipmentSlot {

        public EquipmentSlot(ItemBase item) {
            this.item = item;
        }

        public EquipmentSlot (GameObject item) {
        	
        	//make sure the item we're adding is actually an item
        	if (item.GetComponent<ItemBase>() == null) { return; }
        	
            this.item = item.GetComponent<ItemBase>();
        }

        public ItemBase item;
        public bool currentlyEquipped = false;
    }

    public Transform handsTf;

    public ItemBase emptyHandsPrefab;

    public ItemBase equippedItem;

    public Dictionary<ItemBase.ItemType, EquipmentSlot> equipSlots;
    public List<ItemBase> startingItems;
    //public List<EquipmentSlot> equipmentSlots;

    void Awake() {
        equipSlots = new Dictionary<ItemBase.ItemType, EquipmentSlot>();

        AddItemToInventory((ItemBase)Instantiate(emptyHandsPrefab));//always ensure we have an empty hands
        EquipItem(ItemBase.ItemType.none);
        
        foreach (ItemBase item in startingItems) {
            GameObject tempItem = (GameObject)Instantiate(item.gameObject);
            AddItemToInventory(tempItem);
            tempItem.SetActive(false);
        }
        

    }


    /*
	// Use this for initialization
	void Start () {
        //ChangeEquippedItem();
        //EquipItem(ItemBase.ItemType.gun);
        //PrepareAllEquipableItems();
	}
	
	// Update is called once per frame
	void Update () {
        //HandleEquippedItem();
	}
    */

    private void PrepareEquippableItem(ItemBase item) {
        item.transform.position = handsTf.position;
        item.transform.rotation = handsTf.rotation;
        item.transform.parent = handsTf;
    }

    private void PrepareAllEquipableItems() {
        foreach (KeyValuePair<ItemBase.ItemType, EquipmentSlot> slot in equipSlots) {

            PrepareEquippableItem(slot.Value.item);

            if (slot.Key != ItemBase.ItemType.none) {
                slot.Value.item.gameObject.SetActive(false);
            }
        }
    }

    public void AddItemToInventory(ItemBase item) {
        equipSlots.Add(item.thisItemType, new EquipmentSlot(item));
        PrepareEquippableItem(item);
    }

    public void AddItemToInventory(GameObject item) {
        AddItemToInventory(item.GetComponent<ItemBase>());
    }

    public void EquipItem (ItemBase.ItemType itemType) {

        if (equippedItem != null &&
            equippedItem.thisItemType == itemType &&
            equippedItem.thisItemType != ItemBase.ItemType.none)
        {

            EquipItem(ItemBase.ItemType.none);
    
            return; 
        }

        if (!equipSlots.ContainsKey(itemType)) { return; }//only allow equipping if item is in the Player's inventory

        if (equippedItem != null) {
            PutAwayEquippedItem();//put away the previously equipped item
        }

        //reset all items to unequiped
        foreach (KeyValuePair<ItemBase.ItemType, EquipmentSlot> kvp in equipSlots) {
            kvp.Value.currentlyEquipped = false;
        }
        
        equipSlots[itemType].currentlyEquipped = true;
        equippedItem = equipSlots[itemType].item;

        PutEquippedItemInHand();//show newly equipped item
    }

    private void HandleEquippedItem() {
        
    }

    private void PutEquippedItemInHand() {

        equippedItem.EnableItem();

        equippedItem.Equip();

    }

    private void PutAwayEquippedItem() {
        equippedItem.Unequip();
    }

    public void UseItem() {
		if(equippedItem != null) {
        	equippedItem.Use();
        }
        //Debug.Log("UseItem()");
    }

    public void UseItemAlternate() {
        if (equippedItem != null) {
            equippedItem.UseAlternate();
        }
    }

    public bool ItemInInventory(ItemBase.ItemType type) {
        if (equipSlots.ContainsKey(type)) {
            return true;
        }
        else {
            return false;
        }
    }

}
