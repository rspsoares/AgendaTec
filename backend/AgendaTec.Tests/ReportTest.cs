using System;
using System.Linq;
using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using AgendaTec.Business.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgendaTec.Tests
{
    [TestClass]
    public class ReportTest
    {
        private readonly IReportFacade reportFacade;

        public ReportTest()
        {
            reportFacade = new ReportFacade();
        }

        [TestMethod]
        public void Report_ScheduleTest()
        {
            ProfilesHelper.Initialize();
            var result = reportFacade.GetScheduleReport(5, DateTime.Parse("2019-01-01"), DateTime.Parse("2019-12-31"), out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(result.Any());
        }
    }
}
