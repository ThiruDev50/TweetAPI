using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace TweetAPI.Models
{
    public class AllUsers
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string MailId { get; set; }
        public string Gender { get; set; }
        public string ProfilePictureBase64 { get; set; }
        public string UserBio { get; set; }



    }
}
