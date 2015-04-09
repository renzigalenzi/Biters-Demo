using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Collections.Specialized;

public enum TileType
{
    Blank, EnterZero, EnterOne, ExitZero, ExitOne, And, Xnor, Nor, Not, Or, Xor, Nand, BeltVertical, BeltHorizontal, 
	BeltUpLeft, BeltUpRight, BeltDownRight, BeltDownLeft, 
	BeltUpT, BeltRightT, BeltDownT, BeltLeftT, BeltCross, EnterRandom, Count
}




public enum TileDirection
{
	None, Up, Right, Down, Left
}
public enum WinCondition
{
	NoPiece, Correct, Incorrect
}

public class GridSquare
{
	public List<string> GridSquareExitQueue{ get; set; }
    public GameObject GridSquareGameObject { get; set; }
    public Instantiation GridSquareInstantiation { get; set; }
    public TileType GridSquareTileType { get; set; }
	public TileDirection GridSquareTileDirection { get; set; }
	public WinCondition GridSquareHasWinningPiece { get; set; }
    public int GridSquareXPosition { get; set; }
    public int GridSquareYPosition { get; set; }
	public int GridSquareTimeToNextSpawn { get; set; }
	public List<GridSquare> MovePossibilities = new List<GridSquare>();
	public Quaternion OriginalRotationValue { get; set; }

	public GameObject subObject;
	private TileType subType;

    public GridSquare()
    {
        GridSquareGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		OriginalRotationValue = GridSquareGameObject.transform.rotation;
        GridSquareInstantiation = new Instantiation();
        GridSquareTileType = TileType.Blank;
		GridSquareTileDirection = TileDirection.None;
        GridSquareXPosition = 0;
        GridSquareYPosition = 0;
		GridSquareTimeToNextSpawn = 0;
		GridSquareHasWinningPiece = WinCondition.NoPiece;
    }
	public GridSquare(GameObject gObject)
	{
		GridSquareGameObject = gObject;
		OriginalRotationValue = GridSquareGameObject.transform.rotation;
		GridSquareTileType = TileType.Blank;
		GridSquareTileDirection = TileDirection.None;
		GridSquareXPosition = 0;
		GridSquareYPosition = 0;
		GridSquareTimeToNextSpawn = 0;
		GridSquareHasWinningPiece = WinCondition.NoPiece;
	}

	public GridSquare(Instantiation gridSquareInstantiation, TileType gridSquareTileType, int gridSquareXPosition, int gridSquareYPosition)
    { 
        GridSquareGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GridSquareInstantiation = gridSquareInstantiation;
		GridSquareTileType = gridSquareTileType;
        GridSquareXPosition = gridSquareXPosition;
        GridSquareYPosition = gridSquareYPosition;
		GridSquareTimeToNextSpawn = 0;

        GridSquareGameObject.transform.position = new Vector3(GridSquareXPosition - Instantiation.XOFFSET, Instantiation.YOFFSET - GridSquareYPosition, 0);
		GridSquareGameObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
		OriginalRotationValue = GridSquareGameObject.transform.rotation;

		RotateSquareByMaterial ();
		AssignMaterialToInstantiation ();
		GridSquareTileDirection = TileDirection.None;
		GridSquareHasWinningPiece = WinCondition.NoPiece;

        GridSquareInstantiation.InstantiationGridSquareGameObjectGrid[GridSquareXPosition, GridSquareYPosition] = GridSquareGameObject;
        GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition] = this;

