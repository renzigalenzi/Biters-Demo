using UnityEngine;
using System.Collections;

public class ReturnButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	} // end Start
	
	public void OnClick() {
		
		Application.LoadLevel ("PlayerMenu"); 
		
	} // end OnClick

} // end ReturnButton
