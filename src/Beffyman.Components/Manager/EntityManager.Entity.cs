using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		/// <summary>
		/// Creates a new entity that will be active on the start of the next update
		/// </summary>
		/// <returns></returns>
		public Entity CreateEntity()
		{
			var entity = _entityPool.Get();
			entity.Manager = this;
			entity.Ready = false;
			_entitiesToBeAdded.Push(entity);

			return entity;
		}

		/// <summary>
		/// Lists the entity to be destroyed on the next update
		/// </summary>
		/// <param name="entity"></param>
		public void DestroyEntity(Entity entity)
		{
			if (entity == null)
			{
				return;
			}

			//!? Don't destroy here, wait until the update actually gets rid of it
			entity.Ready = false;
			RemoveAllComponents(entity);
			_entitiesToBeDestroyed.Push(entity);
		}

		/// <summary>
		/// Buffer array used to do batch operations on the Entities
		/// </summary>
		private Entity[] _entityBuffer = new Entity[50];

		private void FlushEntityCollections()
		{
			if (_entitiesToBeDestroyed.Count != 0)
			{
				while (_entitiesToBeDestroyed.Count != 0)
				{
					Array.Clear(_entityBuffer, 0, _entityBuffer.Length);
					var bufferLength = _entitiesToBeDestroyed.TryPopRange(_entityBuffer);
					for (int i = 0; i < bufferLength; i++)
					{
						var destroyedEntity = _entityBuffer[i];
						_entityPool.Return(destroyedEntity);
						_entities.Remove(destroyedEntity);
					}
				}
			}

			if (_entitiesToBeAdded.Count != 0)
			{
				while (_entitiesToBeAdded.Count != 0)
				{
					Array.Clear(_entityBuffer, 0, _entityBuffer.Length);
					var bufferLength = _entitiesToBeAdded.TryPopRange(_entityBuffer);

					_entities.AddRange(RuntimeHelpers.GetSubArray(_entityBuffer, new Range(0, bufferLength)));

					for (int i = 0; i < bufferLength; i++)
					{
						_entityBuffer[i].Ready = true;
					}
				}
			}

#warning may not need this?
			Array.Clear(_entityBuffer, 0, _entityBuffer.Length);
		}

	}
}
