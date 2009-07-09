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

namespace MonoDevelop.Database.Modeler
{
	public class Column : SimpleTextFigure
	{
		//string other_attributes;
		public Column (string columnName) : base(columnName)
		{
			//other_attributes="";
			this.SetAttribute (FigureAttribute.FontSize, 6);
		}
	}
		/* todo: finish attributes
		private string _datatype;
		private int _datalenght;
		private bool _primarykey;
		private bool _unique;
		*/

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

	//public class Relationship : 

	public class TableModel
	{

		public TableModel (string tableName)
		{
			_tableName = tableName;
			_columns = new ArrayList ();
			_indexes = new ArrayList ();
			_triggers = new ArrayList ();
			_columns.Add (new Column ("DummyColumn1"));
			_columns.Add (new Column ("DummyColumn2"));
			_columns.Add (new Column ("DummyColumn3"));
			_columns.Add (new Column ("DummyColumn4"));
			_columns.Add (new Column ("DummyColumn5"));
			_columns.Add (new Column ("DummyColumn6"));
			_columns.Add (new Column ("DummyColumn7"));
			_columns.Add (new Column ("DummyColumn8"));
			_columns.Add (new Column ("DummyColumn9"));
			_columns.Add (new Column ("DC10DoubleClickToEdit"));

			_indexes.Add (new Index ("DummyIndex1"));
			_indexes.Add (new Index ("DummyIndex2"));
			_triggers.Add (new Trigger ("DummyTrigger1"));
			_triggers.Add (new Trigger ("DummyTrigger2"));
			_triggers.Add (new Trigger ("DummyTrigger3"));
			_triggers.Add (new Trigger ("DummyTrigger4"));

		}

		//observer is table that uses this table as Foreign Key (FK)
		public ArrayList observers {
			get { return _relationships; }
		}

		//relations (FK) from this table to others
		public ArrayList relationships {
			get { return _observers; }
		}

		public ArrayList columns {
			get { return _columns; }
		}

		public ArrayList indexes {
			get { return _indexes; }
		}

		public ArrayList triggers {
			get { return _triggers; }
		}

		public string tableName {
			get { return _tableName; }
		}

		private ArrayList _columns;
		private ArrayList _indexes;
		private ArrayList _triggers;
		private ArrayList _observers;
		private ArrayList _relationships;
		//todo: use non-generic arrays?
		private string _tableName;
	}
}
