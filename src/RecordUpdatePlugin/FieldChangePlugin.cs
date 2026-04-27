namespace RecordUpdatePlugin
{
    public interface IRecordService
    {
        /// <summary>
        /// Persist the specified changes for the record with the given id.
        /// Implementations should apply the changes and return true on success.
        /// </summary>
        /// <param name="id">Unique identifier of the record to update.</param>
        /// <param name="changes">A dictionary mapping field names to new values.</param>
        /// <returns>True if the update succeeded; otherwise false.</returns>
        bool UpdateRecord(string id, System.Collections.Generic.IDictionary<string, object> changes);
    }

    public class FieldChangePlugin
    {
        private readonly IRecordService _service;

        public FieldChangePlugin(IRecordService service)
        {
            _service = service;
        }

        /// <summary>
        /// Determine whether a field value changed and, if so, update the record using the
        /// provided <see cref="IRecordService"/> implementation.
        /// </summary>
        /// <param name="recordId">The id of the record to update. Must not be null.</param>
        /// <param name="fieldName">The name of the field that changed. Must not be null.</param>
        /// <param name="oldValue">The previous value of the field.</param>
        /// <param name="newValue">The new value of the field.</param>
        /// <returns>
        /// True if an update was attempted and the underlying service reported success;
        /// false when no change was detected or when the service reported failure.
        /// </returns>
        public bool UpdateOnFieldChange(string recordId, string fieldName, object oldValue, object newValue)
        {
            // Validate required arguments early to fail fast for incorrect usage.
            if (recordId == null) throw new System.ArgumentNullException(nameof(recordId));
            if (fieldName == null) throw new System.ArgumentNullException(nameof(fieldName));

            // If the old and new values are equal, there is nothing to update.
            // Use object.Equals so we correctly handle nulls and value types.
            if (object.Equals(oldValue, newValue))
            {
                // No change detected — do not call the update service.
                return false;
            }

            // Prepare a simple changeset: a dictionary that maps the changed field name
            // to its new value. Implementations of IRecordService interpret this
            // dictionary to perform the actual persistence (e.g., database or API call).
            var changes = new System.Collections.Generic.Dictionary<string, object>
            {
                [fieldName] = newValue
            };

            // Delegate the actual update to the provided service and return its result.
            return _service.UpdateRecord(recordId, changes);
        }
    }
}
