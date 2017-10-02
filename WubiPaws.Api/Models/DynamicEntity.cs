using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WubiPaws.Api.Models
{
    public  class DynamicEntity : Document
    {

        [JsonProperty(PropertyName = "entityType")]
        public virtual string EntityType { get; set; }
        [JsonProperty(PropertyName = "createdat")]
        public virtual DateTimeOffset? CreatedAt { get; set; }
        [JsonProperty(PropertyName = "updatedat")]
        public virtual DateTimeOffset? UpdatedAt { get; set; }

       
    }
}
