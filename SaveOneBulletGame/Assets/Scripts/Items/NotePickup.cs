using UnityEngine;
using System.Collections;

public class NotePickup : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        InventoryController inv = other.GetComponent<InventoryController>();

        if (!inv.ItemInInventory(ItemBase.ItemType.note)) {

        }
        else {
            NoteItem nItem = inv.equipSlots[ItemBase.ItemType.note].item as NoteItem;//upcast b/c we know it's going to be a note


        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
