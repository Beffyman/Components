using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Beffyman.Components.Internal
{
	internal sealed class PooledCountdownEvent : CountdownEvent
	{
		private static readonly InfiniteScaleObjectPool<PooledCountdownEvent> Pool = new InfiniteScaleObjectPool<PooledCountdownEvent>();

		public PooledCountdownEvent() : base(0)
		{

		}

		public static void Return(PooledCountdownEvent obj)
		{
			Pool.Return(obj);
		}

		public static PooledCountdownEvent Get(int initialCount)
		{
			var pce = Pool.Get();
			pce.Reset(initialCount);
			return pce;
		}


		public void SleepSpinWaitReturn()
		{
			while (CurrentCount > 0)
			{
				//Dunno if this or this.Wait(); is better for this... I've heard the wait has an initial delay
				Thread.Sleep(0);
			}
			Return(this);
		}
	}
}
