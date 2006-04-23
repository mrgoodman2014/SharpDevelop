﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public class ToolBarDropDownButton : ToolStripDropDownButton , IStatusUpdate
	{
		object caller;
		Codon codon;
		ArrayList subItems;
		
		public ToolBarDropDownButton(Codon codon, object caller, ArrayList subItems)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller        = caller;
			this.codon         = codon;
			this.subItems			= subItems;

			if (codon.Properties.Contains("label")){
				Text = StringParser.Parse(codon.Properties["label"]);
			}
			if (Image == null && codon.Properties.Contains("icon")) {
				Image = ResourceService.GetBitmap(StringParser.Parse(codon.Properties["icon"]));
			}
			
			UpdateStatus();
			UpdateText();
		}

		void CreateDropDownItems()
		{
			DropDownItems.Clear();
			foreach (object item in subItems)
			{
				if (item is ToolStripItem)
				{
					DropDownItems.Add((ToolStripItem)item);
					if (item is IStatusUpdate)
					{
						((IStatusUpdate)item).UpdateStatus();
						((IStatusUpdate)item).UpdateText();
					}
				}
				else
				{
					ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
					DropDownItems.AddRange(submenuBuilder.BuildSubmenu(codon, caller));
				}
			}
		}
		protected override void OnDropDownShow(EventArgs e)
		{
			if (codon != null && !this.DropDown.Visible)
			{
				CreateDropDownItems();
			}
			base.OnDropDownShow(e);
		}

		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				return isEnabled;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				bool isVisible = failedAction != ConditionFailedAction.Exclude;
				if (base.Visible != isVisible) {
					base.Visible = isVisible;
				}
				
				if (this.Visible && codon.Properties.Contains("icon")) {
					Image = ResourceService.GetBitmap(StringParser.Parse(codon.Properties["icon"]));
				}
			}
		}
		
		public virtual void UpdateText()
		{
			if (codon != null) {
				if (codon.Properties.Contains("tooltip")) {
					ToolTipText = StringParser.Parse(codon.Properties["tooltip"]);
				}
				
				if (codon.Properties.Contains("label")){
					Text = StringParser.Parse(codon.Properties["label"]);
				}
			}
		}
	}
}
