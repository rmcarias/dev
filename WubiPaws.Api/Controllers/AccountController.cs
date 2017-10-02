using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WubiPaws.Api.Data;
using System.Threading.Tasks;
using WubiPaws.Api.Models;
using Microsoft.Azure.Documents;

namespace WubiPaws.Api.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseApiController
    {
        
        private AccountRepository _accountRepo;
        public AccountController()
        {
            
            _accountRepo = new AccountRepository(new DocumentDbRepositoryImpl<AccountEntity>());
        }



        [HttpPost]
        [Route("auth")]
        [ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> PostAuth([FromBody]dynamic postBody)
        {
            
            string uname = postBody.userName.Value;
            string password = postBody.userPassword.Value;

            if (string.IsNullOrEmpty(uname) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Invalid username/password");
            }

            var accountAuthorized = new Tuple<Guid, bool>(Guid.NewGuid(), true); //await _accountRepo.IsAuthorized(uname, password);
            if (!accountAuthorized.Item2)
            {
                return Unauthorized();
            }

            var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, uname));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Common.ServiceApiKey));
            AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var currentUtc = new SystemClock().UtcNow;
            var expiresIn = ApiCallContext == ApiCallerContext.MobileApp ? TimeSpan.FromDays(1) : TimeSpan.FromMinutes(30);
            var expiresSeconds = expiresIn.TotalSeconds;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(expiresIn);
           
            string accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            return Ok(JObject.FromObject(new {
                        access_token = accessToken,
                        token_type = "bearer",
                        expires_in =  expiresSeconds,
                        expires_utc = ticket.Properties.ExpiresUtc.ToString(),
                        user_name = uname,
                        id = accountAuthorized.Item1.ToString(),
                        scope = ApiCallerContext.MobileApp.ToString()
            }));
        }

        [HttpPost]
        [Route("register")]
        [ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> RegisterAccount([FromBody] dynamic postBody)
        {
            var data = (JObject)JObject.FromObject(postBody);
            AccountEntity account = new AccountEntity();
            account.LoadFromDynamic(data);
            var doc =  await _accountRepo.RegisterNewAccount(account);
            return Created(string.Format("api/account/user/{0}",doc.Id), account);
        }

        //[AppAuthorizettribute]
        [Route("{key}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserInfo([FromUri]string key)
        {
            var accountResult = await _accountRepo.Find(key);
            return Ok(accountResult);
        }

        [AppAuthorizettribute]
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            var accounts = await _accountRepo.QueryAllAccounts();
            return Ok(accounts);
        }

        
    }
}
