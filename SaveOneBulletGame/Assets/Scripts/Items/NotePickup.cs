using UnityEngine;
using System.Collections;

public class NotePickup : MonoBehaviour {

    public string noteText;

    void OnTriggerEnter(Collider other) {
        InventoryController inv = other.transform.root.gameObject.GetComponent<InventoryController>();

        //Debug.Log(inv.gameObject.name);

        //Debug.Log(inv.name);
        
        
        if (!inv.ItemInInventory(ItemBase.ItemType.note)) {

        }
        else {
            NoteItem nItem = inv.equipSlots[ItemBase.ItemType.note].item as NoteItem;//upcast b/c we know it's going to be a note

            nItem.AddNote(noteText);

            Destroy(gameObject);
        }
        
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetNoteText(string text) {
        noteText = text;
    }
}
