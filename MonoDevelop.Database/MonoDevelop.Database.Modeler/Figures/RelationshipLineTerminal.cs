// 
// CrowFootLineTerminal.cs
//  
// Author:
//       Xiul <ziul1979@gmail.com>
// 
// Copyright (c) 2009 Luis Ochoa
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


using Cairo;
using System;
using MonoHotDraw.Util;
using MonoHotDraw.Figures;


namespace MonoDevelop.Database.Modeler
{

	/* Crow's foot notation: 
	 * http://www.gc.maricopa.edu/business/sylvester/cis164/er2b.htm
	 * http://www.tdan.com/view-articles/7474
	 * http://folkworm.ceri.memphis.edu/ew/SCHEMA_DOC/comparison/erd.htm
	 */

	public enum kindNotation
	{
		CrowsFoot,
		Barker
	}


	public enum kindRelationshipTerminal
	{
		ZeroMore,
		OneMore,
		OneOne,
		ZeroOne,
	}

	/* Crow's Foot:
	 * ZeroMore: >o----...
	 * OneMore:  >|----...
	 * OneOne:   -||---...
	 * ZeroOne:  -o|---...
	 */

	public enum kindOptionality
	{
		optional,
		mandatory
	}

	public enum kindRelationship
	{
		Identify,
		NonIndentify
	}

	public class RelationshipLineTerminal : LineTerminal
	{

		public RelationshipLineTerminal (RelationshipFigure owner) : this(owner, 10.0, 20.0, kindRelationshipTerminal.OneMore, kindNotation.CrowsFoot, false)
		{
		}

		public RelationshipLineTerminal (RelationshipFigure owner, double lDistance, double pDistance, kindRelationshipTerminal kind, kindNotation notation, bool identifying) : base()
		{
			lineDistance = lDistance;
			pointDistance = pDistance;
			parent=owner;
			this.kind = kind;
			this.Identifying = identifying;
			this.notation = notation;
		}

		public override PointD Draw (Context context, PointD a, PointD b)
		{
			context.Save ();
			double[] Dashes = new double[] {
				
			};
			context.SetDash (Dashes, 0);
			PointD[] points = new PointD[8]; //a,b,leftPoint1,rightPoint1,middlePoint1,leftPoint2,rightPoint2,middlePoint2
			//get parallel lines points
			points[0] = a;
			points[1] = b;
			Geometry.GetArrowPoints (points[0], points[1], lineDistance, pointDistance, out points[2], out points[3], out points[4]);
			Geometry.GetArrowPoints (points[0], points[1], lineDistance, pointDistance + 3, out points[5], out points[6], out points[7]);

			switch (kind) {
			case kindRelationshipTerminal.OneMore:
				if (notation == kindNotation.CrowsFoot)
					DrawCrowFootOneMore (context, points); else
					DrawBarkerMany (context, points);
				break;
			case kindRelationshipTerminal.ZeroMore:
				if (notation == kindNotation.CrowsFoot)
					DrawCrowFootZeroMore (context, points); else
					DrawBarkerMany (context, points);
				break;
			case kindRelationshipTerminal.OneOne:
				if (notation == kindNotation.CrowsFoot)
					DrawCrowFootOneOne (context, points); else
					DrawBarkerOne (context, points);
				break;
			case kindRelationshipTerminal.ZeroOne:
				if (notation == kindNotation.CrowsFoot)
					DrawCrowFootZeroOne (context, points); else
					DrawBarkerOne (context, points);
				break;
			}
			context.Restore ();
			return points[4];
		}

		//todo: fix it
		public override RectangleD InvalidateRect (PointD b)
		{
			double distance = Math.Max (lineDistance * 2, pointDistance);
			RectangleD r = new RectangleD (b.X, b.Y, 0.0, 0.0);
			r.Inflate (distance, distance);
			return r;
		}

		private void DrawCrowFootOneMore (Context context, PointD[] points)
		{

			PointD[] anchors=calcAnchorPoints(points[0], points[4]);
			context.MoveTo (points[4]);
			context.LineTo (anchors[0]);
			context.MoveTo (points[4]);
			context.LineTo (anchors[1]);
			context.MoveTo (points[4]);
			context.LineTo (points[0]);
			context.MoveTo (points[2]);
			context.LineTo (points[3]);
			context.MoveTo (points[5]);
			context.LineTo (points[6]);
			context.Stroke ();
		}

