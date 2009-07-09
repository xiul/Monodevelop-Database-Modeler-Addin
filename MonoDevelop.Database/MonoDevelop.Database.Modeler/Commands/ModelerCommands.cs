// 
// ModelerCommands.cs
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
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Database.Modeler
{
	public enum DatabaseModelerCommands
	{
		ShowModeler
	}

	public class ShowModelerPageOnStartUpHandler : CommandHandler
	{
		protected override void Run ()
		{
			/*if (!PropertyService.Get("WelcomePage.ShowOnStartup", true))
				return;*/

			ModelerCanvas wpmv = new ModelerCanvas ();
			IdeApp.Workbench.OpenDocument (wpmv, true);
		}
	}

	public class ShowModelerPageHandler : CommandHandler
	{
		protected override void Run ()
		{

						/*foreach (Document d in IdeApp.Workbench.Documents) {
				if (d.GetContent<CanvasPageView>() != null) {
					d.Select();
					return;
				}
			}*/
ModelerCanvas wpmv = new ModelerCanvas ();
			IdeApp.Workbench.OpenDocument (wpmv, true);
		}

		protected override void Update (CommandInfo info)
		{
			base.Update (info);
		}
	}

}
