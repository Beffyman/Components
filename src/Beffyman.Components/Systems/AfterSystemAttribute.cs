using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Systems
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class AfterSystemAttribute : Attribute
	{
		public Type[] AfterSystems { get; }

		public AfterSystemAttribute(params Type[] afterSystems)
		{
			AfterSystems = afterSystems;
		}
	}
}
