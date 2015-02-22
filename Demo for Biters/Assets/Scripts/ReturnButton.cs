using UnityEngine;
using System.Collections;

public class ReturnButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	} // end Start
	
	public void OnClick() {
		
		Application.LoadLevel ("MainMenu"); 
		
	} // end OnClick

} // end ReturnButton
