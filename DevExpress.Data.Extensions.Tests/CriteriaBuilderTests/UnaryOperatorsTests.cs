using NUnit.Framework;

namespace DevExpress.Data.Extensions.Tests {
    [TestFixture]
    public class UnaryOperatorsTests {
        [Test]
        public void BitwiseNotTest() {
            string expected = "~ [Roles] = 251";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => ~c.Roles == 251).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MinusTest() {
            string expected = "- [Age] = -20";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => -c.Age == -20).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NotTest() {
            string expected = "Not ([Name] = 'John' Or [Age] = 30)";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => !(c.Name == "John" || c.Age == 30)).ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
