using UnityEngine;
using UnityEngine.UI;

public class NoteUIHandler : MonoBehaviour {

    private Text uiText;

    public string text;

    void Awake() {
        uiText = GetComponent<Text>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void AssignTextToUI() {
        uiText.text = text;
    }

    public void SetText(TextAsset tAsset) {
        this.text = tAsset.text;
        AssignTextToUI();

    }

    public void  SetText(string s) {
        text = s;
        AssignTextToUI();

    }
}