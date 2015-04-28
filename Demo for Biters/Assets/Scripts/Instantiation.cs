using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;

public class Instantiation : MonoBehaviour
{
    public GameObject[,] InstantiationGridSquareGameObjectGrid { get; set; }    // Textures
    public GridSquare[,] InstantiationGridSquareGrid { get; set; }              // Functionality
    public int InstantiationGridWidth { get; set; }
    public int InstantiationGridHeight { get; set; }
	public List<Monster> InstantiationMonsters { get; set; }
	public List<RotationGroup> InstantiationRotationGroups{ get; set; }
	public List<TutorialEvent> InstantiationTutorialEvents{ get; set; }
    public int InstantiationNextMonsterId { get; set; }
    public int InstantiationSpawnDelay { get; set; }
	public double PlayerHealth = 0;
	public GUISkin window;//for GUI skin work
	public GUISkin tutorial;//for Tutorial Pop-ups
	private static float MaxTime = 3.0f;
	private float fLevelStartTimer = MaxTime; 

	private float StartTextAlpha = 1.0f;
	public AudioSource sound;
	public AudioClip clip; 

	bool bRightMouseClicked = false;
	bool bLevelWon = false;
	bool bLevelLost = false;
	public Text LevelText;
	public Image TextureNeedle;
	private Vector3 OriginalNeedlePosition;
	public Image RotationPoint;

	public int selGridInt = 1;
	public string[] selStrings = new string[] {"1/2", ">", ">>", ">>>"};

	Vector3 RightClickedOriginPoint;
	Vector3 RightClickedCurrentPoint;
	float RayDistance = 4.5f;

		String InstantiationTutorialString = "";

	public AudioClip gateSound;
	
	public Dictionary<string,Material> MaterialDictionary { get; set; }
	

	public const int XOFFSET = 4;
	public const int YOFFSET = 4;

