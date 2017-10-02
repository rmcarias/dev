using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Web.Hosting;
using System.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WubiPaws.Api.Models;
using System.Web.Http;

namespace WubiPaws.Api.Data
{

    public interface IDataAccessRepository<T> where T : DynamicEntity,new()
    {
        string DatabaseId { get; set; }
        string CollectionId { get; set; }
        IQueryable<T> QueryWhere(Expression<Func<T, bool>> predicate = null);
        Task<IQueryable<T>> QueryWhereAsync(Expression<Func<T, bool>> predicate = null);
        List<dynamic> Query(string sqlText, SqlParameterCollection paramsCollection = null);
        Task<List<dynamic>> QueryAsync(string sqlText, SqlParameterCollection paramsCollection = null);
        Task<T> LookupAsync(string id);
        T Lookup(string id);
        Task<T> Save(DynamicEntity objEntity, bool isNew);
        Task<T> Save(JObject jobject, bool isNew);
        Task<bool> DeleteObject(string key);
    }

    public  class DocumentDbRepositoryImpl<TDocument> : IDataAccessRepository<TDocument> where TDocument:DynamicEntity, new()
    {
        public DocumentDbRepositoryImpl()
        {
            var attribute = typeof(TDocument).GetCustomAttributes(typeof(DocumentAttribute), true).FirstOrDefault() as DocumentAttribute;
            if (attribute == null)
                throw new ArgumentException("the model class must be decorated with the Document attribute");
            Init(attribute.DatabaseId, attribute.CollectionId);
        }

        public DocumentDbRepositoryImpl(string databaseId, string collectionId)
        {
            Init(databaseId, collectionId);
        }

        private void Init(string databaseId, string collectionId)
        {
            this.databaseId = databaseId;
            this.collectionId = collectionId;
        }

        private  string databaseId;
        public  String DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(databaseId))
                {
                    databaseId = ConfigurationManager.AppSettings["DatabaseId"];
                }

                return databaseId;
            }
            set
            {
                databaseId = value;
            }
        }

        private  string collectionId;
        public  String CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(collectionId))
                {
                    collectionId = ConfigurationManager.AppSettings["DocumentDbCollectionId"];
                }

                return collectionId;
            }
            set
            {
                collectionId = value;
            }
        }

        private  Database database;
        private  Database Database
        {
            get
            {
                if (database == null)
                {
                    database = ReadOrCreateDatabase();
                }

                return database;
            }
        }

        private  DocumentCollection collection;
        private  DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        private  DocumentClient client;
        private  DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["DocumentDbUri"];
                    string authKey = ConfigurationManager.AppSettings["DbAuthKey"];
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                    
                }

                return client;
            }
        }

        
        private async Task<TDocument> InsertDocument(DynamicEntity objEntity)
        {
            string entityType = objEntity.EntityType;
            if (entityType == null || entityType.Length == 0)
            {
                objEntity.EntityType = objEntity.GetType().Name;

            }
            objEntity.CreatedAt = DateTimeOffset.Now;
            objEntity.UpdatedAt = DateTimeOffset.Now;
            return await Client.CreateDocumentAsync(Collection.SelfLink, objEntity).ContinueWith<TDocument>(t => GetDocFromResponse(t));

            
        }

        private async Task<TDocument> UpdateDocument(DynamicEntity objEntity)
        {
            Document doc = GetDocument(objEntity.Id);
            objEntity.UpdatedAt = DateTimeOffset.Now;
            return await Client.ReplaceDocumentAsync(doc.SelfLink, objEntity).ContinueWith<TDocument>(t => GetDocFromResponse(t));
        }

        private Document GetDocument(string id)
        {
            return Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                        .Where(d => d.Id == id)
                        .AsEnumerable()
                        .FirstOrDefault();
        }

        private DocumentCollection ReadOrCreateCollection(string databaseLink)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                      .Where(c => c.Id == CollectionId)
                      .AsEnumerable()
                      .FirstOrDefault();

            if (col == null)
            {
                col = Client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = CollectionId }).Result;
            }
            return col;
        }

        private Database ReadOrCreateDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                    .Where(d => d.Id == DatabaseId)
                    .AsEnumerable()
                    .FirstOrDefault();
            
            if (db == null)
            {
               db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }
           return db;
        }
        private TDocument GetDocFromResponse(Task<ResourceResponse<Document>> source)
        {
            if (source.IsFaulted)
            {
                new InvalidOperationException("Parent task is faulted.", source.Exception);
            }

            return GetDocEntity(source.Result.Resource);
        }

        private TDocument GetDocEntity(Document source)
        {
            if (source == null)
            {
                new ArgumentNullException("source");
            }

            return JsonConvert.DeserializeObject<TDocument>(JsonConvert.SerializeObject(source));

        }

        #region Interface Impl
        public Task<IQueryable<TDocument>> QueryWhereAsync(Expression<Func<TDocument, bool>> predicate = null) 
        {
            return Task<IQueryable<TDocument>>.Factory.StartNew(() =>
            {

                var result = QueryWhere();
                return result;
            });
        }

        public  Task<List<dynamic>> QueryAsync(string sqlText, SqlParameterCollection paramsCollection = null)
        {
            return  Task<List<dynamic>>.Factory.StartNew(() =>
            {

                var result = Query(sqlText, paramsCollection);
                return result;
            });
        }

        public List<dynamic> Query(string sqlText, SqlParameterCollection paramsCollection = null)
        {
            var sql = new SqlQuerySpec(sqlText);
            if (paramsCollection != null)
            {
                sql.Parameters = paramsCollection;
            }
            return Client.CreateDocumentQuery(Collection.DocumentsLink, sql)
                    .AsEnumerable()
                    .ToList<dynamic>();
        }

        public  IQueryable<TDocument> QueryWhere(Expression<Func<TDocument, bool>> predicate = null) 
        {
            if (predicate != null)
            {

                return Client.CreateDocumentQuery<TDocument>(Collection.DocumentsLink)
                      .Where(predicate)
                      .ToList()
                      .AsQueryable(); 
            }
            return Client.CreateDocumentQuery<TDocument>(Collection.DocumentsLink)
                     .ToList()
                     .AsQueryable();
        }

       

        public async Task<TDocument> Save(DynamicEntity objEntity, bool isNew)
        {
            if (isNew)
                return await InsertDocument(objEntity);

            return await UpdateDocument(objEntity);
        }

        public async Task<TDocument> Save(JObject objEntity, bool isNew)
        {
            DynamicEntity d = new DynamicEntity();
            d.LoadFromDynamic(objEntity);
            return await Save(d, isNew);
        }

        public async Task<bool> DeleteObject(string key)
        {
            try
            {
                var doc = GetDocument(key);


                if (doc == null)
                {
                    return false;
                }

                await Client.DeleteDocumentAsync(doc.SelfLink);

                return true;


            }
            catch (Exception ex)
            {
               
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public TDocument Lookup(string id)
        {
            var qry = this.QueryWhere(d => d.Id == id)
                           .Select<TDocument, TDocument>(d => d);

            var result = qry.ToList<TDocument>();

            return SingleResult
                    .Create<TDocument>(result.AsQueryable())
                    .Queryable.FirstOrDefault();
        }

        public Task<TDocument> LookupAsync(string id)
        {
            try
            {
                return Task<SingleResult<TDocument>>.Run(() => Lookup(id));

            }
            catch (Exception ex)
            {
               
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        #endregion


       
    }
   
  
}
