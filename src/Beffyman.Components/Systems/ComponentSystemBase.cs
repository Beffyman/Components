using System;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Manager;

namespace Beffyman.Components.Systems
{
	public abstract class ComponentSystemBase
	{
		/// <summary>
		/// Was this system started this update?
		/// </summary>
		public bool WasStarted { get; private set; } = true;

		/// <summary>
		/// Was the system stopped this update?
		/// </summary>
		public bool WasStopped { get; private set; }

		private bool _enabled = true;
		/// <summary>
		/// Is this system currently running updates?
		/// </summary>
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (_enabled == value)
				{
					return;
				}

				if (value)
				{
					WasStarted = true;
				}
				else
				{
					WasStopped = false;
				}
				_enabled = value;
			}
		}

		public EntityManager Manager { get; private set; }

		public bool ShouldRunSystem()
		{
#warning TODO
			return Enabled;
		}

		internal abstract void Update(in UpdateStep step);
		internal void Create()
		{
			OnCreate();
		}

		internal void Destroy()
		{
			OnDestroy();
		}

		internal void Start()
		{
			OnStart();
			WasStarted = false;
		}

		internal void Stop()
		{
			OnStop();
			WasStopped = false;
		}

		protected virtual void OnCreate() { }
		protected virtual void OnDestroy() { }
		protected virtual void OnStart() { }
		protected virtual void OnStop() { }
		protected abstract void OnUpdate(in UpdateStep step);

		internal virtual void Load(EntityManager manager)
		{
			Manager = manager;
		}
	}
}
