using System;

namespace Microsoft.Maui.Controls
{
#if !NETSTANDARD1_0
	/// <include file="../../docs/Microsoft.Maui.Controls/UnsolvableConstraintsException.xml" path="Type[@FullName='Microsoft.Maui.Controls.UnsolvableConstraintsException']/Docs" />
	[Serializable]
#endif
	public class UnsolvableConstraintsException : Exception
	{
		public UnsolvableConstraintsException()
		{
		}

		public UnsolvableConstraintsException(string message)
			: base(message)
		{
		}

		public UnsolvableConstraintsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UnsolvableConstraintsException(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}
}