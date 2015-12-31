using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace find
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "mongodb://localhost";

            IMongoClient client = new MongoClient(connectionString);

            var database = client.GetDatabase("test-db");
            var people = database.GetCollection<BsonDocument>("People");

            var person = new BsonDocument();
            person["FirstName"] = "Mahmood";
            person["LastName"] = "Ramzani";
            person["Age"] = 29;

            var filter=new BsonDocument();

            var cursor = people.Find(filter);
            cursor

            Console.WriteLine(person);

            Console.ReadKey();
        }
    }
}
