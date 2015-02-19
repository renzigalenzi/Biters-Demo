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
    public int InstantiationNextMonsterId { get; set; }
    public int InstantiationSpawnDelay { get; set; }
	private float timer = 60.0f; 

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

		if(persistentScript != null && persistentScript.SelectedLevel != null)
        	LoadLevel(persistentScript.SelectedLevel);
		else
			LoadLevel("Level - 1.csv");
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
                while(line != null)
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
			if(line != null)
			{
				string[] entries = line.Split(',');
                InstantiationGridWidth = entries.Length;
                InstantiationGridHeight++;
			}
            line = reader.ReadLine();
			while(line != null)
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
		GUI.Box (new Rect (Screen.width - 50, 0, 50, 20), "" + timer.ToString ("f0")); 
		if (timer <= 0) 
		{ 
			if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 60, 100, 25), "Try Again?")) 
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
				if(
					pObject.renderer.sharedMaterial != MaterialDictionary["And"] && 
					pObject.renderer.sharedMaterial != MaterialDictionary["Or"] &&
					pObject.renderer.sharedMaterial != MaterialDictionary["Nand"] &&
					pObject.renderer.sharedMaterial != MaterialDictionary["Xor"]
					)
				{
					return;
				}
				TileType tileType = TileType.Or;
				if(pObject.renderer.sharedMaterial == MaterialDictionary["Or"])
				{
					pObject.renderer.material = MaterialDictionary["And"];
					tileType = TileType.And;
				}
				else if(pObject.renderer.sharedMaterial == MaterialDictionary["And"])
				{
					pObject.renderer.material = MaterialDictionary["Nand"];
					tileType = TileType.Nand;
				}
				else if(pObject.renderer.sharedMaterial == MaterialDictionary["Nand"])
				{
					pObject.renderer.material = MaterialDictionary["Xor"];
					tileType = TileType.Xor;
				}
				else if(pObject.renderer.sharedMaterial == MaterialDictionary["Xor"])
				{
					pObject.renderer.material = MaterialDictionary["Or"];
					tileType = TileType.Nand;
				}
				AudioSource.PlayClipAtPoint(gateSound, Camera.main.transform.position);
                InstantiationGridSquareGrid[XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y].GridSquareTileType = tileType;
			}
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
		if(x < -XOFFSET)
			camera.transform.position = new Vector3(-XOFFSET, y, z);
		if(x > InstantiationGridWidth - XOFFSET)
			camera.transform.position = new Vector3(InstantiationGridWidth - XOFFSET, y, z);
		if(y < 0 - InstantiationGridHeight)
			camera.transform.position = new Vector3(x, 0 - InstantiationGridHeight, z);
		if(y > 0)
			camera.transform.position = new Vector3(x, 0, z);
		
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
				else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterOne && InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn == 0)
				{
                    Monster monster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, NumberType.One, x, y, MovementDirection.None);
					InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
					InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn = InstantiationSpawnDelay;
				}
                else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterZero || InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterOne)
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
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0, 0, 0);
			cube.transform.localScale = new Vector3(10, 10, 10);
			cube.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			cube.renderer.material = MaterialDictionary["Lose"];
		}
		else if (numWinningExits == totalNumExits)
		{
			PrintMessage ("YOU WIN!");
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0, 0, 0);
			cube.transform.localScale = new Vector3(10, 10, 10);
			cube.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			cube.renderer.material = MaterialDictionary["Win"];
		}
		else
		{
			PrintMessage ("GOOD JOB KEEP GOING!");
		}

		monster.DestroyEntirely ();
	}

	public static void PrintMessage(string printStatement)
	{
		print (printStatement);
	}
}
