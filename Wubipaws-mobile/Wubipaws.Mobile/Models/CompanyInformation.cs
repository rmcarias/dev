using System;

namespace Wubipaws.Mobile
{
	public class CompanyInformation
	{
		public CompanyInformation ()
		{
		}

		public string CompanyName
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public string PhoneNumber
		{
			get;
			set;
		}

		public static CompanyInformation GetDefault ()
		{

			return new CompanyInformation {
				CompanyName = "Fido & Co.",
				Email = " info@fidoandcompany.com",
				PhoneNumber = "619-295-WOOF(9663)"
			};
		}
	}
}

