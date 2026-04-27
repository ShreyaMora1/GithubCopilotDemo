using NUnit.Framework;
using RecordUpdatePlugin;

// Unit tests for FieldChangePlugin.
// These tests exercise the public behavior of UpdateOnFieldChange using a
// lightweight in-process test double (FakeRecordService) so there are no
// external dependencies. Each test follows the Arrange / Act / Assert pattern.
namespace RecordUpdatePlugin.Tests
{
    /// <summary>
    /// Tests for <see cref="FieldChangePlugin"/>.
    /// Tests are intentionally simple and focused on behavior verification,
    /// not integration with external services.
    /// </summary>
    public class FieldChangePluginTests
    {
        [Test]
        public void UpdateOnFieldChange_WhenValueChanged_CallsServiceAndReturnsTrue()
        {
            // Arrange: create a fake service and the plugin under test.
            var fake = new TestDoubles.FakeRecordService();
            var plugin = new FieldChangePlugin(fake);

            // Act: simulate a field change from "open" to "closed".
            var result = plugin.UpdateOnFieldChange("rec1", "status", "open", "closed");

            // Assert: the plugin should return true and the fake service should
            // have received the expected record id and changes dictionary.
            Assert.IsTrue(result);
            Assert.AreEqual("rec1", fake.LastId);
            Assert.IsNotNull(fake.LastChanges);
            Assert.IsTrue(fake.LastChanges.ContainsKey("status"));
            Assert.AreEqual("closed", fake.LastChanges["status"]);
        }

        [Test]
        public void UpdateOnFieldChange_WhenValueSame_DoesNotCallServiceAndReturnsFalse()
        {
            // Arrange
            var fake = new TestDoubles.FakeRecordService();
            var plugin = new FieldChangePlugin(fake);

            // Act: no actual change (old == new)
            var result = plugin.UpdateOnFieldChange("rec1", "status", "open", "open");

            // Assert: no update should be attempted and the fake should have no recorded call.
            Assert.IsFalse(result);
            Assert.IsNull(fake.LastId);
            Assert.IsNull(fake.LastChanges);
        }

        [Test]
        public void UpdateOnFieldChange_NullRecordId_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null recordId should throw.
            var fake = new TestDoubles.FakeRecordService();
            var plugin = new FieldChangePlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.UpdateOnFieldChange(null, "status", "a", "b"));
        }

        [Test]
        public void UpdateOnFieldChange_NullFieldName_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null fieldName should throw.
            var fake = new TestDoubles.FakeRecordService();
            var plugin = new FieldChangePlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.UpdateOnFieldChange("rec1", null, "a", "b"));
        }

        [Test]
        public void UpdateOnFieldChange_ServiceReturnsFalse_ReturnsFalse()
        {
            // Arrange: configure the fake to report a failure from the update.
            var fake = new TestDoubles.FakeRecordService { UpdateResult = false };
            var plugin = new FieldChangePlugin(fake);

            // Act
            var result = plugin.UpdateOnFieldChange("rec1", "status", "open", "closed");

            // Assert: plugin should return the service result (false), but the service
            // must have been invoked with the expected values.
            Assert.IsFalse(result);
            Assert.AreEqual("rec1", fake.LastId);
            Assert.IsNotNull(fake.LastChanges);
        }

        [Test]
        public void UpdateOnFieldChange_NewValueIsNull_CallsServiceWithNullValue()
        {
            // Arrange
            var fake = new TestDoubles.FakeRecordService();
            var plugin = new FieldChangePlugin(fake);

            // Act: set the new value to null to ensure the plugin can handle nulls.
            var result = plugin.UpdateOnFieldChange("rec1", "status", "open", null);

            // Assert: update should be attempted and the changes dictionary should
            // contain the field mapped to a null value.
            Assert.IsTrue(result);
            Assert.AreEqual("rec1", fake.LastId);
            Assert.IsTrue(fake.LastChanges.ContainsKey("status"));
            Assert.IsNull(fake.LastChanges["status"]);
        }
    }
}
