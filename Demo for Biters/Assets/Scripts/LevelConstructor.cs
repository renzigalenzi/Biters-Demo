using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Collections.Specialized;

public class LevelConstructor : MonoBehaviour {


	public GameObject[,] LevelConstructorGridSquareGameObjectGrid { get; set; }    // Textures
	public GridSquare[,] LevelConstructorGridSquareGrid { get; set; }// Functionality
	public Dictionary<string,Material> MaterialDictionary { get; set; }
	public int LevelConstructorGridWidth { get; set; }
	public int LevelConstructorGridHeight { get; set; }
	public List<Monster> LevelConstructorMonsters { get; set; }
	public int LevelConstructorNextMonsterId { get; set; }
	public int LevelConstructorSpawnDelay { get; set; }
	
	public AudioClip gateSound;

	public Material SelectedMaterial;
	public TileType SelectedTile;

	public const int XOFFSET = 4;
	public const int YOFFSET = 4;
	private Vector2 scrollPosition = Vector2.zero;
	private bool boolDragging = false;
	private bool Saving = false;
	private string SaveFileName = "Level - ";
	private bool Loading = false;
	private string LoadFileName = "Level - ";
	public int[] LastClickedPoint{ get; set; }
	
	public void Start()
	{
		MaterialDictionary = new Dictionary<string, Material> ();
		UnityEngine.Object[] Materials = Resources.LoadAll("", typeof(Material));
		//Object[] Materials = Resources.LoadAll("Materials", typeof(Material));
		foreach(Material mat in Materials)
		{
			//check if material is a tile
			for(int tile = 0; tile < (int)TileType.Count; tile ++)
			{
				if(mat.name == Enum.GetName(typeof(TileType),(TileType)tile))
					MaterialDictionary.Add(mat.name, mat);
			}
		}

		SelectedMaterial = MaterialDictionary["Blank"];
		SelectedTile = TileType.Blank;

		LoadBlankMap();
	}

