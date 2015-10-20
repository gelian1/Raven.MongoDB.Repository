﻿using DB.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Repository
{
    /// <summary>
    /// MongoSessionAsync
    /// </summary>
    public class MongoSessionAsync
    {
        #region 私有方法

        /// <summary>
        /// Mongo自增长ID数据序列
        /// </summary>
        private MongoSequence _sequence;
        /// <summary>
        /// MongoDB WriteConcern
        /// </summary>
        private WriteConcern _writeConcern;
        /// <summary>
        /// MongoClient
        /// </summary>
        private MongoClient _mongoClient;
        /// <summary>
        /// MongoDatabase
        /// </summary>
        public IMongoDatabase Database { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connString">数据库链接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="writeConcern">WriteConcern选项</param>
        /// <param name="sequence">Mongo自增长ID数据序列对象</param>
        /// <param name="isSlaveOK"></param>
        /// <param name="readPreference"></param>
        public MongoSessionAsync(string connString, string dbName, WriteConcern writeConcern = null, MongoSequence sequence = null, bool isSlaveOK = false, ReadPreference readPreference = null)
        {
            this._writeConcern = writeConcern ?? WriteConcern.Unacknowledged;
            this._sequence = sequence ?? new MongoSequence();

            var databaseSettings = new MongoDatabaseSettings();
            databaseSettings.WriteConcern = this._writeConcern;
            databaseSettings.ReadPreference = readPreference ?? ReadPreference.SecondaryPreferred;

            _mongoClient = new MongoClient(connString);
            Database = _mongoClient.GetDatabase(dbName, databaseSettings);
        }

        #endregion


        /// <summary>
        /// 根据数据类型得到集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>() where T : class, new()
        {
            return Database.GetCollection<T>(typeof(T).Name);
        }

        /// <summary>
        /// 创建自增长ID
        /// <remarks>默认自增ID存放 [Sequence] 集合</remarks>
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns></returns>
        public async Task<long> CreateIncIDAsync<T>() where T : class, new()
        {
            long id = 1;
            var collection = Database.GetCollection<BsonDocument>(this._sequence.SequenceName);
            var typeName = typeof(T).Name;

            var query = Builders<BsonDocument>.Filter.Eq(this._sequence.CollectionName, typeName);
            var update = Builders<BsonDocument>.Update.Inc(this._sequence.IncrementID, 1L);
            var options = new FindOneAndUpdateOptions<BsonDocument, BsonDocument>();
            options.IsUpsert = true;
            options.ReturnDocument = ReturnDocument.After;

            var result = await collection.FindOneAndUpdateAsync(query, update, options);
            id = result[this._sequence.IncrementID].AsInt64;

            return id;
        }


        #region 获取字段

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="fieldsExp"></param>
        /// <returns></returns>
        public ProjectionDefinition<T> IncludeFields<T>(Expression<Func<T, object>> fieldsExp) where T : class, new()
        {
            var builder = Builders<T>.Projection;

            if (fieldsExp != null)
            {
                List<ProjectionDefinition<T>> fieldDocument = new List<ProjectionDefinition<T>>();
                var body = (fieldsExp.Body as NewExpression);
                if (body == null || body.Members == null)
                {
                    throw new Exception("fieldsExp表达式格式错误， eg: x => new { x.Field1, x.Field2 }");
                }
                foreach (var m in body.Members)
                {
                    fieldDocument.Add(builder.Include(m.Name));
                }
                return builder.Combine(fieldDocument);
            }
            return null;
        }

        #endregion

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="filterExp"></param>
        ///// <param name="fieldExp"></param>
        ///// <param name="sortExp"></param>
        ///// <param name="sortType"></param>
        ///// <param name="limit"></param>
        ///// <param name="skip"></param>
        ///// <returns></returns>
        //public async Task<IAsyncCursor<T>> FindAsync<T>(Expression<Func<T, bool>> filterExp = null
        //    , Expression<Func<T, object>> fieldExp = null
        //    , Expression<Func<T, object>> sortExp = null, SortType sortType = SortType.Ascending
        //    , int limit = 0, int skip = 0)
        //    where T : class, new()
        //{
        //    FilterDefinition<T> filter = null;
        //    ProjectionDefinition<T, T> projection = null;
        //    SortDefinition<T> sort = null;

        //    if (filterExp != null)
        //    {
        //        filter = Builders<T>.Filter.Where(filterExp);
        //    }

        //    if (sortExp != null)
        //    {
        //        if (sortType == SortType.Ascending)
        //        {
        //            sort = Builders<T>.Sort.Ascending(sortExp);
        //        }
        //        else
        //        {
        //            sort = Builders<T>.Sort.Descending(sortExp);
        //        }
        //    }

        //    if (fieldExp != null)
        //    {
        //        projection = Builders<T>.Projection.Include(fieldExp);
        //    }
        //    var option = CreateFindOptions(projection, sort, sortType, limit, skip);
        //    var result = await this.GetCollection<T>().FindAsync(filter, option);

        //    return result;
        //}

        ///// <summary>
        ///// 查询记录
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="filterExp"></param>
        ///// <param name="field"></param>
        ///// <param name="sort"></param>
        ///// <param name="sortType"></param>
        ///// <param name="limit"></param>
        ///// <param name="skip"></param>
        ///// <returns></returns>
        //public async Task<IAsyncCursor<T>> FindAsync<T>(Expression<Func<T, bool>> filterExp
        //    , FieldDefinition<T> field = null
        //    , SortDefinition<T> sort = null, SortType sortType = SortType.Ascending
        //    , int limit = 0, int skip = 0)
        //    where T : class, new()
        //{
        //    FilterDefinition<T> filter = null;
        //    ProjectionDefinition<T, T> projection = null;
        //    if (filterExp != null)
        //    {
        //        filter = Builders<T>.Filter.Where(filterExp);
        //    }
        //    if (field != null)
        //    {
        //        projection = Builders<T>.Projection.Include(field);
        //    }
        //    var option = CreateFindOptions(projection, sort, sortType, limit, skip);
        //    var result = await this.GetCollection<T>().FindAsync(filter, option);

        //    return result;
        //}

        ///// <summary>
        ///// 查询记录
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="filter"></param>
        ///// <param name="fieldExp"></param>
        ///// <param name="sort"></param>
        ///// <param name="sortType"></param>
        ///// <param name="limit"></param>
        ///// <param name="skip"></param>
        ///// <returns></returns>
        //public async Task<IAsyncCursor<T>> FindAsync<T>(FilterDefinition<T> filter
        //    , Expression<Func<T, object>> fieldExp = null
        //    , SortDefinition<T> sort = null, SortType sortType = SortType.Ascending
        //    , int limit = 0, int skip = 0)
        //    where T : class, new()
        //{
        //    ProjectionDefinition<T, T> projection = null;
        //    if (fieldExp != null)
        //    {
        //        projection = Builders<T>.Projection.Include(fieldExp);
        //    }
        //    var option = CreateFindOptions(projection, sort, sortType, limit, skip);
        //    var result = await this.GetCollection<T>().FindAsync(filter, option);

        //    return result;
        //}

        ///// <summary>
        ///// 查询记录
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="filter"></param>
        ///// <param name="field"></param>
        ///// <param name="sort"></param>
        ///// <param name="sortType"></param>
        ///// <param name="limit"></param>
        ///// <param name="skip"></param>
        ///// <returns></returns>
        //public async Task<IAsyncCursor<T>> FindAsync<T>(FilterDefinition<T> filter
        //    , FieldDefinition<T> field = null
        //    , SortDefinition<T> sort = null, SortType sortType = SortType.Ascending
        //    , int limit = 0, int skip = 0)
        //    where T : class, new()
        //{
        //    ProjectionDefinition<T, T> projection = null;
        //    if (field != null)
        //    {
        //        projection = Builders<T>.Projection.Include(field);
        //    }
        //    var option = CreateFindOptions(projection, sort, sortType, limit, skip);
        //    var result = await this.GetCollection<T>().FindAsync(filter, option);

        //    return result;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="projection"></param>
        /// <param name="sort"></param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public FindOptions<T, T> CreateFindOptions<T>(ProjectionDefinition<T, T> projection = null
            , SortDefinition<T> sort = null
            , int limit = 0, int skip = 0)
        {
            var option = new FindOptions<T, T>();
            if (limit > 0)
            {
                option.Limit = limit;
            }
            if (skip > 0)
            {
                option.Skip = skip;
            }

            if (projection != null)
            {
                option.Projection = projection;
            }

            if (sort != null)
            {
                option.Sort = sort;
            }

            return option;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="projection"></param>
        /// <param name="sortExp"></param>
        /// <param name="sortType"></param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public FindOptions<T, T> CreateFindOptions<T>(ProjectionDefinition<T, T> projection = null
            , Expression<Func<T, object>> sortExp = null, SortType sortType = SortType.Ascending
            , int limit = 0, int skip = 0)
        {
            var option = new FindOptions<T, T>();
            if (limit > 0)
            {
                option.Limit = limit;
            }
            if (skip > 0)
            {
                option.Skip = skip;
            }

            if (projection != null)
            {
                option.Projection = projection;
            }

            SortDefinition<T> sort = null;
            if (sortExp != null)
            {
                if (sortType == SortType.Ascending)
                {
                    sort = Builders<T>.Sort.Ascending(sortExp);
                }
                else
                {
                    sort = Builders<T>.Sort.Descending(sortExp);
                }
            }
            if (sort != null)
            {
                option.Sort = sort;
            }

            return option;
        }

    }
}