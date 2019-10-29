using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Repository.IEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.Entity.Mongodb
{
    [BsonIgnoreExtraElements]
    public class User : IAutoIncr<long>
    {
        [BsonId]
        public long ID
        {
            get;
            set;
        }

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

        public string Stamp { get; set; }

        public User()
        {
            CreateTime = DateTime.Now;
        }
    }
}
