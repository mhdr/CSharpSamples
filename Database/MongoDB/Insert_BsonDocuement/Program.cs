﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Insert_BsonDocuement
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "mongodb://localhost";

            IMongoClient client=new MongoClient(connectionString);

            var database = client.GetDatabase("test-db");
            var people = database.GetCollection<BsonDocument>("People");

            var person = new BsonDocument();
            person["FirstName"] = "Mahmood";
            person["LastName"] = "Ramzani";
            person["Age"] = 29;

            people.InsertOneAsync(person);

            Console.WriteLine(person);

            Console.ReadKey();
        }
    }
}
