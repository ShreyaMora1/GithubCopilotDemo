namespace RecordUpdatePlugin
{
    public interface IRecordService
    {
        bool UpdateRecord(string id, System.Collections.Generic.IDictionary<string, object> changes);
    }

    public class FieldChangePlugin
    {
        private readonly IRecordService _service;

        public FieldChangePlugin(IRecordService service)
        {
            _service = service;
        }

        // Updates the record when the field value changes. Returns true if update performed.
        public bool UpdateOnFieldChange(string recordId, string fieldName, object oldValue, object newValue)
        {
            if (recordId == null) throw new System.ArgumentNullException(nameof(recordId));
            if (fieldName == null) throw new System.ArgumentNullException(nameof(fieldName));

            if (object.Equals(oldValue, newValue))
            {
                return false;
            }

            var changes = new System.Collections.Generic.Dictionary<string, object>
            {
                [fieldName] = newValue
            };

            return _service.UpdateRecord(recordId, changes);
        }
    }
}
