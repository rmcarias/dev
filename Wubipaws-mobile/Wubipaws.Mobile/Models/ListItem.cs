using System;

namespace Wubipaws.Mobile
{
	public class ListItem
	{
		public ListItem ()
		{
			
		}

		public ListItem (string text, object value)
		{
			this.Text = text;
			this.Value = value;
		}

		public string Text
		{
			get;
			set;
		}

		public object Value
		{
			get;
			set;
		}
	}
}

