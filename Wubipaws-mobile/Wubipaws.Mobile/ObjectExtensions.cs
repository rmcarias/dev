using System;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Wubipaws.Mobile
{

	public static class ObjectExtensions
	{
		public static bool IsDateTime (this string text)
		{
			DateTime dt = DateTime.Now;
			return DateTime.TryParse (text, out dt);
		}

		public static bool IsNullEmptyOrWhiteSpace (this string text)
		{
			return string.IsNullOrWhiteSpace (text);

		}

		public static bool IsAlphaNumeric (this string input)
		{
			if (String.IsNullOrEmpty (input))
				return false;

			return Regex.IsMatch (input.Trim (), "^[a-zA-Z0-9]+$");
		}

		public static T CastTo<T> (this Object value, T targetType)
		{
			// targetType above is just for compiler magic
			// to infer the type to cast x to
			return (T)value;
		}


		public static void PopulatePicker (this Picker pickerControl, IList<ListItem> items)
		{
			if (pickerControl != null && items != null)
			{
				foreach (ListItem item in items)
				{
					pickerControl.Items.Add (item.Text);
				}
			}
		}

		public static List<ListItem> GenderOptions ()
		{
			return new List<ListItem> () { 
				new ListItem ("Male", "male"),
				new ListItem ("Female", "female")
			};
		}

		/// <summary>
		/// Attempts to parse a Json formatted string into a Newtonsoft.Json.Linq.JObject 
		/// </summary>
		/// <param name="jsonText">The properly formmatted Json string</param>
		/// <param name="jsonResult">The JObject representation</param>
		/// <returns>True|False if parsing succeeded.</returns>
		public static bool JObjectTryParse (string jsonText, out JObject jsonResult, DateParseHandling dateParseHandling = DateParseHandling.DateTime)
		{
			jsonResult = null;
			bool ok = true;
			try
			{
				var reader = new JsonTextReader (new StringReader (jsonText));
				reader.DateParseHandling = dateParseHandling;
				reader.CloseInput = true;
				jsonResult = JObject.Load (reader);

			} catch (Exception)
			{
				ok = false;
			}
			return ok;
		}

		public static bool TryDeserializeJson<T> (string jsonText, out T objectInstance)
		{
			bool ok = true;
			objectInstance = default(T);
			try
			{
				objectInstance = JsonConvert.DeserializeObject<T> (jsonText, new JsonSerializerSettings {
					NullValueHandling = NullValueHandling.Ignore
				});

			} catch (Exception)
			{
				ok = false;
			}
			return ok;
		}

		public static bool TryPopuateObject (string jsonText, object objTargetInstance)
		{
			var ok = true;

			try
			{
				JsonConvert.PopulateObject (jsonText, objTargetInstance, new JsonSerializerSettings {
					NullValueHandling = NullValueHandling.Ignore
				});
				
			} catch (Exception)
			{
				ok = false;
			}
			return ok;
		}

		public static bool JObjectHasKey (this JObject jsonJObject, string key)
		{
			if (string.IsNullOrWhiteSpace (key) || jsonJObject == null)
				return false;

			return (from k in jsonJObject.Descendants ()
			        where k.Type == JTokenType.Property && ((JProperty)k).Name == key
			        select k).FirstOrDefault () != null;
		}

		/// <summary>
		/// Helper method to split string from either PascalCase
		/// or camelCase into words. For example:  
		/// StatusEnum.RegisteredUser.ToString() => {"Registered","User"}
		/// </summary>
		/// <param name="source">The PascalCase | camelCase string</param>
		/// <returns></returns>
		public static string SplitPascalCase (this string source)
		{
			return String.Join (" ", Regex.Split (source, @"(?<!^)(?=[A-Z0-9])"));
		}

		public static string ToSafeString (this object o)
		{
			return o == null ? String.Empty : o.ToString ();
		}

		public static bool ContainsAny (this string input, params string[] tokens)
		{
			if (string.IsNullOrEmpty (input))
				return false;

			foreach (var testMatchToken in tokens)
			{
				if (input.Contains (testMatchToken))
					return true;
			}

			return false;
		}

		public static bool IsValidEmail (this string input)
		{
			return !IsNullEmptyOrWhiteSpace (input) && Regex.IsMatch (input,
				@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
		}

		public static string ToJsonISODateTime (this string date)
		{
			DateTime dt = DateTime.Today;
			var ok = DateTime.TryParse (date, out dt);

			return ok ? dt.ToString ("s") : date;
		}
	}


}

