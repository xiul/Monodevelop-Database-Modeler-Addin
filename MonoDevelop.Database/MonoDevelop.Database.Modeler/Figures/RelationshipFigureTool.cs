// 
// RelationshipFigureTool.cs
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


using Gdk;
using Gtk;
using MonoHotDraw;
using MonoHotDraw.Figures;
using MonoHotDraw.Tools;
using MonoHotDraw.Util;

namespace MonoDevelop.Database.Modeler
{

	public class RelationshipFigureTool : CompositeFigureTool
	{
		public RelationshipFigureTool (IDrawingEditor editor, IFigure fig, ITool dt) : base(editor, fig, dt)
		{

		}

	/*	public override void MouseDown (MouseEvent ev)
		{

			if (Figure is RelationshipFigure)
				DelegateTool = new PolyLineFigureTool (Editor, (IConnectionFigure)Figure, DefaultTool);

			if (DelegateTool != null) {
				DelegateTool.MouseDown (ev);
			}

		}*/

		public override void Deactivate ()
		{
			base.Deactivate ();
		}
		
		public override void Activate () {
			base.Activate ();
			Gtk.Widget widget = Editor.View as Gtk.Widget;
			
			widget.GdkWindow.Cursor = CursorFactory.GetCursorFromType (CursorType.Dot);
		}
		
		public override void MouseMove (MouseEvent ev) {
			Widget widget = (Widget) ev.View;
			IFigure figure = ev.View.Drawing.FindFigure (ev.X, ev.Y);
			if (figure != null) {
				widget.GdkWindow.Cursor = CursorFactory.GetCursorFromType (Gdk.CursorType.Dot);
			}
			else {
				widget.GdkWindow.Cursor = CursorFactory.GetCursorFromType (Gdk.CursorType.Target);
			}
		}
		
	}
}
