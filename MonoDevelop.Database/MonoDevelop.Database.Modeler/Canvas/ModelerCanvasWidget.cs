// 
// ModelerCanvasWidget.cs
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
using Gtk;
using Cairo;
using MonoHotDraw.Tools;
using MonoDevelop.Database.Components;
using MonoDevelop.Database.Sql;

namespace MonoDevelop.Database.Modeler
{

	//[System.ComponentModel.ToolboxItem(true)]
	public partial class ModelerCanvasWidget : Gtk.Bin
	{

		public ModelerCanvasWidget (ModelerCanvas owner, modelController controller)
		{
			this.Build ();
			_owner = owner;
			_controller = controller;
			VBox mainVbox = new VBox (false, 6);
			mainVbox.BorderWidth = 6;
			this.Add(mainVbox);

			//Create Toolbar
			Toolbar toolbar = new Toolbar ();
			toolbar.Name = "toolbar";
			toolbar.ShowArrow = true;
			toolbar.IconSize = Gtk.IconSize.LargeToolbar;
			toolbar.ToolbarStyle = ToolbarStyle.BothHoriz;			
			toolbar.Sensitive=true;
			toolbar.Activate();
			mainVbox.Add(toolbar);
			Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(mainVbox[toolbar]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = true;			
			
			//Create SCrollWindow
			mainScrolledWindow = new ScrolledWindow();
			mainScrolledWindow.Activate();
			mainScrolledWindow.SetPolicy( Gtk.PolicyType.Always,Gtk.PolicyType.Always);
			mainScrolledWindow.CanFocus = true;
			mainScrolledWindow.Name = "mainScrolledWindow";
			mainScrolledWindow.ShadowType = ((Gtk.ShadowType)(1));
			mainVbox.Add(mainScrolledWindow);
			Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(mainVbox[mainScrolledWindow]));
 			w2.Position = 1;
			w2.Fill=true;
			w2.Expand=true;
			
			//Create Toolbar Buttons
			
			//Add New Table
			buttonNew = new ToolButton(new Gtk.Image (Gtk.Stock.New, IconSize.Button),"Add Table");	                                           
			buttonNew.Sensitive = true;
			buttonNew.TooltipMarkup = "Add a new empty table";
			buttonNew.IsImportant = true;
			buttonNew.Clicked += new EventHandler (OnbuttonNewActionActivated);
			buttonNew.Activate();
			buttonNew.Show();
			toolbar.Add (buttonNew);
			//Create a Relationship between two tables
			buttonRelationship = new ToolButton(new Gtk.Image (Gtk.Stock.New, IconSize.Button),"Relationship");	                                           
			buttonRelationship.Sensitive = true;
			buttonRelationship.TooltipMarkup = "Add a new relationship between tables";
			buttonRelationship.IsImportant = true;
			buttonRelationship.Clicked += new EventHandler (OnbuttonRelationshipActivated);
			buttonRelationship.Activate();
			buttonRelationship.Show();
			toolbar.Add (buttonRelationship);	
			//Delete Selected Figure(s)
			buttonDelete = new ToolButton(new Gtk.Image (Gtk.Stock.New, IconSize.Button),"Remove");	                                           
			buttonDelete.Sensitive = true;
				buttonDelete.TooltipMarkup = "Remove selected figure(s) from diagram (table or relationship)";
			buttonDelete.IsImportant = true;
			buttonDelete.Clicked += new EventHandler (buttonDeleteRelationshipActivated);
			buttonDelete.Activate();
			buttonDelete.Show();
			toolbar.Add (buttonDelete);				
			//Select Active Database
			comboConnections = new DatabaseConnectionContextComboBox ();
			selectedConnection = comboConnections.DatabaseConnection;
			comboConnections.Changed += new EventHandler (ConnectionChanged);
			ToolItem comboItem = new ToolItem ();
			comboItem.Child = comboConnections;
			comboItem.Show();
			toolbar.Add (new SeparatorToolItem ());
			toolbar.Add (comboItem);
			//Show all items
			mainVbox.ShowAll ();
		}

		public ScrolledWindow getScroller ()
		{
			return this.mainScrolledWindow;
		}
		
		protected virtual void OnbuttonNewActionActivated (object sender, System.EventArgs e)
		{
			if (selectedConnection == null)
				MonoDevelop.Core.Gui.MessageService.ShowError ("A connection with a database should be established first");
			else
			{
				IEditSchemaProvider provider = (IEditSchemaProvider) SelectedConnectionContext.SchemaProvider;
				_controller.addNewTable ("newTable", SelectedConnectionContext, provider);
			}
			
		}

		protected virtual void OnbuttonRelationshipActivated (object sender, System.EventArgs e)
		{
			RelationshipFigure rel = new RelationshipFigure ();
			_owner.Tool = new ConnectionCreationTool (_owner, rel);
		}
		
		//TODO: add keyboard del shortcut
		protected virtual void buttonDeleteRelationshipActivated (object sender, System.EventArgs e){
			_controller.removeSelected();
		}

		public DatabaseConnectionContext SelectedConnectionContext {
			get { return selectedConnection; }
			set {
				if (selectedConnection != value) {
					selectedConnection = value;
					buttonNew.Sensitive = value != null;
					buttonRelationship.Sensitive = value != null;
					comboConnections.DatabaseConnection = value;
				}
			}
		}

		private void ConnectionChanged (object sender, EventArgs args)
		{
			selectedConnection = comboConnections.DatabaseConnection;
			buttonNew.Sensitive = selectedConnection!=null;
			buttonRelationship.Sensitive = selectedConnection!=null;
		}		
		
		private modelController _controller;
		private ModelerCanvas _owner;
		private ScrolledWindow mainScrolledWindow;
		private DatabaseConnectionContext selectedConnection;
		private ToolButton buttonNew, buttonRelationship, buttonDelete;
		private DatabaseConnectionContextComboBox comboConnections;
	}
}