	public void AffectMap (int colAffect, int rowAffect)
	{

		int oldx = LevelConstructorGridWidth;
		int oldy = LevelConstructorGridHeight;
		int newx = LevelConstructorGridWidth + rowAffect;
		int newy = LevelConstructorGridHeight + colAffect;

		GameObject[,] tempMap = new GameObject[newx, newy];
		GridSquare[,] tempGrid = new GridSquare[newx, newy];
		LastClickedPoint = new int[] {-1,-1};

		for (int x = 0; x <newx; x++)
		{
			for( int y = 0; y < newy; y++)
			{
				tempMap[x,y]=GameObject.CreatePrimitive(PrimitiveType.Cube);
				tempGrid[x,y] = new GridSquare(tempMap[x,y]);
				tempGrid[x,y].GridSquareXPosition = x;
				tempGrid[x,y].GridSquareYPosition = y;
				tempGrid[x,y].GridSquareGameObject.transform.position = new Vector3(x - XOFFSET, YOFFSET - y, 0);
				tempGrid[x,y].GridSquareGameObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);

				if(x < newx && x < oldx && y < newy && y < oldy)
				{
					tempGrid[x,y].GridSquareTileType = LevelConstructorGridSquareGrid[x,y].GridSquareTileType;
					tempGrid[x,y].GridSquareGameObject.renderer.material = 
						LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.renderer.material;
				}
				else
				{
					tempGrid[x,y].GridSquareTileType = TileType.Blank;
					tempGrid[x,y].GridSquareGameObject.renderer.material = MaterialDictionary["Blank"];
				}
			}
		}
		foreach (var tile in LevelConstructorGridSquareGrid) 
		{
			Destroy(tile.GridSquareGameObject.gameObject);
		}
		LevelConstructorGridSquareGameObjectGrid = tempMap;
		LevelConstructorGridSquareGrid = tempGrid;
		LevelConstructorGridWidth += rowAffect;
		LevelConstructorGridHeight += colAffect;
	}
	public void LoadBlankMap()
	{
		LevelConstructorGridWidth = 8;
		LevelConstructorGridHeight = 8;

		LevelConstructorGridSquareGameObjectGrid = new GameObject[LevelConstructorGridWidth, LevelConstructorGridHeight];
		LevelConstructorGridSquareGrid = new GridSquare[LevelConstructorGridWidth, LevelConstructorGridHeight];
		LastClickedPoint = new int[] {-1,-1};
		for(int y = 0; y < LevelConstructorGridHeight; y++)
		{
			for(int x = 0; x < LevelConstructorGridWidth; x++)
			{
				LevelConstructorGridSquareGameObjectGrid[x,y]=GameObject.CreatePrimitive(PrimitiveType.Cube);
				LevelConstructorGridSquareGrid[x,y] = new GridSquare(LevelConstructorGridSquareGameObjectGrid[x,y]);
				LevelConstructorGridSquareGrid[x,y].GridSquareTileType = TileType.Blank;
				LevelConstructorGridSquareGrid[x,y].GridSquareXPosition = x;
				LevelConstructorGridSquareGrid[x,y].GridSquareYPosition = y;
				LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.transform.position = new Vector3(x - XOFFSET, YOFFSET - y, 0);
				LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
				LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.renderer.material = MaterialDictionary["Blank"];
			}
		}
	}
	public bool LoadLevel(string fileName)
	{
		LastClickedPoint = new int[] {-1,-1};
		string filePath = "Assets/Levels/" + fileName + ".csv";
		try
		{
			InitializeGrid(filePath);
			StreamReader reader = new StreamReader(filePath, Encoding.Default);
			using(reader)
			{
				int y = 0;
				string line = reader.ReadLine();
				while(line != null)
				{
					string[] entries = line.Split(',');
					for(int x = 0; x < entries.Length; x++)
					{
						int intType = Convert.ToInt32(entries[x]);
						LevelConstructorGridSquareGameObjectGrid[x,y]=GameObject.CreatePrimitive(PrimitiveType.Cube);
						LevelConstructorGridSquareGrid[x,y] = new GridSquare(LevelConstructorGridSquareGameObjectGrid[x,y]);
						LevelConstructorGridSquareGrid[x,y].GridSquareTileType = (TileType)intType;
						LevelConstructorGridSquareGrid[x,y].GridSquareXPosition = x;
						LevelConstructorGridSquareGrid[x,y].GridSquareYPosition = y;
						LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.transform.position = new Vector3(x - XOFFSET, YOFFSET - y, 0);
						LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
						LevelConstructorGridSquareGrid[x,y].AssignMaterialToLevelConstructor(this);
					}
					y++;
					line = reader.ReadLine();
				}
				reader.Close();
				return true;
			}
		}
		catch(Exception e)
		{
			Console.WriteLine("There was a problem during file reading", e.Message);
			return false;
		}
	}
	private void InitializeGrid(string filePath)
	{
		LevelConstructorGridWidth = 0;
		LevelConstructorGridHeight = 0;
		StreamReader reader = new StreamReader(filePath, Encoding.Default);
		using(reader)
		{
			string line = reader.ReadLine();
			if(line != null)
			{
				string[] entries = line.Split(',');
				LevelConstructorGridWidth = entries.Length;
				LevelConstructorGridHeight++;
			}
			line = reader.ReadLine();
			while(line != null)
			{
				LevelConstructorGridHeight++;
				line = reader.ReadLine();
			}
		}
		foreach(GameObject obj in LevelConstructorGridSquareGameObjectGrid)
		{
			Destroy(obj);
		}
		LevelConstructorGridSquareGameObjectGrid = new GameObject[LevelConstructorGridWidth, LevelConstructorGridHeight];
		LevelConstructorGridSquareGrid = new GridSquare[LevelConstructorGridWidth, LevelConstructorGridHeight];
	}
	
	void Update () 
	{
		GetMouseRays();
	}
	void OnMouseDrag()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 100))
		{
			GameObject pObject = hit.transform.gameObject;
			if(LastClickedPoint[0] != -1 && WithinOneTile(pObject) && SelectedMaterial != MaterialDictionary["Blank"] )
			{
				GetNewMaterial(pObject);
			}
			LastClickedPoint = new int[]{XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y};

			UpdateTilesAroundClicked();

			pObject.renderer.material = SelectedMaterial;
			LevelConstructorGridSquareGrid[XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y].GridSquareTileType = SelectedTile;

		}
	}
	bool WithinOneTile(GameObject pObject)
	{
		int x = XOFFSET + (int)pObject.transform.position.x;
		int y = YOFFSET - (int)pObject.transform.position.y;

		if (x + 1 == LastClickedPoint [0] && y == LastClickedPoint [1] || 
						x - 1 == LastClickedPoint [0] && y == LastClickedPoint [1] || 
						x == LastClickedPoint [0] && y + 1 == LastClickedPoint [1] || 
						x == LastClickedPoint [0] && y - 1 == LastClickedPoint [1])
			return true;
		else
			return false;

	}

	void GetNewMaterial(GameObject pObject)
	{
		int x = XOFFSET + (int)pObject.transform.position.x;
		int y = YOFFSET - (int)pObject.transform.position.y;

		if( x !=  LastClickedPoint[0] )
		{
			SelectedMaterial = MaterialDictionary["BeltHorizontal"];
			for(int tile = 0; tile < (int)TileType.Count; tile ++)
			{
				if("BeltHorizontal" == Enum.GetName(typeof(TileType),(TileType)tile))
					SelectedTile = (TileType)tile;
			}
		}
		if( y !=  LastClickedPoint[1] )
		{
			SelectedMaterial = MaterialDictionary["BeltVertical"];
			for(int tile = 0; tile < (int)TileType.Count; tile ++)
			{
				if("BeltVertical" == Enum.GetName(typeof(TileType),(TileType)tile))
					SelectedTile = (TileType)tile;
			}
		}

	}
	void UpdateTilesAroundClicked()
	{
		int x = LastClickedPoint [0];
		int y = LastClickedPoint [1];

		if (SelectedMaterial == MaterialDictionary ["BeltHorizontal"]) 
		{
			if (x < LevelConstructorGridWidth -1)
				ChangePiece(x+1,y,x,y, TileDirection.Left);
			if (x > 0)
				ChangePiece(x-1,y,x,y, TileDirection.Right);
		}
		if (SelectedMaterial == MaterialDictionary ["BeltVertical"]) 
		{
			if (y < LevelConstructorGridHeight -1)
				ChangePiece(x,y+1,x,y, TileDirection.Up);
			if(y > 0)
				ChangePiece(x,y-1,x,y, TileDirection.Down);
		}
	}
	void ChangePiece(int Xnew, int Ynew, int Xclicked, int Yclicked, TileDirection direction)
	{
		if(LevelConstructorGridSquareGrid[Xclicked,Yclicked].TileAccepts(LevelConstructorGridSquareGrid[Xnew,Ynew], direction)
		   || !LevelConstructorGridSquareGrid[Xnew,Ynew].IsBelt())
		{
			return;
		}
		else//find out what it connects to already and append it
		{
			Material tempMat = SelectedMaterial;
			TileType tempTile = SelectedTile;

			LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities.Clear();
			LevelConstructorGridSquareGrid[Xnew,Ynew].AssignDirection(LevelConstructorGridSquareGrid, LevelConstructorGridWidth, LevelConstructorGridHeight);
			if(LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities.Count == 3)
				return;
			else if(LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities.Count == 2)
			{
				return;
			}
			else if(LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities.Count == 1)
			{
				//find who can accept the direction, make it from there to here
				int Xmove = LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities[0].GridSquareXPosition;
				int Ymove = LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities[0].GridSquareYPosition;

				makeCorner(Xnew - Xmove, Ynew-Ymove, Xnew-Xclicked, Ynew-Yclicked, ref tempMat, ref tempTile);
			}



			LevelConstructorGridSquareGrid[Xnew,Ynew].GridSquareGameObject.renderer.material = tempMat;
			LevelConstructorGridSquareGrid[Xnew,Ynew].GridSquareTileType = tempTile;
				
		}
	}
	void makeCorner(int dX1, int dY1, int dX2, int dY2, ref Material tempMat, ref TileType tempTile)
	{
		string key = "BeltUpRight";

		if( ( dX1  == -1 && dY2 == -1 ) || ( dY1 == -1 && dX2 == -1) )
		{
			key = "BeltUpRight";
		}
		if( ( dX1  == +1 && dY2 == -1 ) || ( dY1 == -1 && dX2 == +1) )
		{
			key = "BeltUpLeft"; 
		}
		if( ( dX1  == -1 && dY2 == +1 ) || ( dY1 == +1 && dX2 == -1) )
		{
			key = "BeltDownRight"; 
		}
		if( ( dX1  == +1 && dY2 == +1 ) || ( dY1 == +1 && dX2 == +1) )
		{
			key = "BeltDownLeft"; 
		}

		tempMat = MaterialDictionary [key]; 
		for(int tile = 0; tile < (int)TileType.Count; tile ++)
		{
			if(key == Enum.GetName(typeof(TileType),(TileType)tile))
				tempTile = (TileType)tile;
		}
	}
	void GetMouseRays()
	{
		if(!Saving && !Loading && Input.GetMouseButtonDown(0))
		{
			boolDragging = true;
		}
		if (Input.GetMouseButtonUp (0)) 
		{
			boolDragging = false;
			LastClickedPoint = new int[]{-1,-1};
		}
		if (boolDragging)
		{
			OnMouseDrag();
		}
		GameObject camera = GameObject.Find ("Main Camera");
		int moveX = 0;
		int moveY = 0;
		float magicNumber = 40;

		if( Input.mousePosition.x < magicNumber && Input.mousePosition.x >= 0 )
			moveX = (int)(-1*magicNumber/(Input.mousePosition.x+1));
		if( Input.mousePosition.x > Screen.width - magicNumber && Input.mousePosition.x <= Screen.width)
			moveX = (int)(1*magicNumber/(Screen.width-Input.mousePosition.x+1));
		if( Input.mousePosition.y < magicNumber && Input.mousePosition.y >= 0 ) 
			moveY = (int)(-1*magicNumber/(Input.mousePosition.y+1));
		if( Input.mousePosition.y > Screen.height - magicNumber && Input.mousePosition.y <= Screen.height ) 
			moveY = (int)(1 * 20 / (Screen.height - Input.mousePosition.y + 1));
		if (Input.GetAxis ("Mouse ScrollWheel") != 0) 
		{
			moveY = (int)(20*Input.GetAxis ("Mouse ScrollWheel"));
		}

		camera.transform.position += new Vector3(1/magicNumber*moveX, 1/magicNumber* moveY , 0f);
		MakeSureCameraCanSeeMap ();

	}
	void MakeSureCameraCanSeeMap ()
	{
		float x = camera.transform.position.x;
		float y = camera.transform.position.y;
		float z = camera.transform.position.z;
		if(x < XOFFSET - 5)
			camera.transform.position = new Vector3(XOFFSET - 5f, y, z);
		if(x > LevelConstructorGridWidth)
			camera.transform.position = new Vector3(LevelConstructorGridWidth, y, z);
		if(y < YOFFSET - LevelConstructorGridHeight)
			camera.transform.position = new Vector3(x, YOFFSET - LevelConstructorGridHeight, z);
		if(y > YOFFSET )
			camera.transform.position = new Vector3(x, YOFFSET, z);

	}
	void OnGUI () 
	{
		MakeScrollView ();
		MakeMapControls ();
		MakeSelected ();
		MakeExportButton ();
		MakeLoadLevelButton ();
		MakeExitButton ();
	}
	void MakeExitButton()
	{
		if(GUI.Button(new Rect(Screen.width- 140, 0, 100, 20),"Main Menu"))
		{
			Application.LoadLevel ("MainMenu"); 
		}
	}
	void MakeLoadLevelButton()
	{
		if(GUI.Button (new Rect (Screen.width- 240, 40, 100, 20), "Load"))
		{
			Loading = true;
		}
		if(Loading)
		{
			LoadFileName = GUI.TextField(new Rect (Screen.width/2-100, Screen.height/2-20, 200, 40), LoadFileName);
			if (GUI.Button (new Rect (Screen.width/2-100, Screen.height/2+20, 100, 40), "LOAD")) 
			{
				LoadLevel(LoadFileName);
				print ("loaded file : " + LoadFileName);
				Loading = false;
			}
			if (GUI.Button (new Rect (Screen.width/2, Screen.height/2+20, 100, 40), "CANCEL")) 
			{
				Loading = false;
			}
		}
	}
	void MakeExportButton()
	{
		if (GUI.Button (new Rect (Screen.width- 140, 40, 100, 20), "EXPORT")) 
		{
			Saving = true;
		}
		if(Saving)
		{

			SaveFileName = GUI.TextField(new Rect (Screen.width/2-100, Screen.height/2-20, 200, 40), SaveFileName);
			if (GUI.Button (new Rect (Screen.width/2-100, Screen.height/2+20, 100, 40), "SAVE")) 
			{
				SaveFile(SaveFileName);
				print ("saved file : " + SaveFileName);
				Saving = false;
			}
			if (GUI.Button (new Rect (Screen.width/2, Screen.height/2+20, 100, 40), "CANCEL")) 
			{
				Saving = false;
			}
		}
	}
	void MakeMapControls()
	{
		if (GUI.Button (new Rect (Screen.width- 100, 100, 60, 20), "+Row")) 
		{
			AffectMap(0,1);
		}
		if (GUI.Button (new Rect (Screen.width- 100, 125, 60, 20), "-Row")) 
		{
			AffectMap(0,-1);
		}
		if (GUI.Button (new Rect (Screen.width- 100, 150, 60, 20), "+Col")) 
		{
			AffectMap(1,0);
		}
		if (GUI.Button (new Rect (Screen.width- 100, 175, 60, 20), "-Col")) 
		{
			AffectMap(-1,0);
		}
	}
	void MakeSelected()
	{
		GUI.Box (new Rect (Screen.width - 100, Screen.height - 100, 60, 60), SelectedMaterial.mainTexture);
	}
	public void SaveFile(string fileName)
	{
		string filePath = "Assets/Levels/" + fileName + ".csv";
		try
		{
			StreamWriter writer = new StreamWriter(filePath);
			using(writer)
			{
				for(int y = 0; y < LevelConstructorGridHeight; y++)
				{
					for(int x = 0; x < LevelConstructorGridWidth; x++)
					{
						if(x == LevelConstructorGridWidth -1)
							writer.Write((int)LevelConstructorGridSquareGrid[x,y].GridSquareTileType);
						else
							writer.Write((int)LevelConstructorGridSquareGrid[x,y].GridSquareTileType + ",");
					}
					writer.Write("\n");
				}
			}
		}
		catch(Exception e)
		{
			Console.WriteLine("There was a problem during file reading", e.Message);
		}
	}
	void MakeScrollView()
	{
		int numButtons = (int)MaterialDictionary.Count;
		scrollPosition = GUI.BeginScrollView(new Rect(Math.Min(Screen.width,40), Math.Min(Screen.height,20),
		                                              Math.Min(Screen.width-10,80), Screen.height-20), 
		                                     scrollPosition, new Rect(0, 0, 100, numButtons * 70));
		int i = 0;
		foreach(string key in MaterialDictionary.Keys)
		{
			if(GUI.Button(new Rect(0, i*70, 60, 60),MaterialDictionary[key].mainTexture))
			{
				SelectedMaterial = MaterialDictionary[key]; 
				for(int tile = 0; tile < (int)TileType.Count; tile ++)
				{
					if(key == Enum.GetName(typeof(TileType),(TileType)tile))
						SelectedTile = (TileType)tile;
				}
			}
			i++;
		}
		GUI.EndScrollView();
	}
	/*public bool IsBeltMaterial(Material m)
	{
		List<TileType> Belts = new List<Materials> (new TileType[] {TileType.BeltVertical, TileType.BeltHorizontal, 
			TileType.BeltUpLeft, TileType.BeltUpRight, TileType.BeltDownRight, TileType.BeltDownLeft, 
			TileType.BeltUpT, TileType.BeltRightT, TileType.BeltDownT, TileType.BeltLeftT});
		
		return Belts.Contains (GridSquareTileType);
	}*/
	public static void PrintMessage(string printStatement)
	{
		print (printStatement);
	}
}
