using NUnit.Framework;
using RecordUpdatePlugin;
using RecordUpdatePlugin.Tests.TestDoubles;

namespace RecordUpdatePlugin.Tests
{
    /// <summary>
    /// Tests for <see cref="AccountUpdatePlugin"/>.
    /// These tests exercise the public behavior of OnAccountUpdated using a
    /// lightweight in-process test double (FakeTaskService) so there are no
    /// external dependencies. Each test follows the Arrange / Act / Assert pattern.
    /// </summary>
    public class AccountUpdatePluginTests
    {
        [Test]
        public void OnAccountUpdated_WhenValueChanged_CallsServiceAndReturnsTrue()
        {
            // Arrange: create a fake service and the plugin under test.
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            // Act: simulate a field change from "active" to "inactive".
            var result = plugin.OnAccountUpdated("acc1", "status", "active", "inactive");

            // Assert: the plugin should return true and the fake service should
            // have received a task creation request with expected values.
            Assert.IsTrue(result);
            Assert.IsNotNull(fake.LastRequest);
            Assert.AreEqual("acc1", fake.LastRequest.AccountId);
            Assert.IsTrue(fake.LastRequest.TaskTitle.Contains("status"));
            Assert.IsTrue(fake.LastRequest.Description.Contains("active"));
            Assert.IsTrue(fake.LastRequest.Description.Contains("inactive"));
        }

        [Test]
        public void OnAccountUpdated_BothValuesNull_DoesNotCallService()
        {
            // Arrange
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            // Act: both old and new are null -> considered equal
            var result = plugin.OnAccountUpdated("acc1", "status", null, null);

            // Assert: no task should be created
            Assert.IsFalse(result);
            Assert.IsNull(fake.LastRequest);
        }

        [Test]
        public void OnAccountUpdated_OldNull_NewNonNull_CallsService()
        {
            // Arrange
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            // Act: old is null, new has a value -> change detected
            var result = plugin.OnAccountUpdated("acc1", "status", null, "premium");

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(fake.LastRequest);
            Assert.AreEqual("acc1", fake.LastRequest.AccountId);
            Assert.IsTrue(fake.LastRequest.Description.Contains("premium"));
        }

        [Test]
        public void OnAccountUpdated_ValueTypeChange_Ints_CallsService()
        {
            // Arrange: verify plugin handles value types (ints) correctly
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            // Act
            var result = plugin.OnAccountUpdated("acc1", "rating", 3, 5);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(fake.LastRequest);
            Assert.AreEqual("acc1", fake.LastRequest.AccountId);
            Assert.IsTrue(fake.LastRequest.Description.Contains("3"));
            Assert.IsTrue(fake.LastRequest.Description.Contains("5"));
        }

        [Test]
        public void OnAccountUpdated_EmptyFieldName_AllowsEmptyKey()
        {
            // The plugin only disallows null field names. Empty string should be allowed
            // and used in the task description.
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            var result = plugin.OnAccountUpdated("acc1", "", "a", "b");

            Assert.IsTrue(result);
            Assert.IsNotNull(fake.LastRequest);
            Assert.AreEqual("acc1", fake.LastRequest.AccountId);
        }

        [Test]
        public void OnAccountUpdated_ConsecutiveCalls_CreateTaskEachTime()
        {
            // Arrange
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            // Act: first change
            var r1 = plugin.OnAccountUpdated("acc1", "status", "a", "b");
            var firstRequest = fake.LastRequest;

            // Act: second change on same account
            var r2 = plugin.OnAccountUpdated("acc1", "type", "b", "c");
            var secondRequest = fake.LastRequest;

            // Assert: both calls should have been attempted
            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.AreEqual("acc1", fake.LastRequest.AccountId);
            Assert.AreNotSame(firstRequest, secondRequest);
            Assert.IsTrue(secondRequest.TaskTitle.Contains("type"));
        }

        [Test]
        public void OnAccountUpdated_WhenValueSame_DoesNotCallServiceAndReturnsFalse()
        {
            // Arrange
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            // Act: no actual change (old == new)
            var result = plugin.OnAccountUpdated("acc1", "status", "active", "active");

            // Assert: no task should be created and the fake should have no recorded request.
            Assert.IsFalse(result);
            Assert.IsNull(fake.LastRequest);
        }

        [Test]
        public void OnAccountUpdated_NullAccountId_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null accountId should throw.
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.OnAccountUpdated(null, "status", "a", "b"));
        }

        [Test]
        public void OnAccountUpdated_NullFieldName_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null fieldName should throw.
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.OnAccountUpdated("acc1", null, "a", "b"));
        }

        [Test]
        public void OnAccountUpdated_ServiceReturnsFalse_ReturnsFalse()
        {
            // Arrange: configure the fake to report a failure from the service.
            var fake = new FakeTaskService { TaskIdToReturn = null };
            var plugin = new AccountUpdatePlugin(fake);

            // Act
            var result = plugin.OnAccountUpdated("acc1", "status", "active", "inactive");

            // Assert: plugin should return false when service returns null, but the service
            // must have been invoked with the expected values.
            Assert.IsFalse(result);
            Assert.IsNotNull(fake.LastRequest);
        }

        [Test]
        public void OnAccountUpdated_NewValueIsNull_CallsServiceWithNullValue()
        {
            // Arrange
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            // Act: set the new value to null to ensure the plugin can handle nulls.
            var result = plugin.OnAccountUpdated("acc1", "status", "active", null);

            // Assert: task should be created and the description should mention the null value.
            Assert.IsTrue(result);
            Assert.IsNotNull(fake.LastRequest);
            Assert.AreEqual("acc1", fake.LastRequest.AccountId);
            Assert.IsTrue(fake.LastRequest.Description.Contains("active"));
        }

        [Test]
        public void OnAccountUpdated_TaskTitleIncludesFieldName()
        {
            // Verify that the task title includes the field name for clarity.
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            plugin.OnAccountUpdated("acc1", "invoiceEmail", "old@example.com", "new@example.com");

            Assert.IsTrue(fake.LastRequest.TaskTitle.Contains("invoiceEmail"));
        }

        [Test]
        public void OnAccountUpdated_PriorityIsMedium()
        {
            // Verify that account update tasks are created with medium priority.
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            plugin.OnAccountUpdated("acc1", "status", "a", "b");

            Assert.AreEqual("Medium", fake.LastRequest.Priority);
        }

        [Test]
        public void OnAccountUpdated_DescriptionIncludesOldAndNewValues()
        {
            // Verify that the task description includes both old and new values for reference.
            var fake = new FakeTaskService();
            var plugin = new AccountUpdatePlugin(fake);

            plugin.OnAccountUpdated("acc1", "tier", "silver", "gold");

            Assert.IsTrue(fake.LastRequest.Description.Contains("silver"));
            Assert.IsTrue(fake.LastRequest.Description.Contains("gold"));
        }
    }
}
