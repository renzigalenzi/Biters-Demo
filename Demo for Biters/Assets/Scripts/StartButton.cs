using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Time.timeScale = 1; 
		AudioListener.volume = 50; 
	
	}
	
	// Update is called once per frame
	public void OnClick () {

		Application.LoadLevel ("Demo");
	
	}
}
