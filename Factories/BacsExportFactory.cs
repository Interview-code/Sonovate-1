using System;
using Sonovate.CodeTest.Domain;
using Sonovate.CodeTest.Services;

namespace Sonovate.CodeTest.Factories
{
    internal class BacsExportFactory : IBacsExportFactory
    {
        private const string INVALID_BACS_EXPORT_TYPE = "Invalid BACS Export Type.";


        public IBacsExportStartegy CreateBacsExporter(BacsExportFactorySetting setting)
        {

            return setting.BacsExportType switch
            {
                BacsExportType.None => new InvalidBacsExportStrategyService(),
                BacsExportType.Agency => setting.EnableAgencyPayments ? new AgencyPaymentBacsExportStrategyService() : null,
                BacsExportType.Supplier => new SupplierBacsExportStrategyService(),
                _ => throw new Exception(INVALID_BACS_EXPORT_TYPE),
            };
        }
    }
}
