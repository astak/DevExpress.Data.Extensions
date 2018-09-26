using System.Linq;
using NUnit.Framework;

namespace DevExpress.Data.Extensions.Tests {
    [TestFixture]
    public class BinaryOperatorTests {
        [Test]
        public void PropertyTest() {
            string expected = "[Name]";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ComplexPropertyTest() {
            string expected = "[MyReferenceObject.ReferenceObjectName]";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.MyReferenceObject.ReferenceObjectName).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ValueTest() {
            string expected = "'test'";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => "test").ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EqualTest() {
            string expected = "[Name] = 'John'";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name == "John").ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NotEqualTest() {
            string expected = "[Name] <> 'John'";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name != "John").ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GreaterTest() {
            string expected = "[Age] > 20";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Age > 20).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GreaterOrEqualTest() {
            string expected = "[Age] >= 20";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Age >= 20).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LessTest() {
            string expected = "[Age] < 20";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Age < 20).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LessOrEqualTest() {
            string expected = "[Age] <= 20";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Age <= 20).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BitwiseAndTest() {
            string expected = "[Roles] & 1 = 1";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => (c.Roles & 1) == 1).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BitwiseOrTest() {
            string expected = "[Roles] | 253 = 255";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => (c.Roles | 253) == 255).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BitwiseXorTest() {
            string expected = "[Roles] ^ 253 = 255";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => (c.Roles ^ 253) == 255).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MinusTest() {
            string expected = "[Age] - 30 > 0";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Age - 30 > 0).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PlusTest() {
            string expected = "[Age] + 50 = 100";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Age + 50 == 100).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DivideTest() {
            string expected = "[Accounts][].Max([Amount]) / [Accounts][].Min([Amount]) > 10.0m";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => 
                c.Accounts.Max(a => a.Amount) / c.Accounts.Min(a => a.Amount) > 10).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MultipleTest() {
            string expected = "[Accounts][].Sum([Amount]) * 20.0m >= 3000.0m";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Accounts.Sum(a => a.Amount) * 20 >= 3000).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ModuloTest() {
            string expected = "[Age] % 50 = 0";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Age % 50 == 0).ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}