using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Utils
{
	/// <summary>
	/// Increments or decrements a number.
	/// </summary>
	/// <returns>The new number.</returns>
	/// <param name="i">The index.</param>
	/// <param name="min">Minimum value.</param>
	/// <param name="max">Maximum value.</param>
	/// <param name="increment">If set to <c>true</c> increment.</param>
	/// <param name="wrapAround">If set to <c>true</c> wrap around to min after hitting max.</param>
	public static int IncrementDecrement(this int num, int min, int max, bool increment, bool wrapAround)
	{
		if (increment)
		{
			if (wrapAround)
			{
				num ++;
				if (num >= max)
				{
					num = min;
				}
			}
			else
			{
				if (num < max - 1)
				{
					num ++;
				}
			}
		}
		else
		{
			if (wrapAround)
			{
				num --;
				if (num < min)
				{
					num = max - 1;
				}
			}
			else
			{
				if (num > min)
				{
					num --;
				}
			}
		}
		
		return num;
	}

	/// <summary>
	/// Check if a number is even.
	/// </summary>
	/// <returns><c>true</c>, if number is even, <c>false</c> otherwise.</returns>
	public static bool EvenNumber(this int i)
	{
		return i % 2 == 0;
	}
}