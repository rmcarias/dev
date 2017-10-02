using System;
using Newtonsoft.Json.Linq;
using XLabs.Forms.Mvvm;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace Wubipaws.Mobile
{
	// Define other methods and classes here
	public class DynamicJObjectViewModel
	{
		public DynamicJObjectViewModel (JObject other)
		{

			this._propBag = other;

		}

		private JObject _propBag = new JObject ();

		public string this [string path]
		{
			get {
//				var res = Value<string> (path);
//				return  res == null ? string.Empty : res;


				//workaround for nested path selection
				var nestedPath = path.Replace (":", ".");
				var token = _propBag.SelectToken (nestedPath);
				if (token != null)
				{
					if (token.Type == JTokenType.Object || token.Type == JTokenType.Array)
					{
						return token.ToString ();
					}
					return token.Value<string> ();
				}
				return string.Empty;

			}
			set { 
				
				//workaround for nested path selection
				if (path.Contains (":"))
				{
					var nestedPath = path.Split (new[]{ ':' }, StringSplitOptions.RemoveEmptyEntries);

					var rootToken = nestedPath [0];
					var childToken = nestedPath [1];

					if (_propBag.SelectToken (rootToken) != null)
					{

						if (_propBag.SelectToken (rootToken).SelectToken (childToken) != null)
						{
							_propBag [rootToken] [childToken] = value;
						} else
						{
							(_propBag [rootToken] as JObject).Add (new JProperty (childToken, value));
						}
					} else
					{

						_propBag.Add (rootToken, new JObject (new JProperty (childToken, value)));
					}
				} else
				{
					
					if (_propBag.SelectToken (path) != null)
					{						
						_propBag [path] = value;
					} else
					{
						
						_propBag.Add (new JProperty (path, value));
					}
				}
			
			}
		}

		public T Value<T> (string path)
		{
			var token = _propBag.SelectToken (path);
			if (token != null)
			{
				return  token.Value<T> ();
			}
			return default(T);
		}



		public override string ToString ()
		{
			return this._propBag.ToString ();
		}
	}
}

