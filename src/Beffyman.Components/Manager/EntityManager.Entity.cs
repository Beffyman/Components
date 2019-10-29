using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		public IReadOnlyCollection<Entity> Entities => _entities;

		public Entity CreateEntity()
		{
			var entity = _entityPool.Get();
			entity.Manager = this;
			_entities.Add(entity);

			entity.Ready = true;
			return entity;
		}

		public bool DestroyEntity(Entity entity)
		{
			if (entity == null)
			{
				return false;
			}

			entity.Ready = false;
			RemoveAllComponents(entity);
			return _entities.TryRemove(entity);
		}

		private void DestroyEntities()
		{
			_entities.Clear();
		}
	}
}
