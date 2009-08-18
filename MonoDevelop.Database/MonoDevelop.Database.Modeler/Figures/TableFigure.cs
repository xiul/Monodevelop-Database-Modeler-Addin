// 
// TableFigure.cs
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
using Cairo;
using Gdk;
using MonoHotDraw;
using MonoHotDraw.Commands;
using MonoHotDraw.Figures;
using MonoHotDraw.Tools;
using MonoHotDraw.Util;
using MonoHotDraw.Handles;
using MonoHotDraw.Locators;
using System.Collections.Generic;
using MonoDevelop.Database.Sql;




namespace MonoDevelop.Database.Modeler
{

	public delegate void NotifyObserverEventHandler (bool refresh, bool changeConnection, TableFigure notifier, kindOptionality optionality);

	public interface IRelationshipNotifier
	{
		event NotifyObserverEventHandler NotifyChanged;
		void AddObserver (IRelationshipObserver observer);
		void RemoveObserver (IRelationshipObserver observer);
	}


	public interface IRelationshipObserver
	{
		void Update (bool refresh, bool changeConnection, TableFigure notifier, kindOptionality optionality);
		//TODO: change string for correct one
	}

	public class TableFigure : CompositeFigure, IRelationshipObserver, IRelationshipNotifier
	{
		public TableFigure (TableModel metadata)
		{
			Model = metadata;
			_width = 100;
			_height = 100;
			_showingTriggers = false;
			_showingIndexes = false;
			populateTable ();
			syncFigureMetrics ();
			iconsWidth = IconFactory.GetIcon ("Resources.primarykey.png").Width * 2;
			//TODO: iconfactory should select largest icon and then add not just add first
			DisplayBox = new RectangleD (0.0, 0.0, _width, _height);
			OnFigureChanged (new FigureEventArgs (this, DisplayBox));
		}

		//todo: fix metrics because add extra space at bottom
		private void syncFigureMetrics ()
		{
			double newWidth = 0, newHeight = 10;
			//Add Columns metrics
			foreach (AbstractColumnFigure col in _tableModel.columns) {
				if (col.BasicDisplayBox.Width > newWidth) {
					newWidth = col.BasicDisplayBox.Width;
				}
				newHeight += col.BasicDisplayBox.Height - 2;
			}
			newWidth += 10.0 + iconsWidth;


			//Add indexes label & components metrics
			newHeight += _indexLabel.BasicDisplayBox.Height - 2;
			if (showIndexes) {
				foreach (Index indx in _tableModel.indexes) {
					if (indx.BasicDisplayBox.Width > newWidth) {
						newWidth = indx.BasicDisplayBox.Width;
					}
					newHeight += indx.BasicDisplayBox.Height - 2;
				}
			}

			//Add triggers label & components metrics
			newHeight += _triggerLabel.BasicDisplayBox.Height - 2;
			if (showTriggers) {
				foreach (Trigger trg in _tableModel.triggers) {
					if (trg.BasicDisplayBox.Width > newWidth) {
						newWidth = trg.BasicDisplayBox.Width;
					}
					newHeight += trg.BasicDisplayBox.Height - 2;
				}
			}

			//Set default width
			if (newWidth < 100)
				newWidth = 100;


			RectangleD r = DisplayBox;
			if (newWidth != _width) {
				r.Width = newWidth;
				_width = newWidth;
				DisplayBox = r;
				OnFigureChanged (new FigureEventArgs (this, DisplayBox));
			}
			if (newHeight != _height) {
				r.Height = newHeight;
				_height = newHeight;
				DisplayBox = r;
				OnFigureChanged (new FigureEventArgs (this, DisplayBox));
			}
		}

		public void unPopulateTable ()
		{

			Model.columns.Clear ();
			Model.triggers.Clear ();
			Model.indexes.Clear ();
			_handles.Clear ();

			while (this.Figures.Count > 0) {
				this.Remove (this.Figures[0]);
			}

			OnFigureChanged (new FigureEventArgs (this, DisplayBox));
		}

