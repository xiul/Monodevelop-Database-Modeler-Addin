// 
// ButtonHandle.cs
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
using System.Runtime.Serialization;
using MonoHotDraw.Figures;
using MonoHotDraw.Commands;
using MonoHotDraw.Util;
using MonoHotDraw.Handles;
using MonoHotDraw.Locators;
using MonoHotDraw;

namespace MonoDevelop.Database.Modeler
{
	public class IndexLocator : ILocator
	{

		public IndexLocator ()
		{

		}

		public PointD Locate (IFigure owner)
		{
			if (owner != null) {
				TableFigure ownerTable = (TableFigure)owner;
				return new PointD ((ownerTable.DisplayBox.X + 10), (ownerTable.DisplayBox.Y + ownerTable.calcIndexLabelHeightPos () + 4));
			}
			return new PointD (0, 0);
		}
	}


	public class TriggerLocator : ILocator
	{

		public TriggerLocator ()
		{

		}

		public PointD Locate (IFigure owner)
		{
			if (owner != null) {
				TableFigure ownerTable = (TableFigure)owner;
				return new PointD ((ownerTable.DisplayBox.X + 10), (ownerTable.DisplayBox.Y + ownerTable.calcTriggerLabelHeightPos () + 4));
			}
			return new PointD (0, 0);
		}
	}


	public class ButtonHandle : AbstractHandle
	{
		public ButtonHandle (IFigure owner, ILocator locator) : base(owner)
		{
			_locator = locator;
			_clicked = false;
		}

		public override Gdk.Cursor CreateCursor ()
		{
			return CursorFactory.GetCursorFromType (Gdk.CursorType.CenterPtr);
		}

		public override RectangleD DisplayBox {
			get {
				RectangleD rect = new RectangleD (Locate ());
				rect.Inflate (4.0, 4.0);
				return rect;
			}
		}

		public override void Draw (Context context)
		{
			context.Save();
			context.LineWidth = LineWidth;
			if (!_clicked) {
				context.MoveTo (DisplayBox.TopLeft);
				context.LineTo (DisplayBox.TopRight);
				double middlePoint = Math.Abs (DisplayBox.BottomLeft.X - DisplayBox.BottomRight.X) / 2;
				context.LineTo (DisplayBox.BottomLeft.X + middlePoint, DisplayBox.BottomLeft.Y);
				context.LineTo (DisplayBox.TopLeft);
			} else {
				context.MoveTo (DisplayBox.TopLeft);
				context.LineTo (DisplayBox.BottomLeft);
				double middlePoint = Math.Abs (DisplayBox.TopRight.Y - DisplayBox.BottomRight.Y) / 2;
				context.LineTo (DisplayBox.BottomRight.X, DisplayBox.TopRight.Y + middlePoint);
				context.LineTo (DisplayBox.TopLeft);

			}
			context.Color = FillColor;
			context.FillPreserve ();
			context.Color = LineColor;
			context.Stroke ();
			context.Restore();
		}

		public override void InvokeStart (double x, double y, IDrawingView view)
		{
			if (Owner is TableFigure) {
				_clicked = !_clicked;
				TableFigure f = (TableFigure)Owner;
				if (_locator is IndexLocator) {
					//Indexes
					if (f.showIndexes) {
						foreach (Index indx in f.Model.indexes) {
							f.Remove (indx);
						}
						f.showIndexes = false;
					} else {
						foreach (Index indx in f.Model.indexes) {
							f.Add (indx);
						}
						f.showIndexes = true;
					}
				}
				if (_locator is TriggerLocator) {
					//Triggers
					if (f.showTriggers) {
						foreach (Trigger trg in f.Model.triggers) {
							f.Remove (trg);
						}
						f.showTriggers = false;
					} else {
						foreach (Trigger trg in f.Model.triggers) {
							f.Add (trg);
						}
						f.showTriggers = true;
					}
				}
			}
		}


		public override void InvokeStep (double x, double y, IDrawingView view)
		{
		}
		public override void InvokeEnd (double x, double y, IDrawingView view)
		{
		}


		public override PointD Locate ()
		{
			if (_locator != null) {
				return _locator.Locate (Owner);
			} else {
				return new PointD (0, 0);
			}
		}

		private ILocator _locator;
		private bool _clicked;
	}

}
