using System.Collections.Generic;
using RecordUpdatePlugin;

namespace RecordUpdatePlugin.Tests.TestDoubles
{
    /// <summary>
    /// Simple in-memory test double for <see cref="IRecordService"/>.
    /// This fake records the last call parameters so tests can assert on them.
    /// The <see cref="UpdateResult"/> property allows tests to simulate
    /// success (true) or failure (false) from the persistence layer.
    /// </summary>
    internal class FakeRecordService : IRecordService
    {
        // Records the id passed to the last UpdateRecord call.
        public string LastId;

        // Records the changes dictionary passed to the last UpdateRecord call.
        public IDictionary<string, object> LastChanges;

        // Configure what the fake should return when UpdateRecord is called.
        // Tests set this to true or false to simulate success/failure.
        public bool UpdateResult = true;

        /// <summary>
        /// Record the parameters and return the configured result.
        /// Tests use the recorded values to verify the plugin's interactions.
        /// </summary>
        public bool UpdateRecord(string id, IDictionary<string, object> changes)
        {
            LastId = id;
            LastChanges = changes;
            return UpdateResult;
        }
    }
}
