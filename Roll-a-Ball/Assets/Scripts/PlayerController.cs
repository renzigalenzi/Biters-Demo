using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed; 
	public Text countText; 
	public Text winText; 
	private int count;
	public float timer = 10.0f; 

	void Start() { 

			count = 0; 
			// SetCountText (); 
			winText.text = "";

		} // end 
	
	void FixedUpdate() { 

		float moveHorizontal = Input.GetAxis ("Horizontal"); 
		float moveVertical = Input.GetAxis ("Vertical"); 

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical); 

		rigidbody.AddForce(movement * speed * Time.deltaTime); 
	} // end 

	void OnTriggerEnter(Collider other) { 

		if (other.gameObject.tag == "Pickup") { 

			other.gameObject.SetActive (false); 
			count = count + 1; 
			SetCountText(); 

		} //end if 

	} // end 

	void SetCountText() { 

		// countText.text = "Count: " + count.ToString (); 
		if (count >= 11) { 

			winText.text = "You win!"; 

		} // end if statement 
		
	} // end 

	// Use this for initialization
	void OnGUI () {
		
		GUI.Box (new Rect (Screen.width - 50, 0, 50, 20), "" + timer.ToString ("f0")); 
		GUI.Box (new Rect (0, 0, 100, 20), "Count: " + count.ToString ()); 

		if (Application.loadedLevelName == "mini-game") { 

			GUI.Box (new Rect (Screen.width/2, 0, 50, 20), "Level 1"); 

		} else if (Application.loadedLevelName == "Lvl2") { 

			GUI.Box (new Rect (Screen.width/2, 0, 50, 20), "Level 2");

		} // end if else

		if (timer <= 0) { 

			Time.timeScale = 0.00001f;
			if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 60, 100, 25), "Try Again?")) {
				 
				Time.timeScale = 1.0f; 
				Application.LoadLevel (Application.loadedLevelName); 
				
			} // end inner if 

		} // end outer if 
		
	} // end OnGUI
	
	// Update is called once per frame
	void Update () {

		// Time.timeScale = 1.0f; 
		
		if (count < 11) timer -= Time.deltaTime; 

		if (count >= 11) { 

			if (Application.loadedLevelName == "mini-game") { 
				
				Application.LoadLevel("Lvl2");  
				
			} // end if statement 

		} // end if statement  
		
		if (timer <= 0) { 
			
			timer = 0.0f; 
			
		} // end if  
		
	} // end Update 

} // end 
