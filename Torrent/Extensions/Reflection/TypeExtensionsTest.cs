namespace Torrent.Extensions
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using static Helpers.Utils.EnumUtils;
    public static partial class TypeExtensions
    {
        [TestFixture]
        public class TypeHelperTest
        {
            [Test]
            public void TestGetFriendlyName()
            {
                Assert.AreEqual("string", typeof (string).GetFriendlyName());
                Assert.AreEqual("int[]", typeof (int[]).GetFriendlyName());
                Assert.AreEqual("int[][]", typeof (int[][]).GetFriendlyName());
                Assert.AreEqual("KeyValuePair<int, string>", typeof (KeyValuePair<int, string>).GetFriendlyName());
                Assert.AreEqual("Tuple<int, string>", typeof (Tuple<int, string>).GetFriendlyName());
                Assert.AreEqual("Tuple<KeyValuePair<object, long>, string>",
                                typeof (Tuple<KeyValuePair<object, long>, string>).GetFriendlyName());
                Assert.AreEqual("List<Tuple<int, string>>", typeof (List<Tuple<int, string>>).GetFriendlyName());
                Assert.AreEqual("Tuple<short[], string>", typeof (Tuple<short[], string>).GetFriendlyName());
            }
        }
    }
}
