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

		protected override ref JobHandle OnUpdate(ref JobHandle jobs, in UpdateStep step)
		{
			var job = new GravityJob(Gravity);

			Schedule(job);

			return ref jobs;
		}

		private readonly struct GravityJob : IJobForEach<Transform, RigidBody>
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
