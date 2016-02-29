﻿using Repository.IEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repository
{
    /// <summary>
    /// 仓储模式接口
    /// 
    /// </summary>
    /// <typeparam name="TEntity">实体类型泛型</typeparam><typeparam name="TKey">Key类型泛型</typeparam>
    /// <remarks>
    /// add by liangyi on 2012/10/26
    /// </remarks>
    public interface IRepository<TEntity, TKey> where TEntity : IEntity<TKey>, new()
    {
        /// <summary>
        /// 根据id获取实体
        /// 
        /// </summary>
        /// <param name="id"/>
        /// <returns/>
        TEntity Get(TKey id);

        /// <summary>
        /// 插入
        /// 
        /// </summary>
        /// <param name="entity"/>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 全部实体更新
        /// 
        /// </summary>
        /// <param name="entityToUpdate">更新的实体</param>
        TEntity Update(TEntity entityToUpdate);

        /// <summary>
        /// 删除
        /// 
        /// </summary>
        /// <param name="id"/>
        void Delete(TKey id);

        /// <summary>
        /// 删除
        /// 
        /// </summary>
        /// <param name="entityToDelete"/>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// 返回数量
        /// 
        /// </summary>
        /// <param name="filterPredicate"/>
        /// <returns/>
        long Count(Expression<Func<TEntity, bool>> filterPredicate = null);

        /// <summary>
        /// 是否存在
        /// 
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns/>
        bool IsExists(Expression<Func<TEntity, bool>> filterPredicate);
    }

}
