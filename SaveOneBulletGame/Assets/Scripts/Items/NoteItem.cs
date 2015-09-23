using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Items/NoteItem")]
public class NoteItem : ItemBase {
    /*
    [System.Serializable]
    public class NoteText {
        public TextAsset text;
        public bool hasBeenUsed;
    }
    
    public List<NoteText> notes;
    */
    //public List<
    //public TextAsset test;

    public NoteUIHandler noteUI;

    public int noteIndex;
    public List<string> collectedNotes;

    public string currentNote;

    
    protected override void Awake() {
        base.Awake();

        collectedNotes = new List<string>();
    }


    void Update() {
        AssignCurrentlyViewedNote();
    }
        

    public void AddNote(string note) {
        collectedNotes.Add(note);
        noteIndex = collectedNotes.Count - 1;

        AssignCurrentlyViewedNote(noteIndex);
    }

    
    private void AssignCurrentlyViewedNote(int index) {
        if (index > 0) {
            noteUI.SetText(collectedNotes[index]);
        }
    }
    
    private void AssignCurrentlyViewedNote() {
        AssignCurrentlyViewedNote(noteIndex);
    }

    public override void Use() {
        base.Use();
        
        if (collectedNotes.Count > 0) {
            ++noteIndex;
            if (noteIndex == collectedNotes.Count) {
                noteIndex = 0;
            }
            AssignCurrentlyViewedNote(noteIndex);
        }
        
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
