using System;
using System.Threading;
using Beffyman.Components.Internal;
using Beffyman.Components.Manager;

namespace Beffyman.Components
{
	/// <summary>
	/// Object which can have components attached to it to participate in systems
	/// </summary>
	public partial class Entity
	{
		private static int nextId = 0;

		/// <summary>
		/// Unique Id of the entity, this may be reused though if a previous entity was destroyed
		/// </summary>
		public uint Id { get; } = unchecked((uint)Interlocked.Increment(ref nextId));

		/// <summary>
		/// Manager that controls this <see cref="Entity"/>
		/// </summary>
		public EntityManager Manager { get; set; }

		/// <summary>
		/// Is this <see cref="Entity"/> ready to be used?
		/// </summary>
		public bool Ready { get; internal set; }
	}
}