		private void populateTable ()
		{
			//Create Labels
			_tableName = new PlainSimpleTextFigure (_tableModel.Name);
			_tableName.SetAttribute (FigureAttribute.FontSize, 7);
			_tableName.SetAttribute (FigureAttribute.FontColor, new Cairo.Color (0, 0, 0.501961));
			//TODO: don't change name at each letter changed
			_tableName.TextChanged += delegate { Model.Name = _tableName.Text;};

			_indexLabel = new PlainSimpleTextFigure ("Indexes");
			_indexLabel.SetAttribute (FigureAttribute.FontColor, new Cairo.Color (0, 0, 0.501961));
			_indexLabel.SetAttribute (FigureAttribute.FontSize, 6);

			_triggerLabel = new PlainSimpleTextFigure ("Triggers");
			_triggerLabel.SetAttribute (FigureAttribute.FontColor, new Cairo.Color (0, 0, 0.501961));
			_triggerLabel.SetAttribute (FigureAttribute.FontSize, 6);
			//Add Table Columns & Name
			this.Add (_tableName);
			_tableName.MoveTo (DisplayBox.X + 2, DisplayBox.Y + 2);
			foreach (AbstractColumnFigure col in _tableModel.columns) {
				this.Add (col);
			}
			
			//TODO: Add Foreign Keys that table model have
			
			//Add Table Indexes Label (items added at ButtonHandle)
			this.Add (_indexLabel);
			//Add Table Triggers Label (items added at ButtonHandle)
			this.Add (_triggerLabel);

			OnFigureChanged (new FigureEventArgs (this, DisplayBox));
		}

		
		//todo: fix if table columns count=0
		public double calcIndexLabelHeightPos ()
		{
			if (_tableModel.columns.Count > 0) {
				AbstractColumnFigure last = _tableModel.columns[_tableModel.columns.Count - 1] as AbstractColumnFigure;
				return ((last.BasicDisplayBox.Y - DisplayBox.Y) + _indexLabel.BasicDisplayBox.Height);
			} else
				return ((_tableName.BasicDisplayBox.Y - DisplayBox.Y) + _indexLabel.BasicDisplayBox.Height);
		}

		//todo: fix if table indexes count=0
		public double calcTriggerLabelHeightPos ()
		{
			if (_tableModel.indexes.Count > 0 && showIndexes) {
				Index last = _tableModel.indexes[_tableModel.indexes.Count - 1] as Index;
				return ((last.BasicDisplayBox.Y - DisplayBox.Y) + _triggerLabel.BasicDisplayBox.Height);
			} else
				return ((_indexLabel.BasicDisplayBox.Y - DisplayBox.Y) + _triggerLabel.BasicDisplayBox.Height);
		}

		//TODO: improve this function
		public override void BasicDraw (Cairo.Context context)
		{
			if (DisplayBox.Width == 0 || DisplayBox.Height == 0) {
				return;
			}

			//Draw table
			context.LineWidth = 1.0;
			context.Save ();
			context.Translate (DisplayBox.X, DisplayBox.Y);
			context.Scale (DisplayBox.Width, DisplayBox.Height);
			context.Rectangle (0.0, 0.0, 1, 1);
			context.Restore ();
			_tableName.BasicDraw (context);
			syncFigureMetrics ();
			//Draw Table name Line
			PointD start0 = new PointD (DisplayBox.X, DisplayBox.Y + 1);
			context.Color = new Cairo.Color (0.8, 0.8, 1, 0.7);
			context.Rectangle (start0, _width, _tableName.BasicDisplayBox.Height - 5);
			context.FillPreserve ();
			context.Stroke ();
			//Draw Indexes Line
			PointD start = new PointD (DisplayBox.X, DisplayBox.Y + calcIndexLabelHeightPos ());
			context.Color = new Cairo.Color (0.8, 0.8, 1, 0.7);
			context.Rectangle (start, _width, _indexLabel.BasicDisplayBox.Height - 4);
			context.FillPreserve ();
			context.Stroke ();
			//Draw Triggers Line
			PointD start2 = new PointD (DisplayBox.X, DisplayBox.Y + calcTriggerLabelHeightPos ());
			context.Color = new Cairo.Color (0.8, 0.8, 1, 0.7);
			context.Rectangle (start2, _width, _triggerLabel.BasicDisplayBox.Height - 4);
			context.FillPreserve ();
			context.Stroke ();
		}

		public override void Draw (Context context)
		{
			context.Save ();
			BasicDraw (context);
			foreach (IFigure fig in FiguresEnumerator) {
				fig.Draw (context);
			}
			context.Restore ();
		}

