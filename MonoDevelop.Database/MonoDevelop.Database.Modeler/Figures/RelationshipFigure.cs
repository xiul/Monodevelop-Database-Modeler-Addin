// 
// RelationshipFigure.cs
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
using Cairo;
using MonoHotDraw;
using MonoHotDraw.Figures;
using MonoHotDraw.Handles;
using MonoHotDraw.Tools;
using MonoHotDraw.Util;


namespace MonoDevelop.Database.Modeler
{
	/* Notation 1 - For crow's foot notation: http://www.gc.maricopa.edu/business/sylvester/cis164/er2b.htm
	 * http://www.tdan.com/view-articles/7474
	 * http://folkworm.ceri.memphis.edu/ew/SCHEMA_DOC/comparison/erd.htm
	 * 
	 * Notation 2 - http://www.essentialstrategies.com/publications/modeling/barker.htm
	 *  
	 */ 
	
	public class RelationshipFigure : LineConnection
	{

		public RelationshipFigure () : base()
		{
			StartTerminal = new CrowFootLineTerminal (8.0, 8.0, kindCrowFootTerminal.ZeroOne);
			EndTerminal = new CrowFootLineTerminal (8.0, 20.0, kindCrowFootTerminal.ZeroMore);
		}

		public override bool CanConnectEnd (IFigure figure)
		{
			if (figure is TableFigure) {
				if (!figure.Includes (StartFigure)) {
					return true;
				}
			}
			return false;
		}

		public override bool CanConnectStart (IFigure figure)
		{
			if (figure is TableFigure) {
				if (!figure.Includes (EndFigure)) {
					return true;
				}
			}
			return false;
		}

		public override RectangleD InvalidateDisplayBox {
			get {
				RectangleD rect = DisplayBox;
				rect.Inflate (5.0, 5.0);

				return rect;
			}
		}

		public override ITool CreateFigureTool (IDrawingEditor editor, ITool dt)
		{
			return new RelationshipFigureTool (editor, this, dt);
		}



	}
}
