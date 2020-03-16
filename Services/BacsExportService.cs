using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Raven.Client.Documents;
using Sonovate.CodeTest.Domain;
using Sonovate.CodeTest.Factories;
using Sonovate.CodeTest.Services;

namespace Sonovate.CodeTest
{

    public class BacsExportService
    {

        private readonly IDocumentStore documentStore;
        private readonly IBacsExportFactory bacsExportFactory;

        /// <summary>
        /// this constructor is helping us to be able to mock some of the dependencies for unit testing as well as future extension will be easier.
        /// </summary>
        /// <param name="documentStore">an instance of IDocumentStore</param>
        /// <param name="bacsExportFactory"></param>
        public BacsExportService(IDocumentStore documentStore , IBacsExportFactory bacsExportFactory)
        {
            this.bacsExportFactory = bacsExportFactory;
            this.documentStore = documentStore;
            this.documentStore.Initialize();
        }

        /// <summary>
        /// this constructor has already being used inside application.cs , so  I can't get rid of it
        /// </summary>
        public BacsExportService()
        {
            documentStore = new DocumentStore {Urls = new[]{"http://localhost"}, Database = "Export"};
            documentStore.Initialize();

            bacsExportFactory = new BacsExportFactory();
        }


        /// <summary>
        /// using strategy design pattern will get the right exporter instance and execute it.
        /// </summary>
        /// <param name="bacsExportType">the type of bacs</param>
        /// <returns>just a task because it is an async method</returns>
        public async Task ExportZip(BacsExportType bacsExportType)
        {
            var factorySetting = new BacsExportFactorySetting()
            {
                 BacsExportType = bacsExportType,
                 EnableAgencyPayments= (Application.Settings["EnableAgencyPayments"] == "true")
            };
           
            try
            {
                var exportStrategy =  bacsExportFactory.CreateBacsExporter(factorySetting);

                var exportSetting = new BacsExportSetting()
                {
                    StartDate = DateTime.Now.AddMonths(-1),
                    EndDate = DateTime.Now,
                    BacsDocumentStore = this.documentStore
                };

                if ( exportStrategy != null)
                   await exportStrategy.Export<BacsExportSetting>(exportSetting);
            }
            catch (InvalidOperationException inOpEx)
            {
                throw new Exception(inOpEx.Message);
            }
        }
    }
}