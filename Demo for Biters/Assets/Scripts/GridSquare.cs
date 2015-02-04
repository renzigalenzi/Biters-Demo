using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileType
{
    Blank, EnterZero, EnterOne, ExitZero, ExitOne, And, Or, BeltUp, BeltRight, BeltDown, BeltLeft,
	BeltUpLeft, BeltUpRight, BeltRightUp, BeltRightDown, BeltDownRight, BeltDownLeft, BeltLeftDown, BeltLeftUp,
	BeltUpT, BeltRightT, BeltDownT, BeltLeftT
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
    public GameObject GridSquareGameObject { get; set; }
    public Instantiation GridSquareInstantiation { get; set; }
    public TileType GridSquareTileType { get; set; }
	public TileDirection GridSquareTileDirection { get; set; }
	public WinCondition GridSquareHasWinningPiece { get; set; }
    public int GridSquareXPosition { get; set; }
    public int GridSquareYPosition { get; set; }
	public int GridSquareTimeToNextSpawn { get; set; }

    public GridSquare()
    {
        GridSquareGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GridSquareInstantiation = new Instantiation();
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
        switch(GridSquareTileType)
        {
            case TileType.Blank:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.Blank;
				GridSquareTileDirection = TileDirection.None;
                break;
            case TileType.EnterZero:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.EnterZero;
				GridSquareTileDirection = TileDirection.None;
				break;
            case TileType.EnterOne:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.EnterOne;
				GridSquareTileDirection = TileDirection.None;    
				break;
            case TileType.ExitZero:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.ExitZero;
				GridSquareTileDirection = TileDirection.None;    
				break;
            case TileType.ExitOne:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.ExitOne;
				GridSquareTileDirection = TileDirection.None;    
				break;
            case TileType.And:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.And;
            	GridSquareTileDirection = TileDirection.None;    
				break;
            case TileType.Or:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.Or;
				GridSquareTileDirection = TileDirection.None;    
				break;
            case TileType.BeltUp:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltUp;
				GridSquareTileDirection = TileDirection.Up;    
				break;
            case TileType.BeltRight:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltRight;
				GridSquareTileDirection = TileDirection.Right;    
				break;
            case TileType.BeltDown:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltDown;
				GridSquareTileDirection = TileDirection.Down;    
				break;
            case TileType.BeltLeft:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltLeft;
				GridSquareTileDirection = TileDirection.Left;    
				break;
			case TileType.BeltUpLeft:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltUpLeft;
				GridSquareTileDirection = TileDirection.Up;    
				break;
			case TileType.BeltUpRight:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltUpRight;
				GridSquareTileDirection = TileDirection.Up;    
				break;
			case TileType.BeltRightUp:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltRightUp;
				GridSquareTileDirection = TileDirection.Right;    
				break;
			case TileType.BeltRightDown:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltRightDown;
				GridSquareTileDirection = TileDirection.Right;    
				break;
			case TileType.BeltDownRight:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltDownRight;
				GridSquareTileDirection = TileDirection.Down;    
				break;
			case TileType.BeltDownLeft:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltDownLeft;
				GridSquareTileDirection = TileDirection.Down;    
				break;
			case TileType.BeltLeftDown:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltLeftDown;
				GridSquareTileDirection = TileDirection.Left;    
				break;
			case TileType.BeltLeftUp:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltLeftUp;
				GridSquareTileDirection = TileDirection.Left;    
				break;
			case TileType.BeltUpT:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltUpT;
				GridSquareTileDirection = TileDirection.Up;    
				break;
			case TileType.BeltRightT:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltRightT;
				GridSquareTileDirection = TileDirection.Right;    
				break;
			case TileType.BeltDownT:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltDownT;
				GridSquareTileDirection = TileDirection.Down;    
				break;
			case TileType.BeltLeftT:
				GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltLeftT;
				GridSquareTileDirection = TileDirection.Left;    
				break;
            default:
                Instantiation.PrintMessage("Invalid GridSquareTileType - GridSquare(Instantiation gridSquareInstantiation, TileType gridSquareTileType, int gridSquareXPosition, int gridSquareYPosition)");
                break;
        }
		GridSquareHasWinningPiece = WinCondition.NoPiece;

        GridSquareInstantiation.InstantiationGridSquareGameObjectGrid[GridSquareXPosition, GridSquareYPosition] = GridSquareGameObject;
        GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition] = this;
    }

    public List<GridSquare> GetNeighborsMovingAway()
    {
        List<GridSquare> neighbors = new List<GridSquare>();

        if (GridSquareXPosition > 0 && GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition - 1, GridSquareYPosition].GridSquareTileDirection == TileDirection.Left)
        {
            neighbors.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition - 1, GridSquareYPosition]);
        }
		if (GridSquareXPosition < GridSquareInstantiation.InstantiationGridWidth - 1 && GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition + 1, GridSquareYPosition].GridSquareTileDirection == TileDirection.Right)
        {
            neighbors.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition + 1, GridSquareYPosition]);
        }
		if (GridSquareYPosition > 0 && GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition - 1].GridSquareTileDirection == TileDirection.Up)
        {
            neighbors.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition - 1]);
        }
		if (GridSquareYPosition < GridSquareInstantiation.InstantiationGridHeight - 1 && GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition + 1].GridSquareTileDirection == TileDirection.Down)
        {
            neighbors.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition + 1]);
        }

        return neighbors;
    }

	public void CalculateNewDirection(Monster monster) // @RCH: Fix this to work off of current tile's Out direction, instead of neighbors' in direction
	{
	    switch(GridSquareTileType)
	    {
	        case TileType.BeltUp:
			case TileType.BeltRightUp:
			case TileType.BeltLeftUp:
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Up);
            	break;
			case TileType.BeltRight:
			case TileType.BeltUpRight:
			case TileType.BeltDownRight:
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Right);
            	break;
			case TileType.BeltDown:
			case TileType.BeltRightDown:
			case TileType.BeltLeftDown:
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Down);
            	break;
			case TileType.BeltLeft:
			case TileType.BeltUpLeft:
			case TileType.BeltDownLeft:
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Left);
            	break;
			case TileType.BeltUpT:
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Left);
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Right);
				break;
			case TileType.BeltRightT:
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Up);
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Down);
				break;
			case TileType.BeltDownT:
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Right);
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Left);
				break;
			case TileType.BeltLeftT:
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Down);
				new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, MovementDirection.Up);
				break;
			case TileType.EnterZero:
			case TileType.EnterOne:
			case TileType.And:
			case TileType.Or:
				List<GridSquare> neighbors = GetNeighborsMovingAway();
				foreach(GridSquare neighbor in neighbors)
				{
					MovementDirection direction = MovementDirection.None;
					if(neighbor.GridSquareTileDirection == TileDirection.Up)
						direction = MovementDirection.Up;
					else if(neighbor.GridSquareTileDirection == TileDirection.Right)
						direction = MovementDirection.Right;
			       	else if(neighbor.GridSquareTileDirection == TileDirection.Down)
						direction = MovementDirection.Down;
			       	else if(neighbor.GridSquareTileDirection == TileDirection.Left)
						direction = MovementDirection.Left;
					new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, direction);
				}
				break;
	        default:
	            Instantiation.PrintMessage("Invalid GridSquareTileType - CalculateNewDirection(Monster monster)");
	            break;
    	}
        GridSquareInstantiation.InstantiationMonsters.Remove(monster);
		Instantiation.Destroy(monster.MonsterGameObject, 0f);
	}
}
