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
	
	public GridSquare()
	{

	}
	
	public GridSquare(int type, int direction, bool reacts, int posX, int posY)
	{
		Type = type;
		Direction = direction;
		PosX = posX;
		PosY = posY;
		Reacts = reacts;
		Count = 0;
	}

}
