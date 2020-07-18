using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class FuckedEncodingHelper
    {
        public static async Task FixFuckedEncoding(Stream source, Stream destination)
        {
            using var sr = new StreamReader(source, Encoding.UTF8);

            var str = await sr.ReadToEndAsync();

            var strFixed = FixFuckedEncoding(str);

            await using var sw = new StreamWriter(destination, Encoding.UTF8);
            await sw.WriteAsync(strFixed);
        }

        public static string FixFuckedEncoding(string src)
        {
            var wind1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252) ??
                           throw new Exception($"Couldn't find '1252' encoding.");

            var bytes = wind1252.GetBytes(src);

            var strFixed = Encoding.UTF8.GetString(bytes);
            return strFixed;
        }
    }
}