using CommandLine;
using Couchbase;
using System;
using System.Collections.Generic;

namespace CouchbaseTest
{
    class Options
    {
        [Option('s', "server", Default ="http://localhost")]
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

        [Option('v', "value", Default = "(empty)")]
        public string Value { get; set; }

        [Option('r', "read", Default = false)]
        public bool Read { get; set; }

        [Option('w', "write", Default = false)]
        public bool Write { get; set; } 

        [Option('d', "delete", Default = false)]
        public bool Delete { get; set; } 
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
            try
            {
                var cluster = Cluster.ConnectAsync(options.Url, options.Username, options.Password).Result;
                var bucket = cluster.BucketAsync(options.Bucket).Result;
                var collection = bucket.DefaultCollection();

                if (options.Write)
                {
                    collection.UpsertAsync(options.Key, options.Value, new Couchbase.KeyValue.UpsertOptions().Expiry(TimeSpan.Parse(options.Timeout))).Wait();
                }
                if (options.Read)
                {
                    var result = collection.GetAsync(options.Key).Result;
                    Console.WriteLine(result.ContentAs<string>());
                }
                if (options.Delete)
                {
                    collection.RemoveAsync(options.Key).Wait();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
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
