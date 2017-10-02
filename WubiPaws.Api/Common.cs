using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace WubiPaws.Api
{
    public static class Common
    {
        public static string ServiceApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("ServiceApiKey");
            }
        }

        public static bool IsRequestTokenValid(this HttpRequestMessage request)
        {
            IEnumerable<string> headerValues = new string[0] { };
            var isAuthorizedRequest = false;
            try
            {
                if (request.Headers.TryGetValues("X-RequestVerificationToken", out headerValues))
                {
                    var value = headerValues.FirstOrDefault();
                    if (value != null)
                    {
                        isAuthorizedRequest = PasswordHash.ValidatePassword(Common.ServiceApiKey, value);
                    }
                }
            }
            catch (Exception)
            {
                
            }
            return isAuthorizedRequest;
        }
    }
}