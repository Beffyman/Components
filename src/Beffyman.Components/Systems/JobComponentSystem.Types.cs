using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Beffyman.Components.Internal;

namespace Beffyman.Components.Systems
{
	public abstract partial class JobComponentSystem
	{
		protected ref struct JobHandle
		{

		}


		protected interface IJobForEach
		{
			void Execute();
		}


		//private static void Handle<T>(T job)
		//{

		//}



		protected interface IJobForEach<T>
			where T : IComponent
		{
			void Execute(in T arg);
		}




		protected interface IJobForEach<TFirst, TSecond>
			where TFirst : IComponent
			where TSecond : IComponent
		{
			void Execute(in TFirst arg1, in TSecond arg2);
		}




		protected interface IJobForEach<TFirst, TSecond, TThird>
			where TFirst : IComponent
			where TSecond : IComponent
			where TThird : IComponent
		{
			void Execute(in TFirst arg1, in TSecond arg2, in TThird arg3);
		}
	}
}