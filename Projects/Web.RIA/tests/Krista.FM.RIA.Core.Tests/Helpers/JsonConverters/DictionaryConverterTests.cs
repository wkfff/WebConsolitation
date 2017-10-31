using System.Collections.Generic;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Helpers.JsonConverters;

using Newtonsoft.Json;

using NUnit.Framework;

namespace Krista.FM.RIA.Core.Tests.Helpers.JsonConverters
{
    [TestFixture]
    class DictionaryConverterTests
    {
        private Dictionary<string, object> column;

        [SetUp]
        public void Setup()
        {
            column = new Dictionary<string, object>();
            column.Add("id", "NAME");
            column.Add("width", 150);
            column.Add("sortable", true);
            column.Add("editor", new TextField {AllowBlank = false});
            column.Add("editor2", new NumberField {AllowBlank = true, AllowDecimals = false});
            const string ImgEdit = "+ \"<div style='float: right'><img title='Откр' src='/icons/application_form_edit-png/ext.axd' width='16' " +
                                   "height='16' class='EditRefCell'> </div>\"";
            string funcRender = "function (v, p, r, rowIndex, colIndex, ds) {{s = {0}<div style='float: left'>{0} + v +{0}</div>{0}{1};return s;}}"
                .FormatWith('"', ImgEdit);
            column.Add("renderer", funcRender);
        }

        [Test]
        public void CanConvert()
        {
            bool result = new DictionaryConverter().CanConvert(new Dictionary<string, object>().GetType());
            Assert.AreEqual(result, true);

            List<DataRowConverter> notAttributes = new List<DataRowConverter>();
            result = new DictionaryConverter().CanConvert(notAttributes.GetType());
            Assert.AreEqual(result, false);
        }

        [Test]
        public void WriteJson()
        {
            string result = JSON.Serialize(column, new List<JsonConverter> { new DictionaryConverter() });
            Assert.AreEqual(result, "{  id: 'NAME', width: 150, sortable: true, editor: new Ext.form.TextField({allowBlank: false }), editor2: new Ext.form.NumberField({allowBlank: true, allowDecimals: false }), renderer: function (v, p, r, rowIndex, colIndex, ds) {s = \"<div style='float: left'>\" + v +\"</div>\"+ \"<div style='float: right'><img title='Откр' src='/icons/application_form_edit-png/ext.axd' width='16' height='16' class='EditRefCell'> </div>\";return s;}}");
        }
    }
}
