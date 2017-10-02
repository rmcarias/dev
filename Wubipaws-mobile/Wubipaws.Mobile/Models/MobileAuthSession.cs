using System;

namespace Wubipaws.Mobile
{
	public interface IMobileAuthSession
	{
		string Id
		{
			get;

		}


		string LoginEmail
		{
			get;

		}

		string LoginPassword
		{
			get;

		}

		string AccountName
		{
			get;	
			set;
		}

		string UserName
		{
			get;

		}

		string AuthToken
		{
			get;

		}

		DateTime ExpiresUtc
		{
			get;

		}

		bool IsAuthorized
		{
			get;
			set;
		}

		void KillSession ();

	}

	public class MobileAuthSession : IMobileAuthSession
	{
		public MobileAuthSession (string authToken, 
		                          string expires, 
		                          string userName,
		                          string loginEmail,
		                          string loginPassword,
		                          string id)
		{
			this.authToken = authToken;
			var expiresDt = DateTime.UtcNow;
			DateTime.TryParse (expires, out expiresDt);
			expiresUtc = expiresDt.ToUniversalTime ();
			this.userName = userName;
			this.loginEmail = loginEmail;
			this.id = id;
			this.loginPassword = loginPassword;
		}

		private string id;

		public string Id
		{
			get{ return id; }

		}

		private string userName;

		public string UserName
		{
			get{ return userName; }

		}

		public string AccountName
		{
			get;	
			set;
		}

		private string loginPassword;

		public string LoginPassword
		{
			get{ return loginPassword; }

		}

		private string loginEmail;

		public string LoginEmail
		{
			get{ return loginEmail; }

		}

		private string authToken;

		public string AuthToken
		{
			get{ return authToken; }

		}

		private DateTime expiresUtc;

		public DateTime ExpiresUtc
		{
			get { return expiresUtc; }

		}

		public bool IsAuthorized
		{
			get;
			set;
		}

		public void KillSession ()
		{
			authToken = userName = String.Empty;
			expiresUtc = DateTime.UtcNow.AddDays (-2);
			IsAuthorized = false;
		}
	}
}

