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

namespace MonoDevelop.Database.Modeler
{

	[System.ComponentModel.ToolboxItem(true)]
	public partial class ModelerCanvasWidget : Gtk.Bin
	{

		public ModelerCanvasWidget (ModelerCanvas owner, modelController controller)
		{
			this.Build ();
			_owner = owner;
			_controller = controller;
		}

		public ScrolledWindow getScroller ()
		{
			return this.mainScrolledWindow;
		}

		protected virtual void OnCloseActionActivated (object sender, System.EventArgs e)
		{
			_controller.xxxremoveColumna ();
		}

		protected virtual void OnAplicarActionActivated (object sender, System.EventArgs e)
		{
			_controller.addFigure ("tableNameDummy");
		}

		protected virtual void OnConvertActionActivated (object sender, System.EventArgs e)
		{
			RelationshipFigure rel = new RelationshipFigure ();
			_owner.Tool = new ConnectionCreationTool (_owner, rel);
		}

		protected virtual void OnAddActionActivated (object sender, System.EventArgs e)
		{
			_controller.xxxaddColumna ();
		}


		private modelController _controller;
		private ModelerCanvas _owner;
	}
}
