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
using MonoHotDraw;
using MonoHotDraw.Commands;
using MonoHotDraw.Figures;
using MonoHotDraw.Tools;
using MonoHotDraw.Util;
using MonoHotDraw.Handles;
using MonoHotDraw.Locators;
using System.Collections.Generic;



namespace MonoDevelop.Database.Modeler
{
	public class TableFigure : CompositeFigure
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
			DisplayBox = new RectangleD (0.0, 0.0, _width, _height);
			OnFigureChanged (new FigureEventArgs (this, DisplayBox));
		}

		//todo: fix metrics because add extra space at bottom
		private void syncFigureMetrics ()
		{
			double newWidth = 0, newHeight = 0;
			//Add Columns metrics
			foreach (Column col in _tableModel.columns) {
				if (col.BasicDisplayBox.Width > newWidth) {
					newWidth = col.BasicDisplayBox.Width;
				}
				newHeight += col.BasicDisplayBox.Height - 2;
			}
			newWidth += 10.0;

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

		private void populateTable ()
		{
			//Create Labels
			_tableName = new SimpleTextFigure (_tableModel.tableName);
			_tableName.SetAttribute (FigureAttribute.FontSize, 6);

			_indexLabel = new SimpleTextFigure ("Indexes");
			_indexLabel.SetAttribute (FigureAttribute.FontColor, new Color (0, 0, 0.501961));
			_indexLabel.SetAttribute (FigureAttribute.FontSize, 6);

			_triggerLabel = new SimpleTextFigure ("Triggers");
			_triggerLabel.SetAttribute (FigureAttribute.FontColor, new Color (0, 0, 0.501961));
			_triggerLabel.SetAttribute (FigureAttribute.FontSize, 6);
			//Add Table Columns & Name
			this.Add (_tableName);
			_tableName.MoveTo (DisplayBox.X + 2, DisplayBox.Y + 2);
			foreach (Column col in _tableModel.columns) {
				this.Add (col);
			}
			//Add Table Indexes Label
			this.Add (_indexLabel);
			//Add Table Triggers Label
			this.Add (_triggerLabel);
		}

		//todo: fix if table columns count=0
		public double calcIndexLabelHeightPos ()
		{
			Column last = _tableModel.columns[_tableModel.columns.Count - 1] as Column;
			return ((last.BasicDisplayBox.Y - DisplayBox.Y) + _indexLabel.BasicDisplayBox.Height);
		}

		//todo: fix if table indexes count=0
		public double calcTriggerLabelHeightPos ()
		{
			Index last = _tableModel.indexes[_tableModel.indexes.Count - 1] as Index;
			if (showIndexes)
				return ((last.BasicDisplayBox.Y - DisplayBox.Y) + _triggerLabel.BasicDisplayBox.Height); else
				return ((_indexLabel.BasicDisplayBox.Y - DisplayBox.Y) + _triggerLabel.BasicDisplayBox.Height);
		}

		//todo: use PointD
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
			//Draw Indexes
			PointD start = new PointD (DisplayBox.X, DisplayBox.Y + calcIndexLabelHeightPos ());
			context.Color = new Color (0.8, 0.8, 1, 0.7);
			context.Rectangle (start, _width, _indexLabel.BasicDisplayBox.Height - 4);
			context.FillPreserve ();
			context.Stroke ();
			//Draw Triggers
			PointD start2 = new PointD (DisplayBox.X, DisplayBox.Y + calcTriggerLabelHeightPos ());
			context.Color = new Color (0.8, 0.8, 1, 0.7);
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

		public void AddColumn (string Name)
		{
			Column newColumn = new Column ("ColumnaX" + System.DateTime.Now.Millisecond);
			_tableModel.columns.Add (newColumn);
			this.Add (newColumn);
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
			_tableName.MoveTo (DisplayBox.X + 2, DisplayBox.Y + 2);
			double y = 0.0;
			foreach (Column col in _tableModel.columns) {
				y += col.BasicDisplayBox.Height - 4;
				col.MoveTo ((DisplayBox.X + 5), (DisplayBox.Y + y));
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

				if (figure != null && view.IsFigureSelected (Figure)) {
					DelegateTool = new SimpleTextTool (Editor, figure, DefaultTool);
				} else {
					DelegateTool = DefaultTool;
				}

				DelegateTool.MouseDown (ev);
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
	}

}
