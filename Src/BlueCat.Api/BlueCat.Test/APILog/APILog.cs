using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Repository.IEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.Test
{
    [BsonIgnoreExtraElements]
    public class APILog : IEntity<string>
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        public string APIName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
    }
}
