namespace Beffyman.Components.Systems
{
	public abstract partial class JobComponentSystem
	{
#warning What point even is there to this type of job?  Will probably remove it later
		//protected interface IJobForEach
		//{
		//	void Execute();
		//}


		protected interface IJobForEach<T>
			where T : IComponent
		{
			void Execute(T arg);
		}


		protected interface IJobForEach<TFirst, TSecond>
			where TFirst : IComponent
			where TSecond : IComponent
		{
			void Execute(TFirst arg1, TSecond arg2);
		}


		protected interface IJobForEach<TFirst, TSecond, TThird>
			where TFirst : IComponent
			where TSecond : IComponent
			where TThird : IComponent
		{
			void Execute(TFirst arg1, TSecond arg2, TThird arg3);
		}
	}
}