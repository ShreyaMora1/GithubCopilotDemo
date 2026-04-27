using NUnit.Framework;
using RecordUpdatePlugin;

namespace RecordUpdatePlugin.Tests
{
    public class FieldChangePluginTests
    {
        [Test]
        public void UpdateOnFieldChange_WhenValueChanged_CallsServiceAndReturnsTrue()
        {
            var fake = new TestDoubles.FakeRecordService();
            var plugin = new FieldChangePlugin(fake);

            var result = plugin.UpdateOnFieldChange("rec1", "status", "open", "closed");

            Assert.IsTrue(result);
            Assert.AreEqual("rec1", fake.LastId);
            Assert.IsNotNull(fake.LastChanges);
            Assert.IsTrue(fake.LastChanges.ContainsKey("status"));
            Assert.AreEqual("closed", fake.LastChanges["status"]);
        }

        [Test]
        public void UpdateOnFieldChange_WhenValueSame_DoesNotCallServiceAndReturnsFalse()
        {
            var fake = new TestDoubles.FakeRecordService();
            var plugin = new FieldChangePlugin(fake);

            var result = plugin.UpdateOnFieldChange("rec1", "status", "open", "open");

            Assert.IsFalse(result);
            Assert.IsNull(fake.LastId);
            Assert.IsNull(fake.LastChanges);
        }
    }
}
