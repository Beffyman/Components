using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Beffyman.Components.Systems;
using System.Reflection;
using Beffyman.Components.Internal;
using System.Threading.Tasks;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		private ComponentSystemBase[] LoadSystemComponents()
		{
			//Load component types
			//Use the Assemblies if provided, if not, use the static 3
			//If types were provided, prioritize those, unless assemblies was also provided, then combine

			Type[] allComponentSystems = null;
			IEnumerable<Assembly> assemblies = Options.ComponentSystemAssemblies;
			if (assemblies == null && Options.ComponentSystemTypes == null)
			{
				assemblies = new Assembly[] { Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly() };
			}

			if (assemblies != null)
			{
				assemblies = assemblies.Distinct().Where(x => x != typeof(EntityManager).Assembly).ToArray();

				allComponentSystems = assemblies.SelectMany(x => x.DefinedTypes)
					.Where(x => typeof(ComponentSystemBase).IsAssignableFrom(x))
					.Cast<Type>()
					.Union(Options.ComponentSystemTypes ?? Enumerable.Empty<Type>())
					.ToArray();
			}
			else
			{
				allComponentSystems = Options.ComponentSystemTypes.Distinct().ToArray();
			}



			//Validate that the component system types can be created. (Empty Constructor)

			var anyInvalidConstructors = allComponentSystems.Where(x => x.GetConstructors().Count() != 1 || x.GetConstructors().Any(x => x.GetParameters().Any())).ToArray();

			if (anyInvalidConstructors.Any())
			{
				throw new InvalidOperationException($"The following {nameof(Type)}s are invalid due to a non-empty constructor being defined.{string.Join(Environment.NewLine, anyInvalidConstructors.Select(x => x.FullName))}");
			}

			//Create the systems, but we need to



			Dictionary<Type, HashSet<Type>> beforeComponents = new Dictionary<Type, HashSet<Type>>(TypeEqualityComparer.Instance);
			Dictionary<Type, HashSet<Type>> afterComponents = new Dictionary<Type, HashSet<Type>>(TypeEqualityComparer.Instance);

			foreach (var systemType in allComponentSystems)
			{
				var beforeSystems = systemType.GetCustomAttributes<BeforeSystemAttribute>();

				foreach (var beforeAttr in beforeSystems)
				{
					foreach (var beforeSys in beforeAttr.BeforeSystems)
					{
						if (!beforeComponents.ContainsKey(beforeSys))
						{
							beforeComponents.Add(beforeSys, new HashSet<Type>(TypeEqualityComparer.Instance));
						}

						beforeComponents[beforeSys].Add(systemType);
					}
				}


				var afterSystems = systemType.GetCustomAttributes<AfterSystemAttribute>();
				foreach (var afterAttr in afterSystems)
				{
					foreach (var afterSys in afterAttr.AfterSystems)
					{
						if (!afterComponents.ContainsKey(afterSys))
						{
							afterComponents.Add(afterSys, new HashSet<Type>(TypeEqualityComparer.Instance));
						}

						afterComponents[afterSys].Add(systemType);
					}
				}
			}

			var systems = new Queue<Type>(allComponentSystems);

			List<Type> orderedComponents = new List<Type>();

			//Give a healthy length^2 until we throw an error in case of a after/before loop
			int failurePoint = (allComponentSystems.Length + 1) * allComponentSystems.Length;
			int attempts = 0;
			while (systems.Count != 0)
			{
				attempts++;
				if (attempts >= failurePoint)
				{
					throw new InvalidOperationException($"There was an error with the ComponentSystem dependency tree.  Please check all {nameof(BeforeSystemAttribute)} and {nameof(AfterSystemAttribute)} usages.{Environment.NewLine}Remaining components to order:{Environment.NewLine}{string.Join(Environment.NewLine, allComponentSystems.Except(orderedComponents).Select(x => x.FullName))}");
				}

				var systemToInsert = systems.Dequeue();

				//Do checks for if all dependencies are met
				if (beforeComponents.ContainsKey(systemToInsert))
				{
					bool beforeDependenciesMet = true;
					foreach (var mustBeBefore in beforeComponents[systemToInsert])
					{
						beforeDependenciesMet &= orderedComponents.Contains(mustBeBefore);
					}

					if (!beforeDependenciesMet)
					{
						systems.Enqueue(systemToInsert);
						continue;
					}
				}

				if (afterComponents.ContainsKey(systemToInsert))
				{
					bool afterDependenciesMet = true;
					foreach (var mustBeAfter in afterComponents[systemToInsert])
					{
						afterDependenciesMet &= orderedComponents.Contains(mustBeAfter);
					}

					if (!afterDependenciesMet)
					{
						systems.Enqueue(systemToInsert);
						continue;
					}
				}

				int index = 0;
				if (afterComponents.ContainsKey(systemToInsert))
				{
					foreach (var mustBeAfter in afterComponents[systemToInsert])
					{
						var leastIndex = orderedComponents.IndexOf(mustBeAfter) + 1;
						if (index >= leastIndex)
						{
							index = leastIndex;
						}
					}
				}

				//Now we can actually insert the system in order
				if (beforeComponents.ContainsKey(systemToInsert))
				{
					foreach (var mustBeBefore in beforeComponents[systemToInsert])
					{
						var mostIndex = orderedComponents.IndexOf(mustBeBefore) + 1;
						if (index <= mostIndex)
						{
							index = mostIndex;
						}
					}
				}

				orderedComponents.Insert(index, systemToInsert);
			}

			//Now do the loading of the systems

			var componentSystems = orderedComponents.Select(x => (ComponentSystemBase)Activator.CreateInstance(x)).ToArray();
			for (int i = 0; i < componentSystems.Length; i++)
			{
				var system = componentSystems[i];

				//Assign defaults
				system.Load(this);
			}

			return componentSystems;
		}

		private void UpdateSystems(in UpdateStep step)
		{
			for (int i = 0; i < _componentSystems.Length; i++)
			{
				var system = _componentSystems[i];

				if (_firstUpdate)
				{
					system.Create();
					_firstUpdate = false;
				}

				if (system.WasStarted)
				{
					system.Start();
				}

				if (system.Enabled)
				{
					_componentSystems[i].Update(step);
				}

				if (system.WasStopped)
				{
					system.Stop();
				}
			}
		}

		private void DestroySystems()
		{
			for (int i = 0; i < _componentSystems.Length; i++)
			{
				var system = _componentSystems[i];

				system.Destroy();
			}
		}

		public T GetSystem<T>() where T : ComponentSystemBase
		{
			if (_indexedComponentSystems.ContainsKey(typeof(T)))
			{
				return _indexedComponentSystems[typeof(T)] as T;
			}

			return null;
		}

		public ComponentSystemBase GetSystem(Type type)
		{
			if (_indexedComponentSystems.ContainsKey(type))
			{
				return _indexedComponentSystems[type];
			}

			return null;
		}
	}
}
