using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class TutorialEvent 
{
	int m_xLocation;
	int m_yLocation;
	string m_message;
	bool m_triggered;
	bool m_completed;
	
	public TutorialEvent()
	{
		m_xLocation = 0;
		m_yLocation = 0;
		m_message = "";
		m_triggered = false;
		m_completed = false;
	}
	
	public TutorialEvent(int x, int y, string message)
	{
		m_xLocation = x;
		m_yLocation = y;
		m_message = message;
		m_triggered = false;
		m_completed = false;
	}
	
	public string getMessage()
	{
		return m_message;
	}
	
	public int x()
	{
		return m_xLocation;
	}
	public int y()
	{
		return m_yLocation;
	} 
	public void setTrigger(bool trigger)
	{
		m_triggered = trigger;
	}
	public bool isTriggered()
	{
		return m_triggered;
	}
	public void setCompleted(bool completed)
	{
		m_completed = completed;
	}
	public bool isCompleted()
	{
		return m_completed;
	}
	public void setTriggerLoc(int x, int y)
	{
		m_xLocation= x;
		m_yLocation= y;
	}
	public bool triggersAt(int x, int y)
	{
		bool retVal = false;
		if(m_xLocation == x && m_yLocation == y)
			retVal = true;

		return retVal;
	}
}
