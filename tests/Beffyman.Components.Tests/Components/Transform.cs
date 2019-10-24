using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Beffyman.Components.Tests.Components
{
	public sealed class Transform : IComponent
	{
		public Vector2 Position { get; set; }
		public float Rotation { get; set; }
	}
}
