using System;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../docs/Microsoft.Maui.Controls/CheckedChangedEventArgs.xml" path="Type[@FullName='Microsoft.Maui.Controls.CheckedChangedEventArgs']/Docs" />
	public class CheckedChangedEventArgs : EventArgs
	{
		public CheckedChangedEventArgs(bool value)
		{
			Value = value;
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/CheckedChangedEventArgs.xml" path="//Member[@MemberName='Value']/Docs" />
		public bool Value { get; private set; }
	}
}