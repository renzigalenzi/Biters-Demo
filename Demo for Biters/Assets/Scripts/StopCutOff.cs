using UnityEngine;
using System.Collections;

public class StopCutOff : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	} // end Awake

	// Use this for initialization
	void Start () {
	
	} // end Start
	
	// Update is called once per frame
	void Update () {
	
	} // end Update

} // end StopCutOff

