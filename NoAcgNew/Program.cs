using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NoAcgNew
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var encoding = Console.OutputEncoding;
            Console.OutputEncoding = new System.Text.UTF8Encoding(false);

            CreateHostBuilder(args).Build().Run();

            Console.OutputEncoding = encoding;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Cache");
                    webBuilder.UseStartup<Startup>().ConfigureAppConfiguration(
                        o =>
                        {
                            foreach (var jsonFilename in Directory.EnumerateFiles(
                                Directory.GetCurrentDirectory() + "\\Setting", "*.json",
                                SearchOption.AllDirectories))
                                o.AddJsonFile(jsonFilename, optional: true, reloadOnChange: true);
                        });
                });
    }
}