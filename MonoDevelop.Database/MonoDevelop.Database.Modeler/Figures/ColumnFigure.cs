// 
// ColumnFigure.cs
//  
// Author:
//       xiul <${AuthorEmail}>
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
using System.Collections;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.ConnectionManager;


namespace MonoDevelop.Database.Modeler
{
	public class ColumnFigure : PlainSimpleTextFigure
	{

		public ColumnFigure (ColumnSchema column) : base(column.Name)
		{
			columnModel=column;
			primaryIcon = IconFactory.GetIcon("Resources.primarykey.png");
			mandatoryIcon = IconFactory.GetIcon("Resources.mandatory.png");
			optionalIcon = IconFactory.GetIcon("Resources.optional.png");
			Initialize();
		}
		
		
		public ColumnFigure ( ) : base("Column")
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

		public override void BasicDraw (Cairo.Context context)
		{
			base.BasicDraw (context);
			if(columnModel!=null){
				if(columnModel.Constraints.GetConstraint (ConstraintType.PrimaryKey) != null){  //Column is pk
					primaryIcon.Show (context, Math.Round (this.BasicDisplayBox.X-primaryIcon.Width), Math.Round (this.BasicDisplayBox.Y));
				}
				else if (columnModel.IsNullable){
					mandatoryIcon.Show (context, Math.Round (this.BasicDisplayBox.X-mandatoryIcon.Width), Math.Round (this.BasicDisplayBox.Y));
				}else{
					optionalIcon.Show (context, Math.Round (this.BasicDisplayBox.X-optionalIcon.Width), Math.Round (this.BasicDisplayBox.Y));
					}
			}
		}
		
	/*	public override void BasicDrawSelected (Cairo.Context context)
		{ //do nothing
		}
	*/	
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
		private ImageSurface primaryIcon, mandatoryIcon, optionalIcon;
	}
}
