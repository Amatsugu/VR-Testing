using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MathUtils
{
	private MathUtils()
	{

	}

	public static float Map(float value, float min, float max, float a, float b)
	{
		return Lerp(a, b, (value - min) / (max - min));
	}

	public static float Lerp(float a, float b, float time)
	{
		return a + (b - a) * time;
	}
}
