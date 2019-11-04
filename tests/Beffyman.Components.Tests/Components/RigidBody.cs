using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Beffyman.Components.Tests.Components
{
	public class RigidBody : IComponent
	{
		public RigidBody()
		{
			Mass = 1;
			Restitution = 1;
		}

		private float _mass;
		public float Mass
		{
			get
			{
				return _mass;
			}
			set
			{
				if (value <= 0)
				{
					_mass = 1.0f;
				}
				else
				{
					_mass = value;
				}
				InverseMass = 1.0f / _mass;
			}
		}

		public float InverseMass { get; private set; }

		private float _linearDamping;
		public float LinearDamping
		{
			get
			{
				return _linearDamping;
			}
			set
			{
				if (value < 0.0f || value > 1.0f)
				{
					_linearDamping = Math.Clamp(value, 0f, 1.0f);
				}
				else
				{
					_linearDamping = value;
				}
			}
		}


		public Vector2 Velocity { get; set; }
		public float Restitution { get; set; }

		public Vector2 Force { get; set; }
	}
}
