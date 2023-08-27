using System.Collections.Generic;

/// <summary>
/// 寻峰算法
/// </summary>
public static class FindPeaks
{
	private static float[] oneDiff(float[] data)
	{
		float[] result = new float[data.Length - 1];
		for (int i = 0; i < result.Length; i++)
		{
			result[i] = data[i + 1] - data[i];
		}

		return result;
	}

	private static int[] trendSign(float[] data)
	{
		int[] sign = new int[data.Length];
		for (int i = 0; i < sign.Length; i++)
		{
			if (data[i] > 0) sign[i] = 1;
			else if (data[i] == 0) sign[i] = 0;
			else sign[i] = -1;
		}

		for (int i = sign.Length - 1; i >= 0; i--)
		{
			if (sign[i] == 0 && i == sign.Length - 1)
			{
				sign[i] = 1;
			}
			else if (sign[i] == 0)
			{
				if (sign[i + 1] >= 0)
				{
					sign[i] = 1;
				}
				else
				{
					sign[i] = -1;
				}
			}
		}

		return sign;
	}

	private static int[] getPeaksIndex(int[] diff)
	{
		List<int> data = new List<int>();
		for (int i = 0; i != diff.Length - 1; i++)
		{
			if (diff[i + 1] - diff[i] == -2)
			{
				data.Add(i + 1);
			}
		}

		int[] result = new int[data.Count];
		for (int i = 0; i < result.Length; i++)
		{
			result[i] = data[i];
		}

		return result; //相当于原数组的下标
	}

	//寻峰算法
	public static int[] getPeaks(float[] datas)
	{
		int[] index = getPeaksIndex(trendSign(oneDiff(datas)));
		return index;
	}

}