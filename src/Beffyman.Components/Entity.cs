using System;
using Beffyman.Components.Manager;

namespace Beffyman.Components
{
	/// <summary>
	/// Object which can have components attached to it to participate in systems
	/// </summary>
	public class Entity
	{
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
