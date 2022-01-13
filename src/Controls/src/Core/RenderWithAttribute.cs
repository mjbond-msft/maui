using System;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../docs/Microsoft.Maui.Controls/RenderWithAttribute.xml" path="Type[@FullName='Microsoft.Maui.Controls.RenderWithAttribute']/Docs" />
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RenderWithAttribute : Attribute
	{

		public RenderWithAttribute(Type type) : this(type, new[] { typeof(VisualMarker.DefaultVisual) })
		{
		}

		public RenderWithAttribute(Type type, Type[] supportedVisuals)
		{
			Type = type;
			SupportedVisuals = supportedVisuals ?? new[] { typeof(VisualMarker.DefaultVisual) };
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/RenderWithAttribute.xml" path="//Member[@MemberName='SupportedVisuals']/Docs" />
		public Type[] SupportedVisuals { get; }
		/// <include file="../../docs/Microsoft.Maui.Controls/RenderWithAttribute.xml" path="//Member[@MemberName='Type']/Docs" />
		public Type Type { get; }
	}
}