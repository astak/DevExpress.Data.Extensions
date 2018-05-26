using System;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using NUnit.Framework;

namespace DevExpress.Data.Extensions.Tests {
    [TestFixture]
    public class FunctionOperatorsTests {
        [Test]
        public void CustomTest() {
            using (new CustomFunctionContext(new CustomFunction())) {
                string expected = "CustomFunctionName([Name], 'John')";
                string actual = CriteriaBuilder.Build<TestContext, string>(c => CustomFunctions.CustomFunctionName(c.Name, "John")).ToString();
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CustomNotRegisteredTest() {
            Assert.Throws<NotSupportedException>(() => 
            CriteriaBuilder.Build<TestContext, string>(c => CustomFunctions.CustomNotRegistered(c.Name, "John")));
        }

        [Test]
        public void ConcatTest() {
            string expected = "Concat([Name], 'o%')";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => string.Concat(c.Name, "o%")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ConcatManyTest() {
            string expected = "Concat([Name], 'a', 'b')";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => string.Concat(c.Name, "a", "b")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ContainsTest() {
            string expected = "Contains([Name], 'J')";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name.Contains("J")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EndsWithTest() {
            string expected = "EndsWith([Name], 'n')";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name.EndsWith("n")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IifTest() {
            string expected = "Iif([Name] = 'Bob', 1, [Name] = 'Dan', 2, [Name] = 'Sam', 3, 4)";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.Name == "Bob" ? 1 : 
                c.Name == "Dan" ? 2 : 
                c.Name == "Sam" ? 3 : 4).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LenTest() {
            string expected = "Len([Name]) > 3";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name.Length > 3).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LowerTest() {
            string expected = "Lower([Name]) Like '%jo%'";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => CustomFunctions.Like(c.Name.ToLower(), "%jo%")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StartsWithTest() {
            string expected = "StartsWith([Name], 'J')";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name.StartsWith("J")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SubstringTest() {
            string expected = "Substring([Name], 0, 2) = 'Bo'";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name.Substring(0, 2) == "Bo").ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SubstringTwoOperandsTest() {
            string expected = "Substring([Name], 2) = 'b'";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name.Substring(2) == "b").ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStrTest() {
            string expected = "ToStr([ZipCode]) = '14127'";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.ZipCode.ToString() == "14127").ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStrConvertTest() {
            string expected = "ToStr([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => Convert.ToString(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TrimTest() {
            string expected = "Trim([Name]) = 'Bob'";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => c.Name.Trim() == "Bob").ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TrimOperandsNotSupportedTest() {
            Assert.Throws<NotSupportedException>(() => CriteriaBuilder.Build<TestContext, bool>(c => c.Name.Trim('b') == "o"));
        }

        [Test]
        public void UpperTest() {
            string expected = "Upper([Name]) Like '%JO%'";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => CustomFunctions.Like(c.Name.ToUpper(), "%JO%")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AbsTest() {
            string expected = "Abs([Roles])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => Math.Abs(c.Roles)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AcosTest() {
            string expected = "Acos([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Acos(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddDaysTest() {
            string expected = "AddDays([RegistrationDate], 1.0)";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.AddDays(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddHoursTest() {
            string expected = "AddHours([RegistrationDate], 1.0)";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.AddHours(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddMillisecondsTest() {
            string expected = "AddMilliSeconds([RegistrationDate], 1.0)";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.AddMilliseconds(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddMinutesTest() {
            string expected = "AddMinutes([RegistrationDate], 1.0)";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.AddMinutes(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddMonthsTest() {
            string expected = "AddMonths([RegistrationDate], 1)";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.AddMonths(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddSecondsTest() {
            string expected = "AddSeconds([RegistrationDate], 1.0)";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.AddSeconds(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddTicksTest() {
            string expected = "AddTicks([RegistrationDate], 1L)";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.AddTicks(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddTimeSpanTest() {
            string expected = "AddTimeSpan([RegistrationDate], #00:00:01#)";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.Add(TimeSpan.FromSeconds(1))).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddYearsTest() {
            string expected = "AddYears([RegistrationDate], 1)";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.AddYears(1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AsciiTest() {
            string expected = "Ascii([Name])";
            string actual = CriteriaBuilder.Build<TestContext, byte>(c => CustomFunctions.Ascii(c.Name)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AsinTest() {
            string expected = "Asin([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Asin(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AtnTest() {
            string expected = "Atn([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Atan(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Atn2Test() {
            string expected = "Atn2([Angle], 1.2)";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Atan2(c.Angle, 1.2)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BigMulTest() {
            string expected = "BigMul([Age], 2)";
            string actual = CriteriaBuilder.Build<TestContext, long>(c => Math.BigMul(c.Age, 2)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CeilingTest() {
            string expected = "Ceiling([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Ceiling(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CharTest() {
            string expected = "Char([Age])";
            string actual = CriteriaBuilder.Build<TestContext, char>(c => (char)c.Age).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CharFunctionTest() {
            string expected = "Char([Age])";
            string actual = CriteriaBuilder.Build<TestContext, char>(c => Convert.ToChar(c.Age)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CharIndexTest() {
            string expected = "CharIndex('o', [Name])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.Name.IndexOf("o")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CharIndexStartSpecifiedTest() {
            string expected = "CharIndex('o', [Name], 2)";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.Name.IndexOf("o", 2)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CharIndexNumberOfCharactersSpecifiedTest() {
            string expected = "CharIndex('o', [Name], 2, 3)";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.Name.IndexOf("o", 2, 3)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CosTest() {
            string expected = "Cos([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Cos(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CoshTest() {
            string expected = "Cosh([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Cosh(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExpTest() {
            string expected = "Exp([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Exp(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FloorTest() {
            string expected = "Floor([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Floor(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetDateTest() {
            string expected = "GetDate([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, DateTime>(c => c.RegistrationDate.Date).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetDayTest() {
            string expected = "GetDay([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.RegistrationDate.Day).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetDayOfWeekTest() {
            string expected = "GetDayOfWeek([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, DayOfWeek>(c => c.RegistrationDate.DayOfWeek).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetDayOfYearTest() {
            string expected = "GetDayOfYear([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.RegistrationDate.DayOfYear).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetHourTest() {
            string expected = "GetHour([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.RegistrationDate.Hour).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetMilliSecondTest() {
            string expected = "GetMilliSecond([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.RegistrationDate.Millisecond).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetMinuteTest() {
            string expected = "GetMinute([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.RegistrationDate.Minute).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetMonthTest() {
            string expected = "GetMonth([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.RegistrationDate.Month).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetSecondTest() {
            string expected = "GetSecond([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.RegistrationDate.Second).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetTimeOfDayTest() {
            string expected = "GetTimeOfDay([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, TimeSpan>(c => c.RegistrationDate.TimeOfDay).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetYearTest() {
            string expected = "GetYear([RegistrationDate])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => c.RegistrationDate.Year).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InsertTest() {
            string expected = "Insert([Name], 1, 'a')";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name.Insert(1, "a")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsNullOrEmptyTest() {
            string expected = "IsNullOrEmpty([Name])";
            string actual = CriteriaBuilder.Build<TestContext, bool>(c => string.IsNullOrEmpty(c.Name)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LogTest() {
            string expected = "Log([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Log(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Log10Test() {
            string expected = "Log10([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Log10(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MaxTest() {
            string expected = "Max([Angle], 0.5)";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Max(c.Angle, .5)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MinTest() {
            string expected = "Min([Angle], 0.5)";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Min(c.Angle, .5)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PadLeftTest() {
            string expected = "PadLeft([Name], 10)";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name.PadLeft(10)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PadLeftCharacterTest() {
            string expected = "PadLeft([Name], 10, '\u0066'c)";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name.PadLeft(10, '\u0066')).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PadRightTest() {
            string expected = "PadRight([Name], 10)";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name.PadRight(10)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PadRightCharacterTest() {
            string expected = "PadRight([Name], 10, '\u0066'c)";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name.PadRight(10, '\u0066')).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PowerTest() {
            string expected = "Power([Angle], 2.0)";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Pow(c.Angle, 2)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RemoveTest() {
            string expected = "Remove([Name], 2)";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name.Remove(2)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RemoveWithNumberToTest() {
            string expected = "Remove([Name], 2, 1)";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name.Remove(2, 1)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReplaceTest() {
            string expected = "Replace([Name], 'oh', '**')";
            string actual = CriteriaBuilder.Build<TestContext, string>(c => c.Name.Replace("oh", "**")).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RoundTest() {
            string expected = "Round([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Round(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RoundWithDecimalPlacesTest() {
            string expected = "Round([Angle], 2)";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Round(c.Angle, 2)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SignTest() {
            string expected = "Sign([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => Math.Sign(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SinTest() {
            string expected = "Sin([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Sin(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SinhTest() {
            string expected = "Sinh([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Sinh(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SqrTest() {
            string expected = "Sqr([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Sqrt(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TanTest() {
            string expected = "Tan([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Tan(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TanhTest() {
            string expected = "Tanh([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Math.Tanh(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToDecimalTest() {
            string expected = "ToDecimal([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, decimal>(c => Convert.ToDecimal(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToDecimalCastTest() {
            string expected = "ToDecimal([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, decimal>(c => (decimal)c.Angle).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToDoubleTest() {
            string expected = "ToDouble([Age])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => Convert.ToDouble(c.Age)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToDoubleCastTest() {
            string expected = "ToDouble([Age])";
            string actual = CriteriaBuilder.Build<TestContext, double>(c => (double)c.Age).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToFloatTest() {
            string expected = "ToFloat([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, float>(c => Convert.ToSingle(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToFloatCastTest() {
            string expected = "ToFloat([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, float>(c => (float)c.Angle).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToIntTest() {
            string expected = "ToInt([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => Convert.ToInt32(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToIntCastTest() {
            string expected = "ToInt([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, int>(c => (int)c.Angle).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToLongTest() {
            string expected = "ToLong([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, long>(c => Convert.ToInt64(c.Angle)).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToLongCastTest() {
            string expected = "ToLong([Angle])";
            string actual = CriteriaBuilder.Build<TestContext, long>(c => (long)c.Angle).ToString();
            Assert.AreEqual(expected, actual);
        }
    }

    public sealed class CustomFunction : ICustomFunctionOperator {
        public const string Name = "CustomFunctionName";

        string ICustomFunctionOperator.Name {
            get { return Name; }
        }

        object ICustomFunctionOperator.Evaluate(params object[] operands) {
            throw new NotImplementedException();
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands) {
            return typeof(string);
        }
    }

    public static class CustomFunctions {
        public static string CustomFunctionName(string arg1, string arg2) {
            throw new NotImplementedException();
        }

        public static string CustomNotRegistered(string arg1, string arg2) {
            throw new NotImplementedException();
        }

        public static bool Like(string arg1, string arg2) {
            throw new NotImplementedException();
        }

        public static byte Ascii(string arg1) {
            throw new NotImplementedException();
        }
    }

    public sealed class CustomFunctionContext : IDisposable {
        private string CustomFunctionName;
        
        public CustomFunctionContext(ICustomFunctionOperator customFunction) {
            CustomFunctionName = customFunction.Name;
            CriteriaOperator.RegisterCustomFunction(customFunction);
        }
        void IDisposable.Dispose() {
            CriteriaOperator.UnregisterCustomFunction(CustomFunctionName);
        }
    }
}
