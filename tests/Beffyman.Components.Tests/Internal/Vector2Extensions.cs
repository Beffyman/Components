using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Beffyman.Components.Tests.Internal
{
	internal static class Vector2Extensions
	{
		private const float Epsilon = float.Epsilon * 1e20f * 1e20f;
		public static bool IsZero(this Vector2 vector)
		{
			var length = MathF.Abs(vector.Length());
			return (length <= Epsilon);
		}
	}
}
