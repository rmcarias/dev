using System.Web.Http;

namespace WubiPaws.Api.Controllers
{
    [RoutePrefix("api/app")]
    [ValidateAntiForgeryToken]
    public class AppController : ApiController
    {
        [Route("wubi/status")]        
        public IHttpActionResult GetAppStatus()
        {
            return Ok("Wubi's Paws are OK!");
        }

    }

  
  
}
