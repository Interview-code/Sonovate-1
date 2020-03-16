using System;
using Raven.Client.Documents;

namespace Sonovate.CodeTest.Domain
{
    internal class BacsExportSetting : IBacsExportSetting
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IDocumentStore BacsDocumentStore { get; set; }
    }
}
