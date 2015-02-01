using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileType
{
    Blank, EnterZero, EnterOne, ExitZero, ExitOne, And, Or, BeltUp, BeltRight, BeltDown, BeltLeft
}

public class GridSquare
{
    public GameObject GridSquareGameObject { get; set; }
    public Instantiation GridSquareInstantiation { get; set; }
    public TileType GridSquareTileType { get; set; }
    public int GridSquareXPosition { get; set; }
    public int GridSquareYPosition { get; set; }
	public int GridSquareTimeToNextSpawn { get; set; }

    public GridSquare()
    {
        GridSquareGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GridSquareInstantiation = new Instantiation();
        GridSquareTileType = TileType.Blank;
        GridSquareXPosition = 0;
        GridSquareYPosition = 0;
		GridSquareTimeToNextSpawn = 0;
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
        switch(GridSquareTileType)
        {
            case TileType.Blank:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.Blank;
                break;
            case TileType.EnterZero:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.EnterZero;
                break;
            case TileType.EnterOne:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.EnterOne;
                break;
            case TileType.ExitZero:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.ExitZero;
                break;
            case TileType.ExitOne:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.ExitOne;
                break;
            case TileType.And:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.And;
                break;
            case TileType.Or:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.Or;
                break;
            case TileType.BeltUp:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltUp;
                break;
            case TileType.BeltRight:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltRight;
                break;
            case TileType.BeltDown:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltDown;
                break;
            case TileType.BeltLeft:
                GridSquareGameObject.renderer.material = GridSquareInstantiation.BeltLeft;
                break;
            default:
                Instantiation.PrintMessage("Invalid GridSquareTileType - GridSquare(Instantiation gridSquareInstantiation, TileType gridSquareTileType, int gridSquareXPosition, int gridSquareYPosition)");
                break;
        }
        GridSquareInstantiation.InstantiationGridSquareGameObjectGrid[GridSquareXPosition, GridSquareYPosition] = GridSquareGameObject;
        GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition] = this;
    }

    public List<GridSquare> GetNeighborsMovingAway()
    {
        List<GridSquare> neighbors = new List<GridSquare>();

        if (GridSquareXPosition > 0 && GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition - 1, GridSquareYPosition].GridSquareTileType == TileType.BeltLeft)
        {
            neighbors.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition - 1, GridSquareYPosition]);
        }
        if (GridSquareXPosition < GridSquareInstantiation.InstantiationGridWidth - 1 && GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition + 1, GridSquareYPosition].GridSquareTileType == TileType.BeltRight)
        {
            neighbors.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition + 1, GridSquareYPosition]);
        }
        if (GridSquareYPosition > 0 && GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition - 1].GridSquareTileType == TileType.BeltUp)
        {
            neighbors.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition - 1]);
        }
        if (GridSquareYPosition < GridSquareInstantiation.InstantiationGridHeight - 1 && GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition + 1].GridSquareTileType == TileType.BeltDown)
        {
            neighbors.Add(GridSquareInstantiation.InstantiationGridSquareGrid[GridSquareXPosition, GridSquareYPosition + 1]);
        }

        return neighbors;
    }

	public void CalculateNewDirection(Monster monster)
	{
        List<GridSquare> neighbors = GetNeighborsMovingAway();
        foreach(GridSquare neighbor in neighbors)
        {
            MovementDirection movementDirection = MovementDirection.None;
            switch(neighbor.GridSquareTileType)
            {
                case TileType.BeltUp:
                    movementDirection = MovementDirection.Up;
                    break;
                case TileType.BeltRight:
                    movementDirection = MovementDirection.Right;
                    break;
                case TileType.BeltDown:
                    movementDirection = MovementDirection.Down;
                    break;
                case TileType.BeltLeft:
                    movementDirection = MovementDirection.Left;
                    break;
                default:
                    Instantiation.PrintMessage("Invalid GridSquareTileType - CalculateNewDirection(Monster monster)");
                    break;
            }
			new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, movementDirection);
        }
        if(neighbors.Count == 0)
        {
            MovementDirection movementDirection = MovementDirection.None;
            switch(GridSquareTileType)
            {
                case TileType.BeltUp:
                    movementDirection = MovementDirection.Up;
                    break;
                case TileType.BeltRight:
                    movementDirection = MovementDirection.Right;
                    break;
                case TileType.BeltDown:
                    movementDirection = MovementDirection.Down;
                    break;
                case TileType.BeltLeft:
                    movementDirection = MovementDirection.Left;
                    break;
                default:
                    Instantiation.PrintMessage("Invalid GridSquareTileType - CalculateNewDirection(Monster monster)");
                    break;
            }
			new Monster(GridSquareInstantiation, GridSquareInstantiation.InstantiationNextMonsterId, MovementType.Moving, monster.MonsterNumberType, monster.MonsterXPosition, monster.MonsterYPosition, movementDirection);
        }
        GridSquareInstantiation.InstantiationMonsters.Remove(monster);
		Instantiation.Destroy(monster.MonsterGameObject, 0f);
	}
}
