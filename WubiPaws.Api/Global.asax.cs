using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Routing;
using WubiPaws.Api.Data;

namespace WubiPaws.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        
        protected  void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            SerializeSettings(GlobalConfiguration.Configuration);
        }

        private void SerializeSettings(HttpConfiguration config)
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings();
            jsonSetting.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            jsonSetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings = jsonSetting;

        }
    }
}
