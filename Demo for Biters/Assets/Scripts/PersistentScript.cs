using UnityEngine;
using System.Collections;

public class PersistentScript : MonoBehaviour {

	public string SelectedLevel{ get; set; }



	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
