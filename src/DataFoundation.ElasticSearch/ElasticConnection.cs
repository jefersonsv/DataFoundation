﻿using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Serilog;

namespace DataFoundation.ElasticSearch
{
    public class ElasticConnection
    {
        Uri node;
        string index;
        string type;

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/elasticsearch-net.html
        /// </summary>
        public ElasticLowLevelClient LowClient { get; private set; }

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/nest.html
        /// </summary>
        public ElasticClient HighClient { get; private set; }

        public static void CreateIndex(string elasticSearchConnectionString, string index, int shards, int replicas)
        {
            CreateIndexRequest req = new CreateIndexRequest(index);
            req.Settings = new IndexSettings
            {
                NumberOfReplicas = replicas,
                NumberOfShards = shards
            };

            var client = new Nest.ElasticClient(new Uri(elasticSearchConnectionString));
            var res = client.CreateIndex(req);
        }

        public async static Task<ICatResponse<CatIndicesRecord>> ListIndexAsync(string elasticSearchConnectionString)
        {
            var client = new Nest.ElasticClient(new Uri(elasticSearchConnectionString));
            return await client.CatIndicesAsync();
        }


        public async static Task<IDeleteIndexResponse> DeleteIndexAsync(string elasticSearchConnectionString, string index)
        {
            var req = new DeleteIndexRequest(index);

            var client = new Nest.ElasticClient(new Uri(elasticSearchConnectionString));
            return await client.DeleteIndexAsync(req);
        }

        public ElasticConnection(string index, string elasticSearchConnectionString = null, string type = null)
        {
            if (string.IsNullOrEmpty(elasticSearchConnectionString))
                elasticSearchConnectionString = "http://127.0.0.1:9200";

            node = new Uri(elasticSearchConnectionString);
            var config = new ConnectionConfiguration(node);
            LowClient = new ElasticLowLevelClient(config);
            HighClient = new ElasticClient(node);
            this.index = index;
            this.type = type ?? index;
        }

        public async Task DeleteByIdAsync(string id)
        {
            Log.Verbose($"Deleting elastic document: {id}");
            var doc = new Nest
                .DocumentPath<dynamic>(new Nest.Id(id))
                .Index(this.index)
                .Type(this.type);

            Nest.IDeleteResponse resp = await this.HighClient.DeleteAsync<dynamic>(doc);
        }

        public async Task<IDeleteByQueryResponse> DeleteByAsync(string field, string equalsValue)
        {
            Log.Verbose($"Deleting elastic document with {field} = {equalsValue}");

            var query = new QueryContainer(
                new TermQuery
                {
                    Field = field,
                    Value = equalsValue
                });
     
            var doc = new Nest
                .DocumentPath<dynamic>(query)
                .Index(this.index)
                .Type(this.type);

            var resp = await this.HighClient.DeleteByQueryAsync<dynamic>(q => q
                .Index(this.index)
                .Type(this.type)
                .Query(rq => rq
                    .Term(field, equalsValue)
                )
            );

            return resp;
        }

        public dynamic QueryAll(int limit = 10)
        {
            var searchResponse = this.HighClient.Search<dynamic>(s => s
                    .Index(this.index)
                    .Type(this.type)
                    .Size(limit)
                    .Query(q => q
                        .MatchAll()
                    )
                );

            return searchResponse;
        }

        public async Task<string> QueryAsync(string json)
        {
            PostData post = PostData.String(json);
            var res = await this.LowClient.SearchAsync<StringResponse>(this.index, this.index, post);

            if (!res.Success)
                throw new Exception("Cannot perform QueryAsync. " + res.DebugInformation);
            
            return System.Text.Encoding.Default.GetString(res.ResponseBodyInBytes);
        }

        public async Task<dynamic> QueryAllAsync(int limit = 10)
        {
            var searchResponse = await this.HighClient.SearchAsync<dynamic>(s => s
                    .Index(this.index)
                    .Type(this.type)
                    .Size(limit)
                    .Query(q => q
                        .MatchAll()
                    )
                );

            return searchResponse;
        }

        public void Index(string id, string json)
        {
            PostData post = PostData.String(json);
            var res = LowClient.Index<Elasticsearch.Net.StringResponse>(index, type, id, post);
        }

        public async Task<StringResponse> IndexAsync(string id, string json)
        {
            PostData post = PostData.String(json);
            return await LowClient.IndexAsync<Elasticsearch.Net.StringResponse>(index, type, id, post);
        }

        public void Index(string json)
        {
            PostData post = PostData.String(json);
            var res = LowClient.Index<Elasticsearch.Net.StringResponse>(index, type, post);
        }

        public async Task<StringResponse> IndexAsync(string json)
        {
            PostData post = PostData.String(json);
            return await LowClient.IndexAsync<Elasticsearch.Net.StringResponse>(index, type, post);
        }

        

    }
}
