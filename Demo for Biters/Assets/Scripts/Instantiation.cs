using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class Instantiation : MonoBehaviour
{
    public GameObject[,] InstantiationGridSquareGameObjectGrid { get; set; }    // Textures
    public GridSquare[,] InstantiationGridSquareGrid { get; set; }              // Functionality
    public int InstantiationGridWidth { get; set; }
    public int InstantiationGridHeight { get; set; }
	public List<Monster> InstantiationMonsters { get; set; }
	public List<RotationGroup> InstantiationRotationGroups{ get; set; }
    public int InstantiationNextMonsterId { get; set; }
    public int InstantiationSpawnDelay { get; set; }
	private float timer = 200.0f; 

	bool bLevelWon = false;
	bool bLevelLost = false;
	PersistentScript persistentScript;

	public AudioClip gateSound;
	
	public Dictionary<string,Material> MaterialDictionary { get; set; }
	

	public const int XOFFSET = 4;
	public const int YOFFSET = 4;

    public void Start()
    {
		GameObject persistentGameObject = GameObject.Find("PersistentData");
		if( persistentGameObject != null)
			persistentScript = (PersistentScript)persistentGameObject.GetComponent(typeof(PersistentScript));

        InstantiationMonsters = new List<Monster>();
		InstantiationRotationGroups = new List<RotationGroup> ();
        InstantiationNextMonsterId = 0;
        InstantiationSpawnDelay = 10000;

		MaterialDictionary = new Dictionary<string, Material> ();
		UnityEngine.Object[] Materials = Resources.LoadAll("", typeof(Material));
		//Object[] Materials = Resources.LoadAll("Materials", typeof(Material));
		foreach(Material mat in Materials)
		{
			MaterialDictionary.Add(mat.name, mat);
		}

       
		Time.timeScale = 1.0f; 

		// PrintMessage("The currLevel is currently: " + Game.current.player.currLevel);
		LoadLevel(Game.current.player.currLevel);
    }
    public bool LoadLevel(string fileName)
    {
        string filePath = "Assets/Levels/" + fileName;
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
                        new GridSquare(this, (TileType)intType, x, y);
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
						InstantiationRotationGroups.Add(g);
						line = reader.ReadLine();
					}while(line != null);
				}
                reader.Close();

				foreach (GridSquare square in InstantiationGridSquareGrid) 
				{
					square.AssignDirection();
				}
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
		InstantiationGridWidth = 0;
		InstantiationGridHeight = 0;
        StreamReader reader = new StreamReader(filePath, Encoding.Default);
		using(reader)
		{
			string line = reader.ReadLine();
			if(line != null && line[0] != '#')
			{
				string[] entries = line.Split(',');
                InstantiationGridWidth = entries.Length;
                InstantiationGridHeight++;
			}
            line = reader.ReadLine();
			while(line != null && line[0] != '#')
			{
                InstantiationGridHeight++;
                line = reader.ReadLine();
			}
		}
        InstantiationGridSquareGameObjectGrid = new GameObject[InstantiationGridWidth, InstantiationGridHeight];
        InstantiationGridSquareGrid = new GridSquare[InstantiationGridWidth, InstantiationGridHeight];
	}
	
	void Update () 
	{
		GetMouseRays();
		UpdateSpawnTile();
		UpdateMonsterAction();
		timer -= Time.deltaTime; 
		
		if (timer <= 0) 
		{ 
			timer = 0.0f; 
		} 
	}
	void PauseGame() 
	{ 
		Time.timeScale = 0.000001f; 
	} 
	void OnGUI () 
	{
		MakeMapControls ();
		GUI.Box (new Rect (Screen.width - 50, 0, 50, 20), "" + timer.ToString ("f0")); 

		if(bLevelWon)
		{
			if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 60, 100, 25), "Continue")) 
			{
				Application.LoadLevel (Application.loadedLevelName); 
			} 
		}
		else if(bLevelLost || timer <= 0)
		{
			if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 60, 100, 25), "Retry?")) 
			{
				Application.LoadLevel (Application.loadedLevelName); 
			} 
		}
	}

	void GetMouseRays()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 100))
			{
				// All cubes will need a ray layer later so that when you click the monster sprites, it won't interfere with changing gates
				GameObject pObject = hit.transform.gameObject;
				RotationGroup rg;
				rg = GetRotationalGroup(XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y);
				if(
					pObject.GetComponent<Renderer>().sharedMaterial != MaterialDictionary["And"] && 
					pObject.GetComponent<Renderer>().sharedMaterial != MaterialDictionary["Or"] &&
					pObject.GetComponent<Renderer>().sharedMaterial != MaterialDictionary["Nand"] &&
					pObject.GetComponent<Renderer>().sharedMaterial != MaterialDictionary["Xor"] &&
					pObject.GetComponent<Renderer>().sharedMaterial != MaterialDictionary["Xnor"] &&
					pObject.GetComponent<Renderer>().sharedMaterial != MaterialDictionary["Nor"] &&
					rg == null
					)
				{
					return;
				}
				if(rg != null)
				{
					RotateTiles(rg);
				}
				else
				{
					TileType tileType = TileType.Or;
					if(pObject.GetComponent<Renderer>().sharedMaterial == MaterialDictionary["Or"])
					{
						pObject.GetComponent<Renderer>().material = MaterialDictionary["And"];
						tileType = TileType.And;
					}
					else if(pObject.GetComponent<Renderer>().sharedMaterial == MaterialDictionary["And"])
					{
						pObject.GetComponent<Renderer>().material = MaterialDictionary["Nand"];
						tileType = TileType.Nand;
					}
					else if(pObject.GetComponent<Renderer>().sharedMaterial == MaterialDictionary["Nand"])
					{
						pObject.GetComponent<Renderer>().material = MaterialDictionary["Xor"];
						tileType = TileType.Xor;
					}
					else if(pObject.GetComponent<Renderer>().sharedMaterial == MaterialDictionary["Xor"])
					{
						pObject.GetComponent<Renderer>().material = MaterialDictionary["Xnor"];
						tileType = TileType.Xnor;
					}
					else if(pObject.GetComponent<Renderer>().sharedMaterial == MaterialDictionary["Xnor"])
					{
						pObject.GetComponent<Renderer>().material = MaterialDictionary["Nor"];
						tileType = TileType.Nor;
					}
					else if(pObject.GetComponent<Renderer>().sharedMaterial == MaterialDictionary["Nor"])
					{
						pObject.GetComponent<Renderer>().material = MaterialDictionary["Or"];
						tileType = TileType.Or;
					}
					AudioSource.PlayClipAtPoint(gateSound, Camera.main.transform.position);
	                InstantiationGridSquareGrid[XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y].GridSquareTileType = tileType;
				}
			}
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
		if(x < -XOFFSET)
			GetComponent<Camera>().transform.position = new Vector3(-XOFFSET, y, z);
		if(x > InstantiationGridWidth - XOFFSET)
			GetComponent<Camera>().transform.position = new Vector3(InstantiationGridWidth - XOFFSET, y, z);
		if(y < 0 - InstantiationGridHeight)
			GetComponent<Camera>().transform.position = new Vector3(x, 0 - InstantiationGridHeight, z);
		if(y > 0)
			GetComponent<Camera>().transform.position = new Vector3(x, 0, z);
		
	}

	void MakeMapControls()
	{
		if (GUI.Button (new Rect (Screen.width- 250, 50, 50, 20), ">")) 
		{
			Time.timeScale = 1.0f;
		}
		if (GUI.Button (new Rect (Screen.width- 200, 50, 50, 20), ">>")) 
		{
			Time.timeScale = 2.0f;
		}
		if (GUI.Button (new Rect (Screen.width- 150, 50, 50, 20), ">>>")) 
		{
			Time.timeScale = 4.0f;
		}
	}

	void UpdateSpawnTile()
	{
		for(int x = 0; x < InstantiationGridWidth; x++) 
		{
            for(int y = 0; y < InstantiationGridHeight; y++) 
			{
				if(InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterZero && InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn == 0)
				{
					Monster monster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, NumberType.Zero, x, y, MovementDirection.None);
					InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
                    InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn = InstantiationSpawnDelay;
				}
				if(InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterOne && InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn == 0)
				{
					Monster monster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, NumberType.One, x, y, MovementDirection.None);
					InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
					InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn = InstantiationSpawnDelay;
				}
				else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterRandom && InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn == 0)
				{
					var number = UnityEngine.Random.Range(0,2);
					NumberType numType = number == 0 ? NumberType.Zero : NumberType.One;
					Monster monster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, numType, x, y, MovementDirection.None);
					InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
					InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn = InstantiationSpawnDelay;
				}
				else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterZero || InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterOne|| InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterRandom)
				{
					InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn--;
				}
			}
		}
	}

	void UpdateMonsterAction()
	{
		List<Monster> tempList = new List<Monster>();
		foreach (Monster monster in InstantiationMonsters)
		{
			tempList.Add (monster);
		}
        foreach(Monster m in tempList)
        {
			Monster monster = InstantiationMonsters.Find(n => n.MonsterId == m.MonsterId); 
            if(monster != null)
			{
				switch(monster.MonsterMovementType)
	            {
	                case MovementType.Moving:
	                    MoveMonster(monster);
	                    break;
	                case MovementType.Waiting:
	                    AssignNewStatus(monster);
	                    break;
	                default:
	                    Instantiation.PrintMessage("Invalid MonsterMovementType - UpdateMonsterAction()");
	                    break;
	            }
			}
        }
	}

	void AssignNewStatus(Monster monster)
	{
        int x = XOFFSET + (int)Math.Round(monster.MonsterGameObject.transform.position.x);
		int y = YOFFSET - (int)Math.Round(monster.MonsterGameObject.transform.position.y);
        switch(InstantiationGridSquareGrid[x, y].GridSquareTileType)
        {
			case TileType.EnterZero:
			case TileType.EnterOne:
			case TileType.EnterRandom:
				InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
				break;
            case TileType.ExitZero: // @RCH: Once win condition is set, remove this
			case TileType.ExitOne: // @RCH: Once win condition is set, remove this
				CheckForWin(x,y,monster);
                break;
            case TileType.And:
            case TileType.Or:
			case TileType.Nand:
			case TileType.Xor:
			case TileType.Xnor:
			case TileType.Nor:
			monster.MonsterMovementType = MovementType.Waiting;
                bool shouldRemove = false;
                int index1 = 0;
                int index2 = 0;
                foreach(Monster m in InstantiationMonsters)
                {
                    if(m.MonsterXPosition == x && m.MonsterYPosition == y && m.MonsterId != monster.MonsterId && m.MonsterMovementType == MovementType.Waiting)
                    {
						InstantiationGridSquareGrid[x, y].SubtractDirection(m);
						InstantiationGridSquareGrid[x, y].SubtractDirection(monster);
                        shouldRemove = true;
                        index1 = InstantiationMonsters.IndexOf(monster);
                        index2 = InstantiationMonsters.IndexOf(m);
                        NumberType numberType = NumberType.Zero;
                        if(InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.And && monster.MonsterNumberType == NumberType.One && m.MonsterNumberType == NumberType.One)
                        {
                            numberType = NumberType.One;
                        }
                        else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.Or && (monster.MonsterNumberType == NumberType.One || m.MonsterNumberType == NumberType.One))
                        {
                            numberType = NumberType.One;
                        }
						else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.Xor && ( (monster.MonsterNumberType == NumberType.One && m.MonsterNumberType == NumberType.Zero) || (monster.MonsterNumberType == NumberType.Zero && m.MonsterNumberType == NumberType.One) ) )
						{
							numberType = NumberType.One;
						}
						else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.Nand && !(monster.MonsterNumberType == NumberType.One && m.MonsterNumberType == NumberType.One) )
						{
							numberType = NumberType.One;
						}
						else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.Xnor && ( (monster.MonsterNumberType == NumberType.One && m.MonsterNumberType == NumberType.One) || (monster.MonsterNumberType == NumberType.Zero && m.MonsterNumberType == NumberType.Zero) ) )
						{
							numberType = NumberType.One;
						}
						else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.Nor && (monster.MonsterNumberType == NumberType.Zero && m.MonsterNumberType == NumberType.Zero) )
						{
							numberType = NumberType.One;
						}
						Monster tempMonster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, numberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.None);
						InstantiationGridSquareGrid[x, y].CalculateNewDirection(tempMonster);
                        break;
                    }
                }
                if(shouldRemove)
                {
                    Destroy(InstantiationMonsters[index1].MonsterGameObject, 0f);
                    Destroy(InstantiationMonsters[index2].MonsterGameObject, 0f);
					if(index1 > index2)
					{
						InstantiationMonsters.RemoveAt(index1);
						InstantiationMonsters.RemoveAt(index2);
					}
					else
					{
						InstantiationMonsters.RemoveAt(index2);
						InstantiationMonsters.RemoveAt(index1);
					}
                }
                break;
			case TileType.Not:
			{
				monster.MonsterNumberType = monster.MonsterNumberType == NumberType.Zero ? NumberType.One: NumberType.Zero; 
				monster.MonsterGameObject.GetComponent<Renderer>().material = monster.MonsterGameObject.GetComponent<Renderer>().material == 
							MaterialDictionary["BiterZero"] ? MaterialDictionary["BiterOne"] : MaterialDictionary["BiterZero"];
				InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
				break;
			}
			case TileType.BeltUpLeft:
			case TileType.BeltUpRight:
			case TileType.BeltDownRight:
			case TileType.BeltDownLeft:
			case TileType.BeltHorizontal:
			case TileType.BeltVertical:
			case TileType.BeltUpT:
			case TileType.BeltRightT:
			case TileType.BeltDownT:
			case TileType.BeltLeftT:
			case TileType.BeltCross:
				InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
                break;
            default:
                Instantiation.PrintMessage("Invalid GridSquareTileType - AssignNewStatus(Monster monster)");
                break;
        }
	}

	void MoveMonster(Monster monster)
	{
        int moveX = 0;
        int moveY = 0;

        if(monster.MonsterMovementDirection == MovementDirection.Up)
        {
            moveY = 1;
        }
        else if (monster.MonsterMovementDirection == MovementDirection.Right)
        {
            moveX = 1;
        }
        else if (monster.MonsterMovementDirection == MovementDirection.Down)
        {
            moveY = -1;
        }
        else if (monster.MonsterMovementDirection == MovementDirection.Left)
        {
            moveX = -1;
        }

		monster.MonsterGameObject.transform.position += new Vector3(monster.MonsterMovementIncrement * moveX * Time.timeScale, monster.MonsterMovementIncrement * moveY * Time.timeScale, 0);

        if(monster.FinishedMovingTile())
		{
            monster.MonsterMovementType = MovementType.Waiting;
        }
	}

	void CheckForWin(int x, int y, Monster monster)
	{
		//assign the current tile to win or lose from blank. cant be assigned unless blank or win.(if you lose you lose)
		//check all tiles, if any of the end pieces are lose then return you lose
		//if the amount of winning tiles == the number of end pieces then you win
		bool PlayerLost = false;
		int numWinningExits = 0;
		int totalNumExits = 0;

		if(monster.MonsterNumberType == NumberType.One && InstantiationGridSquareGrid[x,y].GridSquareTileType == TileType.ExitOne || 
		   monster.MonsterNumberType == NumberType.Zero && InstantiationGridSquareGrid[x,y].GridSquareTileType == TileType.ExitZero)
		{
			if(InstantiationGridSquareGrid[x,y].GridSquareHasWinningPiece != WinCondition.Incorrect)
			{
				InstantiationGridSquareGrid[x,y].GridSquareHasWinningPiece = WinCondition.Correct;
			}
		}
		else
		{
			InstantiationGridSquareGrid[x,y].GridSquareHasWinningPiece = WinCondition.Incorrect;
		}

		for (int i = 0; i < InstantiationGridSquareGrid.GetLength(0); i++) 
		{
			for (int j = 0; j < InstantiationGridSquareGrid.GetLength(1); j++) 
			{
				if(InstantiationGridSquareGrid[i,j].GridSquareTileType == TileType.ExitOne ||
				   InstantiationGridSquareGrid[i,j].GridSquareTileType == TileType.ExitZero)
				{
					if(InstantiationGridSquareGrid[i,j].GridSquareHasWinningPiece == WinCondition.Incorrect)
						PlayerLost = true;
					if(InstantiationGridSquareGrid[i,j].GridSquareHasWinningPiece == WinCondition.Correct)
						numWinningExits ++;

					totalNumExits++;
				}
			}
		}

		if (PlayerLost)
		{
			PrintMessage ("YOU LOSE!");
			bLevelLost = true;
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0, 0, 0);
			cube.transform.localScale = new Vector3(10, 10, 10);
			cube.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			cube.GetComponent<Renderer>().material = MaterialDictionary["Lose"];
		}
		else if (numWinningExits == totalNumExits)
		{
			PrintMessage ("YOU WIN!");
			SetNextPlayerLevel();
			bLevelWon = true;
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0, 0, 0);
			cube.transform.localScale = new Vector3(10, 10, 10);
			cube.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			cube.GetComponent<Renderer>().material = MaterialDictionary["Win"];
		}
		else
		{
			PrintMessage ("GOOD JOB KEEP GOING!");
		}

		monster.DestroyEntirely ();
	}

	// returns true if the level has not already been reached by the Player -- Prevents duplicate entries 
	public bool CheckLevel(string lvl) { 

		for (int i = 0; i < Game.current.player.levelsList.Count; i++) { 

			if (lvl == Game.current.player.levelsList[i]) { 

				return false; 

			} // end if statement 

		} // end for loop  

		return true; 

	} // end CheckLevel  

	public void SetNextPlayerLevel()
	{
		List<string> levelsList = new List<string>();
		GetLevels (ref levelsList);

		for(int i = 0; i < levelsList.Count; i++)
		{
			if(Game.current.player.currLevel == levelsList[i] && i < levelsList.Count - 1)
			{
				if(levelsList[i+1] != null) {
					Game.current.player.currLevel = levelsList[i+1];
					// Only call this if the level has not already been reached 
					if (CheckLevel (levelsList[i+1])) { 
						Game.current.player.highestLevel = levelsList[i+1];
						Game.current.player.levelsList.Add(levelsList[i+1]); 
					} // end if statement 
					Save.SaveThis (); 
				} // end if statement 
				break;
			}
		}
	}
	void GetLevels(ref List<string> levelsList)
	{
		if (levelsList != null)
			levelsList.Clear ();
		levelsList = new List<string> ();
		string dirName = Directory.GetCurrentDirectory () + "/Assets/Levels";
		DirectoryInfo dir = new DirectoryInfo(dirName);
		FileInfo[] info = dir.GetFiles("*.csv");
		foreach (FileInfo f in info) 
		{ 
			levelsList.Add(f.Name);
		}
	}
	public RotationGroup GetRotationalGroup(int x, int y)
	{
		RotationGroup returnRotation = null;
		foreach (RotationGroup r in InstantiationRotationGroups) 
		{
			if(r.Contains(x,y))
				returnRotation = r;
		}
		return returnRotation;
	}
	public void RotateTiles (RotationGroup rg)
	{
		TileType tempTileType = InstantiationGridSquareGrid [rg.xAt (0), rg.yAt (0)].GridSquareTileType;
		Material tempTileMaterial = InstantiationGridSquareGrid [rg.xAt (0), rg.yAt (0)].GridSquareGameObject.GetComponent<Renderer>().sharedMaterial;

		for (int i = 0; i < rg.getCount() -1 ; i++) 
		{
			InstantiationGridSquareGrid [rg.xAt (i), rg.yAt (i)].GridSquareTileType = 
				InstantiationGridSquareGrid [rg.xAt (i + 1), rg.yAt (i + 1)].GridSquareTileType;
			InstantiationGridSquareGrid [rg.xAt (i), rg.yAt (i)].GridSquareGameObject.GetComponent<Renderer>().material = 
				InstantiationGridSquareGrid [rg.xAt (i+1), rg.yAt (i+1)].GridSquareGameObject.GetComponent<Renderer>().sharedMaterial;
		}

		InstantiationGridSquareGrid [rg.xAt (rg.getCount () - 1), rg.yAt (rg.getCount () - 1)].GridSquareTileType = tempTileType;
		InstantiationGridSquareGrid [rg.xAt (rg.getCount () - 1), rg.yAt (rg.getCount () - 1)].GridSquareGameObject.GetComponent<Renderer>().sharedMaterial = tempTileMaterial;
		
		
	}
	public static void PrintMessage(string printStatement)
	{
		print (printStatement);
	}
}
