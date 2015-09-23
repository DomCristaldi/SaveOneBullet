using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NoteManager : MonoBehaviour {

    public List<string> allNotes;

    void Awake() {
        allNotes = new List<string>();

        foreach (TextAsset tAsset in Resources.LoadAll<TextAsset>("Writing")) {
            allNotes.Add(tAsset.text);
        }
             
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
