using UnityEngine;
using System.Collections;

public class CamraController : MonoBehaviour {

	public GameObject player;
	private Vector3 offset; 

	// Use this for initalization 
	void Start() { 

		offset = transform.position; 

	} // end start 

	void LateUpdate() { 

		transform.position = player.transform.position + offset; 

	} // end LateUpdate  


}
