using System;
using System.IO;
using XLabs.Ioc;
using System.Diagnostics;
using System.Collections.Generic;

namespace Wubipaws.Mobile
{

	public class HybridWebViewModel
	{
		public HybridWebViewModel ()
		{
			PathToDataDirectory = Resolver.Resolve<IAppSettings> ().AppDataDirectory;
			ShowHeader = true;
			NativeCallbacks = new Dictionary<string, Action<string>> ();
		}

		public string Title
		{
			get;
			set;
		}

		public string PathToDataDirectory
		{
			get;
			private set;
		}

		public dynamic ViewData
		{
			get;
			set;
		}

		public bool ShowHeader
		{
			get;
			set;
		}

		public Dictionary<string,Action<string>> NativeCallbacks
		{
			get;
			set;
		}

	}
}

