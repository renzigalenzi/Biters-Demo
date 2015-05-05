using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioClip NewMusic; //Pick an audio track to play.
	
	void Awake () 
	{ 	var go = GameObject.Find("Game Music"); //Finds the game object called Game Music, if it goes by a different name, change this. 
		go.GetComponent<AudioSource>().clip = NewMusic;//Replaces the old audio with the new one set in the inspector. 
		go.GetComponent<AudioSource>().Play(); //Plays the audio. 
	}
}
