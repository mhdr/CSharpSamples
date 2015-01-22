using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bulk_Insert
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();

            var database = server.GetDatabase("test-db");
            var people = database.GetCollection("People");
            var bulk = people.InitializeOrderedBulkOperation();

            for (int i = 0; i < 1000; i++)
            {
                var person = new BsonDocument();
                person["FirstName"] = "Mahmood";
                person["LastName"] = "Ramzani";
                person["Age"] = 29;

                bulk.Insert(person);
            }
            
            var result= bulk.Execute();

            Console.WriteLine(result.InsertedCount);

            Console.ReadKey();
        }
    }
}
