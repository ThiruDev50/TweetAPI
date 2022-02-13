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
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpGet]
        [Route("GetAllUsers")]

        public JsonResult GetAllusers()
        {
            var mongoClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var database = mongoClient.GetDatabase("TweetDB");
            IMongoCollection<AllUsers> _collection = database.GetCollection<AllUsers>("UsersInfo");
            var condition = Builders<AllUsers>.Filter.Empty;
            var fields = Builders<AllUsers>.Projection.Include("UserId").Include("UserName").Include("MailId").Include("Gender").Include("ProfilePictureBase64").Include("UserBio");
            var results = _collection.Find(condition).Project<AllUsers>(fields).ToList().AsQueryable();
            return new JsonResult(results);
        }
        [HttpPost]

        public JsonResult Post(Register register)
        {
            try
            {
                MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));

                int LastUserId = dbClient.GetDatabase("TweetDB").GetCollection<Register>("UsersInfo").AsQueryable().Count();
                register.UserId = LastUserId + 1;
                dbClient.GetDatabase("TweetDB").GetCollection<Register>("UsersInfo").InsertOne(register);

                return new JsonResult("Added successfully");
            }
            //Exception
            catch(Exception ex)
            {
                return new JsonResult(ex);
            }
            

        }
        [HttpPut]
        public JsonResult Put(Register register)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var filter = Builders<Register>.Filter.Eq("MailId", register.MailId);
            var update = Builders<Register>.Update.Set("Password", register.Password).Set("ContactNumber", register.ContactNumber).Set("ProfilePictureBase64", register.ProfilePictureBase64).Set("UserName", register.UserName).Set("UserId", register.UserId).Set("Gender", register.Gender).Set("MailId", register.MailId).Set("UserBio", register.UserBio);
            dbClient.GetDatabase("TweetDB").GetCollection<Register>("UsersInfo").UpdateOne(filter, update);

            return new JsonResult("Updated successfully");

        }
        [HttpDelete]
        public JsonResult Delete(Register login)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
            var filter = Builders<Register>.Filter.Eq("MailId", login.MailId);

            dbClient.GetDatabase("Tweet").GetCollection<Register>("UsersInfo").DeleteOne(filter);

            return new JsonResult("Deleted successfully");

        }
    }
}

