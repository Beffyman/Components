using System;
using Beffyman.Components.Manager;
using Beffyman.Components.Tests.Components;
using Xunit;

namespace Beffyman.Components.Tests
{
	public class EntityManagerTests
	{
		private readonly EntityManager Manager;

		public EntityManagerTests()
		{
			Manager = new EntityManager();
		}

		[Fact]
		public void EntityOperations()
		{
			var entity = Manager.CreateEntity();

			Assert.Empty(Manager.Entities);
			Manager.Update();
			Assert.Single(Manager.Entities);

			Manager.DestroyEntity(entity);

			Assert.Single(Manager.Entities);
			Manager.Update();
			Assert.Empty(Manager.Entities);
		}

		[Fact]
		public void ComponentOperations()
		{
			var entity = Manager.CreateEntity();
			Manager.Update();

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
		}
	}
}
