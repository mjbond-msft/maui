using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls.Shapes
{
	/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/LineSegment.xml" path="Type[@FullName='Microsoft.Maui.Controls.Shapes.LineSegment']/Docs" />
	public class LineSegment : PathSegment
	{
		public LineSegment()
		{

		}

		public LineSegment(Point point)
		{
			Point = point;
		}

		/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/LineSegment.xml" path="//Member[@MemberName='PointProperty']/Docs" />
		public static readonly BindableProperty PointProperty =
			BindableProperty.Create(nameof(Point), typeof(Point), typeof(LineSegment), new Point(0, 0));

		/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/LineSegment.xml" path="//Member[@MemberName='Point']/Docs" />
		public Point Point
		{
			set { SetValue(PointProperty, value); }
			get { return (Point)GetValue(PointProperty); }
		}
	}
}