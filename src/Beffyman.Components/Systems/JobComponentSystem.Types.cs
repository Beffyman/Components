using System;
using System.Collections.Generic;
using System.Linq;
using Beffyman.Components.Internal;

namespace Beffyman.Components.Systems
{
	public abstract partial class JobComponentSystem
	{
		internal static IEnumerable<Type[]> GetJobParameterTypes(IEnumerable<Type> systemTypes)
		{
			var jobTypes = systemTypes.SelectMany(x => x.GetNestedTypes().Where(y => typeof(IJob).IsAssignableFrom(y))).ToArray();

			List<Type[]> typeGroups = new List<Type[]>();
			typeGroups.AddRange(jobTypes.SelectMany(x => x.GetInterfaces().Where(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IJobForEach<>)).Select(z => z.GetGenericArguments())));
			typeGroups.AddRange(jobTypes.SelectMany(x => x.GetInterfaces().Where(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IJobForEach<,>)).Select(z => z.GetGenericArguments())));
			typeGroups.AddRange(jobTypes.SelectMany(x => x.GetInterfaces().Where(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IJobForEach<,,>)).Select(z => z.GetGenericArguments())));

			return typeGroups;
		}

		/// <summary>
		/// Base interface for lookups
		/// </summary>
		public interface IJob { }


		protected interface IJobForEach<T> : IJob
			where T : IComponent
		{
			void Execute(T arg);
		}


		protected interface IJobForEach<TFirst, TSecond> : IJob
			where TFirst : IComponent
			where TSecond : IComponent
		{
			void Execute(TFirst arg1, TSecond arg2);
		}


		protected interface IJobForEach<TFirst, TSecond, TThird> : IJob
			where TFirst : IComponent
			where TSecond : IComponent
			where TThird : IComponent
		{
			void Execute(TFirst arg1, TSecond arg2, TThird arg3);
		}
	}
}