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
	
	public enum kindCrowFootTerminal
	{
		ZeroMore,
		OneMore,
		OneOne,
		ZeroOne
	}
	
	/*
	 * ZeroMore: >o----...
	 * OneMore:  >|----...
	 * OneOne:   -||---...
	 * ZeroOne:  -o|---...
	 */	
	
	public enum kindRelationship
	{
		Identify,
		NonIndentify
	}
	
	/* 
	 * Indentifiying Relationship (Fk and Pk): Solid Line
	 * NonIndentifiying Relationship (only Fk): Dashed Line
	 */

	
	public class CrowFootLineTerminal : LineTerminal
	{

		public CrowFootLineTerminal () : this(10.0, 20.0, kindCrowFootTerminal.OneMore)
		{

		}

		public CrowFootLineTerminal (double lDistance, double pDistance, kindCrowFootTerminal kind) : base()
		{
			_lineDistance = lDistance;
			_pointDistance = pDistance;
			_kind = kind;
		}

		//todo: improve terminal this is just a stub not real and final terminal
		public override PointD Draw (Context context, PointD a, PointD b)
		{
			context.Save();
			
			//get parallel lines points
			PointD leftPoint = new PointD ();
			PointD middlePoint = new PointD ();
			PointD rightPoint = new PointD ();
			PointD leftPoint2 = new PointD ();
			PointD middlePoint2 = new PointD ();
			PointD rightPoint2 = new PointD ();

			Geometry.GetArrowPoints (a, b, _lineDistance, _pointDistance, out leftPoint, out rightPoint, out middlePoint);

			Geometry.GetArrowPoints (a, b, _lineDistance, _pointDistance + 3, out leftPoint2, out rightPoint2, out middlePoint2);

			if (_kind == kindCrowFootTerminal.OneMore) {
				PointD pointOne, pointTwo;
				if (Math.Abs (a.X - b.X) > 100) {
					pointOne = new PointD (a.X, a.Y + 5);
					pointTwo = new PointD (a.X, a.Y - 5);
				} else {
					pointOne = new PointD (a.X + 5, a.Y);
					pointTwo = new PointD (a.X - 5, a.Y);
				}
				context.MoveTo (middlePoint);
				context.LineTo (pointOne);
				context.MoveTo (middlePoint);
				context.LineTo (pointTwo);
				context.MoveTo (middlePoint);
				context.LineTo (a);
				context.MoveTo (leftPoint);
				context.LineTo (rightPoint);
				context.MoveTo (leftPoint2);
				context.LineTo (rightPoint2);
			} else if (_kind == kindCrowFootTerminal.ZeroMore) {
				PointD pointOne, pointTwo;
				if (Math.Abs (a.X - b.X) > 100) {
					pointOne = new PointD (a.X, a.Y + 5);
					pointTwo = new PointD (a.X, a.Y - 5);
				} else {
					pointOne = new PointD (a.X + 5, a.Y);
					pointTwo = new PointD (a.X - 5, a.Y);
				}
				context.MoveTo (middlePoint);
				context.LineTo (pointOne);
				context.MoveTo (middlePoint);
				context.LineTo (pointTwo);
				context.MoveTo (middlePoint);
				context.LineTo (a);
				context.Stroke ();
				//Add circle
				context.Save();
      			context.Color = new Color (1, 1, 1);
      			context.Arc (middlePoint.X, middlePoint.Y,2.0, 0.0, 2.0 * Math.PI);
      			context.FillPreserve();
				context.Color = new Color (0, 0, 0);
				context.LineWidth = 1;
      			context.Stroke ();
				context.Restore();
			} else if (_kind == kindCrowFootTerminal.OneOne) {
				context.MoveTo (middlePoint);
				context.LineTo (a);
				context.MoveTo (leftPoint);
				context.LineTo (rightPoint);
				context.MoveTo (leftPoint2);
				context.LineTo (rightPoint2);
			} else if (_kind == kindCrowFootTerminal.ZeroOne) {
				context.MoveTo (middlePoint);
				context.LineTo (a);
				context.MoveTo (leftPoint);
				context.LineTo (rightPoint);
				context.Stroke ();
				//Add circle here
				context.Save();
      			context.Color = new Color (1, 1, 1);
      			context.Arc (middlePoint2.X, middlePoint2.Y,2.0, 0.0, 2.0 * Math.PI);
      			context.FillPreserve();
				context.Color = new Color (0, 0, 0);
				context.LineWidth = 1;
      			context.Stroke ();
				context.Restore();				
			}
			
			context.Restore();
			
			return middlePoint;
		}

		//todo: fix it
		public override RectangleD InvalidateRect (PointD b)
		{
			double distance = Math.Max (_lineDistance * 2, _pointDistance);
			RectangleD r = new RectangleD (b.X, b.Y, 0.0, 0.0);
			r.Inflate (distance, distance);
			return r;
		}


		private double _lineDistance;
		private double _pointDistance;
		private kindCrowFootTerminal _kind;


	}
}
