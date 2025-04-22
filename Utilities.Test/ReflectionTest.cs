using Utilities.Extensions;

namespace Utilities.Test
{
    public class ReflectionTestClass
    {
        public string StringProperty { get; set; } = "";
        public bool BoolProperty { get; set; }
        public int IntProperty { get; set; }
        public double DoubleProperty { get; set; }
    }

    [TestClass]
    public class ReflectionTest
    {
        [TestMethod]
        public void TestUpdateTicket()
        {
            var reflectionObject1 = new ReflectionTestClass();
            var reflectionObject2 = new ReflectionTestClass
            {
                StringProperty = "string",
                BoolProperty = true,
                IntProperty = 7,
                DoubleProperty = 2.5,
            };

            Assert.AreNotEqual(reflectionObject1.StringProperty, reflectionObject2.StringProperty);
            Assert.AreNotEqual(reflectionObject1.BoolProperty, reflectionObject2.BoolProperty);
            Assert.AreNotEqual(reflectionObject1.IntProperty, reflectionObject2.IntProperty);
            Assert.AreNotEqual(reflectionObject1.DoubleProperty, reflectionObject2.DoubleProperty);
            reflectionObject1.Update(reflectionObject2);
            Assert.AreEqual(reflectionObject1.StringProperty, reflectionObject2.StringProperty);
            Assert.AreEqual(reflectionObject1.BoolProperty, reflectionObject2.BoolProperty);
            Assert.AreEqual(reflectionObject1.IntProperty, reflectionObject2.IntProperty);
            Assert.AreEqual(reflectionObject1.DoubleProperty, reflectionObject2.DoubleProperty);
        }
    }
}
