using System;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../docs/Microsoft.Maui.Controls/ControlTemplate.xml" path="Type[@FullName='Microsoft.Maui.Controls.ControlTemplate']/Docs" />
	public class ControlTemplate : ElementTemplate
	{
		public ControlTemplate()
		{
		}

		public ControlTemplate(Type type) : base(type)
		{
		}

		public ControlTemplate(Func<object> createTemplate) : base(createTemplate)
		{
		}
	}
}