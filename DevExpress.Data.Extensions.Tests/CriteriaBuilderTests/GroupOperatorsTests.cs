using NUnit.Framework;

namespace DevExpress.Data.Extensions.Tests {
    [TestFixture]
    public class GroupOperatorsTests {
        [Test]
        public void AndTest() {
            string expected = "[Name] = 'John' And [Age] = 30";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name == "John" && c.Age == 30).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OrTest() {
            string expected = "[Name] = 'John' Or [Age] = 30";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name == "John" || c.Age == 30).ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
