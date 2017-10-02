using System;

namespace Wubipaws.Mobile
{
	public class SettingsListItemViewModel
	{
		public SettingsListItemViewModel ()
		{
		}

		public string HeaderText
		{
			get;
			set;
		}

		public string ItemText
		{
			get;
			set;
		}

		public Action<object> OnItemClicked
		{
			get;
			set;
		}


	}
}