		private void DrawCrowFootZeroMore (Context context, PointD[] points)
		{
			PointD[] anchors=calcAnchorPoints(points[0], points[4]);			
			context.MoveTo (points[4]);
			context.LineTo (anchors[0]);
			context.MoveTo (points[4]);
			context.LineTo (anchors[1]);
			context.MoveTo (points[4]);
			context.LineTo (points[0]);
			context.Stroke ();
			//Add circle
			context.Save ();
			context.Color = new Color (1, 1, 1);
			context.Arc (points[4].X, points[4].Y, 2.0, 0.0, 2.0 * Math.PI);
			context.FillPreserve ();
			context.Color = new Color (0, 0, 0);
			context.LineWidth = 1;
			context.Stroke ();
			context.Restore ();
		}

		private void DrawCrowFootOneOne (Context context, PointD[] points)
		{
			context.MoveTo (points[4]);
			context.LineTo (points[0]);
			context.MoveTo (points[2]);
			context.LineTo (points[3]);
			context.MoveTo (points[5]);
			context.LineTo (points[6]);
			context.Stroke ();
		}

		private void DrawCrowFootZeroOne (Context context, PointD[] points)
		{
			context.MoveTo (points[4]);
			context.LineTo (points[0]);
			context.MoveTo (points[2]);
			context.LineTo (points[3]);
			context.Stroke ();
			//Add circle
			context.Save ();
			context.Color = new Color (1, 1, 1);
			context.Arc (points[7].X, points[7].Y, 2.0, 0.0, 2.0 * Math.PI);
			context.FillPreserve ();
			context.Color = new Color (0, 0, 0);
			context.LineWidth = 1;
			context.Stroke ();
			context.Restore ();
		}

		private void DrawBarkerMany (Context context, PointD[] points)
		{

			PointD[] anchors=calcAnchorPoints(points[0], points[4]);
			context.Stroke ();
			context.LineTo (points[4]);
			context.LineTo (anchors[0]);
			context.LineTo (anchors[1]);
			context.LineTo (points[4]);
			context.FillPreserve ();
			//Identifying
			if (Identifying) {
				context.MoveTo (points[2]);
				context.LineTo (points[3]);
			}
			context.Stroke ();
		}

		private void DrawBarkerOne (Context context, PointD[] points)
		{
			context.MoveTo (points[4]);
			context.LineTo (points[0]);
			//Identifying
			if (Identifying) {
				context.MoveTo (points[2]);
				context.LineTo (points[3]);
			}
			context.Stroke ();

		}

		public kindNotation terminalNotation {
			get { return notation; }
			set { notation = value; }
		}

		public kindRelationshipTerminal terminalKind {
			get { return kind; }
			set { kind = value; }
		}

		public bool terminalIdentifiying {
			get { return Identifying; }
			set { Identifying = value; }
		}
		
		private PointD[] calcAnchorPoints(PointD a, PointD middle){
			PointD[] anchors = new PointD[2];
			double distanceYtop=0,distanceYbottom=0;
			
			if(parent.EndFigure!=null){
			 	distanceYtop=parent.EndFigure.DisplayBox.TopLeft.Y - middle.Y;
				distanceYbottom=parent.EndFigure.DisplayBox.BottomLeft.Y - middle.Y;
			}
			
			if (distanceYtop > 8 || distanceYbottom<-8){
				anchors[0] = new PointD (a.X + 5, a.Y);
				anchors[1] = new PointD (a.X - 5, a.Y);
			}else{
				anchors[0] = new PointD (a.X, a.Y + 5);
				anchors[1] = new PointD (a.X, a.Y - 5);
			} 

			return anchors;
		}
		
		private double lineDistance;
		private double pointDistance;
		private kindRelationshipTerminal kind;
		private kindNotation notation;
		private bool Identifying;
		private RelationshipFigure parent;

	}
}
