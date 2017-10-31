using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator;

using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class DomainTypeGeneratorTests
    {
        [Test]
        public void GenerateTest()
        {
            var classes = new List<ClassDescriptor>
            {
                new RowClassDescriptor
                {
                    Name = "xdkz_grbs01__aydkz_______f__rw",
                    Properties =
                        {
                            new PropertyDescriptor { Name = "ID", DataType = typeof(int), Required = true },
                            new PropertyDescriptor { Name = "IntField", DataType = typeof(int), Required = true },
                            new PropertyDescriptor { Name = "IntNullable", DataType = typeof(int), Required = false },
                            new PropertyDescriptor { Name = "StringField", DataType = typeof(string), Required = true },
                            new PropertyDescriptor { Name = "StringNullable", DataType = typeof(string), Required = false },
                            new PropertyDescriptor { Name = "DecimalField", DataType = typeof(decimal), Required = true },
                            new PropertyDescriptor { Name = "DecimalNullable", DataType = typeof(decimal), Required = false },
                            new PropertyDescriptor { Name = "DateTimeField", DataType = typeof(DateTime), Required = true },
                            new PropertyDescriptor { Name = "DateTimeNullable", DataType = typeof(DateTime), Required = false },
                        }
                }
            };

            var assembly = FormsDomainGenerator.Assembly("Test", classes);

            var type = assembly.GetType("Krista.FM.Domain.xdkz_grbs01__aydkz_______f__rw");
            Assert.NotNull(type);

            var inst = assembly.CreateInstance(type.FullName);
            Assert.NotNull(inst);
            Assert.AreEqual("Krista.FM.Domain.xdkz_grbs01__aydkz_______f__rw", inst.GetType().FullName);

            object value = inst.GetType().GetProperty("ID", BindingFlags.Instance | BindingFlags.Public, null, typeof(int), Type.EmptyTypes, null).GetValue(inst, BindingFlags.GetProperty, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(0, value);

            value = inst.GetType().GetProperty("IntNullable").GetValue(inst, BindingFlags.GetProperty, null, null, CultureInfo.CurrentCulture);
            Assert.IsNull(value);

            value = inst.GetType().GetProperty("StringField").GetValue(inst, BindingFlags.GetProperty, null, null, CultureInfo.CurrentCulture);
            Assert.IsNull(value);
        }
    }
}
