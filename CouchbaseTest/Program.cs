using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using System;
using System.Collections.Generic;

namespace CouchbaseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var configuration = new ClientConfiguration 
            { 
                Servers = new List<Uri>{new Uri("http://localhost:8091/")}
            };
            var authenticator = new PasswordAuthenticator ("", "");
            ClusterHelper.Initialize(configuration, authenticator);

            var bucket = ClusterHelper.GetBucket("EphemeralBucket");

            bucket.Upsert("my-key", "my-value", TimeSpan.FromMinutes(5));

            
        }
    }
}
