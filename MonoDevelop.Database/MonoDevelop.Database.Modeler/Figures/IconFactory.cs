// 
// IconFactory.cs
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

using Cairo;
using Gdk;
using MonoHotDraw.Util;
using System.Collections.Generic;
using System;

namespace MonoDevelop.Database.Modeler
{


	public sealed class IconFactory
	{

		static IconFactory ()
		{
			_icons = new Dictionary <string, ImageSurface> (); 
		}
		
		/*				iconPrimaryKey = new PixbufFigure(Pixbuf.LoadFromResource("Resources.primarykey.png"));
			iconMandatoryColumn = new PixbufFigure(Pixbuf.LoadFromResource("Resources.mandatory.png"));
			iconOptionalColumn = new PixbufFigure(Pixbuf.LoadFromResource("Resources.optional.png"));
			*/

		
		public IconFactory () {
		}
		
		public static ImageSurface GetIcon (string iconName) {
			ImageSurface surface;
			Pixbuf       pixbuf;
			if (_icons.TryGetValue (iconName, out surface) == false) {
				//TODO: Validate error
				pixbuf  = Pixbuf.LoadFromResource (iconName);
				surface = GdkCairoHelper.PixbufToImageSurface (pixbuf);
				_icons.Add (iconName, surface);
			}
			return surface;
		}
		
		private static Dictionary <string, ImageSurface> _icons;
	}
}
