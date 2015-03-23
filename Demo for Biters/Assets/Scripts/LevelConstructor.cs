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
	public List<RotationGroup> LevelConstructorRotationGroups{ get; set; }
	
	public AudioClip gateSound;

	public Material SelectedMaterial;
	public TileType SelectedTile;
	public RotationGroup currentRG;

	public const int XOFFSET = 4;
	public const int YOFFSET = 4;
	private Vector2 TileScrollPosition = Vector2.zero;
	private Vector2 GroupScrollPosition = Vector2.zero;
	private bool boolDragging = false;
	private bool Saving = false;
	private string SaveFileName = "Level - ";
	private bool Loading = false;
	private string LoadFileName = "Level - ";
	public int[] LastClickedPoint{ get; set; }

	private int RGXClicked = -1;
	private int RGYClicked = -1;
	//Light lightComp = lightGameObject.AddComponent<Light>();
	//lightComp.color = Color.blue;
	//lightGameObject.transform.position = new Vector3(0, 5, 0);/// <summary>
	/// S//////////////////	/// </summary>


	public void Start()
	{
		MaterialDictionary = new Dictionary<string, Material> ();
		UnityEngine.Object[] Materials = Resources.LoadAll("", typeof(Material));
		//Object[] Materials = Resources.LoadAll("Materials", typeof(Material));

		foreach(Material mat in Materials)
		{
			MaterialDictionary.Add(mat.name, mat);
		}

		SelectedMaterial = MaterialDictionary["Blank"];
		SelectedTile = TileType.Blank;
		currentRG = new RotationGroup ();

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
					tempGrid[x,y].GridSquareGameObject.GetComponent<Renderer>().material = 
						LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.GetComponent<Renderer>().material;
				}
				else
				{
					tempGrid[x,y].GridSquareTileType = TileType.Blank;
					tempGrid[x,y].GridSquareGameObject.GetComponent<Renderer>().material = MaterialDictionary["Blank"];
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
		LevelConstructorRotationGroups = new List<RotationGroup> ();
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
				LevelConstructorGridSquareGrid[x,y].OriginalRotationValue = LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.transform.rotation;
				LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.GetComponent<Renderer>().material = MaterialDictionary["Blank"];
				LevelConstructorGridSquareGrid[x,y].RotateSquareByMaterial();
			}
		}
	}
	public bool LoadLevel(string fileName)
	{
		LevelConstructorRotationGroups = new List<RotationGroup> ();
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
				while(line != null && line[0] != '#')
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
						LevelConstructorGridSquareGrid[x,y].OriginalRotationValue = LevelConstructorGridSquareGrid[x,y].GridSquareGameObject.transform.rotation;
						LevelConstructorGridSquareGrid[x,y].AssignMaterialToLevelConstructor(this);
						LevelConstructorGridSquareGrid[x,y].RotateSquareByMaterial();
					}
					y++;
					line = reader.ReadLine();
				}
				if(line != null && line[0] == '#')
				{
					do
					{
						RotationGroup g = new RotationGroup();
						line = line.Trim( new char[] {'#'});
						string[] entries = line.Split(',');
						for(int x = 0; x < entries.Length; x+=2)
						{
							g.Add(Convert.ToInt32(entries[x]), Convert.ToInt32(entries[x+1]));
						}
						LevelConstructorRotationGroups.Add(g);
						line = reader.ReadLine();
					}while(line != null);
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
			if(line != null && line[0] != '#')
			{
				string[] entries = line.Split(',');
				LevelConstructorGridWidth = entries.Length;
				LevelConstructorGridHeight++;
			}
			line = reader.ReadLine();
			while(line != null && line[0] != '#')
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
			if(SelectedMaterial != MaterialDictionary["RG"])
			{
				if(LastClickedPoint[0] != -1 && WithinOneTile(pObject) && SelectedMaterial != MaterialDictionary["Blank"] )
				{
					GetNewMaterial(pObject);
				}
				LastClickedPoint = new int[]{XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y};

				UpdateTilesAroundClicked();

				pObject.GetComponent<Renderer>().material = SelectedMaterial;
				LevelConstructorGridSquareGrid[XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y].GridSquareTileType = SelectedTile;
				LevelConstructorGridSquareGrid[XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y].RotateSquareByMaterial ();
			}
			else
			{
				RGXClicked = XOFFSET + (int)pObject.transform.position.x;
				RGYClicked = YOFFSET - (int)pObject.transform.position.y;
			}
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

		if (IsBeltMaterial(SelectedMaterial))
		{
			TileType temp = LevelConstructorGridSquareGrid[x,y].GridSquareTileType;
			for(int tile = 0; tile < (int)TileType.Count; tile ++)
			{
				if(SelectedMaterial.name == Enum.GetName(typeof(TileType),(TileType)tile))
					LevelConstructorGridSquareGrid[x,y].GridSquareTileType = (TileType)tile;
			}
			if (x < LevelConstructorGridWidth -1 && LevelConstructorGridSquareGrid[x,y].TileAccepts(LevelConstructorGridSquareGrid[x,y], TileDirection.Left))
			{
				LevelConstructorGridSquareGrid[x,y].GridSquareTileType = temp;
				ChangePiece(x+1,y,x,y, TileDirection.Right);
			}
			if (x > 0 && LevelConstructorGridSquareGrid[x,y].TileAccepts(LevelConstructorGridSquareGrid[x,y], TileDirection.Right))
			{
				LevelConstructorGridSquareGrid[x,y].GridSquareTileType = temp;
				ChangePiece(x-1,y,x,y, TileDirection.Left);
			}
			if (y < LevelConstructorGridHeight -1 && LevelConstructorGridSquareGrid[x,y].TileAccepts(LevelConstructorGridSquareGrid[x,y], TileDirection.Up))
			{
				LevelConstructorGridSquareGrid[x,y].GridSquareTileType = temp;
				ChangePiece(x,y+1,x,y, TileDirection.Down);
			}
			if(y > 0 && LevelConstructorGridSquareGrid[x,y].TileAccepts(LevelConstructorGridSquareGrid[x,y], TileDirection.Down))
			{
				LevelConstructorGridSquareGrid[x,y].GridSquareTileType = temp;
				ChangePiece(x,y-1,x,y, TileDirection.Up);
			}
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
			{
				tempMat = MaterialDictionary["BeltCross"];
				tempTile = TileType.BeltCross;
			}
			else if(LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities.Count == 2)
			{
				MakeT(Xnew, Ynew, Xclicked, Yclicked, ref tempMat, ref tempTile);
			}
			else if(LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities.Count == 1)
			{
				//find who can accept the direction, make it from there to here
				int Xmove = LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities[0].GridSquareXPosition;
				int Ymove = LevelConstructorGridSquareGrid[Xnew,Ynew].MovePossibilities[0].GridSquareYPosition;

				MakeCorner(Xnew - Xmove, Ynew-Ymove, Xnew-Xclicked, Ynew-Yclicked, ref tempMat, ref tempTile);
			}



			LevelConstructorGridSquareGrid[Xnew,Ynew].GridSquareGameObject.GetComponent<Renderer>().material = tempMat;
			LevelConstructorGridSquareGrid[Xnew,Ynew].GridSquareTileType = tempTile;
			LevelConstructorGridSquareGrid[Xnew,Ynew].RotateSquareByMaterial ();
				
		}
	}
	void MakeCorner(int dX1, int dY1, int dX2, int dY2, ref Material tempMat, ref TileType tempTile)
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
		if( dX1  == 0 && dX2 == 0 ) 
		{
			key = "BeltVertical"; 
		}
		if( dY1  == 0 && dY2 == 0 )
		{
			key = "BeltHorizontal"; 
		}

		tempMat = MaterialDictionary [key]; 
		for(int tile = 0; tile < (int)TileType.Count; tile ++)
		{
			if(key == Enum.GetName(typeof(TileType),(TileType)tile))
				tempTile = (TileType)tile;
		}
	}
	void MakeT (int x, int y, int Xclicked, int Yclicked, ref Material tempMat, ref TileType tempTile)
	{
		List<TileDirection> missingDirection = new List<TileDirection> ()
		{TileDirection.Down,TileDirection.Up,TileDirection.Right,TileDirection.Left};

		string key = "BeltUpT";
		int Xmove;
		int Ymove;
		int dx, dy;
		Xmove = Xclicked;
		Ymove = Yclicked;
		dx = x - Xmove;
		dy = y-Ymove;
		if (dx == -1)
			missingDirection.Remove(TileDirection.Left);
		if (dx == 1)
			missingDirection.Remove(TileDirection.Right);
		if (dy == -1)
			missingDirection.Remove(TileDirection.Up);
		if (dy == 1)
			missingDirection.Remove(TileDirection.Down);
		foreach (GridSquare square in LevelConstructorGridSquareGrid[x,y].MovePossibilities) 
		{
			Xmove = square.GridSquareXPosition;
			Ymove = square.GridSquareYPosition;
			dx = x - Xmove;
			dy = y-Ymove;
			if (dx == -1)
				missingDirection.Remove(TileDirection.Left);
			if (dx == 1)
				missingDirection.Remove(TileDirection.Right);
			if (dy == -1)
				missingDirection.Remove(TileDirection.Up);
			if (dy == 1)
				missingDirection.Remove(TileDirection.Down);
		}
		if (missingDirection.Count != 1)
			key = Enum.GetName(typeof(TileType),LevelConstructorGridSquareGrid[x,y].GridSquareTileType);
		else if (missingDirection[0] == TileDirection.Left)
			key = "BeltRightT";
		else if (missingDirection[0] == TileDirection.Right)
			key = "BeltLeftT";
		else if (missingDirection[0] == TileDirection.Up)
			key = "BeltDownT";
		else if (missingDirection[0] == TileDirection.Down)
			key = "BeltUpT";

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
			if(SelectedMaterial == MaterialDictionary["RG"] && RGXClicked >= 0 && RGYClicked >= 0)
			{
				currentRG.Add (RGXClicked, RGYClicked);
				RGXClicked = -1;
				RGYClicked = -1;
			}
		}
		if (boolDragging)
		{
			OnMouseDrag();
		}
		GameObject camera = GameObject.Find ("Main Camera");
		int moveX = 0;
		int moveY = 0;
		float magicNumber = 5;

		if( Input.mousePosition.x < magicNumber && Input.mousePosition.x >= 0 )
			moveX = (int)(-1*magicNumber/(Input.mousePosition.x+1));
		if( Input.mousePosition.x > Screen.width - magicNumber && Input.mousePosition.x <= Screen.width)
			moveX = (int)(1*magicNumber/(Screen.width-Input.mousePosition.x+1));
		if (Input.GetAxis ("Mouse ScrollWheel") != 0) 
		{
			moveY = (int)(20*Input.GetAxis ("Mouse ScrollWheel"));
		}

		camera.transform.position += new Vector3(1/magicNumber*moveX, 1/magicNumber* moveY , 0f);
		MakeSureCameraCanSeeMap ();

	}
	void MakeSureCameraCanSeeMap ()
	{
		float x = GetComponent<Camera>().transform.position.x;
		float y = GetComponent<Camera>().transform.position.y;
		float z = GetComponent<Camera>().transform.position.z;
		if(x < XOFFSET - 5)
			GetComponent<Camera>().transform.position = new Vector3(XOFFSET - 5f, y, z);
		if(x > LevelConstructorGridWidth)
			GetComponent<Camera>().transform.position = new Vector3(LevelConstructorGridWidth, y, z);
		if(y < YOFFSET - LevelConstructorGridHeight)
			GetComponent<Camera>().transform.position = new Vector3(x, YOFFSET - LevelConstructorGridHeight, z);
		if(y > YOFFSET )
			GetComponent<Camera>().transform.position = new Vector3(x, YOFFSET, z);

	}
	void OnGUI () 
	{
		MakeTilesScrollView ();
		MakeRotGroupScrollView ();
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
			Application.LoadLevel ("PlayerMenu"); 
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
		if (GUI.Button (new Rect (Screen.width- 140, 40, 100, 20), "Save")) 
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
		if (GUI.Button (new Rect (Screen.width- 100, 200, 60, 20), "+RG")) 
		{
			currentRG = new RotationGroup();
			LevelConstructorRotationGroups.Add (currentRG);
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
				foreach(RotationGroup group in LevelConstructorRotationGroups)
				{
					writer.Write('#');
					for(int i = 0; i < group.getCount(); i++)
					{
						if(i != 0)
						{
							writer.Write (',');
							print (',');
						}
						writer.Write (Convert.ToString(group.xAt(i)) + ',' + Convert.ToString(group.yAt(i)));
					}
					writer.Write("\n");
					print ("\n");
				}
			}
		}
		catch(Exception e)
		{
			Console.WriteLine("There was a problem during file reading", e.Message);
		}
	}
	void MakeTilesScrollView()
	{
		int numButtons = (int)MaterialDictionary.Count;
		Vector2 pivotPoint;
		TileScrollPosition = GUI.BeginScrollView(new Rect(Math.Min(Screen.width,40), Math.Min(Screen.height,20),
		                                              Math.Min(Screen.width-10,80), Screen.height-20), 
		                                         TileScrollPosition, new Rect(0, 0, 60, numButtons * 70));
		int i = 0;
		foreach(string key in MaterialDictionary.Keys)
		{
			//check if material is a tile
			for(int tile = 0; tile < (int)TileType.Count; tile ++)
			{
				if(key == Enum.GetName(typeof(TileType),(TileType)tile))
				{
					Matrix4x4 matrixBackup = GUI.matrix;
					pivotPoint = new Vector2(30, 30+ 70* i);
					GUIUtility.RotateAroundPivot(GetRotation((TileType)tile), pivotPoint);
					if(GUI.Button(new Rect(0, i*70, 60, 60),MaterialDictionary[key].mainTexture))
					{
						SelectedMaterial = MaterialDictionary[key];
						SelectedTile = (TileType)tile;
					}
					i++;
					GUI.matrix = matrixBackup;
				}
			}
		}
		GUI.EndScrollView();
	}
	void MakeRotGroupScrollView()
	{
		int numButtons = (int)LevelConstructorRotationGroups.Count;
		GroupScrollPosition = GUI.BeginScrollView(new Rect(Math.Min(Screen.width-100,Screen.width), Math.Min(Screen.height,230),
		                                              Math.Min(Screen.width-20,80), 250), 
		                                          GroupScrollPosition, new Rect(0, 0, 100, numButtons * 70));
		int i = 0;
		foreach(RotationGroup group in LevelConstructorRotationGroups)
		{
			if(GUI.Button(new Rect(0, i*70, 60, 60),MaterialDictionary["RG"].mainTexture))
			{
				SelectedMaterial = MaterialDictionary["RG"]; 
				currentRG = group;
			}
			i++;
		}
		GUI.EndScrollView();
	}
	public int GetRotation(TileType type)
	{
		int retValue = 0;
		switch (type) 
		{
		case TileType.BeltUpRight:
		case TileType.BeltUpT:
		case TileType.BeltVertical:
			retValue = 90;
			break;
		case TileType.BeltUpLeft:
		case TileType.BeltRightT:
			retValue = 180;
			break;
		case TileType.BeltDownLeft:
		case TileType.BeltDownT:
			retValue = 270;
			break;
			
		}
		return retValue;

	}
	public bool IsBeltMaterial(Material m)
	{
		List<Material> Belts = new List<Material> (new Material[] {MaterialDictionary["BeltVertical"], MaterialDictionary["BeltHorizontal"], 
			MaterialDictionary["BeltUpLeft"], MaterialDictionary["BeltUpRight"], MaterialDictionary["BeltDownRight"], MaterialDictionary["BeltDownLeft"], 
			MaterialDictionary["BeltUpT"], MaterialDictionary["BeltRightT"], MaterialDictionary["BeltDownT"], MaterialDictionary["BeltLeftT"],
			MaterialDictionary["BeltCross"]});
		
		return Belts.Contains (m);
	}
	public static void PrintMessage(string printStatement)
	{
		print (printStatement);
	}
}
