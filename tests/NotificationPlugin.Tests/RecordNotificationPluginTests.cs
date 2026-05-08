using NUnit.Framework;
using NotificationPlugin;

namespace NotificationPlugin.Tests
{
    /// <summary>
    /// Tests for <see cref="RecordNotificationPlugin"/>.
    /// Tests are intentionally simple and focused on behavior verification,
    /// not integration with external services.
    /// </summary>
    public class RecordNotificationPluginTests
    {
        [Test]
        public void NotifyRecordCreated_WithValidInput_CallsServiceAndReturnsTrue()
        {
            // Arrange: create a fake service and the plugin under test.
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            // Act: simulate a record creation notification.
            var result = plugin.NotifyRecordCreated("rec1", "Account");

            // Assert: the plugin should return true and the fake service should
            // have received the expected record id and message.
            Assert.That(result, Is.True);
            Assert.That(fake.LastRecordId, Is.EqualTo("rec1"));
            Assert.That(fake.LastMessage, Is.Not.Null);
            Assert.That(fake.LastMessage!, Contains.Substring("Account"));
            Assert.That(fake.LastMessage!, Contains.Substring("rec1"));
        }

        [Test]
        public void NotifyRecordCreated_NullRecordId_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null recordId should throw.
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.NotifyRecordCreated(null!, "Account"));
        }

        [Test]
        public void NotifyRecordCreated_NullRecordType_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null recordType should throw.
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.NotifyRecordCreated("rec1", null!));
        }

        [Test]
        public void NotifyRecordCreated_ServiceReturnsFalse_ReturnsFalse()
        {
            // Arrange: configure the fake to report a failure from the notification service.
            var fake = new TestDoubles.FakeNotificationService { SendResult = false };
            var plugin = new RecordNotificationPlugin(fake);

            // Act
            var result = plugin.NotifyRecordCreated("rec1", "Account");

            // Assert: plugin should return the service result (false), but the service
            // must have been invoked with expected values.
            Assert.That(result, Is.False);
            Assert.That(fake.LastRecordId, Is.EqualTo("rec1"));
            Assert.That(fake.LastMessage, Is.Not.Null);
        }

        [Test]
        public void NotifyRecordCreated_EmptyRecordId_AllowsEmptyId()
        {
            // Arrange
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            // Act: empty record id (not null) should be allowed
            var result = plugin.NotifyRecordCreated(string.Empty, "Account");

            // Assert
            Assert.That(result, Is.True);
            Assert.That(fake.LastRecordId, Is.EqualTo(string.Empty));
            Assert.That(fake.LastMessage, Is.Not.Null);
        }

        [Test]
        public void NotifyFieldUpdated_IntValues_IncludesNumbersInMessage()
        {
            // Arrange
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            // Act: use value types (ints)
            var result = plugin.NotifyFieldUpdated("rec1", "count", 1, 2);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(fake.LastRecordId, Is.EqualTo("rec1"));
            Assert.That(fake.LastMessage, Is.Not.Null);
            Assert.That(fake.LastMessage!, Contains.Substring("1"));
            Assert.That(fake.LastMessage!, Contains.Substring("2"));
        }

        [Test]
        public void NotifyFieldUpdated_SameOldAndNew_StillSendsNotification()
        {
            // Arrange
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            // Act: old and new values are equal; plugin currently always notifies
            var result = plugin.NotifyFieldUpdated("rec1", "status", "open", "open");

            // Assert
            Assert.That(result, Is.True);
            Assert.That(fake.LastRecordId, Is.EqualTo("rec1"));
            Assert.That(fake.LastMessage, Is.Not.Null);
        }

        [Test]
        public void NotifyFieldUpdated_WithValidInput_CallsServiceAndReturnsTrue()
        {
            // Arrange: create a fake service and the plugin under test.
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            // Act: simulate a field update notification.
            var result = plugin.NotifyFieldUpdated("rec1", "status", "active", "inactive");

            // Assert: the plugin should return true and the fake service should
            // have received the expected record id and message.
            Assert.That(result, Is.True);
            Assert.That(fake.LastRecordId, Is.EqualTo("rec1"));
            Assert.That(fake.LastMessage, Is.Not.Null);
            Assert.That(fake.LastMessage!, Contains.Substring("status"));
            Assert.That(fake.LastMessage!, Contains.Substring("active"));
            Assert.That(fake.LastMessage!, Contains.Substring("inactive"));
        }

        [Test]
        public void NotifyFieldUpdated_NullRecordId_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null recordId should throw.
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.NotifyFieldUpdated(null!, "status", "a", "b"));
        }

        [Test]
        public void NotifyFieldUpdated_NullFieldName_ThrowsArgumentNullException()
        {
            // Validate argument checking: a null fieldName should throw.
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            Assert.Throws<System.ArgumentNullException>(() => plugin.NotifyFieldUpdated("rec1", null!, "a", "b"));
        }

        [Test]
        public void NotifyFieldUpdated_WithNullValues_CallsServiceAndReturnsTrue()
        {
            // Arrange: create a fake service and the plugin under test.
            var fake = new TestDoubles.FakeNotificationService();
            var plugin = new RecordNotificationPlugin(fake);

            // Act: simulate a field update with null values.
            var result = plugin.NotifyFieldUpdated("rec1", "notes", null!, "Some note");

            // Assert: the plugin should return true even with null values.
            Assert.That(result, Is.True);
            Assert.That(fake.LastRecordId, Is.EqualTo("rec1"));
            Assert.That(fake.LastMessage, Is.Not.Null);
        }

        [Test]
        public void NotifyFieldUpdated_ServiceReturnsFalse_ReturnsFalse()
        {
            // Arrange: configure the fake to report a failure from the notification service.
            var fake = new TestDoubles.FakeNotificationService { SendResult = false };
            var plugin = new RecordNotificationPlugin(fake);

            // Act
            var result = plugin.NotifyFieldUpdated("rec1", "status", "a", "b");

            // Assert: plugin should return the service result (false), but the service
            // must have been invoked with expected values.
            Assert.That(result, Is.False);
            Assert.That(fake.LastRecordId, Is.EqualTo("rec1"));
            Assert.That(fake.LastMessage, Is.Not.Null);
        }
    }
}