		public override void DrawSelected (Context context)
		{

			context.Save ();
			context.Translate (DisplayBox.X, DisplayBox.Y);
			context.Scale (DisplayBox.Width, DisplayBox.Height);
			context.Rectangle (0.0, 0.0, 1, 1);
			context.Restore ();
			context.Save ();
			context.LineWidth = 3;
			context.Color = new Cairo.Color (1, 0.0784314, 0.576471, 1);
			context.Stroke ();
			context.Restore ();

			context.Save ();
			BasicDrawSelected (context);
			foreach (IFigure fig in FiguresEnumerator) {
				fig.Draw (context);
			}
			context.Restore ();
		}

		public override bool ContainsPoint (double x, double y)
		{
			return DisplayBox.Contains (x, y);
		}

		
		
		//add foreign key to this table figure
		public void addFkConstraint(RelationshipFigure r, kindOptionality optionality){
			//TODO: implement more than one fk between same tables [multiple times same fk column is used].
			List<ColumnFkFigure> tmp= Model.addFkConstraint(r.StartTable.Model, optionality);
			foreach(ColumnFkFigure c in tmp){
				this.Add(c);
			}
			OnFigureChanged (new FigureEventArgs (this, DisplayBox));
		}		
		
		public void AddFkConstraintColumn (ColumnSchema sourceCol, kindOptionality optionality)
		{
			AbstractColumnFigure tmp = Model.addFkConstraintColumn(sourceCol, optionality);
			this.Add(tmp);
			OnFigureChanged (new FigureEventArgs (this, DisplayBox));
		}

		public void removeFkConstraintColumn (ColumnFkFigure column){
			Console.WriteLine("Eliminando FkFigure: "+column.originalTableName+"."+ column.originalColumnName);
			Model.removeFkConstraintColumn(column);
			this.Remove(column);
			OnFigureChanged (new FigureEventArgs (this, DisplayBox));
		}
		
		//This is useful for?
		public override RectangleD InvalidateDisplayBox {
			get {
				RectangleD rect = DisplayBox;
				if (_tableName == null) {
					return rect;
				}
				//rect.Add(_name.DisplayBox);
				rect.Inflate (30.0, 30.0);
				return rect;
			}
		}

		/*
		 * This table figure should answer every change at the model, and because this
		 * on every change it should be able to arrange positions for all their content like 
		 * columns, labels, indexes, triggers and others. And only move every object inside 
		 * composite figure isn't enough because this I override BasicMoveBy a return to their 
		 * normal Behavior
		 */
		public override void BasicMoveBy (double x, double y)
		{
			RectangleD r = BasicDisplayBox;
			r.X += x;
			r.Y += y;
			BasicDisplayBox = r;
			foreach (IFigure figure in FiguresEnumerator) {
				AbstractFigure af = figure as AbstractFigure;
				af.BasicMoveBy (x, y);
			}
		}


		//todo: avoid same effort of BasicMoveBy and OnfigureChanged
		protected override void OnFigureChanged (FigureEventArgs e)
		{
			base.OnFigureChanged (e);
			//Arrange position for table title and columns
			_tableName.MoveTo (DisplayBox.X + 2, DisplayBox.Y - 2);
			double y = 0.0;
			foreach (AbstractColumnFigure col in _tableModel.columns) {
				y += col.BasicDisplayBox.Height - 4;
				col.MoveTo ((DisplayBox.X + 5 + iconsWidth), (DisplayBox.Y + y));
			}

			//Arrange position for Indexes
			y = calcIndexLabelHeightPos ();
			_indexLabel.MoveTo ((DisplayBox.X + 20), (DisplayBox.Y + y - 2));
			foreach (Index indx in _tableModel.indexes) {
				y += indx.BasicDisplayBox.Height - 4;
				indx.MoveTo ((DisplayBox.X + 5), (DisplayBox.Y + y));
			}
			//Arrange position for Triggers
			y = calcTriggerLabelHeightPos ();
			_triggerLabel.MoveTo ((DisplayBox.X + 20), (DisplayBox.Y + y - 2));
			foreach (Trigger trg in _tableModel.triggers) {
				y += trg.BasicDisplayBox.Height - 4;
				trg.MoveTo ((DisplayBox.X + 5), (DisplayBox.Y + y));
			}
		}

		public override ITool CreateFigureTool (IDrawingEditor editor, ITool dt)
		{
			return new TableTextFigureTool (editor, this, dt);
		}

		public override IEnumerable<IHandle> HandlesEnumerator {
			get {
				if (_handles == null) {
					InitializeHandles ();
				}
				foreach (IHandle handle in _handles)
					yield return handle;
			}
		}

