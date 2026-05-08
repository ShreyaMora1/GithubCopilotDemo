using NotificationPlugin;

namespace NotificationPlugin.Tests.TestDoubles
{
    /// <summary>
    /// Simple in-memory test double for <see cref="INotificationService"/>.
    /// This fake records the last call parameters so tests can assert on them.
    /// The <see cref="SendResult"/> property allows tests to simulate
    /// success (true) or failure (false) from the notification service.
    /// </summary>
    internal class FakeNotificationService : INotificationService
    {
        // Records the record id passed to the last SendNotification call.
        public string? LastRecordId;

        // Records the message passed to the last SendNotification call.
        public string? LastMessage;

        // Configure what the fake should return when SendNotification is called.
        // Tests set this to true or false to simulate success/failure.
        public bool SendResult = true;

        /// <summary>
        /// Record the parameters and return the configured result.
        /// Tests use the recorded values to verify the plugin's interactions.
        /// </summary>
        public bool SendNotification(string recordId, string message)
        {
            LastRecordId = recordId;
            LastMessage = message;
            return SendResult;
        }
    }
}
