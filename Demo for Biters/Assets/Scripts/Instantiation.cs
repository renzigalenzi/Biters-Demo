using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class Instantiation : MonoBehaviour {

	// We can create a material class later
	public Material BoxOutline;
	public Material And;
	public Material Or;
	public Material BeltX;
	public Material BeltY;
	public Material Enter0;
	public Material Enter1;
	public Material Exit0;
	public Material Exit1;
	public Material Biter0;
	public Material Biter1;
	public Material SelectedMaterial;
	private GameObject[,] BoardGrid; // used for textures, the actual cube
	private GridSquare[,] GridPieces; // used for the functionality of the grid - what the piece does
	private List<Monster> MonsterList = new List<Monster>();
	public int OFFSETX { get; set; }//set in start
	public int OFFSETY { get; set; }//set in start
	public int nextId { get; set; }
	public int SpawnDelayTime = 1;//lower if testing multiple spawns of monsters (time delay between spawns)

    enum TileTypes : int 
	{
		Blank=0, Enter0, Enter1, Exit0, Exit1, And, Or, BeltX, BeltY
	};

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
							string[] subentries = entries[i].Split ('-');
							int type = Convert.ToInt32(subentries[0]);
							int direction = Convert.ToInt32(subentries[1]);
							CreateCube(i, y, type, direction);
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
		nextId = 0;
		BoxOutline = Resources.Load("BoxOutline", typeof(Material)) as Material;
		And = Resources.Load("And", typeof(Material)) as Material;
		Or = Resources.Load("Or", typeof(Material)) as Material;
		BeltX = Resources.Load("BeltX", typeof(Material)) as Material;
		BeltY = Resources.Load("BeltY", typeof(Material)) as Material;
		Enter0 = Resources.Load("Enter0", typeof(Material)) as Material;
		Enter1 = Resources.Load("Enter1", typeof(Material)) as Material;
		Exit0 = Resources.Load("Exit0", typeof(Material)) as Material;
		Exit1 = Resources.Load("Exit1", typeof(Material)) as Material;

		Biter0 = Resources.Load("Biter0", typeof(Material)) as Material;
		Biter1 = Resources.Load("Biter1", typeof(Material)) as Material;

		SelectedMaterial = Or;
		LoadLevel ("filelevel.txt");

	}
	void CreateCube(int x, int y, int type, int direction)//creates a cube(can be temp) in space and applies texture to it
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
				cube.renderer.material = And;
				break;
			case (int)TileTypes.Or:
				cube.renderer.material = Or;
				break;
			case (int)TileTypes.BeltY:
				cube.renderer.material = BeltY;
				break;
			case (int)TileTypes.BeltX:
				cube.renderer.material = BeltX;
				break;
		}
		cube.transform.position = new Vector3(x-OFFSETX, OFFSETY-y, 0);
		BoardGrid[x,y] = cube;
		GridPieces [x, y] = new GridSquare (type, direction, false, x, y);

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
				int type = 0;
				if(SelectedMaterial == Or)
				{
					type = (int)TileTypes.Or;
				}
				else if(SelectedMaterial == And)
				{
					type = (int)TileTypes.And;
				}
				GridPieces[OFFSETX + (int)pObject.transform.position.x, OFFSETY - (int)pObject.transform.position.y].Type = type;
				// Something here to change gate functionality
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
					Spawn((int)TileTypes.Enter0, x, y, GridPieces[x,y].Direction);
					GridPieces[x,y].Count=SpawnDelayTime;
				}
				else if(GridPieces[x,y].Type == (int)TileTypes.Enter1 && GridPieces[x,y].Count == 0)
				{
					Spawn((int)TileTypes.Enter1, x, y, GridPieces[x,y].Direction);
					GridPieces[x,y].Count=SpawnDelayTime;
				}
				else if(GridPieces[x,y].Type == (int)TileTypes.Enter0 || GridPieces[x,y].Type == (int)TileTypes.Enter1)
				{
					GridPieces[x,y].Count--;
				}
			}
		}
	}
	void Spawn(int type, int PosX, int PosY, int direction)//create the monster
	{
		MonsterList.Add (new Monster(this, nextId, type,PosX,PosY,direction));//Allan please replace 0 with enums for directions
		nextId++;
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
		int index1 = 0;
		int index2 = 0;
		bool remove = false;

		switch (GridPieces [tileX, tileY].Type) 
		{
			case (int)TileTypes.BeltY:
				monster.MovementDirection = GridPieces [tileX, tileY].Direction;
				monster.Status = (int)Monster.StatusType.moving;
				break;
			case (int)TileTypes.BeltX:
				monster.MovementDirection = GridPieces [tileX, tileY].Direction;
				monster.Status = (int)Monster.StatusType.moving;
				break;
			case (int)TileTypes.And:
				monster.Status = (int)Monster.StatusType.waiting;
				List<Monster> currentMonsters = new List<Monster>();
				foreach(Monster m in MonsterList)
				{
					currentMonsters.Add(m);
				}
				remove = false;
				index1 = 0;
				foreach(Monster m in currentMonsters)
				{
					if(OFFSETX + (int)Math.Round (m.MonsterGameObject.transform.position.x) == tileX 
				   		&& OFFSETY - (int)Math.Round (m.MonsterGameObject.transform.position.y) == tileY
				   		&& m.Id != monster.Id
				   		&& m.Status == (int)Monster.StatusType.waiting)
					{
						remove = true;
						index1 = MonsterList.IndexOf(m);
						int type = 1;
						if(monster.MonsterType == 2 && m.MonsterType == 2)
						{
							type = 2;
						}
						Spawn(type, tileX, tileY, GridPieces[tileX, tileY].Direction);
						break;
					}
				}
				if(remove)
				{
					index2 = MonsterList.IndexOf(monster);
					Destroy(MonsterList[index1].MonsterGameObject,0f);
					Destroy(MonsterList[index2].MonsterGameObject,0f);
					if(index1 > index2)
					{
						MonsterList.RemoveAt(index1);
						MonsterList.RemoveAt(index2);
					}
					else if(index2 > index1)
					{
						MonsterList.RemoveAt(index2);
						MonsterList.RemoveAt(index1);
					}
				}
			break;
		case (int)TileTypes.Or:
				monster.Status = (int)Monster.StatusType.waiting;
				currentMonsters = new List<Monster>();
				foreach(Monster m in MonsterList)
				{
					currentMonsters.Add(m);
				}
				remove = false;
				index1 = 0;
				foreach(Monster m in currentMonsters)
				{
					if(OFFSETX + (int)Math.Round (m.MonsterGameObject.transform.position.x) == tileX 
					   	&& OFFSETY - (int)Math.Round (m.MonsterGameObject.transform.position.y) == tileY
					   	&& m.Id != monster.Id
				   		&& m.Status == (int)Monster.StatusType.waiting)
					{
						remove = true;
						index1 = MonsterList.IndexOf(m);
						int type = 1;
						if(monster.MonsterType == 2 || m.MonsterType == 2)
						{
							type = 2;
						}
						Spawn(type, tileX, tileY, GridPieces[tileX, tileY].Direction);
						break;
					}
				}
				if(remove)
				{
					index2 = MonsterList.IndexOf(monster);
					Destroy(MonsterList[index1].MonsterGameObject,0f);
					Destroy(MonsterList[index2].MonsterGameObject,0f);
					if(index1 > index2)
					{
						MonsterList.RemoveAt(index1);
						MonsterList.RemoveAt(index2);
					}
					else if(index2 > index1)
					{
						MonsterList.RemoveAt(index2);
						MonsterList.RemoveAt(index1);
					}
				}
				break;
			case (int)TileTypes.Exit0:
			case (int)TileTypes.Exit1:
			if((monster.MonsterType == 1 && GridPieces [tileX, tileY].Type == (int)TileTypes.Exit0)
			   || (monster.MonsterType == 2 && GridPieces [tileX, tileY].Type == (int)TileTypes.Exit1))
				p ("Yay!");
			else
			{
				p ("Oh no");

			}
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

		if (monster.MovementDirection == 1)
				moveY = 1;
		if (monster.MovementDirection == 2)
				moveX = 1;
		if (monster.MovementDirection == 3)
				moveY = -1;
		if (monster.MovementDirection == 4)
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