    public void Start()
    {
		OriginalNeedlePosition = TextureNeedle.gameObject.transform.position;
        InstantiationMonsters = new List<Monster>();
		InstantiationRotationGroups = new List<RotationGroup> ();
		InstantiationTutorialEvents = new List<TutorialEvent> ();
        InstantiationNextMonsterId = 0;
        InstantiationSpawnDelay = 800;

		MaterialDictionary = new Dictionary<string, Material> ();
		UnityEngine.Object[] Materials = Resources.LoadAll("", typeof(Material));
		//Object[] Materials = Resources.LoadAll("Materials", typeof(Material));
		foreach(Material mat in Materials)
		{
			MaterialDictionary.Add(mat.name, mat);
		}

       
		Time.timeScale = 1.0f; 

		//LevelText = gameObject.AddComponent<Text>();
		LevelText.text = Game.current.player.currLevel.Split('.')[0];
		//LevelText.font = (Resources.Load("Courier") as Font);
		//LevelText.material.color = new Color(1, 0.92, 0.016, 1); 
		// PrintMessage("The currLevel is currently: " + Game.current.player.currLevel);
		LoadLevel(Game.current.player.currLevel);
    }
    public bool LoadLevel(string fileName)
    {
		string filePath = "Assets/Levels/" + fileName;
        try
        {
            InitializeGrid(filePath);
			CreateBorder();
            StreamReader reader = new StreamReader(filePath, Encoding.Default);
            using(reader)
            {
                int y = 0;
                string line = reader.ReadLine();
				while(line != null && line[0] != '#' && line[0] != '^' && line[0] != '&')
                {
                    string[] entries = line.Split(',');
					for(int x = 0; x < InstantiationGridWidth; x++)
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
							if(entries[x].Length > 0)
								g.Add(Convert.ToInt32(entries[x]), Convert.ToInt32(entries[x+1]));
						}
						InstantiationRotationGroups.Add(g);
						line = reader.ReadLine();
					}while(line != null && line[0] == '#');
				}
				if(line != null && line[0] == '^')
				{
					do
					{
						List<string> queue = new List<string>();
						line = line.Trim( new char[] {'^'});
						line = line.Trim( new char[] {','});
						string[] entries = line.Split(',');
						int xpos = Convert.ToInt32(entries[0]);
						int ypos = Convert.ToInt32(entries[1]);
						for(int x = 2; x < entries.Length; x++)
						{
							queue.Add(entries[x]);
						}
						InstantiationGridSquareGrid[xpos,ypos].GridSquareExitQueue = queue;
						InstantiationGridSquareGrid[xpos,ypos].OnlyChangeSubCube(Convert.ToInt32(queue[0]));
						line = reader.ReadLine();
					}while(line != null && line[0] == '^');
				}
				if(line != null && line[0] == '&')
				{
					do
					{
						line = line.Trim( new char[] {'&'});
						line = line.Trim( new char[] {','});
						string[] entries = line.Split(',');
						int xpos = Convert.ToInt32(entries[0]);
						int ypos = Convert.ToInt32(entries[1]);
						string message = entries[2];

						InstantiationTutorialEvents.Add(new TutorialEvent(xpos,ypos,message));
						line = reader.ReadLine();
					}while(line != null && line[0] == '&');
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
			if(line != null && line[0] != '#' && line[0] != '^' && line[0] != '&')
			{
				string[] entries = line.Split(',');
				foreach (string entry in entries)
				{
					if(entry.Length > 0)
						InstantiationGridWidth++;
				}
                InstantiationGridHeight++;
			}
            line = reader.ReadLine();
			while(line != null && line[0] != '#' && line[0] != '^' && line[0] != '&')
			{
                InstantiationGridHeight++;
                line = reader.ReadLine();
			}
		}
        InstantiationGridSquareGameObjectGrid = new GameObject[InstantiationGridWidth, InstantiationGridHeight];
        InstantiationGridSquareGrid = new GridSquare[InstantiationGridWidth, InstantiationGridHeight];
	}
	public void CreateBorder()
	{
		//create top
		for(int i = -1; i < InstantiationGridWidth+1; i++)
		{
			CreateWall(i - Instantiation.XOFFSET, Instantiation.YOFFSET + 1);
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.position = new Vector3(i - Instantiation.XOFFSET, Instantiation.YOFFSET + 1, 0);
			go.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			go.GetComponent<Renderer>().material = MaterialDictionary["Border"];
		}
		//create bottom
		for(int i = -1; i < InstantiationGridWidth+1; i++)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.position = new Vector3(i - Instantiation.XOFFSET, Instantiation.YOFFSET - InstantiationGridHeight, 0);
			go.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			go.GetComponent<Renderer>().material = MaterialDictionary["Border"];
		}
		//create left
		for(int i = 0; i < InstantiationGridHeight; i++)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.position = new Vector3(-1 - Instantiation.XOFFSET, Instantiation.YOFFSET - i, 0);
			go.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			go.GetComponent<Renderer>().material = MaterialDictionary["Border"];
		}
		//create right
		for(int i = 0; i < InstantiationGridHeight; i++)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.position = new Vector3(InstantiationGridWidth - Instantiation.XOFFSET, Instantiation.YOFFSET - i, 0);
			go.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			go.GetComponent<Renderer>().material = MaterialDictionary["Border"];
		}
			
	}
	public void CreateWall(int x, int y)
	{
		for( int z = 0; z < 4; z++)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.position = new Vector3(x, y, -z); 
			go.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			if(z == 1 && x == 1)
				go.GetComponent<Renderer>().material = MaterialDictionary["WallF2"];
			else if (z == 1)
				go.GetComponent<Renderer>().material = MaterialDictionary["WallF1"];
			else if (z == 2)
				go.GetComponent<Renderer>().material = MaterialDictionary["WallC1"];
			else if (z == 3)
				go.GetComponent<Renderer>().material = MaterialDictionary["WallT1"];
		}
	}
	void Update () 
	{
		if (fLevelStartTimer > 0) 
		{
			StartTextAlpha = 1 - ((MaxTime-fLevelStartTimer)/(MaxTime));
			fLevelStartTimer -= Time.deltaTime;
		}
		Color color = LevelText.material.color;
		color.a = StartTextAlpha;
		LevelText.material.color = color;
		GetMouseRays();
		
		if (fLevelStartTimer <= 0 && !(bLevelWon || bLevelLost)) 
		{ 
			StartTextAlpha = Math.Max(0.0f,StartTextAlpha - 0.1f);
			fLevelStartTimer = 0.0f;
			UpdateSpawnTile();
			UpdateMonsterAction();
		} 
		if(bLevelWon || bLevelLost)
		{
			StartTextAlpha = 1.0f;
		}
	}
	void PauseGame() 
	{ 
		Time.timeScale = 0.000001f; 
	} 
	void OnGUI () 
	{
		GUI.skin = window; 
		MakeMapControls ();
		GUI.Box (new Rect (Screen.width - 50, 0, 50, 20), "" + fLevelStartTimer.ToString ("f0")); 

		foreach(TutorialEvent TE in InstantiationTutorialEvents)
		{
			if(TE.isTriggered() && !TE.isCompleted())
			{
				Time.timeScale = 0.000001f; 
				GUIStyle style = new GUIStyle(GUI.skin.textField);
				style.wordWrap = true;
				GUI.TextField(new Rect(Screen.width/3, Screen.height/3, Screen.width/4, Screen.height/4), TE.getMessage(), style);
				if (GUI.Button (new Rect (Screen.width/3 + Screen.width/4 - 23, Screen.height/3 - 23, 23, 23), "X")) 
				{
					Time.timeScale = 1.0f; 
					TE.setTrigger(false);
					TE.setCompleted(true);
				}
			}
		}

		if(bLevelWon)
		{
			if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 60, 125, 50), "Continue")) 
			{
				Application.LoadLevel (Application.loadedLevelName); 
			} 
		}
		else if(bLevelLost)
		{
			if (GUI.Button (new Rect (Screen.width - 105, Screen.height - 60, 125, 50), "Retry?")) 
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
				foreach(GridSquare square in InstantiationGridSquareGrid)
				{
					if(pObject == square.subObject)
					{
						pObject = square.GridSquareGameObject;
					}
				}
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
					InstantiationGridSquareGrid[XOFFSET + (int)pObject.transform.position.x, YOFFSET - (int)pObject.transform.position.y].SetSubCubeAsMain();
				}

			}
		}
		if(Input.GetMouseButtonDown(1)&& !bRightMouseClicked)
		{
			bRightMouseClicked=true;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    
			RightClickedOriginPoint = ray.origin + (ray.direction * RayDistance); 
			RightClickedCurrentPoint = ray.origin + (ray.direction * RayDistance);  
		}
		if(Input.GetMouseButtonUp(1))
		{
			bRightMouseClicked=false;
		}
		if(bRightMouseClicked)
		{
			bRightMouseClicked=true;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    
			RightClickedCurrentPoint = ray.origin + (ray.direction * RayDistance);    
		}


		GameObject camera = GameObject.Find ("Main Camera");
		int moveX = 0;
		int moveY = 0;
		float magicNumber = 5;
		
		if( bRightMouseClicked )
		{
			camera.transform.position += new Vector3((float)(RightClickedOriginPoint.x - RightClickedCurrentPoint.x), (float)(RightClickedOriginPoint.y - RightClickedCurrentPoint.y) , 0f);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    
			RightClickedOriginPoint = ray.origin + (ray.direction * RayDistance); 
			RightClickedCurrentPoint = ray.origin + (ray.direction * RayDistance);  
		}
		if (Input.GetAxis ("Mouse ScrollWheel") != 0) 
		{
			moveY = (int)(20*Input.GetAxis ("Mouse ScrollWheel"));
			camera.transform.position += new Vector3(1/magicNumber*moveX, 1/magicNumber* moveY , 0f);
		}

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
		if(y < 2 - InstantiationGridHeight)
			GetComponent<Camera>().transform.position = new Vector3(x, 2 - InstantiationGridHeight, z);
		if(y > 2)
			GetComponent<Camera>().transform.position = new Vector3(x, 2, z);
		
	}

	void MakeMapControls()
	{
		selGridInt = GUI.SelectionGrid(new Rect(Screen.width- 300, 50, 200, 50), selGridInt, selStrings, 4);
		switch(selGridInt)
		{
			case 0:
			    sound.PlayOneShot (clip); 
				Time.timeScale = 0.5f;
				break;
			case 1:
				sound.PlayOneShot (clip); 
				Time.timeScale = 1.5f;
				break;
			case 2:
				sound.PlayOneShot (clip); 
				Time.timeScale = 3.0f;
				break;
			case 3:
				sound.PlayOneShot (clip);
				Time.timeScale = 9.0f;
				break;
			default:
				break;
		}
	}

	void UpdateSpawnTile()
	{
		for(int x = 0; x < InstantiationGridWidth; x++) 
		{
            for(int y = 0; y < InstantiationGridHeight; y++) 
			{
				if(InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterZero && InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn <= 0)
				{
					Monster monster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, NumberType.Zero, x, y, MovementDirection.None);
					InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
                    InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn = InstantiationSpawnDelay;
				}
				if(InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterOne && InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn <= 0)
				{
					Monster monster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, NumberType.One, x, y, MovementDirection.None);
					InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
					InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn = InstantiationSpawnDelay;
				}
				else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterRandom && InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn <= 0)
				{
					var number = UnityEngine.Random.Range(0,2);
					NumberType numType = number == 0 ? NumberType.Zero : NumberType.One;
					Monster monster = new Monster(this, InstantiationNextMonsterId, MovementType.Moving, numType, x, y, MovementDirection.None);
					InstantiationGridSquareGrid[x, y].CalculateNewDirection(monster);
					InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn = InstantiationSpawnDelay;
				}
				else if (InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterZero || InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterOne|| InstantiationGridSquareGrid[x, y].GridSquareTileType == TileType.EnterRandom)
				{
					InstantiationGridSquareGrid[x, y].GridSquareTimeToNextSpawn-= (int) Time.timeScale;
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
				if(m.MonsterXPosition == x && m.MonsterYPosition == y && m.MonsterId != monster.MonsterId && monster.MonsterMovementDirection != m.MonsterMovementDirection && m.MonsterMovementType == MovementType.Waiting)
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
		//int notwincheck = bLevelWon || bLevelLost ? 0 : 1;
		monster.MonsterGameObject.transform.position += new Vector3(monster.MonsterMovementIncrement * moveX * Time.timeScale, monster.MonsterMovementIncrement * moveY * Time.timeScale, 0);

        if(monster.FinishedMovingTile())
		{
			foreach(TutorialEvent TE in InstantiationTutorialEvents)
			{
				if (!TE.isCompleted() && TE.x() == monster.MonsterXPosition && TE.y() == monster.MonsterYPosition)
				{
					TE.setTrigger(true);
				}
			}
            monster.MonsterMovementType = MovementType.Waiting;
        }
	}

	void CheckForWin(int x, int y, Monster monster)
	{
		//assign the current tile to win or lose from blank. cant be assigned unless blank or win.(if you lose you lose)
		//check all tiles, if any of the end pieces are lose then return you lose
		//if the amount of winning tiles == the number of end pieces then you win
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
					{
						PlayerHealth = Math.Max(PlayerHealth - .3, -1);
						InstantiationGridSquareGrid[i,j].GridSquareHasWinningPiece = WinCondition.NoPiece;
						//LevelText.text = "Oh No!";
						//StartTextAlpha = 1.0f;
					}
					if(InstantiationGridSquareGrid[i,j].GridSquareHasWinningPiece == WinCondition.Correct)
					{
						PlayerHealth = Math.Min(PlayerHealth + .3, 1);
						int next;
						if(InstantiationGridSquareGrid[i,j].GridSquareExitQueue != null && InstantiationGridSquareGrid[i,j].GridSquareExitQueue.Count > 0)
						{
							next = Convert.ToInt32(InstantiationGridSquareGrid[i,j].GridSquareExitQueue[0]);
							InstantiationGridSquareGrid[i,j].GridSquareExitQueue.Remove(InstantiationGridSquareGrid[i,j].GridSquareExitQueue[0]);
						}
						else
							next = InstantiationGridSquareGrid[i,j].GridSquareTileType == TileType.ExitOne ? 1 : 0;
						InstantiationGridSquareGrid[i,j].SetNextCubeNumber(next);
						InstantiationGridSquareGrid[i,j].GridSquareHasWinningPiece = WinCondition.NoPiece;
						//LevelText.text = "Keep It Up!";
						//StartTextAlpha = 1.0f;
					}
				}
			}
		}
		TextureNeedle.gameObject.transform.position = OriginalNeedlePosition;
		TextureNeedle.gameObject.transform.rotation = new Quaternion(0,0,0,0);
		Vector3 P = RotationPoint.gameObject.transform.position;
		//Vector3 position = new Vector3(P.x, P.y-OriginalNeedleRotation.rectTransform.rect.height, P.z);
		//Vector3 position = new Vector3(P.x,P.y,P.z);
		TextureNeedle.gameObject.transform.RotateAround(P, Vector3.back,(float)PlayerHealth*35);
		//TextureNeedle.gameObject.transform.position = OriginalNeedlePosition;
		//TextureNeedle.gameObject.transform.rotation = new Quaternion(0,0,0,0);
		if (PlayerHealth <= -1)
		{
			//PrintMessage ("YOU LOSE!");
			bLevelLost = false;
			LevelText.text = "YOU LOSE!";
			/*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0, 0, 0);
			cube.transform.localScale = new Vector3(10, 10, 10);
			cube.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			cube.GetComponent<Renderer>().material = MaterialDictionary["Lose"];*/
		}
		else if (PlayerHealth >= 1)
		{
			//PrintMessage ("YOU WIN!");
			SetNextPlayerLevel();
			bLevelWon = true;
			LevelText.text = "YOU WIN!";
			/*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0, 0, 0);
			cube.transform.localScale = new Vector3(10, 10, 10);
			cube.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			cube.GetComponent<Renderer>().material = MaterialDictionary["Win"];*/
		}
		else
		{
			PrintMessage ("GOOD JOB KEEP GOING!");
		}

		monster.DestroyEntirely ();
	}

	// returns true if the level has not already been reached by the Player -- Prevents duplicate entries 
	public bool CheckLevel(string lvl) 
	{ 
		for(int worlds = 0; worlds < Game.current.player.levelsList.Count; worlds ++)
		{
			for(int levels = 0; levels < Game.current.player.levelsList[worlds].Count; levels ++)
			{
				if(Game.current.player.levelsList[worlds][levels] == lvl)
				{
					return false;
				}
			}
		}
		return true; 

	} // end CheckLevel  

	public void SetNextPlayerLevel()
	{
		List<List<string>> levelsList = LevelMenu.GetLevels();
		//go through each world
		for(int worlds = 0; worlds < levelsList.Count; worlds++)
		{
			//go through each level in the world
			for(int i = 0; i < levelsList[worlds].Count; i++)
			{
				//if the current level == a worlds level, and that level is not the last in the world
				if(Game.current.player.currLevel == levelsList[worlds][i] && i < levelsList[worlds].Count - 1)
				{
					//check to make sure the next world isnt null.
					if(levelsList[worlds][i+1] != null) 
					{
						//set the current level to the next level
						Game.current.player.currLevel = levelsList[worlds][i+1];
						// Only call this if the level has not already been reached 
						if (CheckLevel (levelsList[worlds][i+1])) 
						{ 
							Game.current.player.highestLevel = levelsList[worlds][i+1];
							Game.current.player.levelsList[worlds].Add(levelsList[worlds][i+1]); 
						} // end if statement 
						Save.SaveThis (); 
					} // end if statement 
					return;
				}
				//if the current level is a level and the last level in a world
				else if(Game.current.player.currLevel == levelsList[worlds][i] && i == levelsList[worlds].Count - 1)
				{
					if(levelsList.Count > Game.current.player.world + 1)
					{
						if(levelsList[Game.current.player.world+1].Count > 0 && levelsList[Game.current.player.world+1][0] != null) 
						{
							Game.current.player.currLevel = levelsList[Game.current.player.world+1][0];
							// Only call this if the level has not already been reached 
							if (CheckLevel (levelsList[Game.current.player.world+1][0])) 
							{ 
								Game.current.player.world ++;
								Game.current.player.highestLevel = levelsList[Game.current.player.world][0];
								List<string> temp = new List<string>();
								Game.current.player.levelsList.Add(temp);
								Game.current.player.levelsList[Game.current.player.world].Add(levelsList[Game.current.player.world][0]); 
							} // end if statement 
							Save.SaveThis (); 
							return;
						} // end if statement 
					}
				}
			}
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
			InstantiationGridSquareGrid [rg.xAt (i), rg.yAt (i)].SetSubCubeAsMain();
		}

		InstantiationGridSquareGrid [rg.xAt (rg.getCount () - 1), rg.yAt (rg.getCount () - 1)].GridSquareTileType = tempTileType;
		InstantiationGridSquareGrid [rg.xAt (rg.getCount () - 1), rg.yAt (rg.getCount () - 1)].GridSquareGameObject.GetComponent<Renderer>().sharedMaterial = tempTileMaterial;
		InstantiationGridSquareGrid [rg.xAt (rg.getCount () - 1), rg.yAt (rg.getCount () - 1)].SetSubCubeAsMain();
		
	}
	public static void PrintMessage(string printStatement)
	{
		print (printStatement);
	}
}
