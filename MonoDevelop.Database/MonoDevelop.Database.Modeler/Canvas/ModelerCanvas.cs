// 
// ModelerCanvas.cs
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


using System;
using System.Text;
using Gtk;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoHotDraw;
using MonoHotDraw.Commands;
using MonoHotDraw.Figures;
using MonoHotDraw.Tools;
using MonoHotDraw.Util;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.ConnectionManager;



namespace MonoDevelop.Database.Modeler
{


	public class ModelerCanvas : AbstractViewContent, IDrawingEditor
	{
		public ModelerCanvas ()
		{
			this.ContentName = GettextCatalog.GetString ("DB Modeler");
			this.IsViewOnly = true;
			//Create model view-controller environment
			View = new StandardDrawingView (this);
			_controller = new modelController (View);

			//Create database designer canvas environment
			widget = new ModelerCanvasWidget (this, _controller);
			widget.getScroller ().SetSizeRequest (200, 200);
			widget.getScroller ().BorderWidth = 1;
			widget.getScroller ().Add ((Widget)View);
			Tool = new SelectionTool (this);
			widget.getScroller ().ShowAll ();

			//Add drag and drop support
			TargetEntry[] te2 = new TargetEntry[] { new Gtk.TargetEntry("tree3 ",0,7777) };
			ScrolledWindow xscroller = widget.getScroller ();
			Gtk.Drag.DestSet (xscroller, DestDefaults.All, te2, Gdk.DragAction.Copy);
			xscroller.DragDataReceived += tree3_DragDataReceived;


			//todo: fix this undo manager
			_undoManager = new UndoManager ();
			UndoManager.StackChanged += delegate { UpdateUndoRedoSensitiveness (); };
		}


		private void tree3_DragDataReceived (object o, DragDataReceivedArgs args)
		{

			//Text Drag and Drop
			System.Console.WriteLine ("Datos del drop");
			System.Console.WriteLine ("Args Info: " + args.Info.ToString ());
			System.Console.WriteLine ("Args Type: " + args.GetType ());
			System.Console.WriteLine ("Args Length:\t" + args.Context.Targets.Length);
			System.Console.WriteLine ("Args String Full: " + args.Context.Targets[0].Name);
			System.Console.WriteLine ("Args String Full: " + args.Context.Targets[1].Name);
			System.Console.WriteLine ("Args String Full: " + args.Context.Targets[2].Name);
			System.Console.WriteLine ("Args String Full: " + args.Context.Targets[3].Name);
			System.Console.WriteLine ("Args Data: " + args.SelectionData.Target.ToString ());
			string hash=Encoding.UTF8.GetString(args.SelectionData.Data).Trim();
			System.Console.WriteLine("BUSCO HASH: "+hash);
			foreach (DatabaseConnectionContext context in ConnectionContextService.DatabaseConnections){
				System.Console.Write (context.ConnectionSettings.Name + ": ");
				string hash2=context.SchemaProvider.ConnectionPool.GetHashCode().ToString();
					System.Console.WriteLine("tengo HASH: "+hash2);
				if(hash.Equals(hash2)){
				System.Console.WriteLine("Ese hash esta en conexion: "+context.ConnectionSettings.Name);
					ISchemaProvider Provider = DbFactoryService.CreateSchemaProvider(context,context.ConnectionPool);
				TableSchema t=Provider.CreateTableSchema("prueba");
				System.Console.WriteLine("Tiene :"+t.Columns.Count+" columnas");
				_controller.addFigure (t.FullName);
					foreach (ColumnSchema column in t.Columns) {
						System.Console.WriteLine("column.Name:"+column.Name);
					}
					
					                                      }
			}

			Gtk.Drag.Finish (args.Context, true, false, args.Time);
		}



		public override void Dispose ()
		{
			//	IdeApp.Workbench.RecentOpen.RecentProjectChanged -= recentChangesHandler;
		}

		public override void Load (string fileName)
		{
		}

		public override Widget Control {
			get { return widget; }
		}

		public IDrawingView View {
			get { return _view; }
			set { _view = value; }
		}

		public UndoManager UndoManager {
			get { return _undoManager; }
		}

		public ITool Tool {
			get { return _tool; }
			set {
				if (_tool != null && _tool.Activated) {
					_tool.Deactivate ();
				}

				_tool = value;
				if (value != null) {
					_tool.Activate ();
				}
			}
		}

		public static ModelerCanvas GetViewPage ()
		{
			return new ModelerCanvas ();
		}

		protected virtual void RecentChangesHandler (object sender, EventArgs e)
		{
		}

		private void UpdateUndoRedoSensitiveness ()
		{
		}
		/*
		 * undo.Sensitive = UndoManager.Undoable;
		 * redo.Sensitive = UndoManager.Redoable;
		 */

		private modelController _controller;
		private ModelerCanvasWidget widget;
		private UndoManager _undoManager;
		private IDrawingView _view;
		private ITool _tool;
	}

	/*
	public class CanvasView : CanvasPageView, IDrawingEditor
	{
	}
	*/

}
