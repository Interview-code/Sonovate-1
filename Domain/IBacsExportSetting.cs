using System;
using Raven.Client.Documents;

namespace Sonovate.CodeTest.Domain
{
    public interface IBacsExportSetting
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        IDocumentStore BacsDocumentStore { get; set; }
    }
}
