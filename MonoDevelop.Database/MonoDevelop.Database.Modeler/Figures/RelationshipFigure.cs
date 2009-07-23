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

	public class RelationshipFigure : LineConnection, IPopupRelationshipMenu
	{

		public RelationshipFigure () : base()
		{
			notation = kindNotation.Barker;

			StartTerminal = new RelationshipLineTerminal (8.0, 22.0, kindRelationshipTerminal.OneOne, notation, false);
			EndTerminal = new RelationshipLineTerminal (8.0, 22.0, kindRelationshipTerminal.OneMore, notation, false);
			start = StartTerminal as RelationshipLineTerminal;
			end = EndTerminal as RelationshipLineTerminal;
			identifyRelationship = false;
			optionalityRelationship = kindOptionality.optional;
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

		public override ITool CreateFigureTool (IDrawingEditor editor, ITool defaultTool)
		{
			return new RelationshipMenuTool (editor, this, defaultTool, base.CreateFigureTool (editor, defaultTool));
		}

		public IEnumerable<Gtk.MenuItem> MenuItemsEnumeratorEnd {
			get {
				List<Gtk.CheckMenuItem> items = new List<Gtk.CheckMenuItem> ();

				if (notation == kindNotation.CrowsFoot) {

					Gtk.CheckMenuItem zeroOne = new Gtk.CheckMenuItem ("Zero or One");
					zeroOne.Active = kindRelationshipTerminal.ZeroOne == end.terminalKind;
					zeroOne.Activated += delegate {
						end.terminalKind = kindRelationshipTerminal.ZeroOne;
						optionalityRelationship = kindOptionality.optional;
						identifyRelationship = false;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (zeroOne);

					Gtk.CheckMenuItem one = new Gtk.CheckMenuItem ("One");
					one.Active = kindRelationshipTerminal.OneOne == end.terminalKind;
					one.Activated += delegate {
						end.terminalKind = kindRelationshipTerminal.OneOne;
						optionalityRelationship = kindOptionality.mandatory;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (one);

					Gtk.CheckMenuItem zeroMany = new Gtk.CheckMenuItem ("Zero or Many");
					zeroMany.Active = kindRelationshipTerminal.ZeroMore == end.terminalKind;
					zeroMany.Activated += delegate {
						end.terminalKind = kindRelationshipTerminal.ZeroMore;
						optionalityRelationship = kindOptionality.optional;	
						identifyRelationship = false;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (zeroMany);

					Gtk.CheckMenuItem oneMany = new Gtk.CheckMenuItem ("One or Many");
					oneMany.Active = kindRelationshipTerminal.OneMore == end.terminalKind;
					oneMany.Activated += delegate {
						end.terminalKind = kindRelationshipTerminal.OneMore;
						optionalityRelationship = kindOptionality.mandatory;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (oneMany);

				} else if (notation == kindNotation.Barker) {
					Gtk.CheckMenuItem one = new Gtk.CheckMenuItem ("One");
					one.Active = kindRelationshipTerminal.ZeroOne == end.terminalKind || kindRelationshipTerminal.OneOne == end.terminalKind;
					one.Activated += delegate {
						end.terminalKind = kindRelationshipTerminal.ZeroOne;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (one);

					Gtk.CheckMenuItem many = new Gtk.CheckMenuItem ("Many");
					many.Active = !identifyRelationship && (kindRelationshipTerminal.ZeroMore == end.terminalKind || kindRelationshipTerminal.OneMore == end.terminalKind);
					many.Activated += delegate {
						end.terminalKind = kindRelationshipTerminal.OneMore;
						end.terminalIdentifiying = false;
						identifyRelationship = false;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (many);

					Gtk.CheckMenuItem manyIdentifying = new Gtk.CheckMenuItem ("Many Identifying");
					manyIdentifying.Active = identifyRelationship && (kindRelationshipTerminal.ZeroMore == end.terminalKind || kindRelationshipTerminal.OneMore == end.terminalKind);
					manyIdentifying.Activated += delegate {
						end.terminalKind = kindRelationshipTerminal.OneMore;
						end.terminalIdentifiying = true;
						identifyRelationship = true;
						optionalityRelationship=kindOptionality.mandatory;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (manyIdentifying);
				}

				foreach (Gtk.MenuItem item in items) {
					yield return item;
				}
			}
		}

		public IEnumerable<Gtk.MenuItem> MenuItemsEnumeratorStart {
			get {
				List<Gtk.CheckMenuItem> items = new List<Gtk.CheckMenuItem> ();
				if (notation == kindNotation.CrowsFoot) {
					Gtk.CheckMenuItem one = new Gtk.CheckMenuItem ("One");
					one.Active = kindRelationshipTerminal.OneOne == start.terminalKind;
					one.Activated += delegate {
						start.terminalKind = kindRelationshipTerminal.OneOne;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (one);

				} else if (notation == kindNotation.Barker) {
					Gtk.CheckMenuItem one = new Gtk.CheckMenuItem ("One");
					one.Active = kindRelationshipTerminal.OneOne == start.terminalKind || kindRelationshipTerminal.ZeroOne == start.terminalKind;
					one.Activated += delegate {
						start.terminalKind = kindRelationshipTerminal.OneOne;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (one);
				}

				foreach (Gtk.MenuItem item in items) {
					yield return item;
				}
			}
		}
		

		public IEnumerable<Gtk.MenuItem> MenuItemsEnumeratorMiddle {
			get {
				List<Gtk.CheckMenuItem> items = new List<Gtk.CheckMenuItem> ();
				if (notation == kindNotation.CrowsFoot) {

					Gtk.CheckMenuItem identify = new Gtk.CheckMenuItem ("Identifying");
					identify.Active = identifyRelationship;
					identify.Activated += delegate {
						identifyRelationship = true;
						optionalityRelationship = kindOptionality.mandatory;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (identify);

					Gtk.CheckMenuItem nonidentify = new Gtk.CheckMenuItem ("Non-Identifying");
					nonidentify.Active = !identifyRelationship;
					nonidentify.Activated += delegate {
						identifyRelationship = false;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (nonidentify);


				} else if (notation == kindNotation.Barker) {
					if(!identifyRelationship){
						Gtk.CheckMenuItem optional = new Gtk.CheckMenuItem ("Optional");
						optional.Active = kindOptionality.optional == optionality;
						optional.Activated += delegate {
							optionalityRelationship = kindOptionality.optional;
							OnFigureChanged (new FigureEventArgs (this, DisplayBox));
						};
						items.Add (optional);
					}
					Gtk.CheckMenuItem mandatory = new Gtk.CheckMenuItem ("Mandatory");
					mandatory.Active = kindOptionality.mandatory == optionality;
					mandatory.Activated += delegate {
						optionalityRelationship = kindOptionality.mandatory;
						OnFigureChanged (new FigureEventArgs (this, DisplayBox));
					};
					items.Add (mandatory);

				}

				foreach (Gtk.MenuItem item in items) {
					yield return item;
				}
			}
		}

		public bool identifyRelationship {
			get { return identifying; }
			set {
				identifying = value;
				end.terminalIdentifiying = value;
				if (notation == kindNotation.CrowsFoot && !identifying)
					this.Dashes = Dash.ShortDash; 
				else if (notation == kindNotation.CrowsFoot && identifying)
					this.Dashes = new Double[] {};
			}
		}

		public kindOptionality optionalityRelationship {
			get { return optionality; }
			set {
				optionality = value;
				if (optionality == kindOptionality.optional && notation == kindNotation.Barker) 
					this.Dashes = Dash.ShortDash;
				else if (notation == kindNotation.Barker && optionality == kindOptionality.mandatory)
					this.Dashes = new Double[] {};
				else if (notation == kindNotation.CrowsFoot && optionality == kindOptionality.mandatory){
					if(end.terminalKind==kindRelationshipTerminal.ZeroMore)
						end.terminalKind=kindRelationshipTerminal.OneMore;
					else if (end.terminalKind==kindRelationshipTerminal.ZeroOne)
						end.terminalKind=kindRelationshipTerminal.OneOne;
				}
			}
		}

		private kindNotation notation;
		private bool identifying;
		private kindOptionality optionality;
		private RelationshipLineTerminal start, end;
	}
}
