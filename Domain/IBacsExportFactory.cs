using System;
namespace Sonovate.CodeTest.Domain
{
    public interface IBacsExportFactory
    {
        IBacsExportStartegy CreateBacsExporter(BacsExportFactorySetting setting);
    }
}
