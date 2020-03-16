using System;
using System.Threading.Tasks;

namespace Sonovate.CodeTest.Services
{
    internal class InvalidBacsExportStrategyService : BaseExportStrategyService
    {
        private const string INVALID_EXPORT_TYPE_MESSAGE = "No export type provided.";

        public override Task Export<T>(T exportParameter) => throw new Exception(INVALID_EXPORT_TYPE_MESSAGE);
    }
}
