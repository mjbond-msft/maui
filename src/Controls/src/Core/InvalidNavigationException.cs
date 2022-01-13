using System;
using System.Runtime.Serialization;

namespace Microsoft.Maui.Controls
{
#if !NETSTANDARD1_0
	/// <include file="../../docs/Microsoft.Maui.Controls/InvalidNavigationException.xml" path="Type[@FullName='Microsoft.Maui.Controls.InvalidNavigationException']/Docs" />
	[Serializable]
#endif
	public class InvalidNavigationException : Exception
	{
		public InvalidNavigationException()
		{
		}

		public InvalidNavigationException(string message)
			: base(message)
		{
		}

		public InvalidNavigationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected InvalidNavigationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}