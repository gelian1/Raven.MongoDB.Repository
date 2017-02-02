﻿using MongoDB.Repository.Funcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Repository
{
    /// <summary>
    /// Sequence
    /// </summary>
    public class MongoSequence
    {
        /// <summary>
        /// 存储数据的序列
        /// </summary>
        public string SequenceName { get; set; }
        /// <summary>
        /// 对应的Collection名称,默认为_id
        /// </summary>
        public string CollectionName { get; set; }
        /// <summary>
        /// 对应Collection的自增长ID
        /// </summary>
        public string IncrementID { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sequence">序列表名称</param>
        /// <param name="collectionName">集合字段名称</param>
        /// <param name="incrementID">自增长ID字段名称</param>
        public MongoSequence(string sequence, string collectionName, string incrementID)
        {
            SequenceName = sequence;
            CollectionName = collectionName;
            IncrementID = incrementID;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MongoSequence()
        {
            SequenceName = "_Sequence";
            CollectionName = "_id";
            IncrementID = "IncID";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetMongoDBFunctionName()
        {
            return string.Format("{0}For{1}", nameof(FunctionScript.getNextSequence), SequenceName);
        }
    }
}
