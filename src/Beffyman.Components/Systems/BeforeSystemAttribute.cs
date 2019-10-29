using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Systems
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class BeforeSystemAttribute : Attribute
	{
		public Type[] BeforeSystems { get; }

		public BeforeSystemAttribute(params Type[] beforeSystems)
		{
			BeforeSystems = beforeSystems;
		}
	}
}
