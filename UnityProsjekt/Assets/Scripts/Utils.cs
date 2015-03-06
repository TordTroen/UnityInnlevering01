using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Utils
{
	public static int IncrementDecrement(this int i, int min, int max, bool increment, bool wrapAround)
	{
		if (increment)
		{
			if (wrapAround)
			{
				i ++;
				if (i >= max)
				{
					i = min;
				}
			}
			else
			{
				if (i < max - 1)
				{
					i ++;
				}
			}
		}
		else
		{
			if (wrapAround)
			{
				i --;
				if (i < min)
				{
					i = max - 1;
				}
			}
			else
			{
				if (i > min)
				{
					i --;
				}
			}
		}
		
		return i;
	}
}