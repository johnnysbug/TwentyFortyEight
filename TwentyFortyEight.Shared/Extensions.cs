using System.Collections.Generic;
using CocosSharp;

namespace TwentyFortyEight.Shared
{
	public static class Extensions
	{
		public static void Shuffle<T>(this IList<T> list)  
		{  
			var rng = new CCFastRandom();  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rng.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
	}
}

