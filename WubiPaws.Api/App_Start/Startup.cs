using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(WubiPaws.Api.Startup))]
namespace WubiPaws.Api
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
           
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            //This will used the HTTP header: "Authorization"      Value: "Bearer 1234123412341234asdfasdfasdfasdf"
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            //...Cookie Auth Setup here...//
        }

    }
}