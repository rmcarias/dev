using System;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Wubipaws.Mobile
{
	public class DialogProxyViewModel
	{
		public DialogProxyViewModel ()
		{
		}

		[JsonProperty ("title")]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty ("message")]
		public string Message
		{
			get;
			set;
		}


		private string okText = "OK";

		[JsonProperty ("ok")]
		public string Accept
		{
			get{ return okText; }
			set {
				if (!string.IsNullOrEmpty (value))
				{
					okText = value;
				} 
			}
		}

		[JsonProperty ("cancel")]
		public string Cancel
		{
			get;
			set;
		}

		[JsonIgnore]
		public bool HasCancel
		{
			get{ return !string.IsNullOrEmpty (Cancel); }
		}
	}
}

