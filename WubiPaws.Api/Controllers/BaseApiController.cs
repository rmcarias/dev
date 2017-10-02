using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WubiPaws.Api.Controllers
{
    public class BaseApiController : ApiController
    {
        public enum ApiCallerContext
        {
            MobileApp,
            Desktop
        }
        protected ApiCallerContext ApiCallContext
        {
            get
            {
                ApiCallerContext caller = ApiCallerContext.MobileApp;
                IEnumerable<string> headerValues = new string[0]{ };
                if (this.Request.Headers.TryGetValues("X-AppContext", out headerValues))
                {
                    Enum.TryParse(headerValues.FirstOrDefault(), true, out caller);

                }
               
                return caller;
            }
        }

        
    }
}
