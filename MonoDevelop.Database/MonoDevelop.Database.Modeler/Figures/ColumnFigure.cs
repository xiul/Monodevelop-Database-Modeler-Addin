// 
// ColumnFigure.cs
//  
// Author:
//       Luis Ochoa <ziul1979@gmail.com>
// 
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

using System;
using System.Collections.Generic;
using Gtk;
using Cairo;
using MonoHotDraw;
using MonoHotDraw.Figures;
using MonoHotDraw.Util;
using MonoHotDraw.Tools;
using System.Collections;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.ConnectionManager;


namespace MonoDevelop.Database.Modeler
{
	public abstract class AbstractColumnFigure : PlainSimpleTextFigure
	{
		public AbstractColumnFigure (ColumnSchema column, IFigure owner) : base(column.Name)
		{
			columnModel = column;
			tableFigureOwner = owner;
			dataType = null;
			primaryIcon = IconFactory.GetIcon ("Resources.primarykey.png");
			mandatoryIcon = IconFactory.GetIcon ("Resources.mandatory.png");
			optionalIcon = IconFactory.GetIcon ("Resources.optional.png");
			fkUkIcon = IconFactory.GetIcon ("Resources.foreign_uk.png");
			fkIcon = IconFactory.GetIcon ("Resources.foreign.png");
			Initialize ();
		}

		protected virtual void Initialize ()
		{
			this.TextChanged += OnColumnNameChange;
			this.SetAttribute (FigureAttribute.FontSize, 6);
			OnColumnNameChange (this, new EventArgs ());
			isForeignKey ();
			isUniqueKey ();
			isPrimaryKey ();

		}

		public override void BasicDraw (Cairo.Context context)
		{

			base.BasicDraw (context);
			if (columnModel != null) {
				if (primaryKey) {
					//Column is pk
					primaryIcon.Show (context, Math.Round (this.BasicDisplayBox.X - primaryIcon.Width), Math.Round (this.BasicDisplayBox.Y));
				} else if (!columnModel.IsNullable) { //TODO: bug at addin of MonoDevelop Database is used at inverse way
					optionalIcon.Show (context, Math.Round (this.BasicDisplayBox.X - mandatoryIcon.Width), Math.Round (this.BasicDisplayBox.Y));
				} else {
					mandatoryIcon.Show (context, Math.Round (this.BasicDisplayBox.X - optionalIcon.Width), Math.Round (this.BasicDisplayBox.Y));
				}

				//TODO move to fk specify
				if (foreignKey) {
					if (uniqueKey)
						fkUkIcon.Show (context, Math.Round (this.BasicDisplayBox.X - (optionalIcon.Width * 2 + 3)), Math.Round (this.BasicDisplayBox.Y)); 
					else
						fkIcon.Show (context, Math.Round (this.BasicDisplayBox.X - (optionalIcon.Width * 2 + 3)), Math.Round (this.BasicDisplayBox.Y));
				}
			}
		}

		protected virtual bool isForeignKey ()
		{
			foreignKey = false;
			if (columnModel != null) {
				foreach (ConstraintSchema constraint in (columnModel.Parent as TableSchema).Constraints) {
					//TODO: why is not working well search function at this collection?
					if (constraint.ConstraintType == ConstraintType.ForeignKey) {
						foreach (ColumnSchema col in constraint.Columns)
							if (columnModel.Name.CompareTo (col.Name) == 0)
								foreignKey = true;
					}
				}
			}
			return foreignKey;
		}

		protected virtual bool isUniqueKey ()
		{
			uniqueKey = false;
			if (columnModel != null) {
				foreach (ConstraintSchema constraint in (columnModel.Parent as TableSchema).Constraints) {
					//TODO: why is not working well search function at this collection?
					if (constraint.ConstraintType == ConstraintType.Unique) {
						foreach (ColumnSchema col in constraint.Columns)
							if (columnModel.Name.CompareTo (col.Name) == 0)
								uniqueKey = true;
					}
				}
			}
			return uniqueKey;
		}

		protected virtual bool isPrimaryKey ()
		{
			primaryKey = columnModel.Constraints.GetConstraint (ConstraintType.PrimaryKey) != null;
			if (!primaryKey) {
				if (columnModel.Parent is TableSchema) {
					foreach (ConstraintSchema item in (columnModel.Parent as TableSchema).Constraints) {
						if (item is PrimaryKeyConstraintSchema) {
							foreach (ColumnSchema column in item.Columns) {
								if (column.Name == columnModel.Name)
									primaryKey = true;
							}
						}
					}
				}
			}
			Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! paso isprimarykey en columna: "+ this.Text+ " boleean: " + primaryKey);
			return primaryKey;
		}


		//TODO: Create this function
		protected virtual bool ValidateDataType ()
		{
			return true;
		}

