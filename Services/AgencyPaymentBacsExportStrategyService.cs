using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sonovate.CodeTest.Domain;

namespace Sonovate.CodeTest.Services
{
    internal class AgencyPaymentBacsExportStrategyService : BaseExportStrategyService
    {
        public override async Task Export<T>(T exportSetting)
        {
            var payments = await GetAgencyPayments(exportSetting);
            this.BacsExport(payments, BacsExportType.Agency);
        }


        private async Task<List<BacsResult>> GetAgencyPayments(IBacsExportSetting setting)
        {
            var paymentRepository = new PaymentsRepository();
            var payments = paymentRepository.GetBetweenDates(start: setting.StartDate, end: setting.EndDate);

            if (!payments.Any())
            {
                throw new InvalidOperationException(string.Format("No agency payments found between dates {0:dd/MM/yyyy} to {1:dd/MM/yyyy}", setting.StartDate, setting.EndDate));
            }

            var agencyIds = payments.Select(x => x.AgencyId).Distinct().ToList();

            using (var session = setting.BacsDocumentStore.OpenAsyncSession())
            {
                var agencies = (await session.LoadAsync<Agency>(agencyIds)).Values.ToList();
                return BuildAgencyPaymentsBacs(payments, agencies);
            }
        }



        private List<BacsResult> BuildAgencyPaymentsBacs(IEnumerable<Payment> payments, List<Agency> agencies)
        {
            return (from p in payments
                    let agency = agencies.FirstOrDefault(x => x.Id == p.AgencyId)
                    where agency != null && agency.BankDetails != null
                    let bank = agency.BankDetails
                    select new BacsResult
                    {
                        AccountName = bank.AccountName,
                        AccountNumber = bank.AccountNumber,
                        SortCode = bank.SortCode,
                        Amount = p.Balance,
                        Ref = string.Format("SONOVATE{0}", p.PaymentDate.ToString("ddMMyyyy"))
                    }).ToList();
        }
    }
}
