using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class RotationGroup
{
	List<int> m_xLocation;
	List<int> m_yLocation;

	public RotationGroup()
	{
		m_xLocation = new List<int> ();
		m_yLocation = new List<int> ();
	}

	public RotationGroup(int x, int y)
	{
		m_xLocation = new List<int> ();
		m_yLocation = new List<int> ();

		m_xLocation.Add (x);
		m_yLocation.Add (y);
	}

	public int getCount()
	{
		return m_xLocation.Count;
	}

	public int xAt(int i)
	{
		if(i < m_xLocation.Count)
			return m_xLocation[i];
		else
			return -1;
	}
	public int yAt(int i)
	{
		if(i < m_yLocation.Count)
			return m_yLocation[i];
		else
			return -1;
	} 
	public void Add(int x, int y)
	{
		m_xLocation.Add (x);
		m_yLocation.Add (y);
	}
	public bool Contains(int x, int y)
	{
		bool found = false;

		for(int i = 0; i < m_xLocation.Count; i++)
		{
			if( x == m_xLocation[i] && y == m_yLocation[i])
				found = true;
		}
		return found;
	}
}