		protected virtual void OnColumnNameChange (object sender, EventArgs args)
		{
			if (columnModel != null)
				if (ValidateDataType ())
					//TODO: implement validate
					if (dataType != null)
						this.Text = columnModel.Name + dataType.Name; else
						this.Text = columnModel.Name;
			OnFigureChanged (new FigureEventArgs (this, DisplayBox));
		}


		public bool PrimaryKey {
			get { return primaryKey; }
		}

		public ColumnSchema ColumnModel {
			get { return columnModel; }
			set {
				isForeignKey ();
				isUniqueKey ();
				isPrimaryKey ();
			}
		}
		
		public IFigure FigureOwner {
			get { return tableFigureOwner;}
			set { tableFigureOwner=value;}
		}

		private ImageSurface primaryIcon, mandatoryIcon, optionalIcon, fkUkIcon, fkIcon;
		protected ColumnSchema columnModel;
		protected bool foreignKey, uniqueKey, primaryKey;
		protected DataTypeSchema dataType;
		protected IFigure tableFigureOwner;
	}

	/********************************************************
	 ********************************************************
	 ********************************************************
	 ********************************************************/

	public class ColumnFkFigure : AbstractColumnFigure
	{

		public ColumnFkFigure (ColumnSchema column, IFigure owner, string sourceColumnName, string sourceTableName) : base(column,owner)
		{
			fkSourceColName = sourceColumnName;
			fkSourceTableName = sourceTableName;
			foreignKey=true;
		}

	
		public bool sameForeignKey(string referenceTableName, string referenceColumnName){
			if(fkSourceColName==referenceColumnName && fkSourceTableName==referenceTableName)
				return true;
			else
				return false;
		}
		
		public bool sameReferenceTable(string referenceTableName){
			if(fkSourceTableName==referenceTableName)
				return true;
			else
				return false;			
		}
		
		public string originalColumnName{
			get{return fkSourceColName;}
		}

		public string originalTableName{
			get{return fkSourceTableName;}
		}
		
		private string fkSourceColName;
		private string fkSourceTableName;
	}




	/*********************************************************
	 *********************************************************
	 *********************************************************
	 *********************************************************/


	public class ColumnFigure : AbstractColumnFigure, IPopupMenuFigure
	{

		public ColumnFigure (ColumnSchema column, SortedList<string, DataTypeSchema> DataTypes, IFigure owner) : base(column,owner)
		{
			dataTypes = DataTypes;
			columnDataType = null;
			//TODO: mark active datatype item at beginning
		}

		/*	public ColumnFigure () : base("Column")
		{
			columnModel = null;
			Initialize ();
		}*/

		public virtual IEnumerable<Gtk.MenuItem> MenuItemsEnumerator2 {
			get {
				List<Gtk.CheckMenuItem> items = new List<Gtk.CheckMenuItem> ();

				Gtk.CheckMenuItem item1 = new Gtk.CheckMenuItem ("Not Null");
				item1.TooltipText = "Not Null";
				item1.Active = columnModel.IsNullable && !primaryKey;
				item1.Activated += delegate {
					columnModel.IsNullable = true;
					OnFigureInvalidated (new FigureEventArgs (this, DisplayBox));
					if (primaryKey){
						TogglePrimaryKey ();
						Console.WriteLine("This colum primarykey is: " + primaryKey.ToString());
					}
				};
				items.Add (item1);
				Gtk.CheckMenuItem item2 = new Gtk.CheckMenuItem ("Nullable");
				item2.TooltipText = "Nullable";
				item2.Active = !columnModel.IsNullable;
				item2.Activated += delegate {
					columnModel.IsNullable = false;
					OnFigureInvalidated (new FigureEventArgs (this, DisplayBox));
					if (primaryKey){
						TogglePrimaryKey ();
						Console.WriteLine("This colum primarykey is: " + primaryKey.ToString());
					}					
				};
				items.Add (item2);
				Gtk.CheckMenuItem item3 = new Gtk.CheckMenuItem ("Primary Key");
				item3.TooltipText = "Primary Key";
				item3.Active = primaryKey;
				item3.Activated += delegate {
					if(!primaryKey){
						columnModel.IsNullable = true;
						TogglePrimaryKey ();
					}
				};
				items.Add (item3);

				foreach (Gtk.MenuItem item in items) {
					yield return item;
				}
			}
		}

