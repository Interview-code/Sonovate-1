using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Sonovate.CodeTest.Domain;

namespace Sonovate.CodeTest.Services
{
    internal abstract class BaseExportStrategyService : IBacsExportStartegy
    {
        protected void BacsExport(IEnumerable bacs, BacsExportType type)
        {
            var fileName = string.Format("{0}_BACSExport.csv", type);

            using var csv = new CsvWriter(new StreamWriter(new FileStream(fileName, FileMode.Create)));
            csv.WriteRecords(bacs);
        }

        public abstract Task Export<T>(T exportSetting) where T : IBacsExportSetting;

    }
}
