using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class ImportService
    {
        public async Task<ImportResultModel> ImportParticipantsCsv(string str, bool fixFuckedEncoding)
        {
            if (fixFuckedEncoding)
            {
                str = FuckedEncodingHelper.FixFuckedEncoding(str);
            }

            using var strReader = new StringReader(str);
            using var csvReader = new CsvReader(strReader, CultureInfo.InvariantCulture);
            var i = 0;
            var result = new ImportResultModel();
            while (await csvReader.ReadAsync())
            {
                result.Rows++;
                var record = csvReader.GetRecord<dynamic>();
                result.Add(await ImportRow(record));
                if (i++ > 10)
                {
                    break;
                }
            }

            return result;
        }

        private async Task<ImportResultModel> ImportRow(dynamic recordRow)
        {
            var result = new ImportResultModel();
            var id = recordRow.Id;
            var county = recordRow.County;
            Console.WriteLine(id + " => " + county);
            result.Added = 1;
            return result;
        }
    }
}