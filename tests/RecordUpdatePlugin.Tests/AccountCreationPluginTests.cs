using NUnit.Framework;
using RecordUpdatePlugin;
using RecordUpdatePlugin.Tests.TestDoubles;
using System.Collections.Generic;

// Unit tests for AccountCreationPlugin.
// These tests verify the behavior of OnAccountCreated using a lightweight
// in-process test double (FakeTaskService) so there are no external dependencies.
// Each test follows the Arrange / Act / Assert pattern.
namespace RecordUpdatePlugin.Tests
{
    /// <summary>
    /// Tests for <see cref="AccountCreationPlugin"/>.
    /// Tests verify that the plugin correctly creates tasks when accounts are created
    /// and handles error cases appropriately.
    /// </summary>
    public class AccountCreationPluginTests
    {
        [Test]
        public void OnAccountCreated_ValidInputs_CreatesTaskAndReturnsTrue()
        {
            // Arrange: create a fake task service and the plugin under test.
            var fake = new FakeTaskService();
            var plugin = new AccountCreationPlugin(fake);

            // Act: trigger account creation with valid inputs.
            var result = plugin.OnAccountCreated("acc123", "ACME Corp");

            // Assert: the plugin should return true and the fake service should
            // have received a task creation request with expected values.
            Assert.IsTrue(result);
            Assert.IsNotNull(fake.LastRequest);
            Assert.AreEqual("acc123", fake.LastRequest.AccountId);
            Assert.AreEqual("Setup Account: ACME Corp", fake.LastRequest.TaskTitle);
            Assert.AreEqual("High", fake.LastRequest.Priority);
            Assert.IsTrue(fake.LastRequest.Description.Contains("ACME Corp"));
        }

        [Test]
        public void OnAccountCreated_TaskServiceReturnsNull_ReturnsFalse()
        {
            // Arrange: configure the fake to simulate task creation failure.
            var fake = new FakeTaskService { TaskIdToReturn = null };
            var plugin = new AccountCreationPlugin(fake);

            // Act
            var result = plugin.OnAccountCreated("acc123", "ACME Corp");

            // Assert: plugin should return false when service returns null.
            Assert.IsFalse(result);
            Assert.IsNotNull(fake.LastRequest);
        }

        [Test]
        public void OnAccountCreated_TaskServiceReturnsEmptyString_ReturnsFalse()
        {
            // Arrange: configure the fake to simulate task creation failure with empty string.
            var fake = new FakeTaskService { TaskIdToReturn = string.Empty };
            var plugin = new AccountCreationPlugin(fake);

            // Act
            var result = plugin.OnAccountCreated("acc123", "ACME Corp");

            // Assert: plugin should return false when service returns empty string.
            Assert.IsFalse(result);
        }

        [Test]
        public void OnAccountCreated_NullAccountId_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null accountId should throw.
            var fake = new FakeTaskService();
            var plugin = new AccountCreationPlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.OnAccountCreated(null, "ACME Corp"));
        }

        [Test]
        public void OnAccountCreated_NullAccountName_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null accountName should throw.
            var fake = new FakeTaskService();
            var plugin = new AccountCreationPlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.OnAccountCreated("acc123", null));
        }

        [Test]
        public void OnAccountCreated_TaskTitleContainsAccountName()
        {
            // Verify that the generated task title includes the account name for clarity.
            var fake = new FakeTaskService();
            var plugin = new AccountCreationPlugin(fake);

            plugin.OnAccountCreated("acc123", "Tech Startup Inc");

            Assert.IsTrue(fake.LastRequest.TaskTitle.Contains("Tech Startup Inc"));
        }

        [Test]
        public void OnAccountCreated_DescriptionMentionsOnboarding()
        {
            // Verify that the task description mentions the onboarding workflow.
            var fake = new FakeTaskService();
            var plugin = new AccountCreationPlugin(fake);

            plugin.OnAccountCreated("acc123", "New Customer");

            Assert.IsTrue(fake.LastRequest.Description.Contains("onboarding"));
        }

        [Test]
        public void OnAccountCreated_PriorityAlwaysHigh()
        {
            // Verify that new account setup tasks are always created with high priority.
            var fake = new FakeTaskService();
            var plugin = new AccountCreationPlugin(fake);

            plugin.OnAccountCreated("acc123", "Priority Test");

            Assert.AreEqual("High", fake.LastRequest.Priority);
        }

        [Test]
        public void OnAccountCreated_MultipleAccountCreations_EachCreatesSeparateTask()
        {
            // Verify that multiple account creations result in separate task creation calls.
            var fake = new FakeTaskService();
            var plugin = new AccountCreationPlugin(fake);

            var r1 = plugin.OnAccountCreated("acc1", "Account One");
            var r2 = plugin.OnAccountCreated("acc2", "Account Two");

            // Both should succeed and the last call should be recorded.
            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.AreEqual("acc2", fake.LastRequest.AccountId);
            Assert.AreEqual("Setup Account: Account Two", fake.LastRequest.TaskTitle);
        }

        [Test]
        public void Constructor_NullTaskService_ThrowsArgumentNullException()
        {
            // Validate that plugin constructor rejects a null service.
            Assert.Throws<System.ArgumentNullException>(() => new AccountCreationPlugin(null));
        }
    }
}
