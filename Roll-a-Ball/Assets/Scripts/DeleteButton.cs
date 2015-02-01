using UnityEngine;
using System.Collections;

public class DeleteButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	} // end Start
	
	public void OnClick() {
		
		PlayerPrefs.DeleteAll (); 
		
	} // end OnClick

} // end DeleteButton
