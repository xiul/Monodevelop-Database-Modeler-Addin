// MonoHotDraw. Diagramming Framework
// ColumnTextTool.cs
//
// Authors:
//	Manuel Cerón <ceronman@gmail.com>
//  Luis Ochoa <ziul1979@gmail.com>
//
// Copyright (C) 2006, 2007, 2008, 2009 MonoUML Team (http://www.monouml.org)
// Copyright (c) 2009 xiul
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
using Gdk;
using Gtk;
using System;
using MonoHotDraw;
using MonoHotDraw.Figures;
using MonoHotDraw.Util;
using MonoHotDraw.Tools;
using MonoHotDraw.Commands;

namespace MonoDevelop.Database.Modeler {

	// TODO: Catch enter key
	public class ColumnTextTool: TextTool	{

		public ColumnTextTool (IDrawingEditor editor, AbstractColumnFigure fig, ITool dt) 
			: base (editor, fig, dt) {
			_entry = new Gtk.Entry ();
			_entry.HasFrame = false;
			_entry.Alignment = 0.5f;
			_entry.Changed += new System.EventHandler (ChangedHandler);
			_entry.ModifyFont (fig.PangoLayout.FontDescription.Copy ());
		}
		
		private void ChangedHandler (object sender, EventArgs args) {
			((SimpleTextFigure) Figure).Text = _entry.Text;
			CalculateSizeEntry ();
		}
		
		private void CalculateSizeEntry () {
			int padding = (int)(Figure as SimpleTextFigure).Padding;
			RectangleD r = Figure.DisplayBox;
			r.Inflate(-padding, -padding);
			r.Inflate(5, 5);
			
			// Drawing Coordinates must be translated to View's coordinates in order to 
			// Correctly put the widget in the DrawingView
			PointD point = View.DrawingToView(r.X, r.Y);
			int x = (int) Math.Round(point.X, 0);
			int y = (int) Math.Round(point.Y, 0);
			int w = (int) Math.Max (r.Width, 10.0);
			int h = (int) Math.Max (r.Height, 10.0);
			
			View.MoveWidget (_entry, x, y);
			_entry.SetSizeRequest (w, h);
		}

		public override void MouseDown (MouseEvent ev) {
			View = ev.View;
			
			Gdk.EventType type = ev.GdkEvent.Type;
			
			if (type == EventType.TwoButtonPress) {
				CreateUndoActivity();
				_showingWidget = true;
				_entry.Text = (Figure as AbstractColumnFigure).ColumnModel.Name;
				
				View.AddWidget (_entry, 0,0);
				CalculateSizeEntry ();
				
				_entry.GrabFocus ();
				_entry.Show ();
				
				return;
			}			
			DefaultTool.MouseDown (ev);
		}
		
		public override void Deactivate () {
			View.RemoveWidget (_entry);
			UpdateUndoActivity();
			PushUndoActivity();
			base.Deactivate ();
		}

		private Gtk.Entry _entry;
	}
}
