// 
// modelController.cs
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
using System.Collections;
using MonoHotDraw;
using MonoHotDraw.Figures;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.ConnectionManager;

namespace MonoDevelop.Database.Modeler
{


	public class modelController
	{

		//todo: this is not really the controller just a testing class
		public modelController (IDrawingView view)
		{
			this.view = view;
			diagram = new DatabaseModel();
		}

		public void addNewTable (string name, DatabaseConnectionContext context, ISchemaProvider schemaProvider)
		{	//TODO: improve way of assing last parameter
			TableModel tableModel  = new TableModel (name,context,schemaProvider,true);
			TableFigure tableFigure = new TableFigure (tableModel);
			tableModel.FigureOwner = tableFigure;
			view.Drawing.Add (tableFigure);
			System.Console.WriteLine("added table");
			foreach ( IFigure fig in view.Drawing.FiguresEnumerator){
				System.Console.WriteLine("figura"+fig.ToString());
			}
			diagram.AddTable(tableFigure);
		}
		
		public void addTable (string name, DatabaseConnectionContext context, ISchemaProvider schemaProvider, bool create){
			TableModel tableModel = new TableModel(name,context,schemaProvider,false);
			TableFigure tableFigure = new TableFigure (tableModel);
			tableModel.FigureOwner=tableFigure;
			view.Drawing.Add (tableFigure);
			System.Console.WriteLine("added table22222");
			foreach ( IFigure fig in view.Drawing.FiguresEnumerator){
				System.Console.WriteLine("figura"+fig.ToString());
			}
			diagram.AddTable(tableFigure);
		}
		
		public virtual IEnumerable ModelTablesNames {
			get{
				foreach(TableFigure f in diagram.TableFiguresInModel){
					yield return f.Model.Name;
				}
			}
		}
		
		
		public void removeSelected (){
		if(view.SelectionCount>0){
			foreach ( IFigure fig in view.SelectionEnumerator){
					if(fig is TableFigure){
						//TODO: fix bug view not refresh after delete composite figure
						(fig as TableFigure).unPopulateTable();
						view.Drawing.Remove(fig as TableFigure);
						diagram.removeTable(fig as TableFigure);
					}
					if(fig is RelationshipFigure){
						//TODO: implement (fig as RelationshipFigure) unconnect
						view.Drawing.Remove(fig as RelationshipFigure);
					}
				}
			}
		}
		

			/*		if(_model!=null){
				Column xy = _model.columns[1] as Column;
				_view.Drawing.Remove(xy);
				_model.columns.RemoveAt(1);
				//_tool.Deactivate();
			}*/

		/*public void xxxaddColumna ()
		{
			if (_model != null) {
				_fig.AddColumn ("ColumnaX" + System.DateTime.Now.Millisecond);

			}
		}

		public void refreshView ()
		{
			//StandardDrawingView x = (StandardDrawingView) _view;

		}*/

		private IDrawingView view;
		DatabaseModel diagram;
	//	private TableModel _model;
		//todo: this is not really all model only 1 table
	//	TableFigure _fig;

	}
}
