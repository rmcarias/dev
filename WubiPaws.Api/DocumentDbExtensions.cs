using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WubiPaws.Api
{
    public static class DocumentDbExtensions
    {
        public static JObject AsJObject(this Document doc)
        {
            if (doc != null)
            {
                return JObject.FromObject(doc);
            }
            return new JObject();
        }
       
        public static Document ToDocument(this JObject j)
        {
            Document d = new Document();
            d.LoadFromDynamic(j);
            return d;
        }

        public static void LoadFromDynamic(this Document d, JObject entity)
        {
            try
            {
                using (var s = new StringReader(JsonConvert.SerializeObject(entity)))
                using (var jst = new JsonTextReader(s))
                {

                    d.LoadFrom(jst);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);

            }
        }

        public static SqlParameterCollection ToSqlParams(dynamic parameters)
        {
            var sqlParamCollection = new SqlParameterCollection();
            if (parameters != null)
            {
                IDictionary<string,object> convertedParams = ((JObject)JObject.FromObject(parameters))
                                                            .ToObject<Dictionary<string,object>>();
                if (convertedParams != null)
                {
                    foreach (var item in convertedParams)
                    {
                        sqlParamCollection.Add(new SqlParameter("@" + item.Key, item.Value));
                    }
                }
            }
            return sqlParamCollection;
        }
    }
}