		private void InitializeHandles ()
		{
			_handles = new List<IHandle> ();
			_handles.Add (new ButtonHandle (this, new IndexLocator ()));
			_handles.Add (new ButtonHandle (this, new TriggerLocator ()));
		}


		public override RectangleD BasicDisplayBox {
			get { return _displayBox; }
			set { _displayBox = value; }
		}

		public bool showTriggers {
			get { return _showingTriggers; }
			set { _showingTriggers = value; }
		}

		public bool showIndexes {
			get { return _showingIndexes; }
			set { _showingIndexes = value; }
		}

		public SimpleTextFigure FindTextFigure (double x, double y)
		{
			IFigure fig = FindFigure (x, y);
			if ((fig != null) && (fig is SimpleTextFigure))
				return (SimpleTextFigure)fig;

			return null;
		}
		
		public SimpleTextFigure FindIconTextFigure (double x, double y)
		{	//Adjust X axis to get related TextFigure (Column)
			IFigure fig = FindFigure (x+(iconsWidth/2), y);
			if ((fig != null) && (fig is SimpleTextFigure))
			{
			Console.WriteLine("Icons Width="+ iconsWidth);
			Console.WriteLine("X="+ x);
			Console.WriteLine("XFig= "+fig.DisplayBox.X);
			Console.WriteLine("XCal="+(x-iconsWidth));
				return (SimpleTextFigure)fig;	
			}
			
			return null;
		}

		public TableModel Model {
			get { return _tableModel; }
			set { _tableModel = value; }
		}

		/*
		 * Text Editor Figure Tool for Columns
		 */
		
		public class TableTextFigureTool : CompositeFigureTool
		{

			public TableTextFigureTool (IDrawingEditor editor, IFigure fig, ITool dt) : base(editor, fig, dt)
			{
			}

			public override void MouseDown (MouseEvent ev)
			{
				IDrawingView view = ev.View;
				SimpleTextFigure figure = ((TableFigure)Figure).FindTextFigure (ev.X, ev.Y);
				Gdk.EventButton gdk_event = ev.GdkEvent as EventButton;
				
				if (figure != null && view.IsFigureSelected (Figure) && gdk_event.Button==3) {
					ColumnFigure cfigure = figure is ColumnFigure ? figure as ColumnFigure : null;
					if(cfigure!=null)
						DelegateTool = new PopupMenuTool (Editor, cfigure, DefaultTool, DefaultTool, true);
					} else if (figure != null && view.IsFigureSelected (Figure) && gdk_event.Button==1) {
						if(figure is AbstractColumnFigure){
							AbstractColumnFigure column = figure as AbstractColumnFigure;
							DelegateTool = new ColumnTextTool (Editor, column, DefaultTool);
						}
					} else {
						DelegateTool = DefaultTool;
					}
				if(DelegateTool!=null)
					DelegateTool.MouseDown (ev);
				
				if(figure==null){
					figure = ((TableFigure)Figure).FindIconTextFigure (ev.X, ev.Y);
					if(figure!=null){
						ColumnFigure cfigure = figure is ColumnFigure ? figure as ColumnFigure : null;
						DelegateTool = new PopupMenuTool (Editor, cfigure, DefaultTool, DefaultTool, false);
					}
				}
			}
		}

		public event NotifyObserverEventHandler NotifyChanged;

		public void Update(bool refresh, bool changeConnection, TableFigure notifier, kindOptionality optionality)
		{
			Console.WriteLine();
			if(refresh){
				Console.WriteLine("La tabla "+this.Model.Name+" ya fue Avisada de REFRESCAR!!!!!!!!!!!!!!!!!!!!!!!!!!! de la tabla:" + notifier.Model.Name);
				refreshForeignKeys(notifier, optionality);
			}
			else if(changeConnection)
				Console.WriteLine("La tabla "+this.Model.Name+" ya fue Avisada de CAMBIO DE CONEXION!!!!!!!!!!!!!!!!!!!!!!!!!!! de la tabla:"+ notifier.Model.Name);
			else if(!changeConnection && !refresh){
				Console.WriteLine("La tabla "+this.Model.Name+" ya fue Avisada de ELIMINAR FK !!!!!!!!!!!!!!!!!!!!!!!!!!! de la tabla:"+ notifier.Model.Name);
			}
			
		}		
		
