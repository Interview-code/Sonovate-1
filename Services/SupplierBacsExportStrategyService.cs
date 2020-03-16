using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Raven.Client.Documents;
using Sonovate.CodeTest.Domain;

namespace Sonovate.CodeTest.Services
{
    internal class SupplierBacsExportStrategyService : BaseExportStrategyService
    {
        private const string NOT_AVAILABLE = "NOT AVAILABLE";

        public override async Task Export<T>(T exportSetting)
        {
            var invoiceTransactions = new InvoiceTransactionRepository();
            var candidateInvoiceTransactions = invoiceTransactions.GetBetweenDates(startDate: exportSetting.StartDate, endDate: exportSetting.EndDate);

            if (!candidateInvoiceTransactions.Any())
            {
                throw new InvalidOperationException(string.Format("No supplier invoice transactions found between dates {0} to {1}", exportSetting.StartDate, exportSetting.EndDate));
            }

            var payments = BuildSupplierBacs(candidateInvoiceTransactions);
            
            await new Task( () => this.BacsExport(payments, BacsExportType.Supplier));
        }

        private List<SupplierBacs> BuildSupplierBacs(IEnumerable<InvoiceTransaction> invoiceTransactions)
        {
            var results = new List<SupplierBacs>();

            var transactionsByCandidateAndInvoiceId = invoiceTransactions.GroupBy(transaction => new
                                                        {
                                                            transaction.InvoiceId,
                                                            transaction.SupplierId
                                                        });

            var candidateRepository = new CandidateRepository();

            foreach (var transactionGroup in transactionsByCandidateAndInvoiceId)
            {
                var candidate = candidateRepository.GetById(transactionGroup.Key.SupplierId);

                if (candidate == null)
                {
                    throw new InvalidOperationException(string.Format("Could not load candidate with Id {0}", transactionGroup.Key.SupplierId));
                }

                var bank = candidate.BankDetails;

                var result = new SupplierBacs()
                {
                    AccountName = bank.AccountName,
                    AccountNumber = bank.AccountNumber,
                    SortCode = bank.SortCode,
                    PaymentAmount = transactionGroup.Sum(invoiceTransaction => invoiceTransaction.Gross),
                    InvoiceReference = string.IsNullOrEmpty(transactionGroup.First().InvoiceRef)
                                        ? NOT_AVAILABLE
                                        : transactionGroup.First().InvoiceRef,
                    PaymentReference = string.Format("SONOVATE{0}",
                    transactionGroup.First().InvoiceDate.GetValueOrDefault().ToString("ddMMyyyy"))
                };

              results.Add(result);
            }

            return results;
        }

    }
}
