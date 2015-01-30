using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class Instantiation : MonoBehaviour {

	// We can create a material class later
	public Material BoxOutline;
	public Material AND;
	public Material OR;
	public Material BeltD;
	public Material BeltL;
	public Material Enter0;
	public Material Enter1;
	public Material Exit0;
	public Material Exit1;
	public Material Biter0;
	public Material SelectedMaterial;
	private GameObject[,] BoardGrid; // used for textures, the actual cube
	private GridSquare[,] GridPieces; // used for the functionality of the grid - what the piece does
	private List<Monster> MonsterList = new List<Monster>();
	public int OFFSETX { get; set; }//set in start
	public int OFFSETY { get; set; }//set in start

	public int SpawnDelayTime = 10000;//lower if testing multiple spawns of monsters (time delay between spawns)

    enum TileTypes : int {Blank=0, Enter0, Enter1, Exit0, Exit1, And, Or, BeltD, BeltL};


	private void InitializeBoardGrid(string filePath)//open the file, read in the data and create grid size
	{
		var GridHeight = 0;
		var GridWidth = 0;
		using (var reader = File.OpenText(filePath))
		{
			string line = reader.ReadLine();

			if(line != null)//get the width of the grid in the file
			{
				string[] entries = line.Split(',');
				GridWidth = entries.Length;
				GridHeight++;
			}
			while (reader.ReadLine() != null)//get the height of the grid in the file
			{
				GridHeight++;
			}
		}
		print ("GridWidth = " + GridWidth + " and GridHeight = " + GridHeight); 
		BoardGrid = new GameObject[GridWidth,GridHeight];
		GridPieces = new GridSquare[GridWidth,GridHeight];
	}
	private bool LoadLevel(string fileLevel)//load in the data file entries and create the field
	{
		int y = 0;
		print("Loading file:" + fileLevel);
		string filePath = "Assets/Resources/"+fileLevel;
		try
		{
			string line;
			StreamReader theReader = new StreamReader(filePath, Encoding.Default);
			InitializeBoardGrid(filePath);
			using (theReader)
			{
				do
				{
					line = theReader.ReadLine();
					if (line != null)
					{
						string[] entries = line.Split(',');
						for(int i = 0; i < entries.Length; i++)
						{
							CreateCube(i, y, Convert.ToInt32(entries[i]));
						}
					}
					y++;
				}
				while (line != null);  
				theReader.Close();
				return true;
			}
		}
		catch (Exception e)
		{
			print ("poo it failed" + e.Message);
			Console.WriteLine("failed\n", e.Message);
			return false;
		}
	}

	void Start() //called at initialization, basically the init function
	{
		OFFSETX = 4;
		OFFSETY = 4;
		BoxOutline = Resources.Load("BoxOutline", typeof(Material)) as Material;
		AND = Resources.Load("AND", typeof(Material)) as Material;
		OR = Resources.Load("OR", typeof(Material)) as Material;
		BeltD = Resources.Load("BeltD", typeof(Material)) as Material;
		BeltL = Resources.Load("BeltL", typeof(Material)) as Material;
		Enter0 = Resources.Load("Enter0", typeof(Material)) as Material;
		Enter1 = Resources.Load("Enter1", typeof(Material)) as Material;
		Exit0 = Resources.Load("Exit0", typeof(Material)) as Material;
		Exit1 = Resources.Load("Exit1", typeof(Material)) as Material;

		Biter0 = Resources.Load("Biter0", typeof(Material)) as Material;


		SelectedMaterial = AND;
		LoadLevel ("filelevel.txt");

	}
	void CreateCube(int x, int y, int type)//creates a cube(can be temp) in space and applies texture to it
	{
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		switch (type) 
		{
			case (int)TileTypes.Blank:
				cube.renderer.material = BoxOutline;
				break;
			case (int)TileTypes.Enter0:
				cube.renderer.material = Enter0;
				break;
			case (int)TileTypes.Enter1:
				cube.renderer.material = Enter1;
				break;
			case (int)TileTypes.Exit0:
				cube.renderer.material = Exit0;
				break;
			case (int)TileTypes.Exit1:
				cube.renderer.material = Exit1;
				break;
			case (int)TileTypes.And:
				cube.renderer.material = AND;
				break;
			case (int)TileTypes.Or:
				cube.renderer.material = OR;
				break;
			case (int)TileTypes.BeltD:
				cube.renderer.material = BeltD;
				break;
			case (int)TileTypes.BeltL:
				cube.renderer.material = BeltL;
				break;
		}
		cube.transform.position = new Vector3(x-OFFSETX, OFFSETY-y, 0);
		BoardGrid[x,y] = cube;
		GridPieces [x, y] = new GridSquare (type, false, x, y);

	}
	
	
	// Update is called once per frame
	void Update () 
	{
		GetMouseRays ();
		UpdateSpawnTile ();
		UpdateMonsterAction ();
	}


	void GetMouseRays()//if mouse is clicked find out which thing it was clicked on
	{
		//get the mouse clicked on a square and react to it
		if( Input.GetMouseButtonDown(0) )
		{
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			
			if( Physics.Raycast( ray, out hit, 100 ) )
			{
				// all cubes will need a ray layer later so that when you click the monster sprites wont  
				// interfere with changing gates
				GameObject pObject = hit.transform.gameObject;
				pObject.renderer.material = SelectedMaterial;
				print ("IM HIT!");
			}
		}
	}
	void UpdateSpawnTile()//spawn monsters from the starting points
	{
		//every frame see if you should spawn an enemy
		for (int x = 0; x < GridPieces.GetLength(0); x++) 
		{
			for (int y = 0; y < GridPieces.GetLength(1); y++) 
			{
				if(GridPieces[x,y].Type == (int)TileTypes.Enter0 && GridPieces[x,y].Count == 0)
				{
					Spawn((int)TileTypes.Enter0, x, y);
					GridPieces[x,y].Count=SpawnDelayTime;
				}
				else if(GridPieces[x,y].Type == (int)TileTypes.Enter1 && GridPieces[x,y].Count == 0)
				{
					Spawn((int)TileTypes.Enter1, x, y);
					GridPieces[x,y].Count=SpawnDelayTime;
				}
				else if(GridPieces[x,y].Type == (int)TileTypes.Enter0 || GridPieces[x,y].Type == (int)TileTypes.Enter1)
				{
					GridPieces[x,y].Count--;
				}
			}
		}
	}
	void Spawn(int type, int PosX, int PosY)//create the monster
	{
		MonsterList.Add (new Monster(this,type,PosX,PosY,0));//Allan please replace 0 with enums for directions
	}

	void UpdateMonsterAction()//based on what it is currently doing, what should it do next?
	{
		for (int i = 0; i < MonsterList.Count; i++) 
		{
			switch(MonsterList[i].Status)
			{
				case (int)Monster.StatusType.moving:
					MoveMonsters(MonsterList[i]);
					break;
				case (int)Monster.StatusType.waiting:
					AssignNewStatus(MonsterList[i]);
					break;
				case (int)Monster.StatusType.finished:
					MoveMonsters(MonsterList[i]);
					break;
			}
		}
	}
	void AssignNewStatus(Monster monster)// find out the tile the monster is on and assign it a task accordingly
	{
		int tileX = OFFSETX + (int)Math.Round (monster.MonsterGameObject.transform.position.x);
		int tileY = OFFSETY - (int)Math.Round (monster.MonsterGameObject.transform.position.y);

		switch (GridPieces [tileX, tileY].Type) 
		{
			case (int)TileTypes.BeltD:
				monster.MovementDirection=0;
				monster.Status = (int)Monster.StatusType.moving;
				break;
			case (int)TileTypes.BeltL:
				monster.MovementDirection=2;
				monster.Status = (int)Monster.StatusType.moving;
				break;
			case (int)TileTypes.And:
				monster.Status = (int)Monster.StatusType.waiting;
				break;
			case (int)TileTypes.Or:
				monster.Status = (int)Monster.StatusType.waiting;
				break;
			default:
				monster.Status = (int)Monster.StatusType.waiting;
				break;
		}
	}
	void MoveMonsters(Monster monster)//move the monster object slowly in a direction then check if it is on a new tile.
	{
		int moveX = 0;
		int moveY = 0;

		if (monster.MovementDirection == 0)
				moveY = -1;
		if (monster.MovementDirection == 1)
				moveY = 1;
		if (monster.MovementDirection == 2)
				moveX = 1;
		if (monster.MovementDirection == 3)
				moveX = -1;

		monster.MonsterGameObject.transform.position += 
			new Vector3 ((float)monster.MovementIncrement * moveX * monster.MovementSpeed, 
			             (float)monster.MovementIncrement * moveY * monster.MovementSpeed, 0);
		 
		if (monster.FinishedMovingTile()) 
		{
			monster.Status = (int)Monster.StatusType.waiting;
		}

	}

	//testing any class calls for objects
	public void p(string printStatement)
	{
		print (printStatement);
	}
}
