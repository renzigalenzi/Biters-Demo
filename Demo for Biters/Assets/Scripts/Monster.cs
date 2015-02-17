using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public enum NumberType
{
    Zero, One
}

public enum MovementType
{
    Waiting, Moving
}

public enum MovementDirection
{
    None, Up, Right, Down, Left
}

public class Monster
{
    public GameObject MonsterGameObject { get; set; }
    public Instantiation MonsterInstantiation { get; set; }
    public int MonsterId { get; set; }
    public MovementType MonsterMovementType { get; set; }
    public NumberType MonsterNumberType { get; set; }
	public float MonsterStartingXPosition { get; set;}
	public float MonsterStartingYPosition { get; set; }
    public int MonsterXPosition { get; set; }
    public int MonsterYPosition { get; set; }
    public MovementDirection MonsterMovementDirection { get; set; }
	//public List<int> DirectionsNotToGo{ get; set; }
    public float MonsterMovementIncrement { get; set; }

    public Monster()
    {
        MonsterInstantiation = new Instantiation();
        MonsterId = 0;
        MonsterMovementType = MovementType.Waiting;
        MonsterNumberType = NumberType.Zero;
        MonsterXPosition = 0;
        MonsterYPosition = 0;
        MonsterMovementDirection = MovementDirection.None;
        MonsterMovementIncrement = 0.0F;
    }

    public Monster(Instantiation monsterInstantiation, int monsterId, MovementType monsterMovementType, NumberType monsterNumberType, int monsterXPosition, int monsterYPosition, MovementDirection monsterMovementDirection)
    {
		//DirectionsNotToGo.Add (monsterMovementDirection);

        MonsterGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MonsterInstantiation = monsterInstantiation;
        MonsterId = monsterId;
        MonsterMovementType = monsterMovementType;
        MonsterNumberType = monsterNumberType;
		MonsterXPosition = monsterXPosition;
		MonsterYPosition = monsterYPosition;
        MonsterMovementDirection = monsterMovementDirection;
        MonsterMovementIncrement = 0.01F;

        MonsterGameObject.transform.position = new Vector3(MonsterXPosition - Instantiation.XOFFSET, Instantiation.YOFFSET - MonsterYPosition, -1);
		MonsterGameObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.back);
		MonsterGameObject.transform.localScale = new Vector3(0.5F, 0.5F, 0.1F);
        switch(MonsterNumberType)
        {
            case NumberType.Zero:
				MonsterGameObject.renderer.material = MonsterInstantiation.MaterialDictionary["BiterZero"];
                break;
            case NumberType.One:
				MonsterGameObject.renderer.material = MonsterInstantiation.MaterialDictionary["BiterOne"];
                break;
            default:
                Instantiation.PrintMessage("Invalid MonsterNumberType - Monster(Instantiation monsterInstantiation, int monsterId, MovementType monsterMovementType, NumberType monsterNumberType, int monsterXPosition, int monsterYPosition, MovementDirection monsterMovementDirection)");
                break;
        }
		MonsterStartingXPosition = MonsterGameObject.transform.position.x;
		MonsterStartingYPosition = MonsterGameObject.transform.position.y;

        MonsterInstantiation.InstantiationMonsters.Add(this);
        MonsterInstantiation.InstantiationNextMonsterId++;
    }

	public bool FinishedMovingTile()
	{
		float currentX = MonsterGameObject.transform.position.x;
		float currentY = MonsterGameObject.transform.position.y;

		if(Math.Abs (MonsterStartingXPosition - currentX) >= 1 || Math.Abs (MonsterStartingYPosition - currentY) >= 1)
		{
			MonsterStartingXPosition = (int)Math.Round(currentX);
			MonsterStartingYPosition = (int)Math.Round(currentY);
			MonsterXPosition = (int)Math.Round (currentX) + Instantiation.XOFFSET;
			MonsterYPosition = Instantiation.YOFFSET - (int)Math.Round (currentY);
			return true;
		}
		else
		{
			return false;
		}
	}
	public void DestroyEntirely()
	{
		MonsterInstantiation.InstantiationMonsters.Remove(this);
		Instantiation.Destroy(this.MonsterGameObject, 0f);
	}
}
