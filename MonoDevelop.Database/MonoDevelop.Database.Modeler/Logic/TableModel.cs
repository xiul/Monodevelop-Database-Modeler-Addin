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
			modelTableName = name;
			//TODO: remove this attribute use table model
			newTable = create;
			alteredTable = false;
			tableContext = context;
			tableSchemaProvider = schemaProvider;
			tableSchema = schemaProvider.CreateTableSchema (name);
			Initialize ();
			//Add a first column
			if (create) {
				ColumnSchema columnSchema = new ColumnSchema (tableSchemaProvider, tableSchema, "newColumn");
				if (storeTypes.Count > 0) {
					columnSchema.DataTypeName = storeTypes.Keys[0];
					columns.Add (new ColumnFigure (columnSchema, storeTypes, null));
					tableSchema.Columns.Add (columnSchema);
				} else {
					throw new NotImplementedException ();
				}
			}
			/*ColumnSchema column = new ColumnSchema (schemaProvider, tableSchema, name);*/
			//TODO: delete this only for test purpose	
			indexes.Add (new Index ("NotImplementedYet"));
			triggers.Add (new Trigger ("NotImplementedYet"));

			System.Console.WriteLine ("Tiene Xs: " + tableSchema.Columns.Count);

			System.Console.WriteLine (schemaProvider.GetTableCreateStatement (tableSchema));

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
			//Initialize Datatypes
			dataTypes = tableSchemaProvider.GetDataTypes ();
			storeTypes = new SortedList<string, DataTypeSchema> ();
			foreach (DataTypeSchema dataType in dataTypes)
				storeTypes.Add (dataType.Name, dataType);
			//Create a text figure for each column in model
			if (tableSchema != null) {
				if (tableSchema.Columns != null) {
					foreach (ColumnSchema col in tableSchema.Columns) {
						columns.Add (new ColumnFigure (col, storeTypes, null));
					}
				}
			}
		}

		//TODO: change for IEnumerable?
		public List<ColumnFkFigure> addFkConstraint (TableModel source, kindOptionality optionality)
		{
			List<ColumnFkFigure> items = new List<ColumnFkFigure> ();
			ForeignKeyConstraintSchema fkc = new ForeignKeyConstraintSchema (source.TableSchema.SchemaProvider);
			fkc.ReferenceTableName = source.Name;
			fkc.ReferenceTable = source.TableSchema;
			fkc.Name = source.Name + "_" + TableSchema.Name + "_fk";
			foreach (ColumnFigure col in source.columns) {
				if (col.PrimaryKey) {
					ColumnSchema fkCol = new ColumnSchema (col.ColumnModel);
					//Remove column level pk if any
					ConstraintSchema tmp=null;
					foreach(ConstraintSchema cs in fkCol.Constraints)
						if(cs is PrimaryKeyConstraintSchema)
							tmp=cs;
					if(tmp!=null)
						fkCol.Constraints.Remove(tmp);
					//TODO: create a function that just get three letters from table name using a standard
					fkCol.Name = fkCol.Name + "_" + (col.ColumnModel.Parent as TableSchema).Name + "_fk"; //TODO: should be checked that this name doesn't exists at table yet
					fkCol.Parent = TableSchema;
					if(optionality==kindOptionality.optional)
						fkCol.IsNullable=false;
					else
						fkCol.IsNullable=true;
					fkc.Columns.Add (fkCol);
					TableSchema.Columns.Add (fkCol);
					fkc.ReferenceColumns.Add (col.ColumnModel);
					Console.WriteLine("NO JODA 555 666 CREE Fk figure con:" + " Tabla: "+(col.ColumnModel.Parent as TableSchema).Name +"." +col.ColumnModel.Name);
					ColumnFkFigure fk = new ColumnFkFigure (fkCol, FigureOwner, col.ColumnModel.Name , (col.ColumnModel.Parent as TableSchema).Name);
					this.columns.Add (fk);
					items.Add (fk);
				}
			}
			if(fkc.Columns.Count > 0){
				TableSchema.Constraints.Add (fkc);
				return items;
			}else
				return null;
		}

		public AbstractColumnFigure addFkConstraintColumn (ColumnSchema sourceCol, kindOptionality optionality){
			if(sourceCol.Parent is TableSchema){
				ForeignKeyConstraintSchema fkc = null;
				//Add this column to a constraint or create a new one					
				foreach(ConstraintSchema cs in TableSchema.Constraints){
					if(cs is ForeignKeyConstraintSchema && (cs as ForeignKeyConstraintSchema).ReferenceTableName==(sourceCol.Parent as TableSchema).Name){
						fkc = (cs as ForeignKeyConstraintSchema);
					}
				}
				if(fkc==null){
				   	fkc = new ForeignKeyConstraintSchema ((sourceCol.Parent as TableSchema).SchemaProvider);
					fkc.ReferenceTableName = (sourceCol.Parent as TableSchema).Name;
					fkc.ReferenceTable = (sourceCol.Parent as TableSchema);
					fkc.Name = (sourceCol.Parent as TableSchema).Name + "_" + TableSchema.Name + "_fk";
					TableSchema.Constraints.Add(fkc);
				}
				ColumnSchema fkCol = new ColumnSchema(sourceCol);
				if(optionality==kindOptionality.optional)
					fkCol.IsNullable=false;
				else
					fkCol.IsNullable=true;				
				//Remove column level pk if any
					ConstraintSchema tmp=null;
					foreach(ConstraintSchema cs in fkCol.Constraints)
						if(cs is PrimaryKeyConstraintSchema)
							tmp=cs;
					if(tmp!=null)
						fkCol.Constraints.Remove(tmp);
				fkCol.Name = fkCol.Name + "_" + (sourceCol.Parent as TableSchema).Name + "_fk"; //TODO: should be checked that this name doesn't exists at table yet
				fkCol.Parent = TableSchema;
				fkc.Columns.Add (fkCol);
				TableSchema.Columns.Add (fkCol);
				fkc.ReferenceColumns.Add (sourceCol);
				ColumnFkFigure fk = new ColumnFkFigure (fkCol, FigureOwner, sourceCol.Name, (sourceCol.Parent as TableSchema).Name);
				this.columns.Add (fk);
				return fk;
			}
			return null;
		}
		//column.originalTableName,column.originalColumnName
		public void removeFkConstraintColumn(ColumnFkFigure fkFigure)
		{
				ForeignKeyConstraintSchema fkc = null;
				ColumnSchema colFk=null;
				//Lookup for constraint containing column to remove from FK
				int pos = -1;
			
				foreach(ConstraintSchema cs in TableSchema.Constraints){
					if(cs is ForeignKeyConstraintSchema && (cs as ForeignKeyConstraintSchema).ReferenceTableName==fkFigure.originalTableName){
						fkc = (cs as ForeignKeyConstraintSchema);
						Console.WriteLine("Tengo reference Table: " + fkc.ReferenceTableName);
						foreach(ColumnSchema col in fkc.ReferenceColumns){
						Console.WriteLine("Busco: "+fkFigure.originalTableName+"."+ fkFigure.originalColumnName+" Tengo " +(cs as ForeignKeyConstraintSchema).ReferenceTableName +"."+col.Name);
							if(col.Name==fkFigure.originalColumnName){
								pos = fkc.ReferenceColumns.IndexOf(col);
								colFk = fkc.Columns[pos];
								Console.WriteLine("Eliminando: "+fkFigure.originalTableName+"."+ fkFigure.originalColumnName);
							}
						}
					}
				}
				
				if(pos>=0){
					fkc.Columns.RemoveAt(pos);
					fkc.ReferenceColumns.RemoveAt(pos);
					TableSchema.Columns.Remove (colFk);
					this.columns.Remove(fkFigure);
				
				Console.WriteLine();
				Console.WriteLine();	
				Console.WriteLine("Mande a quitar la columna que era fk: " + fkFigure.Text);
					foreach(AbstractColumnFigure cf in this.columns){
					if(cf is ColumnFkFigure )	
						Console.WriteLine("		%%%%%%%%%%%%%% REVISO Columns en modelo de la tabla: " + cf.ColumnModel.Name + " o "+ (cf as ColumnFkFigure).originalColumnName );
				}
			
				} else
					throw new NotImplementedException ();
				
				if(fkc.Columns.Count==0)
					TableSchema.Constraints.Remove(fkc);
			
			System.Console.WriteLine (SchemaProvider.GetTableCreateStatement (tableSchema));
		}
		
		public void UpdateOptionalityFk(TableModel sourceTable, kindOptionality optionality){
			foreach(ConstraintSchema cs in TableSchema.Constraints){
				if(cs is ForeignKeyConstraintSchema && (cs as ForeignKeyConstraintSchema).ReferenceTableName==sourceTable.Name){
					ForeignKeyConstraintSchema fkc = (cs as ForeignKeyConstraintSchema);
					foreach(ColumnSchema fkCol in fkc.Columns){
						if(optionality==kindOptionality.optional)
							fkCol.IsNullable=false;
						else
							fkCol.IsNullable=true;				
					}
				}
			}

		}
		
		public ColumnFigure addNewColumn(){
				const string initialName="newColumn";
				string usedName=initialName;
				int number=1;
				bool canUseit;
				do{
					canUseit=true;
					foreach(ColumnSchema cs in TableSchema.Columns){
						if (cs.Name.ToLower().CompareTo(usedName.ToLower())==0){
							canUseit=false;
							usedName=initialName+number;
							number++;
							break;
						}
					}
				}while(!canUseit);

			ColumnSchema columnSchema = new ColumnSchema (tableSchemaProvider, tableSchema, usedName);
			ColumnFigure newColumn = null;
			if (storeTypes.Count > 0) {
				columnSchema.DataTypeName = storeTypes.Keys[0];
				newColumn = new ColumnFigure (columnSchema, storeTypes, null);
				columns.Add (newColumn);
				tableSchema.Columns.Add (columnSchema);
			} else {
				throw new NotImplementedException ();
			}
			return newColumn;
		}
		
		//TODO: shouldn't allow this kind of access... must protected which kind of files to store in collections
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
			get { return modelTableName; }
			set {
				modelTableName = value;
				tableSchema.Name = value;
			}
		}

		public TableSchema TableSchema {
			get { return tableSchema; }
		}

		public ISchemaProvider SchemaProvider {
			get { return tableSchemaProvider; }
		}

		//TODO: this variable should be force to be not null to allow continue with operation but cannot be in constructor
		public IFigure FigureOwner {
			get {
				if (tableFigureOwner == null)
					throw new NotImplementedException ();
				return tableFigureOwner;
			}
			set {
				foreach (ColumnFigure colf in columns) {
					colf.FigureOwner = value;
				}

				tableFigureOwner = value;
			}
		}



		//todo: use non-generic arrays?
		private ArrayList tableColumns;
		private ArrayList tableIndexes;
		private ArrayList tableTriggers;


		private DatabaseConnectionContext tableContext;
		private ISchemaProvider tableSchemaProvider;
		private string modelTableName;
		private TableSchema tableSchema;
		private bool newTable;
		private bool alteredTable;
		private IFigure tableFigureOwner;

		private SortedList<string, DataTypeSchema> storeTypes;
		//TODO: Move to DATABASE model because it should be share between all models
		private DataTypeSchemaCollection dataTypes;

	}
}

