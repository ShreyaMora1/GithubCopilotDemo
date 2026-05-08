namespace NotificationPlugin
{
    public interface INotificationService
    {
        /// <summary>
        /// Send a notification for the specified record with the given message.
        /// Implementations should deliver the notification and return true on success.
        /// </summary>
        /// <param name="recordId">Unique identifier of the record related to this notification.</param>
        /// <param name="message">The notification message to send.</param>
        /// <returns>True if the notification was sent successfully; otherwise false.</returns>
        bool SendNotification(string recordId, string message);
    }

    public class RecordNotificationPlugin
    {
        private readonly INotificationService _service;

        public RecordNotificationPlugin(INotificationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Send a notification when a record is created.
        /// </summary>
        /// <param name="recordId">The id of the record that was created. Must not be null.</param>
        /// <param name="recordType">The type of record created. Must not be null.</param>
        /// <returns>
        /// True if a notification was sent and the underlying service reported success;
        /// false when the service reported failure.
        /// </returns>
        public bool NotifyRecordCreated(string recordId, string recordType)
        {
            // Validate required arguments early to fail fast for incorrect usage.
            if (recordId == null) throw new System.ArgumentNullException(nameof(recordId));
            if (recordType == null) throw new System.ArgumentNullException(nameof(recordType));

            // Create a descriptive message for the notification.
            var message = $"Record of type '{recordType}' with id '{recordId}' has been created.";

            // Send the notification through the service.
            return _service.SendNotification(recordId, message);
        }

        /// <summary>
        /// Send a notification when a record is updated.
        /// </summary>
        /// <param name="recordId">The id of the record that was updated. Must not be null.</param>
        /// <param name="fieldName">The name of the field that was updated. Must not be null.</param>
        /// <param name="oldValue">The previous value of the field.</param>
        /// <param name="newValue">The new value of the field.</param>
        /// <returns>
        /// True if a notification was sent and the underlying service reported success;
        /// false when the service reported failure.
        /// </returns>
        public bool NotifyFieldUpdated(string recordId, string fieldName, object oldValue, object newValue)
        {
            // Validate required arguments early to fail fast for incorrect usage.
            if (recordId == null) throw new System.ArgumentNullException(nameof(recordId));
            if (fieldName == null) throw new System.ArgumentNullException(nameof(fieldName));

            // Create a descriptive message for the notification.
            var message = $"Field '{fieldName}' updated from '{oldValue}' to '{newValue}' for record '{recordId}'.";

            // Send the notification through the service.
            return _service.SendNotification(recordId, message);
        }
    }
}
