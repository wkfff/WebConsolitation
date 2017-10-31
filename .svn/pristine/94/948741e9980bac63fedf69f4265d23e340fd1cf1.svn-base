using NUnit.Framework;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class JsonDataSetParserTests
    {
        private string sourceData;
        [SetUp]
        public void Init()
        {
            sourceData = @"{
""Created"":[
        {""ID"":""1"", ""DATA"":""Created1""},
        {""ID"":""2"", ""DATA"":""Created2""},
        {""ID"":""3"", ""DATA"":""Created3""}
    ],
""Updated"":[
        {""ID"":""11"", ""DATA"":""Updated1""},
        {""ID"":""12"", ""DATA"":""Updated2""},
        {""ID"":""13"", ""DATA"":""Updated3""}
    ]
}";
        }

        [Test]
        public void CanParseJsonData_IsNotEmpty()
        {
            var result = JsonDataSetParser.Parse(sourceData);
            Assert.AreEqual(2, result.Count, "Пустой датасет");
        }

        [Test]
        public void CanParseJsonData_FirstTableExists()
        {
            var result = JsonDataSetParser.Parse(sourceData);

            Assert.IsTrue(result.ContainsKey("Created"), "Отсутствует таблица Created");
        }

        [Test]
        public void CanParseJsonData_SecondTableExists()
        {
            var result = JsonDataSetParser.Parse(sourceData);

            Assert.IsTrue(result.ContainsKey("Updated"), "Отсутствует таблица Updated");
        }

        [Test]
        public void CanParseJsonData_FirstTableNotEmpty()
        {
            var result = JsonDataSetParser.Parse(sourceData);

            Assert.AreEqual(3, result["Created"].Count);
        }

        [Test]
        public void CanParseJsonData_SecondTableNotEmpty()
        {
            var result = JsonDataSetParser.Parse(sourceData);

            Assert.AreEqual(3, result["Updated"].Count);
        }

        [Test]
        public void CanParseJsonData_FirstTableContainsFields()
        {
            var result = JsonDataSetParser.Parse(sourceData);

            Assert.IsTrue(result["Created"][0].ContainsKey("ID"));
            Assert.IsTrue(result["Updated"][0].ContainsKey("DATA"));
        }

        [Test]
        public void CanParseJsonData_SecondTableContainsFields()
        {
            var result = JsonDataSetParser.Parse(sourceData);

            Assert.IsTrue(result["Updated"][0].ContainsKey("ID"));
            Assert.IsTrue(result["Updated"][0].ContainsKey("DATA"));
        }

        [Test]
        public void CanParseJsonData_FirstTableContainsValues()
        {
            var result = JsonDataSetParser.Parse(sourceData);

            Assert.AreEqual("2", result["Created"][1]["ID"]);
        }

        [Test]
        public void CanParseJsonData_SecondTableContainsValues()
        {
            var result = JsonDataSetParser.Parse(sourceData);

            Assert.AreEqual("12", result["Updated"][1]["ID"]);
        }
    }
}