		public void refreshForeignKeys(TableFigure sourceFk, kindOptionality optionality){
			PrimaryKeyConstraintSchema fkConsColumns=null;
			//Lookup for pk at table level at reference table
			foreach(ConstraintSchema cs in sourceFk.Model.TableSchema.Constraints){
				if(cs is PrimaryKeyConstraintSchema){
					fkConsColumns=cs as PrimaryKeyConstraintSchema;
					break;
				}
			}
			
			//Lookup for pk at column level at reference table
			if(fkConsColumns==null){
				foreach(ColumnSchema col in sourceFk.Model.TableSchema.Columns){
					fkConsColumns = col.Constraints.GetConstraint (ConstraintType.PrimaryKey) as PrimaryKeyConstraintSchema;
					if(fkConsColumns!=null)
						break;
				}
			}
			
			
			
			//Add new fk(s) column to table
			if(fkConsColumns!=null){
				foreach(ColumnSchema colfk in fkConsColumns.Columns){
					bool exists=false;
					Console.WriteLine("comienzo a buscar :"+colfk.Parent.Name+"."+colfk.Name);
					foreach(AbstractColumnFigure cf in Model.columns){
						if(cf is ColumnFkFigure )
						{
							ColumnFkFigure cfk = cf as ColumnFkFigure;
							Console.WriteLine("		NO JODA 666 COMPARO:" + cfk.originalTableName + "." + cfk.originalColumnName + " CON " + colfk.Parent.Name+"."+colfk.Name);
							if(cfk.sameForeignKey(colfk.Parent.Name,colfk.Name)){
								exists=true;
								Console.WriteLine(" 		!!!!!!!!! MATCHES: " + colfk.Name);
							}
						}
					}
					if(!exists){
						this.AddFkConstraintColumn(colfk,optionality);
					}
				}
			}
			
		  //Remove not existing fk(s) to table
			//if(fkConsColumns!=null){
				bool remove = true;
				ColumnFkFigure removeCfk = null;
				ColumnFkFigure colFigFk = null;
				foreach(AbstractColumnFigure cf in Model.columns){
						if(cf is ColumnFkFigure )
						{
						Console.WriteLine("Busco si elimino a: " + cf.ColumnModel.Name);
							colFigFk = cf as ColumnFkFigure;
							remove = true;
							if(fkConsColumns!=null){
								foreach(ColumnSchema colfk in fkConsColumns.Columns){	
									Console.WriteLine("		Comparo con: " + colfk.Name);
									if(colFigFk.sameForeignKey(colfk.Parent.Name,colfk.Name)){
										remove=false;
										Console.WriteLine("			No la debo remover tiene columna en el fkconstraint " + colfk.Name);
									}
								}
							}
							if(remove){
								removeCfk=colFigFk;
								Console.WriteLine("PORQ ENTRO AQUI CON: " + colFigFk.originalColumnName +"   " + colFigFk.ColumnModel.Name);
								}
						}
				}
				
				if(removeCfk!=null){
					Console.WriteLine("Mando a eliminar "+removeCfk.originalTableName+"."+removeCfk.originalColumnName);
					this.removeFkConstraintColumn(removeCfk);
				}
			//}
			
		}
		
		public void UpdateOptionalityFk(TableModel sourceTable, kindOptionality optionality){
			Model.UpdateOptionalityFk(sourceTable ,optionality);		
		}

		public void AddObserver(IRelationshipObserver observer)
		{
			this.NotifyChanged += observer.Update;
		}

		public void RemoveObserver(IRelationshipObserver observer)
		{
			this.NotifyChanged -= observer.Update;
		}
		
		public void RefreshRelationships(bool refresh, bool changeConnection, TableFigure notifier, kindOptionality optionality)
		{
			if(refresh && changeConnection)
				throw new NotImplementedException ();
			
			if(NotifyChanged!=null)
			{
				NotifyChanged(refresh,changeConnection, notifier, optionality);
			}
		}
				
		
		
		//todo: eliminate variables redundancy later and fix visibility
		private List<IHandle> _handles;
		private TableModel _tableModel;
		private double _width;
		//todo: should be width and height eliminate?
		private double _height;
		private RectangleD _displayBox;
		private SimpleTextFigure _tableName, _triggerLabel, _indexLabel;
		private bool _showingTriggers, _showingIndexes;
		private double iconsWidth;


	}

}
