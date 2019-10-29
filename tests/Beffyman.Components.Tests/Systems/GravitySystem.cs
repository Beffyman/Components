using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Beffyman.Components.Manager;
using Beffyman.Components.Systems;
using Beffyman.Components.Tests.Components;

namespace Beffyman.Components.Tests.Systems
{
	[BeforeSystem(typeof(TransformRigidBodySystem))]
	public sealed class GravitySystem : JobComponentSystem
	{
		public Vector2 Gravity { get; set; }

		protected override void OnUpdate(in UpdateStep step)
		{
			var job = new GravityJob(Gravity);

			Schedule(job);
		}

		private readonly struct GravityJob : IJob<Transform, RigidBody>
		{
			public readonly Vector2 Gravity;

			public GravityJob(Vector2 gravity)
			{
				Gravity = gravity;
			}

			public void Execute(in Transform transform, in RigidBody rigidBody)
			{
				rigidBody.Force += (Gravity * rigidBody.Mass);
			}
		}

	}
}
