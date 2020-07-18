using System;
using System.IO;
using System.Text;
using InfoEducatie.Main.InfoEducatieAdmin;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfoEducatie.Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // var dir = "/home/nm/Desktop/";
            // var sourceFile = dir + "infoeducatie.csv";
            // var destFile = dir + "infoeducatie_fixed.csv";
            //
            // using var fsr = new FileStream(sourceFile, FileMode.Open);
            // using var sr = new StreamReader(fsr, Encoding.UTF8);
            // var str = sr.ReadToEnd();
            // var strFixed = FuckedEncodingHelper.FixFuckedEncoding(str);
            //
            // using var fsw = new FileStream(destFile, FileMode.Create);
            // using var sw = new StreamWriter(fsw);
            // sw.Write(strFixed);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Env.LoadEnvFiles();
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}