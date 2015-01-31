using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridSquare{


	public int Type { get; set; }
	public int Direction { get; set; }
	public bool Reacts { get; set; }
	public int PosX { get; set; }
	public int PosY { get; set; }
	public int Count { get; set; }
	public Instantiation InstanceParent { get; set; }

	public void FindNewDirection(int x, int y, GridSquare[,] GridPieces, ref Monster monster, ref List<Monster> MonsterList)
	{
		List<GridSquare> neighbors = getNeighborsMovingAway (x, y, GridPieces);
		//int index = MonsterList.Find (monster);
		int direction = 0;
		
		foreach (GridSquare neighbor in neighbors) 
		{
			switch(neighbor.Type)
			{
				case (int)Instantiation.TileTypes.BeltU:
					direction = 0;
					break;
				case (int)Instantiation.TileTypes.BeltR:
					direction = 1;
					break;
				case (int)Instantiation.TileTypes.BeltD:
					direction = 2;
					break;
				case (int)Instantiation.TileTypes.BeltL:
					direction = 3;
					break;
			}
			InstanceParent.Spawn(monster.MonsterType, x, y, direction);//create the monster
		}
		if (neighbors.Count == 0) 
		{
			switch(GridPieces[x,y].Type)
			{
			case (int)Instantiation.TileTypes.BeltU:
				direction = 0;
				break;
			case (int)Instantiation.TileTypes.BeltR:
				direction = 1;
				break;
			case (int)Instantiation.TileTypes.BeltD:
				direction = 2;
				break;
			case (int)Instantiation.TileTypes.BeltL:
				direction = 3;
				break;
			}
			InstanceParent.Spawn(monster.MonsterType, x, y, direction);

		}
		MonsterList.Remove (monster);
		Instantiation.Destroy (monster.MonsterGameObject, 0f);
	}
	public List<GridSquare> getNeighborsMovingAway(int x, int y, GridSquare[,] GridPieces)
	{
		List<GridSquare> neighbors = new List<GridSquare> ();
		if (x > 0 && GridPieces [x - 1, y].Type == (int)Instantiation.TileTypes.BeltL)
			neighbors.Add (GridPieces [x - 1, y]);
		if (x < GridPieces.GetLength(0)-1 && GridPieces [x + 1, y].Type == (int)Instantiation.TileTypes.BeltR)
			neighbors.Add (GridPieces [x + 1, y]);
		if (y > 0 && GridPieces [x, y-1].Type == (int)Instantiation.TileTypes.BeltU)
			neighbors.Add (GridPieces [x, y - 1]);
		if (y < GridPieces.GetLength(1) -1 && GridPieces [x, y+1].Type == (int)Instantiation.TileTypes.BeltD)
			neighbors.Add (GridPieces [x, y+1]);

		return neighbors;
	}


	public GridSquare()
	{

	}
	
	public GridSquare(int type, bool reacts, int posX, int posY, Instantiation parent)
	{
		InstanceParent = parent;
		Type = type;
		Direction = 2;
		PosX = posX;
		PosY = posY;
		Reacts = reacts;
		Count = 0;
	}

}
