using System;
using System.Numerics;
using System.Reflection;
using Beffyman.Components.Manager;
using Beffyman.Components.Tests.Components;
using Beffyman.Components.Tests.Systems;
using Xunit;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Beffyman.Components.Tests
{
	public class EntityManagerTests
	{
		private readonly EntityManager Manager;
		private const float DeltaTime = (1f / 60f);

		public EntityManagerTests()
		{
			Manager = new EntityManager(new EntityManagerOptions
			{
				ComponentSystemTypes = new Type[] { typeof(GravitySystem), typeof(TransformRigidBodySystem) }
			});
		}

		[Fact]
		public void EntityOperations()
		{
			Assert.Empty(Manager.Entities);
			var entity = Manager.CreateEntity();
			Assert.Single(Manager.Entities);
			Manager.DestroyEntity(entity);
			Assert.Empty(Manager.Entities);
		}

		[Fact]
		public void ComponentOperations()
		{
			var entity = Manager.CreateEntity();

			Assert.Null(Manager.GetComponent<Transform>(entity));
			Assert.False(Manager.HasComponent<Transform>(entity));

			var transform = Manager.AddComponent<Transform>(entity);
			Assert.NotNull(transform);

			Assert.NotNull(Manager.GetComponent<Transform>(entity));
			Assert.True(Manager.HasComponent<Transform>(entity));

			//Adding the same component will just return the original
			var transform_same = Manager.AddComponent<Transform>(entity);

			Assert.True(Transform.ReferenceEquals(transform, transform_same));
			Assert.Single(Manager.GetComponents(entity));

			Manager.RemoveComponent<Transform>(entity);

			Assert.Empty(Manager.GetComponents(entity));

			var transformCustom = Manager.AddComponent(entity, new Transform
			{
				Rotation = 50f
			});

			var transformCustom2 = Manager.AddComponent(entity, new Transform
			{
				Rotation = 100f
			}, false);

			Assert.Equal(transformCustom, transformCustom2);

			var transformCustom3 = Manager.AddComponent(entity, new Transform
			{
				Rotation = 75f
			}, true);

			Assert.NotEqual(transformCustom, transformCustom3);

			var getComponent = Manager.GetComponent<Transform>(entity);

			Assert.Equal(getComponent, transformCustom3);
			Assert.NotEqual(getComponent, transformCustom);
		}

		[Fact]
		public void SystemOperations()
		{
			var entity = Manager.CreateEntity();
			var transform = Manager.AddComponent<Transform>(entity);
			var rigidBody = Manager.AddComponent<RigidBody>(entity);

			var update = new UpdateStep(DeltaTime);

			Manager.Update(update);

			Assert.NotEqual(Vector2.Zero, rigidBody.Velocity);

		}



		[Fact]
		public void ArcheTypeOperations()
		{
			var entity = Manager.CreateEntity();

			var blankArcheType = Manager.GetArcheType(entity);

			var transform = Manager.AddComponent<Transform>(entity);

			var transformArcheType = Manager.GetArcheType(entity);

			var rigidBody = Manager.AddComponent<RigidBody>(entity);

			var trans_rigid_ArcheType = Manager.GetArcheType(entity);


			Assert.NotEqual(blankArcheType, transformArcheType);
			Assert.NotEqual(blankArcheType, trans_rigid_ArcheType);
			Assert.NotEqual(trans_rigid_ArcheType, transformArcheType);
		}




	}
}
