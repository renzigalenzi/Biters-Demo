using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class Monster{

	public enum StatusType : int {moving = 0, waiting, finished}

	public int Id { get; set; }
	public double MovementIncrement = .01;
	public int MonsterType { get; set; }
	public int StartingPosX { get; set; }
	public int StartingPosY { get; set; }
	public int TargetPosX { get; set; }
	public int TargetPosY { get; set; }
	public int PosX { get; set; }
	public int PosY { get; set; }
	public int MovementDirection { get; set; }
	public int MovementSpeed { get; set; }
	public int Status { get; set; }
	//public int MovementDirection { get; set; }
	public Instantiation InstanceParent { get; set; }

	public GameObject MonsterGameObject{ get; set; }

	public bool FinishedMovingTile()//has the monster reached a new tile?
	{
		bool bReturn;
		float currX = MonsterGameObject.transform.position.x;
		float currY = MonsterGameObject.transform.position.y;

		//since the space of a block is broken down into integers, if the distance traveled by a piece is 
		//an integer and not equal to its starting position, then it has moved 1 unit
		
		if (currX == StartingPosX && currY == StartingPosY) 
		{
			bReturn = false;
		}
		else if (Math.Abs (PosX - currX)>=1 || Math.Abs (PosY - currY)>=1) 
		{
			PosX = (int)Math.Round (currX);
			PosY = (int)Math.Round (currY);
			bReturn = true;
		} 
		else 
		{
			bReturn = false;
		}
		return bReturn;
	}


	public Monster(){}

	public Monster(Instantiation parent, int id, int type, int posX, int posY, int moveDirection)
	{
		InstanceParent = parent;
		Id = id;
		MonsterType = type;
		StartingPosX = posX;
		StartingPosY = posY;
		MovementDirection = moveDirection;
		Status = (int)StatusType.moving;
		MovementSpeed = 1;

		MonsterGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		MonsterGameObject.transform.position = new Vector3(posX-InstanceParent.OFFSETX, InstanceParent.OFFSETY-posY, -1);
		if (type == 1)
		{
			MonsterGameObject.renderer.material = InstanceParent.Biter0;
		}
		else if (type == 2)
		{
			MonsterGameObject.renderer.material = InstanceParent.Biter1;
		}
		MonsterGameObject.transform.localScale = new Vector3(.5F, .5F, .1F);

		PosX = (int)Math.Round(MonsterGameObject.transform.position.x);
		PosY = (int)Math.Round(MonsterGameObject.transform.position.y);


		//MonsterGameObject.transform.position = new Vector3(posX-InstanceParent.OFFSETX, InstanceParent.OFFSETY-posY, -1);
	}
}
