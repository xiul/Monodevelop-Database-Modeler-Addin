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
using System.Collections.Generic;
using Gtk;
using Cairo;
using MonoHotDraw;
using MonoHotDraw.Figures;
using MonoHotDraw.Util;
using System.Collections;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.ConnectionManager;



namespace MonoDevelop.Database.Modeler
{
	/*
	 * Wrapper classes to integrate monohotdraw with MonoDevelop.Database
	 */

	public class Index : PlainSimpleTextFigure
	{
		public Index (string indexName) : base(indexName)
		{
			this.SetAttribute (FigureAttribute.FontSize, 6);
		}
		//todo: set attributes		
	}

	
	
	
	
	
	
	
	
	public class Trigger : PlainSimpleTextFigure
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

		//TODO: context really needed in every table (I think not then remove later)
		// this should be available fro all tables of the same diagram without taking care if new or altered  
	/*	public TableModel (string name, DatabaseConnectionContext context, ISchemaProvider schemaProvider)
		{
			/*tableName = name;
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
	*/

		public TableModel (string name, DatabaseConnectionContext context, ISchemaProvider schemaProvider, bool create)
		{

			//TableSchema newSchema;
			tableName = name; //TODO: remove this attribute use table model
			newTable=create;
			alteredTable=false;
			tableContext = context;
			tableSchemaProvider = schemaProvider;
			tableSchema = schemaProvider.CreateTableSchema (name);
			Initialize ();
			//Add a first column
			if(create){
				TreeIter iter;
				ColumnSchema columnSchema = new ColumnSchema (schemaProvider, tableSchema, "newColumn");
				if (storeTypes.GetIterFirst (out iter))
					columnSchema.DataTypeName = storeTypes.GetValue (iter, 0) as string;				
				columns.Add (new ColumnFigure(columnSchema));
				tableSchema.Columns.Add(columnSchema);
			
			}
			/*ColumnSchema column = new ColumnSchema (schemaProvider, tableSchema, name);*/
			//TODO: delete this only for test purpose	
			indexes.Add (new Index ("DummyIndex1"));
			indexes.Add (new Index ("DummyIndex2"));
			triggers.Add (new Trigger ("DummyTrigger1"));
			triggers.Add (new Trigger ("DummyTrigger2"));
			
			System.Console.WriteLine("Tiene Xs: "	+tableSchema.Columns.Count);

			System.Console.WriteLine(schemaProvider.GetTableCreateStatement (tableSchema));
			
		}
			
		/*private void AppendColumnSchema (ColumnSchema column)
		{
			bool pk = column.Constraints.GetConstraint (ConstraintType.PrimaryKey) != null;
			storeColumns.AppendValues (pk, column.Name, column.DataType.Name, column.DataType.LengthRange.Default.ToString (), column.IsNullable, column.Comment, column);
		}*/

		private void Initialize ()
		{
			tableColumns = new ArrayList ();
			tableIndexes = new ArrayList ();
			tableTriggers = new ArrayList ();
			if(tableSchema!=null)
			{
				if(tableSchema.Columns!=null){
					foreach (ColumnSchema col in tableSchema.Columns) {
						columns.Add(new ColumnFigure(col));
					}
				}
			}
			
			dataTypes = tableSchemaProvider.GetDataTypes ();
			storeTypes = new ListStore (typeof (string), typeof (object));
			storeTypes.SetSortColumnId (0, SortType.Ascending);
			foreach (DataTypeSchema dataType in dataTypes)
				storeTypes.AppendValues (dataType.Name, dataType);
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
		private ListStore storeTypes;  //TODO: get out this variable from every table model
		private DataTypeSchemaCollection dataTypes;
			
	}
}
