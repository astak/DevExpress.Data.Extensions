using NUnit.Framework;
using System.Linq;

namespace DevExpress.Data.Extensions.Tests {
    [TestFixture]
    public class AggregationOperatorsTest {
        [Test]
        public void AverageTest() {
            string expected = "[Accounts][].Avg([Amount]) = 75.0m";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.Average(a => a.Amount) == 75).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CountTest() {
            string expected = "[Accounts][].Count() > 1";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.Count() > 1).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExistsTest() {
            string expected = "[Accounts][]";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.Any()).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MaximumTest() {
            string expected = "[Accounts][].Max([Amount]) > 75.0m";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.Max(a => a.Amount) > 75).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MinimumTest() {
            string expected = "[Accounts][].Min([Amount]) < 10.0m";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.Min(a => a.Amount) < 10).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SumTest() {
            string expected = "[Accounts][].Sum([Amount]) > 150.0m";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.Sum(a => a.Amount) > 150).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SingleTest() {
            string expected = "[Accounts][].Single() <> null";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.SingleOrDefault() != null).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SingleWithConditionTest() {
            string expected = "[Accounts][[Amount] > 10.0m].Single()";
            string actual = CriteriaBuilder.Build<TestContext, Account>(c => c.Accounts.SingleOrDefault(a => a.Amount > 10)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SingleWithExpressionTest() {
            string expected = "[Accounts][].Single([Amount])";
            string actual = CriteriaBuilder.Build<TestContext, decimal>(c => c.Accounts.SingleOrDefault().Amount).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AggregationWithConditionTest() {
            string expected = "[Accounts][[Amount] > 10.0m].Sum([Amount])";
            string actual = CriteriaBuilder.Build<TestContext, decimal>(c => c.Accounts.Where(a => a.Amount > 10)
                .Sum(a => a.Amount)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExistsWithConditionTest() {
            string expected = "[Accounts][[Amount] > 10.0m]";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.Any(a => a.Amount > 10)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CountWithConditionTest() {
            string expected = "[Accounts][[Amount] > 10.0m].Count() > 1";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.Count(a => a.Amount > 10) > 1).ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
