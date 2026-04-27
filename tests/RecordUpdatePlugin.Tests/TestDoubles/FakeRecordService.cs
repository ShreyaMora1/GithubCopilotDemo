using System.Collections.Generic;
using RecordUpdatePlugin;

namespace RecordUpdatePlugin.Tests.TestDoubles
{
    internal class FakeRecordService : IRecordService
    {
        public string LastId;
        public IDictionary<string, object> LastChanges;
        public bool UpdateResult = true;

        public bool UpdateRecord(string id, IDictionary<string, object> changes)
        {
            LastId = id;
            LastChanges = changes;
            return UpdateResult;
        }
    }
}
