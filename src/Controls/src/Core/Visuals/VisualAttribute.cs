using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../../docs/Microsoft.Maui.Controls/VisualAttribute.xml" path="Type[@FullName='Microsoft.Maui.Controls.VisualAttribute']/Docs" />
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class VisualAttribute : Attribute
	{
		public VisualAttribute(string key, Type visual)
		{
			this.Key = key;
			this.Visual = visual;
		}

		internal string Key { get; }
		internal Type Visual { get; }
	}
}
