﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Beffyman.Components.Systems;

namespace Beffyman.Components.Manager
{
	public class EntityManagerOptions
	{
		/// <summary>
		/// Initial size of the backing object pools
		/// </summary>
		public uint InitialPoolSize { get; set; } = 500;

		/// <summary>
		/// Timestep of the fixed time step
		/// </summary>
		public double FixedTimeStep { get; set; } = (1f / 60f);

		/// <summary>
		/// Should multithreading be allowed?
		/// </summary>
		public bool Multithreading { get; set; } = true;

		/// <summary>
		/// Assemblies to search for <see cref="ComponentSystemBase"/> inherited types
		/// </summary>
		public IEnumerable<Assembly> ComponentSystemAssemblies { get; set; }
		/// <summary>
		/// Types used for the ComponentSystems, this will override the default assembly load if provided, but not disable it if assemblies are provided
		/// </summary>
		public IEnumerable<Type> ComponentSystemTypes { get; set; }

	}
}
