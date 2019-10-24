using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Beffyman.Components.Tests.Components
{
	public class RigidBody : IComponent
	{
		public float Mass { get; set; }
		public Vector2 Velocity { get; set; }
		public float Restitution { get; set; }
	}
}
