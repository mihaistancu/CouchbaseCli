using CommandLine;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using System;
using System.Collections.Generic;

namespace CouchbaseTest
{
    class Options
    {
        [Option('s', "server", Default ="http://localhost:8091/")]
        public string Url { get; set; }

        [Option('u', "user", Required = true)]
        public string Username { get; set; }
            
        [Option('p', "password", Required = true)]
        public string Password { get; set; }

        [Option('b', "bucket", Required = true)]
        public string Bucket { get; set; }

        [Option('k', "key", Required = true)]
        public string Key { get; set; }

        [Option('t', "timeout", Default = "00:05:00")]
        public string Timeout { get; set; }

        [Option('v', "value", Required = true)]
        public string Value { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {  
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleParseError);
        }

        static void Run(Options options)
        {
            var configuration = new ClientConfiguration 
            { 
                Servers = new List<Uri>{new Uri(options.Url)}
            };
            var authenticator = new PasswordAuthenticator (options.Username, options.Password);
            ClusterHelper.Initialize(configuration, authenticator);

            var bucket = ClusterHelper.GetBucket(options.Bucket);

            bucket.Upsert(options.Key, options.Value, TimeSpan.Parse(options.Timeout));

            var result = bucket.Get<string>(options.Key);

            Console.Write(result.Value);
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}
