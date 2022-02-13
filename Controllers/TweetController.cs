using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPI.Models;

namespace TweetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public TweetController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        [Route("NewTweet")]

        public JsonResult Post(Tweet tweet)
        {
         
            try
            {
                MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));

                int LastTweetId = dbClient.GetDatabase("TweetDB").GetCollection<Tweet>("Tweets").AsQueryable().Count();
                tweet.TweetId = LastTweetId + 1;
                dbClient.GetDatabase("TweetDB").GetCollection<Tweet>("Tweets").InsertOne(tweet);
                var newLikesUserIdList = new List<string>{"100","101"};
                var filter = Builders<Tweet>.Filter.Eq("TweetId", LastTweetId + 1);
                var update = Builders<Tweet>.Update.Set("TweetLikersUserId", newLikesUserIdList);
                dbClient.GetDatabase("TweetDB").GetCollection<Tweet>("Tweets").UpdateOne(filter, update);
                return new JsonResult("Tweet successfully");
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }
        [HttpGet]
        [Route("AllTweet")]
        public JsonResult GetAllTweet()
        {
            var mongoClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var database = mongoClient.GetDatabase("TweetDB");
            IMongoCollection<Tweet> _collection = database.GetCollection<Tweet>("Tweets");
            var filter = Builders<Tweet>.Filter.Empty;
            var result = _collection.Find(filter).ToList();
            return new JsonResult(result);
        }
        [HttpPost]
        [Route("MyTweet")]
        public JsonResult PostAllMyTweet(Tweet tweet)
        {
            var mongoClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var database = mongoClient.GetDatabase("TweetDB");
            IMongoCollection<Tweet> _collection = database.GetCollection<Tweet>("Tweets");
            var filter = Builders<Tweet>.Filter.Eq("UserId", tweet.UserId);
            var result = _collection.Find(filter).ToList();
            return new JsonResult(result);
        }
        [HttpPut]
        [Route("AddComment")]
        public JsonResult AddingComments(Tweet tweet)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var filter = Builders<Tweet>.Filter.Eq("TweetId", tweet.TweetId);
            var update = Builders<Tweet>.Update.Set("CommentorUserName", tweet.CommentorUserName).Set("CommentorUserId", tweet.CommentorUserId).Set("CommentorContent", tweet.CommentorContent).Set("CommentorProfilePictureBase64", tweet.CommentorProfilePictureBase64);
            dbClient.GetDatabase("TweetDB").GetCollection<Tweet>("Tweets").UpdateOne(filter, update);

            return new JsonResult("Comment Added");

        }
        [HttpPut]
        [Route("EditMyTweet")]
        public JsonResult EditMyTweet(Tweet tweet)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var filter = Builders<Tweet>.Filter.Eq("TweetId", tweet.TweetId);
            var update = Builders<Tweet>.Update.Set("TweetContent", tweet.TweetContent);
            dbClient.GetDatabase("TweetDB").GetCollection<Tweet>("Tweets").UpdateOne(filter, update);

            return new JsonResult("Post Edited");

        }
        [HttpPut]
        [Route("EditMyComment")]
        public JsonResult EditMyComment(Tweet tweet)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var filter = Builders<Tweet>.Filter.Eq("TweetId", tweet.TweetId);
            var update = Builders<Tweet>.Update.Set("CommentorContent", tweet.CommentorContent);
            dbClient.GetDatabase("TweetDB").GetCollection<Tweet>("Tweets").UpdateOne(filter, update);

            return new JsonResult("Comment Edited");

        }
        [HttpDelete]
        [Route("DeleteMyTweet")]

        public JsonResult DeleteTweet(Tweet tweet)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var filter = Builders<Tweet>.Filter.Eq("TweetId", tweet.TweetId);

            dbClient.GetDatabase("TweetDB").GetCollection<Tweet>("Tweets").DeleteOne(filter);

            return new JsonResult("Deleted successfully");

        }
        [HttpPut]
        [Route("Likes")]
        public JsonResult Likes(Tweet tweet)
        {
            var mongoClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var database = mongoClient.GetDatabase("TweetDB");
            IMongoCollection<Tweet> _collection = database.GetCollection<Tweet>("Tweets");
            var condition = Builders<Tweet>.Filter.Eq("TweetId", tweet.TweetId);
            var fields = Builders<Tweet>.Projection.Include("TweetLikersUserId");
            var currentUserId = tweet.UserId;
            var results = _collection.Find(condition).Project<Likes>(fields).ToList().AsQueryable();
            var likesUsersIdList = results.Select(x=>x.TweetLikersUserId).ToList();
            var isAldredyLiked = false;
            for(int i = 0; i < likesUsersIdList[0].Count; i++)
            {
                if(Int32.Parse(likesUsersIdList[0][i])== currentUserId)
                {
                    isAldredyLiked = true;
                    break;
                }
            }
            var newLikesUserIdList = likesUsersIdList[0];
            if (!isAldredyLiked)
            {
                newLikesUserIdList.Add(currentUserId.ToString());
            }
            else
            {
                newLikesUserIdList.Remove(currentUserId.ToString());
            }
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var filter = Builders<Tweet>.Filter.Eq("TweetId", tweet.TweetId);
            var update = Builders<Tweet>.Update.Set("TweetLikersUserId", newLikesUserIdList);
            dbClient.GetDatabase("TweetDB").GetCollection<Tweet>("Tweets").UpdateOne(filter, update);

            return new JsonResult(newLikesUserIdList.Count);
        }
    }
}
