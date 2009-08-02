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
		public AbstractColumnFigure (ColumnSchema column) : base(column.Name){
			columnModel = column;
			dataType=null;
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
				} else if (columnModel.IsNullable) {
					mandatoryIcon.Show (context, Math.Round (this.BasicDisplayBox.X - mandatoryIcon.Width), Math.Round (this.BasicDisplayBox.Y));
				} else {
					optionalIcon.Show (context, Math.Round (this.BasicDisplayBox.X - optionalIcon.Width), Math.Round (this.BasicDisplayBox.Y));
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
		
		protected virtual bool isPrimaryKey(){
			return primaryKey = columnModel.Constraints.GetConstraint (ConstraintType.PrimaryKey) != null;
		}
				
		
		//TODO: Create this function
		protected virtual bool ValidateDataType ()
		{
			return true;
		}
		
		protected virtual void OnColumnNameChange (object sender, EventArgs args)
		{
			if (columnModel != null)
				if (ValidateDataType ()) //TODO: implement validate
					if(dataType!=null)
						this.Text = columnModel.Name + dataType.Name;
					else
						this.Text = columnModel.Name;
			OnFigureChanged (new FigureEventArgs (this, DisplayBox));
		}
		

		public bool PrimaryKey{
			get { return primaryKey;}
		}		
		
		public ColumnSchema ColumnModel {
			get { return columnModel; }
			set {
				isForeignKey ();
				isUniqueKey ();
				isPrimaryKey ();
			}
		}

		private ColumnSchema columnModel;	
		private ImageSurface primaryIcon, mandatoryIcon, optionalIcon, fkUkIcon, fkIcon;
		protected bool foreignKey, uniqueKey, primaryKey;
		protected DataTypeSchema dataType;
		
	}
	
	/********************************************************
	 ********************************************************
	 ********************************************************
	 ********************************************************/	
	
	public class ColumnFkFigure : AbstractColumnFigure{
	
		public ColumnFkFigure (ColumnSchema column) : base(column)
		{
		}

		protected virtual bool isPrimaryKey(){
			return primaryKey=false;
		}
		
				protected virtual bool isForeignKey ()
		{

			return foreignKey=true;
		}
		
		private ForeignKeyConstraintSchema owner;
	}
	
	
	
	
	/*********************************************************
	 *********************************************************
	 *********************************************************
	 *********************************************************/
	
	
	public class ColumnFigure : AbstractColumnFigure, IPopupMenuFigure
	{

		public ColumnFigure (ColumnSchema column, SortedList<string, DataTypeSchema> DataTypes) : base(column)
		{
			dataTypes=DataTypes;
			lastActiveItem=null; 				//TODO: mark active datatype item at beginning
		}
		
	/*	public ColumnFigure () : base("Column")
		{
			columnModel = null;
			Initialize ();
		}*/		

		public virtual IEnumerable<Gtk.MenuItem> MenuItemsEnumerator {
		get {
			List<Gtk.CheckMenuItem> items = new List<Gtk.CheckMenuItem> ();
				if(dataTypes!=null)
				{
				foreach(KeyValuePair<string, DataTypeSchema> data in dataTypes){
					Gtk.CheckMenuItem item = new Gtk.CheckMenuItem (data.Key);
					item.Active = item == lastActiveItem ? true : false;
					item.Activated += delegate {
							
						/*lastActiveItem = item;
						dataType = item.Data as DataTypeSchema;
						*/
						//TODO: finish this
					};
					items.Add (item);
											}
				}
				
					foreach (Gtk.MenuItem item in items) {
						yield return item;
					}
				}
		}
		/*
		public override ITool CreateFigureTool (IDrawingEditor editor, ITool defaultTool)
		{
			return new PopupMenuTool (editor, this, defaultTool, base.CreateFigureTool (editor, defaultTool));
		}
		*/
		
		

		//(ISchemaProvider provider)
		protected SortedList<string, DataTypeSchema> dataTypes;
		private Gtk.CheckMenuItem lastActiveItem;
	}

}
