using UnityEngine;
using System.Collections;
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
	public int MonsterStartingXPosition { get; set;}
	public int MonsterStartingYPosition { get; set; }
    public int MonsterXPosition { get; set; }
    public int MonsterYPosition { get; set; }
    public MovementDirection MonsterMovementDirection { get; set; }
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
        MonsterGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MonsterInstantiation = monsterInstantiation;
        MonsterId = monsterId;
        MonsterMovementType = monsterMovementType;
        MonsterNumberType = monsterNumberType;
		MonsterStartingXPosition = monsterXPosition;
		MonsterStartingYPosition = monsterYPosition;
        MonsterXPosition = monsterXPosition;
        MonsterYPosition = monsterYPosition;
        MonsterMovementDirection = monsterMovementDirection;
        MonsterMovementIncrement = 0.01F;

        MonsterGameObject.transform.position = new Vector3(MonsterXPosition - Instantiation.XOFFSET, Instantiation.YOFFSET - MonsterYPosition, -1);
        MonsterGameObject.transform.localScale = new Vector3(0.5F, 0.5F, 0.1F);
        switch(MonsterNumberType)
        {
            case NumberType.Zero:
                MonsterGameObject.renderer.material = MonsterInstantiation.BiterZero;
                break;
            case NumberType.One:
                MonsterGameObject.renderer.material = MonsterInstantiation.BiterOne;
                break;
            default:
                Instantiation.PrintMessage("Invalid MonsterNumberType - Monster(Instantiation monsterInstantiation, int monsterId, MovementType monsterMovementType, NumberType monsterNumberType, int monsterXPosition, int monsterYPosition, MovementDirection monsterMovementDirection)");
                break;
        }

        MonsterInstantiation.InstantiationMonsters.Add(this);
        MonsterInstantiation.InstantiationNextMonsterId++;
    }

	public bool FinishedMovingTile()
	{
		float currentX = MonsterGameObject.transform.position.x;
		float currentY = MonsterGameObject.transform.position.y;

		if (currentX == MonsterStartingXPosition && currentY == MonsterStartingYPosition)
		{
			return false;
		}
		else if(Math.Abs (MonsterXPosition - currentX) >= 1 || Math.Abs (MonsterYPosition - currentY) >= 1)
		{
			MonsterXPosition = (int)Math.Round (currentX);
			MonsterYPosition = (int)Math.Round (currentY);
			return true;
		}
		else
		{
			return false;
		}
	}
}