		if(gridSquareTileType == TileType.ExitOne || gridSquareTileType == TileType.ExitZero)
		{
			MakeNextCube(GridSquareXPosition - Instantiation.XOFFSET, Instantiation.YOFFSET - GridSquareYPosition);
		}
		if(IsGate())
		{
			MakeDepthGateCube(GridSquareXPosition - Instantiation.XOFFSET, Instantiation.YOFFSET - GridSquareYPosition);
		}
		if(IsEnter())
		{
			MakeDepthEnterCube(GridSquareXPosition - Instantiation.XOFFSET, Instantiation.YOFFSET - GridSquareYPosition);
		}

    }
	public void MakeNextCube(float x, float y)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Vector3 scale = go.transform.localScale;
		scale.x = .5F; scale.y = .5F;
		go.transform.localScale = scale;
		go.transform.position = new Vector3(x+.25f,y-.25f,-.1f);
		go.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
		go.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)GridSquareTileType)];
		subType = GridSquareTileType;
		subObject = go;
	}
	public void MakeDepthGateCube(float x, float y)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Vector3 scale = go.transform.localScale;
		scale.x = .75F; scale.y = .75F; scale.z = .75F;
		go.transform.localScale = scale;
		go.transform.position = new Vector3(x,y,-.75f);
		go.transform.Rotate (0,180.0f,0);
		go.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)GridSquareTileType)];
		subType = GridSquareTileType;
		subObject = go;
	}
	public void MakeDepthEnterCube(float x, float y)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Vector3 scale = go.transform.localScale;
		scale.x = 0.5F; scale.y = 0.5F; scale.z = 1F;
		go.transform.localScale = scale;
		go.transform.position = new Vector3(x,y,-1.0f);
		go.transform.Rotate (0, 180.0f, 0);
		go.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)GridSquareTileType)];
		subType = GridSquareTileType;
		subObject = go;
	}
	public void SetSubCubeAsMain()
	{
		subType = GridSquareTileType;
		subObject.GetComponent<Renderer>().material = 
			GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)subType)];
	}
	public void OnlyChangeSubCube(int num)
	{
		if(num == 0)
		{
			subType = TileType.ExitZero;
			subObject.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)subType)];
		}
		else if(num == 1)
		{
			subType = TileType.ExitOne;
			subObject.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)subType)];
		}
	}
	public void SetNextCubeNumber(int num)
	{
		GridSquareTileType = subType;
		GridSquareGameObject.GetComponent<Renderer>().material = 
			subObject.GetComponent<Renderer>().material;
		if(num == 0)
		{
			subType = TileType.ExitZero;
			subObject.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)subType)];
		}
		else if(num == 1)
		{
			subType = TileType.ExitOne;
			subObject.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)subType)];
		}

	}
	public void SetNextCubeNumber()
	{
		GridSquareTileType = subType;
		GridSquareGameObject.GetComponent<Renderer>().material = 
			subObject.GetComponent<Renderer>().material;
		if(subType == TileType.ExitOne)
		{
			subType = TileType.ExitZero;
			subObject.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)subType)];
		}
		else if(subType == TileType.ExitZero)
		{
			subType = TileType.ExitOne;
			subObject.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)subType)];
		}
	}
	public void RotateSquareByMaterial()
	{
		GridSquareGameObject.transform.rotation = OriginalRotationValue;
		switch (GridSquareTileType) 
		{
		case TileType.BeltUpRight:
		case TileType.BeltUpT:
		case TileType.BeltVertical:
			GridSquareGameObject.transform.Rotate(0,0,90);
			break;
		case TileType.BeltUpLeft:
		case TileType.BeltRightT:
			GridSquareGameObject.transform.Rotate(0,0,180);
			break;
		case TileType.BeltDownLeft:
		case TileType.BeltDownT:
			GridSquareGameObject.transform.Rotate(0,0,270);
			break;

		}
	}
	public void AssignMaterialToInstantiation()
	{
		GridSquareGameObject.GetComponent<Renderer>().material = GridSquareInstantiation.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)GridSquareTileType)];
	}
	public void AssignMaterialToLevelConstructor(LevelConstructor parent)
	{
		GridSquareGameObject.GetComponent<Renderer>().material = parent.MaterialDictionary[Enum.GetName(typeof(TileType),(TileType)GridSquareTileType)];
	}
	public bool TileAccepts(GridSquare tile, TileDirection direction)
	{
		TileType type = tile.GridSquareTileType;
		//if the tile is a gate return true
		if (type == TileType.And || type == TileType.Or || type == TileType.Nand || type == TileType.Xor || type == TileType.Xnor || type == TileType.Nor || type == TileType.Not)
			return true;

		//if it is an end state return true
		if (type == TileType.ExitOne || type == TileType.ExitZero || type == TileType.EnterZero || type == TileType.EnterOne || type == TileType.EnterRandom)
			return true;

		//otherwise loop through the possible directions
		switch (direction) 
		{
			case TileDirection.Right:
				if(type == TileType.BeltDownLeft||
				   type == TileType.BeltRightT ||
			       type == TileType.BeltUpT ||
				   type == TileType.BeltUpLeft ||
			   	   type == TileType.BeltDownT ||
			  	   type == TileType.BeltHorizontal ||
			  	   type == TileType.BeltCross)
				return true;
				break;
			case TileDirection.Left:
				if(type == TileType.BeltDownRight||
				   type == TileType.BeltLeftT ||
			   	   type == TileType.BeltUpT ||
				   type == TileType.BeltUpRight ||
				   type == TileType.BeltDownT ||
			   	   type == TileType.BeltHorizontal ||
			       type == TileType.BeltCross)
				return true;
			break;
			case TileDirection.Up:
				if(type == TileType.BeltUpRight||
				   type == TileType.BeltUpT ||
				   type == TileType.BeltLeftT ||
				   type == TileType.BeltRightT ||
				   type == TileType.BeltUpLeft ||
			   	   type == TileType.BeltVertical ||
			   	   type == TileType.BeltCross)
				return true;
			break;
			case TileDirection.Down:
				if(
				   type == TileType.BeltDownLeft||
				   type == TileType.BeltDownT ||
				   type == TileType.BeltLeftT ||
				   type == TileType.BeltRightT ||
				   type == TileType.BeltDownRight ||
				   type == TileType.BeltVertical ||
				   type == TileType.BeltCross)
				return true;
			break;
				
		}
		return false;
	}

	public void AssignDirection()
	{
		//look at neighboring pieces and see if there are any pieces that will accept movement, add to movement list
		if (GridSquareXPosition > 0 && TileAccepts(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition], TileDirection.Right) && TileAccepts(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition - 1, GridSquareYPosition], TileDirection.Left) )
		{
			MovePossibilities.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition - 1, GridSquareYPosition]);
		}
		if (GridSquareXPosition < GridSquareInstantiation.InstantiationGridWidth - 1 && TileAccepts(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition], TileDirection.Left) && TileAccepts(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition + 1, GridSquareYPosition], TileDirection.Right) )
		{
			MovePossibilities.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition + 1, GridSquareYPosition]);
		}
		if (GridSquareYPosition > 0 && TileAccepts(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition], TileDirection.Down) && TileAccepts(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition - 1], TileDirection.Up) )
		{
			MovePossibilities.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition - 1]);
		}
		if (GridSquareYPosition < GridSquareInstantiation.InstantiationGridHeight - 1 && TileAccepts(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition], TileDirection.Up) && TileAccepts(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition + 1], TileDirection.Down))
		{
			MovePossibilities.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition + 1]);
		}
	}
	public void AssignDirection(GridSquare[,] grid, int gridW, int gridH)
	{
		//look at neighboring pieces and see if there are any pieces that will accept movement, add to movement list
		if (GridSquareXPosition > 0 && TileAccepts(grid[GridSquareXPosition, GridSquareYPosition], TileDirection.Right) && TileAccepts(grid[GridSquareXPosition - 1, GridSquareYPosition], TileDirection.Left) )
		{
			MovePossibilities.Add(grid[GridSquareXPosition - 1, GridSquareYPosition]);
		}
		if (GridSquareXPosition < gridW - 1 && TileAccepts(grid[GridSquareXPosition, GridSquareYPosition], TileDirection.Left) && TileAccepts(grid[GridSquareXPosition + 1, GridSquareYPosition], TileDirection.Right) )
		{
			MovePossibilities.Add(grid[GridSquareXPosition + 1, GridSquareYPosition]);
		}
		if (GridSquareYPosition > 0 && TileAccepts(grid[GridSquareXPosition, GridSquareYPosition], TileDirection.Down) && TileAccepts(grid[GridSquareXPosition, GridSquareYPosition - 1], TileDirection.Up) )
		{
			MovePossibilities.Add(grid[GridSquareXPosition, GridSquareYPosition - 1]);
		}
		if (GridSquareYPosition < gridH - 1 && TileAccepts(grid[GridSquareXPosition, GridSquareYPosition], TileDirection.Up) && TileAccepts(grid[GridSquareXPosition, GridSquareYPosition + 1], TileDirection.Down))
		{
			MovePossibilities.Add(grid[GridSquareXPosition, GridSquareYPosition + 1]);
		}
	}
	public void SubtractDirection(Monster m)
	{
		TileDirection direction = (TileDirection)m.MonsterMovementDirection;
		int x = 0;
		int y = 0;

		switch (direction) 
		{
		case TileDirection.Left:
			x++;
			break;
		case TileDirection.Right:
			x--;
			break;
		case TileDirection.Up:
			y++;
			break;
		case TileDirection.Down:
			y--;
			break;
		}
		List<GridSquare> tempList = new List<GridSquare>();
		foreach (GridSquare tile in MovePossibilities)
		{
			tempList.Add (tile);
		}
		foreach (GridSquare square in tempList) 
		{
			if(square == GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition + x, GridSquareYPosition + y])
			{
				MovePossibilities.Remove(square);
			}
		}

		//a monster entered this tile from a direction, remove it from possible output directions
	}

	public void CalculateNewDirection(Monster monster) // @RCH: Fix this to work off of current tile's Out direction, instead of neighbors' in direction
	{
		SubtractDirection (monster);
		foreach(GridSquare neighbor in MovePossibilities)
		{
			MovementDirection direction = MovementDirection.None;
			if(neighbor.GridSquareYPosition < GridSquareYPosition)
				direction = MovementDirection.Up;
			else if(neighbor.GridSquareXPosition > GridSquareXPosition)
				direction = MovementDirection.Right;
			else if(neighbor.GridSquareYPosition > GridSquareYPosition)
				direction = MovementDirection.Down;
			else if(neighbor.GridSquareXPosition < GridSquareXPosition)
				direction = MovementDirection.Left;
			else
				Instantiation.PrintMessage("Invalid GridSquareTileDirection - CalculateNewDirection(Monster monster)");

			new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, direction);
		}
        GridSquareInstantiation.InstantiationMonsters.Remove(monster);
		Instantiation.Destroy(monster.MonsterGameObject, 0f);
	}
	public bool IsBelt()
	{
		//Use a Dictionary (and make it static...), otherwise just use a switch statement: Both are faster in every case like this.
		//Also add this function as a static function extension to TileType.

		List<TileType> Belts = new List<TileType> (new TileType[] {TileType.BeltVertical, TileType.BeltHorizontal, 
			TileType.BeltUpLeft, TileType.BeltUpRight, TileType.BeltDownRight, TileType.BeltDownLeft, 
			TileType.BeltUpT, TileType.BeltRightT, TileType.BeltDownT, TileType.BeltLeftT, TileType.BeltCross});

		return Belts.Contains (GridSquareTileType);
	}

	public bool IsGate()
	{
		List<TileType> Gates = new List<TileType> (new TileType[] {TileType.And, TileType.Or, 
			TileType.Nand, TileType.Nor, TileType.Xor, TileType.Xnor});
		
		return Gates.Contains (GridSquareTileType);
	}

	public bool IsEnter()
	{
		bool isEnter = false;

		switch (this.GridSquareTileType) {
		case TileType.EnterOne:
		case TileType.EnterZero:
		case TileType.EnterRandom:
			isEnter = true; break;
		}

		return isEnter;
	}
}
