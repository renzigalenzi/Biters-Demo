using UnityEngine;
using System.Collections;
using System.IO;

public class Movie : MonoBehaviour {

	private Texture2D[] slides;
	private FileInfo[] info; 
	private int currentSlide = 0;
	private Texture currTex;
	// Original Path Extension: "/Users/laurenfuller/Documents/Biters-Demo/DemoBiters/Assets/Resources/Panel1"
	private string myPath = Directory.GetCurrentDirectory () + "/Assets/Resources/Clip1";
	public string extention = "png";   
	public int start2;
	public int start3;
	public int start4;
	public int start5; 
	public AudioSource sound1;
	public AudioSource sound2;
	public AudioSource sound3;
	public AudioSource sound4; 
	public AudioSource sound5; 
	private bool play = true; 
	private int counter = 101; 
	private int totalFrames; 
	
	void Start() {

		Debug.Log ("Locating files, please standby.");
		getFiles(); 
		Application.targetFrameRate = 24; 

		if (slides != null) {
			
			currTex = slides[currentSlide] as Texture;
			currentSlide++;
			sound1.Play(); 

		} else {

			Debug.Log ("Error 404: File Not Found.");

		} // end if statement 

	} // end void Start() 
	
	void Update() {

		if (slides != null)  {

			/*
			if (counter < (totalFrames-1)) { 

				string filePath = info[counter].Directory + "/" + info[counter].Name;
				Debug.Log("["+counter+"] file found: "+filePath);
				
				var bytes = System.IO.File.ReadAllBytes(filePath);
				var tex = new Texture2D(1, 1);
				
				tex.LoadImage(bytes);
				slides[counter] = tex;
				
				counter++; 

				filePath = info[counter].Directory + "/" + info[counter].Name;
				Debug.Log("["+counter+"] file found: "+filePath);
				
				bytes = System.IO.File.ReadAllBytes(filePath);
				tex = new Texture2D(1, 1);
				
				tex.LoadImage(bytes);
				slides[counter] = tex;

				counter++; 

			} // end if statement 
			*/

			// start audio files 
			if (currentSlide == start2) 
				sound2.Play(); 
			if (currentSlide == start3)
				sound3.Play(); 
			if (currentSlide == start4)
				sound4.Play(); 
			if (currentSlide == start5)
				sound5.Play(); 

			if(play && currentSlide < slides.Length) {
		
				currTex = slides[currentSlide] as Texture;
				currentSlide++;
				play = false; 

			} else { 

				play = true; 

			} // end if statement inner	

		} // end if statement outer

		Debug.Log("The FPS is: " + ((1.0f / Time.smoothDeltaTime))); 

	} // end Update()

	void OnGUI() {

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), currTex, ScaleMode.ScaleToFit, true, 2.0f);

	} // end OnGUI()
	
	void getFiles() {

		if (Directory.Exists(myPath)) {

			DirectoryInfo dir = new DirectoryInfo(myPath);
			Debug.Log("Looking for files in directory: " + myPath);
			
			info = dir.GetFiles("*." + extention);
			
			// Get number of files, and set the length for the texture2d array
			int totalFiles =  info.Length;
			totalFrames = info.Length; 
			slides = new Texture2D[totalFiles];

			int i = 0;
			
			//Read all found files
			foreach (FileInfo f in info) {

				string filePath = f.Directory + "/" + f.Name;
				Debug.Log("["+i+"] file found: "+filePath);
				
				var bytes = System.IO.File.ReadAllBytes(filePath);
				var tex = new Texture2D(1, 1);
				
				tex.LoadImage(bytes);
				slides[i] = tex;
				
				i++;

				//if (i == 101) 
				//	break; 

			} // end foreach statement 

		} else {

			Debug.Log ("Directory DOES NOT exist! ");

		} // end if else statement 

	} // end void getFiles()

} // end Movie.cs 