using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Beffyman.Components.Manager;
using Beffyman.Components.Systems;
using Beffyman.Components.Tests.Components;
using Beffyman.Components.Tests.Internal;

namespace Beffyman.Components.Tests.Systems
{
	public sealed class TransformRigidBodySystem : JobComponentSystem
	{
		protected override void OnUpdate(in JobHandle<IntPtr> jobs, in UpdateStep step)
		{
			var job = new TransformRigidBodyJob(step.DeltaTime);

			Schedule(jobs, ref job);
		}

		private readonly struct TransformRigidBodyJob : IJobForEach<Transform, RigidBody>
		{
			public readonly float DeltaTime;
			public TransformRigidBodyJob(float deltaTime)
			{
				DeltaTime = deltaTime;
			}

			public void Execute(in Transform transform, in RigidBody rigidBody)
			{
				transform.Position += rigidBody.Velocity * DeltaTime;

				if (rigidBody.Force != Vector2.Zero)
				{
					rigidBody.Velocity += rigidBody.Force * DeltaTime * rigidBody.InverseMass;
					rigidBody.Force = Vector2.Zero;
				}

				if (rigidBody.LinearDamping != 0f)
				{
					var linearDelta = MathF.FusedMultiplyAdd(rigidBody.LinearDamping, -DeltaTime, 1.0f);

					rigidBody.Velocity *= linearDelta;
				}

				if (rigidBody.Velocity.IsZero())
				{
					rigidBody.Velocity = Vector2.Zero;
				}
			}
		}
	}
}
