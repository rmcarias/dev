using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WubiPaws.Api
{
    public class AppAuthorizettribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {

            return base.IsAuthorized(actionContext);
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (!actionContext.Request.IsRequestTokenValid())
            {
                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode =  HttpStatusCode.Forbidden,
                    RequestMessage = actionContext.ControllerContext.Request
                };
                return Task.FromResult(actionContext.Response);
            }

            return continuation();
        }
    }
}