using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jasily.Awaitablify.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestGenericType()
        {
            var awaitablifier = new Awaitablifier();

            // for null
            Assert.AreEqual(null, await awaitablifier.Awaitablify<string>(null).HasResult<string>());

            // for value type
            Assert.AreEqual(1, await awaitablifier.Awaitablify(1).HasResult<int>());

            // for ref type
            Assert.AreEqual("5", await awaitablifier.Awaitablify("5").HasResult<string>());
            Assert.AreEqual("5", await awaitablifier.Awaitablify("5").HasResult<object>());

            // for value result task
            Assert.AreEqual(1, await awaitablifier.Awaitablify(Task.Run(() => 1)).HasResult<int>());

            // for ref type result task
            Assert.AreEqual("5", await awaitablifier.Awaitablify(Task.Run(() => "5")).HasResult<string>());
            Assert.AreEqual("5", await awaitablifier.Awaitablify(Task.Run(() => "5")).HasResult<object>());
        }

        [TestMethod]
        public async Task TestObject()
        {
            var awaitablifier = new Awaitablifier();

            // for null
            Assert.AreEqual(null, await awaitablifier.Awaitablify(null));

            // for value type
            Assert.AreEqual(1, await awaitablifier.Awaitablify((object)1));

            // for ref type
            Assert.AreEqual("5", await awaitablifier.Awaitablify((object)"5"));

            // for value result task
            Assert.AreEqual(1, await awaitablifier.Awaitablify((object)Task.Run(() => 1)));

            // for ref type result task
            Assert.AreEqual("5", await awaitablifier.Awaitablify((object)Task.Run(() => "5")));

            // for unpack
            var task = Task.Run<Task<Task<string>>>(() => Task.Run<Task<string>>(() => Task.Run(() => "xyz")));
            Assert.AreEqual("xyz", await awaitablifier.UnpackAsync(task));
        }
    }
}
