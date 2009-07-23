// 
// PopupMenuTool.cs
// 
//  
// Authors:
// Mario Carrión <mario@monouml.org>
// Luis Ochoa <ziul1979@gmail.com>
//
// Copyright (C) 2006, 2007, 2008 MonoUML Team (http://www.monouml.org)
// Copyright (C) 2009 Luis Ochoa
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

using Gtk;
using Gdk;
using Cairo;
using System;
using MonoHotDraw;
using MonoHotDraw.Tools;

namespace MonoDevelop.Database.Modeler
{

	public class RelationshipMenuTool : FigureTool
	{

		public RelationshipMenuTool (IDrawingEditor editor, IPopupRelationshipMenu figure, ITool defaultTool, ITool delegateTool) : base(editor, figure, defaultTool)
		{

			_figure = figure;
			DelegateTool = delegateTool;
		}

		public ITool DelegateTool {
			get { return _delegateTool; }
			set {
				if (_delegateTool != null && _delegateTool.Activated) {
					_delegateTool.Deactivate ();
				}

				_delegateTool = value;

				if (_delegateTool != null) {
					_delegateTool.Activate ();
				}
			}
		}

		public override ITool DefaultTool {
			get {
				if (DelegateTool != null) {
					return DelegateTool;
				} else {
					return base.DefaultTool;
				}
			}
			set { base.DefaultTool = value; }
		}

		public override void Activate ()
		{
			if (DefaultTool != null) {
				DefaultTool.Activate ();
			}
			base.Activate ();
		}

		public override void MouseUp (MouseEvent ev)
		{
			PointD startPoint = (_figure as RelationshipFigure).StartPoint;
			PointD endPoint = (_figure as RelationshipFigure).EndPoint;

			Gdk.EventButton gdk_event = ev.GdkEvent as EventButton;
			if (gdk_event.Button == 3) {
				using (Gtk.Menu menu = new Gtk.Menu ()) {
					if (insideCircle (25, startPoint, ev.X, ev.Y)) {
						foreach (Gtk.MenuItem item in _figure.MenuItemsEnumeratorStart) {
							menu.Append (item);
						}
					} else if (insideCircle (25, endPoint, ev.X, ev.Y)) {
						foreach (Gtk.MenuItem item in _figure.MenuItemsEnumeratorEnd) {
							menu.Append (item);
						}
					} else {
						foreach (Gtk.MenuItem item in _figure.MenuItemsEnumeratorMiddle) {
							menu.Append (item);
						}
					}
						
					
					if (menu.Children.Length > 0) {
						menu.ShowAll ();
						menu.Popup ();
					}
				}
			}

			base.MouseUp (ev);
		}

		private bool insideCircle (int radius, PointD center, double x, double y)
		{
			double D = Math.Sqrt (Math.Pow (center.X - x, 2) + Math.Pow (center.Y - y, 2));
			return D <= radius;
		}

		private IPopupRelationshipMenu _figure;
		private ITool _delegateTool;
	}

}
