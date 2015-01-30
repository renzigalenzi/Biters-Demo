using UnityEngine;
using System.Collections;

public class GridSquare{


	public int Type { get; set; }
	public bool Reacts { get; set; }
	public int PosX { get; set; }
	public int PosY { get; set; }
	public int Count { get; set; }
	
	public GridSquare()
	{

	}
	
	public GridSquare(int type, bool reacts, int posX, int posY)
	{
		Type = type;
		PosX = posX;
		PosY = posY;
		Reacts = reacts;
		Count = 0;
	}

}
