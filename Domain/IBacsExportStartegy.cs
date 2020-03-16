using System;
using System.Threading.Tasks;

namespace Sonovate.CodeTest.Domain
{
    public interface IBacsExportStartegy
    {
        Task Export<T>(T exportSetting) where T : IBacsExportSetting;
    }
}
