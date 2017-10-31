using System;
using System.Text.RegularExpressions;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.Server.Scheme.Tests
{
    [TestFixture]
    public class NewYearTransferServiceTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TransferDBToNewYear_constructor_parameters_invalid()
        {
            NewYearTransferService newYearTransferService = new NewYearTransferService();
            newYearTransferService.TransferDBToNewYear(0);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void TransferDBToNewYear_test_valid_parameters()
        {
            NewYearTransferService newYearTransferService = new NewYearTransferService();
            newYearTransferService.TransferDBToNewYear(2012);
        }
    }
}
