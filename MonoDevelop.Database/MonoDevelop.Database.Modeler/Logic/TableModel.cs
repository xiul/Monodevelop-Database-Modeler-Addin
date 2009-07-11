// 
// TableModel.cs
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
using MonoHotDraw;
using MonoHotDraw.Figures;
using System.Collections;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.ConnectionManager;

namespace MonoDevelop.Database.Modeler
{
	/*
	 * Wrapper classes to integrate monohotdraw with MonoDevelop.Database
	 */
	public class Column : SimpleTextFigure
	{

		public Column (ColumnSchema column) : base(column.Name)
		{
			columnModel=column;
			Initialize();
		}
		
		
		public Column ( ) : base("Column")
		{
			columnModel=null;
			Initialize();
		}
		
		private void Initialize(){
			this.TextChanged +=  OnColumnNameChange;
			this.SetAttribute (FigureAttribute.FontSize, 6);
			OnColumnNameChange(this, new EventArgs ());
		}
		
		public ColumnSchema schema {
			get { return columnModel; }
			set { columnModel=schema; }
		}

		protected virtual void OnColumnNameChange (object sender, EventArgs args){
			if(columnModel!=null)
				if(ValidateDataType())
					this.Text=columnModel.Name+" : "+columnModel.DataTypeName.ToUpper();
		}
		
		//TODO: Create this function
		protected bool ValidateDataType(){
			return true;			
		}
		
		private ColumnSchema columnModel;
	}

	public class Index : SimpleTextFigure
	{
		public Index (string indexName) : base(indexName)
		{
			this.SetAttribute (FigureAttribute.FontSize, 6);
		}
		//todo: set attributes		
	}

	public class Trigger : SimpleTextFigure
	{
		public Trigger (string triggerName) : base(triggerName)
		{
			this.SetAttribute (FigureAttribute.FontSize, 6);
		}
		//todo: set attributes		
	}


	//Create a wrapper class to MonoDevelop.Database.Sql.Schema.TableSchema
	public class TableModel
	{

		public TableModel (string name, DatabaseConnectionContext context, ISchemaProvider schemaProvider)
		{
			tableName = name;
			tableContext = context;
			tableSchemaProvider = schemaProvider;
			tableSchema = tableSchemaProvider.CreateTableSchema (name);
			alteredTable=false;
			newTable=false;
			Initialize ();
			//TODO: delete this only for test purpose			
			indexes.Add (new Index ("DummyIndex2"));
			triggers.Add (new Trigger ("DummyTrigger1"));
		}
			/*			
			columns.Add (new Column ("DummyColumn1"));
			columns.Add (new Column ("DummyColumn2"));
			columns.Add (new Column ("DummyColumn3"));
			columns.Add (new Column ("DummyColumn4"));
			columns.Add (new Column ("DummyColumn5"));
			columns.Add (new Column ("DummyColumn6"));
			columns.Add (new Column ("DummyColumn7"));
			columns.Add (new Column ("DummyColumn8"));
			columns.Add (new Column ("DummyColumn9"));
			columns.Add (new Column ("DC10DoubleClickToEdit"));

			_indexes.Add (new Index ("DummyIndex1"));
			_indexes.Add (new Index ("DummyIndex2"));
			_triggers.Add (new Trigger ("DummyTrigger1"));
			_triggers.Add (new Trigger ("DummyTrigger2"));
			_triggers.Add (new Trigger ("DummyTrigger3"));
			_triggers.Add (new Trigger ("DummyTrigger4"));
			 */

		public TableModel (string name)
		{
			tableName = name; //TODO: remov this attribute use table model
			alteredTable=false;
			newTable=true;
			tableContext = null;
			tableSchemaProvider = null;
			tableSchema = null;
			Initialize ();
			
			columns.Add (new Column ());
			columns.Add (new Column ());
			columns.Add (new Column ());
			columns.Add (new Column ());
			columns.Add (new Column ());
			columns.Add (new Column ());
			columns.Add (new Column ());
			columns.Add (new Column ());
			columns.Add (new Column ());
			columns.Add (new Column ());

			indexes.Add (new Index ("DummyIndex1"));
			indexes.Add (new Index ("DummyIndex2"));
			triggers.Add (new Trigger ("DummyTrigger1"));
			triggers.Add (new Trigger ("DummyTrigger2"));
			triggers.Add (new Trigger ("DummyTrigger3"));
			triggers.Add (new Trigger ("DummyTrigger4"));
			
			
			
		}

		private void Initialize ()
		{
			tableColumns = new ArrayList ();
			tableIndexes = new ArrayList ();
			tableTriggers = new ArrayList ();
			newTable=false;
			alteredTable=false;			
			if(tableSchema!=null)
			{
				foreach (ColumnSchema col in tableSchema.Columns) {
					columns.Add(new Column(col));
				}
			}
		}

		/*
		//observer is table that uses this table as Foreign Key (FK)
		public ArrayList observers {
			get { return _relationships; }
		}

		//relations (FK) from this table to others
		public ArrayList relationships {
			get { return _observers; }
		}
		 */

		public ArrayList columns {
			get { return tableColumns; }
		}

		public ArrayList indexes {
			get { return tableIndexes; }
		}

		public ArrayList triggers {
			get { return tableTriggers; }
		}

		public string Name {
			get { return tableName; }
		}

		//todo: use non-generic arrays?
		private ArrayList tableColumns;
		private ArrayList tableIndexes;
		private ArrayList tableTriggers;
		/*
		private ArrayList observers;
		private ArrayList relationships;
		*/

		private DatabaseConnectionContext tableContext;
		private ISchemaProvider tableSchemaProvider;
		private string tableName;
		private TableSchema tableSchema;
		private bool newTable;
		private bool alteredTable;
			
	}
}
