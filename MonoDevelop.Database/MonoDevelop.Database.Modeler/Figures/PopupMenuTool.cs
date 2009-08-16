//
// MonoHotDraw. Diagramming library
//
// Authors:
//	Mario Carrión <mario@monouml.org>
//
// Copyright (C) 2006, 2007, 2008 MonoUML Team (http://www.monouml.org)
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
//
using Gtk;
using Gdk;
using System;
using MonoHotDraw;
using MonoHotDraw.Tools;

namespace MonoDevelop.Database.Modeler {
	
	public class PopupMenuTool : FigureTool {
		
		public PopupMenuTool (IDrawingEditor editor, IPopupMenuFigure figure, ITool defaultTool, ITool delegateTool, bool mainMenu) 
			: base (editor, figure, defaultTool) {
			
			_figure = figure;
			DelegateTool = delegateTool;
			primaryMenu = mainMenu;
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

		public override void Activate () {
			if (DefaultTool != null) {
				DefaultTool.Activate ();
			}
			base.Activate ();
		}

		public override void MouseUp (MouseEvent ev) {
			//FIXME: Does this hardcoded value always apply?
			Gdk.EventButton gdk_event = ev.GdkEvent as EventButton;
			if (gdk_event.Button == 3) {
				using (Gtk.Menu menu = new Gtk.Menu ()) {
					//TODO: fix menu size and font size
					/*Adjustment  xAdj, yAdj;
					xAdj=new Gtk.Adjustment(0, 0, 1000, 1, 100, 10);
					yAdj=new Adjustment(0, 0, 100, 1, 10, 10);
					menu.SetScrollAdjustments(xAdj,yAdj);
					Pango.FontDescription newfont = menu.Style.FontDescription.Copy();
					newfont.Size -= (int)Pango.Scale.PangoScale*2;
					menu.ModifyFont(newfont);
					*/

					if(primaryMenu){
						foreach (Gtk.MenuItem item in _figure.MenuItemsEnumerator) {
							menu.Append (item);
						}
					}else{
						foreach (Gtk.MenuItem item in _figure.MenuItemsEnumerator2) {
							menu.Append (item);
						}						
					}
					if (menu.Children.Length > 0)  {
						menu.ShowAll ();
						menu.Popup ();
					}
				}
			} 
			base.MouseUp (ev);
		}
		
		private bool primaryMenu;
		private IPopupMenuFigure _figure;
		private ITool _delegateTool;
	}
}
