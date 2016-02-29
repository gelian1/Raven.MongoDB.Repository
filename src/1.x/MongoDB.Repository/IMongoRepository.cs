﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DB.Repository;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Repository.IEntity;
using System.Linq.Expressions;

namespace MongoDB.Repository
{
    public interface IMongoRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    {
        /// <summary>
        /// 插入
        /// 
        /// </summary>
        /// <param name="entity"/>
        TEntity Insert(TEntity entity, bool isCreateIncID = true);

        /// <summary>
        /// 批量插入
        /// 
        /// </summary>
        /// <param name="entitys">实体对象集合</param><param name="isCreateIncID">true|进行自增操作;false|不做自增操作</param>
        /// <returns/>
        bool InsertBatch(IEnumerable<TEntity> entitys, bool isCreateIncID = true);

        void Update(Expression<Func<TEntity, bool>> filterPredicate, Func<UpdateBuilder<TEntity>, IMongoUpdate> updatePredicate);

        void Update(Expression<Func<TEntity, bool>> filterPredicate, Func<UpdateBuilder<TEntity>, IMongoUpdate> updatePredicate, UpdateFlags flag);

        /// <summary>
        /// 修改
        /// 
        /// </summary>
        /// <param name="query">查询条件</param><param name="update">更新内容</param><param name="flag">更新方式</param>
        void Update(IMongoQuery query, IMongoUpdate update, UpdateFlags flag = 0);

        /// <summary>
        /// 删除
        /// 
        /// </summary>
        /// <param name="filterPredicate">查询表达式</param>
        void Delete(Expression<Func<TEntity, bool>> filterPredicate);

        /// <summary>
        /// 查询数量
        /// 
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns/>
        long Count(IMongoQuery query);

        /// <summary>
        /// 是否存在
        /// 
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns/>
        bool IsExists(IMongoQuery query);

        /// <summary>
        /// 获取支持linq操作的collection集合
        /// 
        /// </summary>
        /// 
        /// <returns/>
        IQueryable<TEntity> QueryableCollection();

        /// <summary>
        /// 获取数据
        /// 
        /// </summary>
        /// <param name="filterPredicate">筛选条件</param><param name="orderby">排序条件</param><param name="includeProperties"/>
        /// <returns/>
        IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> filterPredicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null);

        /// <summary>
        /// 获取数据(只获取部分字段)
        /// 
        /// </summary>
        /// <param name="filterPredicate">筛选条件</param><param name="fieldPredicate">取需求字段匿名类</param><param name="orderby">排序条件</param><param name="includeProperties">包含virtual字段</param>
        /// <returns/>
        IQueryable<TResult> GetList<TResult>(Expression<Func<TEntity, bool>> filterPredicate, Expression<Func<TEntity, TResult>> fieldPredicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null);

        /// <summary>
        /// 获取数据（分页）
        /// 
        /// </summary>
        /// <param name="filterPredicate">筛选条件</param><param name="orderby">排序条件</param><param name="pageIndex">页码</param><param name="pageSize">每页数据数</param><param name="includeProperties">包含virtual字段</param>
        /// <returns/>
        IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> filterPredicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 获取数据（分页）
        /// 
        /// </summary>
        /// <param name="filterPredicate">筛选条件</param><param name="fieldPredicate">取需求字段匿名类</param><param name="orderby">排序条件</param><param name="pageIndex">页码</param><param name="pageSize">每页数据数</param><param name="includeProperties">包含virtual字段</param>
        /// <returns/>
        IQueryable<TResult> GetList<TResult>(Expression<Func<TEntity, bool>> filterPredicate, Expression<Func<TEntity, TResult>> fieldPredicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby, int pageIndex = 1, int pageSize = 10);

        TEntity FindAndModify(Expression<Func<TEntity, bool>> filterPredicate, Func<UpdateBuilder<TEntity>, IMongoUpdate> updatePredicate, params Expression<Func<TEntity, object>>[] sortBy);

        /// <summary>
        /// 找到并更新
        /// 
        /// </summary>
        /// <param name="filterPredicate"/><param name="update"/><param name="sortBy">默认倒叙</param>
        /// <returns/>
        TEntity FindAndModify(Expression<Func<TEntity, bool>> filterPredicate, IMongoUpdate update, params Expression<Func<TEntity, object>>[] sortBy);
    }
}
