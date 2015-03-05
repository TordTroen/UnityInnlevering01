using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Utils
{
	public static List<string> LinesToList(this string s, string seperator, bool removeEmptyLines)
	{
		// Split string into list
		List<string> list = new List<string>(s.Split(new string[] { seperator }, StringSplitOptions.RemoveEmptyEntries));
		
		// Remove empyt lines
		if (removeEmptyLines)
		{
			for (int i = 0; i < list.Count; i ++)
			{
				if (list[i].Length <= 1)
				{
					list.RemoveAt(i);
				}
			}
		}
		return list;
	}
}