using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WubiPaws.Api.Data;
using System.Threading.Tasks;
using System.Dynamic;
using Microsoft.Azure.Documents;
using System.Web.Http;
using System.IO;
namespace WubiPaws.Api.Models
{

  

    [DocumentAttribute("accountdb","accounts")]
    public class AccountEntity : DynamicEntity
    {
        string entityType = "accountEntity";
        public  AccountEntity() 
        {

        }

        public override string EntityType
        {
            get { return entityType; }
            set {;}
        }
       
    }

    
}