		public void TogglePrimaryKey ()
		{
			PrimaryKeyConstraintSchema pkConstraint = null;
			PrimaryKeyConstraintSchema pkColumnConstraint = null;
			PrimaryKeyConstraintSchema pkTemp = null;
			bool tableLevel=false;
			
			//Table have table, columna level or none primary key
			if (columnModel.Parent is TableSchema) {
				foreach (ConstraintSchema item in (columnModel.Parent as TableSchema).Constraints) {
					if (item is PrimaryKeyConstraintSchema) {
						pkTemp = item as PrimaryKeyConstraintSchema;
						foreach (ColumnSchema column in item.Columns) {
							if (column.Name == columnModel.Name) {
								pkConstraint = item as PrimaryKeyConstraintSchema;
							}
						}
					}
				}
			
				//Table have a column level primary key
				int count=0;
				foreach (ColumnSchema column in (columnModel.Parent as TableSchema).Columns) {
					foreach (ConstraintSchema constraint in column.Constraints) {
						if (constraint is PrimaryKeyConstraintSchema) {
							if(pkColumnConstraint==null && column.Name!=columnModel.Name){
								pkColumnConstraint=constraint as PrimaryKeyConstraintSchema;
								count++;
							}
						}
					}
				}
				if(count>1)
					throw new NotImplementedException ();
			}
			
			if(pkTemp!=null && pkTemp.Columns.Count == 0){ //A pk without columns a table level isn't valid
					(columnModel.Parent as TableSchema).Constraints.Remove (pkTemp);
					pkTemp=null;
				}
			
			if(pkTemp!=null || pkConstraint!=null)
					tableLevel=true;

			if(tableLevel && pkColumnConstraint!=null)
				throw new NotImplementedException ();
			
			if(tableLevel){
				//Remove this column because belong yet to table level pk
				if(pkConstraint!=null){
					pkConstraint.Columns.Remove (columnModel);
					primaryKey = false;
					Console.WriteLine("Remove a table level this column");
					if (pkConstraint.Columns.Count == 1){ //only one column left then change to column level constraint
						(columnModel.Parent as TableSchema).Constraints.Remove (pkConstraint);
						pkConstraint.Columns[0].Constraints.Add(pkConstraint);
						Console.WriteLine("Last column remain move from table level to column level a:"+pkConstraint.Columns[0].Name);
					}
				}else{ //Add this column because don't belong yet
					primaryKey = true;
					Console.WriteLine("Add a table level this column " + pkTemp.Columns.Count );
					pkTemp.Columns.Add (ColumnModel);
					pkTemp = null;
				}
				
			}else{
				if (pkConstraint == null && pkTemp == null) {
					Console.WriteLine("Taban ambos NULL en one pk column level");
					pkConstraint = columnModel.Constraints.GetConstraint (ConstraintType.PrimaryKey) as PrimaryKeyConstraintSchema;
					if (pkConstraint != null) {
						Console.WriteLine("Quito uno solo a nivel de column");
						primaryKey = false;
						ColumnModel.Constraints.Remove (pkConstraint);
					} else {
						if(pkColumnConstraint!=null){
							Console.WriteLine("Constraint a nivel de columna existe en otra col lo convierto a nivel de tabla");
							primaryKey = true;
							if(pkColumnConstraint.Columns.Count==1)
								pkColumnConstraint.Columns[0].Constraints.Remove ((pkColumnConstraint as ConstraintSchema));
							else
								throw new NotImplementedException ();
							pkColumnConstraint.Columns.Add (ColumnModel);
							(columnModel.Parent as TableSchema).Constraints.Add (pkColumnConstraint);

						}else{
							Console.WriteLine("Nuevo Constraint a nivel de esta columna");
							primaryKey = true;
							pkConstraint = new PrimaryKeyConstraintSchema (ColumnModel.SchemaProvider);
							pkConstraint.Columns.Add (ColumnModel);
							ColumnModel.Constraints.Add (pkConstraint);							
						}
					}
				}else{
					throw new NotImplementedException ();
				}
			}
			
			if(tableFigureOwner is TableFigure){
				(tableFigureOwner as TableFigure).RefreshRelationships(true,false,tableFigureOwner as TableFigure);
			}else if (tableFigureOwner==null){
				throw new NotImplementedException ();
			}
		}

		public virtual IEnumerable<Gtk.MenuItem> MenuItemsEnumerator {
			get {
				List<Gtk.CheckMenuItem> items = new List<Gtk.CheckMenuItem> ();
				if (dataTypes != null) {
					foreach (KeyValuePair<string, DataTypeSchema> data in dataTypes) {
						Gtk.CheckMenuItem item = new Gtk.CheckMenuItem (data.Key);
						item.TooltipText = data.Key;
						item.Active = data.Value == columnDataType ? true : false;
						item.Activated += new EventHandler (OnDataTypeSelected);
						items.Add (item);
					}
				}
				foreach (Gtk.MenuItem item in items) {
					yield return item;
				}
			}
		}

		protected virtual void OnDataTypeSelected (object sender, System.EventArgs e)
		{
			if (sender is Gtk.CheckMenuItem) {
				Gtk.CheckMenuItem tmp = sender as Gtk.CheckMenuItem;
				dataTypes.TryGetValue (tmp.TooltipText, out columnDataType);
				columnModel.DataTypeName = columnDataType.Name;
			}
			//columnModel.
			//LengthEdited: column.DataType.LengthRange.Default
			//Default Length: column.DataType.LengthRange.Default.ToString ()
			//Scale: column.DataType.LengthRange.Default.ToString ()
		}

		//(ISchemaProvider provider)
		protected SortedList<string, DataTypeSchema> dataTypes;
		private DataTypeSchema columnDataType;
	}

}
