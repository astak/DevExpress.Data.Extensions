using System;
using System.Globalization;
using System.Linq;
using DevExpress.Data.Filtering;
using NUnit.Framework;

namespace DevExpress.Data.Extensions.Tests {
    [TestFixture]
    public class MiscellaneousOperatorsTests {
        [Test]
        public void ParentRelationshipTraversalTest() {
            string expected = "[Orders][[^.RegistrationDate] = [Date]]";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Orders.Any(o => c.RegistrationDate == o.Date)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InTest() {
            string expected = "[Name] In ('John', 'Bob', 'Nick')";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => new string[] { "John", "Bob", "Nick" }.Contains(c.Name)).ToString();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void InTest2() {
            string expected = "[MyReferenceObject.ReferenceObjectName] In ('John', 'Bob', 'Nick')";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => new string[] { "John", "Bob", "Nick" }.Contains(c.MyReferenceObject.ReferenceObjectName)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InConstantTest() {
            string expected = "[Name] In ('John', 'Bob', 'Nick')";
            string[] collection = new string[] { "John", "Bob", "Nick" };
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => collection.Contains(c.Name)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ClosureTest() {
            string expected = "[Roles] + 1";
            int addition = 1;
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.Roles + addition).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ImplicitConvertTest() {
            Assert.Throws<NotSupportedException>(() => 
            CriteriaBuilder.Build<TestContext, double>(c => (c.EndDate.Date - c.StartDate.Date).Days));
        }

        [Test]
        public void CoalesceTest() {
            string expected = "Iif([Name] Is Null, 'John', [Name])";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name ?? "John").ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanFromSecondsTest() {
            string expected = "#00:00:01#";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.FromSeconds(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanFromDaysTest() {
            string expected = "#1.00:00:00#";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.FromDays(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanFromHoursTest() {
            string expected = "#01:00:00#";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.FromHours(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanFromMilliSecondsTest() {
            string expected = "#00:00:00.0010000#";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.FromMilliseconds(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanFromMinutesTest() {
            string expected = "#00:01:00#";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.FromMinutes(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanFromTicksTest() {
            string expected = "#00:00:00.0000001#";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.FromTicks(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanMaxValueTest() {
            string expected = "#10675199.02:48:05.4775807#";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.MaxValue).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanMinValueTest() {
            string expected = "#-10675199.02:48:05.4775808#";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.MinValue).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanZeroTest() {
            string expected = "#00:00:00#";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.Zero).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeSpanFromDaysMemberTest() {
            Assert.Throws<NotSupportedException>(() => CriteriaBuilder.Build<TestContext, TimeSpan>(c => TimeSpan.FromDays(c.Age)).ToString());
        }

        [Test]
        public void NewDateTimeTest() {
            string expected = "#2017-09-03#";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => new DateTime(2017, 9, 3)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DateTimeFromBinaryTest() {
            string expected = "#0001-01-01 00:00:00#";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => DateTime.FromBinary(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DateTimeFromFileTimeTest() {
            string expected = "#1601-01-01 03:00:00#";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => DateTime.FromFileTime(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DateTimeFromFileTimeUtcTest() {
            string expected = "#1601-01-01 00:00:00#";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => DateTime.FromFileTimeUtc(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DateTimeFromOADateTest() {
            string expected = "#1899-12-31#";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => DateTime.FromOADate(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DateTimeMaxValueTest() {
            string expected = "#9999-12-31 23:59:59.99999#";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => DateTime.MaxValue).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DateTimeMinValueTest() {
            string expected = "#0001-01-01#";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => DateTime.MinValue).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DateTimeNowTest() {
            ConstantValue now = CriteriaBuilder.Build<TestContext, DateTime>(c => DateTime.Now) as ConstantValue;
            Assert.NotNull(now);
            DateTime nowValue = (DateTime)now.Value;
            Assert.Less(DateTime.Now.AddSeconds(-1), nowValue);
            Assert.GreaterOrEqual(DateTime.Now, nowValue);
        }

        [Test]
        public void DateTimeTodayTest() {
            string expected = string.Format(CultureInfo.InvariantCulture, "#{0:yyyy-MM-dd}#", DateTime.Today);
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => DateTime.Today).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DateTimeUtcNowTest() {
            ConstantValue now = CriteriaBuilder.Build<TestContext, DateTime>(c => DateTime.UtcNow) as ConstantValue;
            Assert.IsNotNull(now);
            DateTime nowValue = (DateTime)now.Value;
            Assert.Less(DateTime.UtcNow.AddSeconds(-1), nowValue);
            Assert.GreaterOrEqual(DateTime.UtcNow, nowValue);
        }

        [Test]
        public void InCollectionMethodTest() {
            string expected = "[Name] In ('John', 'Bob')";
            string actual = CriteriaBuilder.Build<TestContext>(c => GetNamesForContainsTest().Contains(c.Name)).ToString();
            Assert.AreEqual(expected, actual);
        }

        private static string[] GetNamesForContainsTest() {
            return new string[] { "John", "Bob" };
        }
    }
}
