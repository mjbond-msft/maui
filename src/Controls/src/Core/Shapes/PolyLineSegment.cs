namespace Microsoft.Maui.Controls.Shapes
{
	/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/PolyLineSegment.xml" path="Type[@FullName='Microsoft.Maui.Controls.Shapes.PolyLineSegment']/Docs" />
	public class PolyLineSegment : PathSegment
	{
		public PolyLineSegment()
		{
			Points = new PointCollection();
		}

		public PolyLineSegment(PointCollection points)
		{
			Points = points;
		}

		/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/PolyLineSegment.xml" path="//Member[@MemberName='PointsProperty']/Docs" />
		public static readonly BindableProperty PointsProperty =
			BindableProperty.Create(nameof(Points), typeof(PointCollection), typeof(PolyLineSegment), null);

		/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/PolyLineSegment.xml" path="//Member[@MemberName='Points']/Docs" />
		public PointCollection Points
		{
			set { SetValue(PointsProperty, value); }
			get { return (PointCollection)GetValue(PointsProperty); }
		}
	}
}