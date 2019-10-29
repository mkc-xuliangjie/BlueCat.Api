using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Repository.IEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.Test
{
    [BsonIgnoreExtraElements]
    public class User2 : IEntity<string>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        public string Name
        {
            get;
            set;
        }

        public int Age
        { get; set; }



        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime
        {
            get;
            set;
        }

        public string Remark { get; set; }

        public User2()
        {
            CreateTime = DateTime.Now;
        }
    }
}
