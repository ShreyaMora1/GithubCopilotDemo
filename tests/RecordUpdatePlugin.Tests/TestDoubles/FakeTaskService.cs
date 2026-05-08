using RecordUpdatePlugin;

namespace RecordUpdatePlugin.Tests.TestDoubles
{
    /// <summary>
    /// Test double for <see cref="ITaskService"/>.
    /// This fake records the last task creation request so tests can assert on it.
    /// The <see cref="TaskIdToReturn"/> property allows tests to simulate
    /// success (non-null id) or failure (null) from the task service.
    /// </summary>
    internal class FakeTaskService : ITaskService
    {
        // Records the last task creation request passed to CreateTask.
        public TaskCreationRequest LastRequest;

        // Configure what the fake should return when CreateTask is called.
        // Tests set this to simulate success (non-null) or failure (null).
        public string TaskIdToReturn = "task-123";

        /// <summary>
        /// Record the request and return the configured task id.
        /// Tests use the recorded request to verify the plugin's behavior.
        /// </summary>
        public string CreateTask(TaskCreationRequest request)
        {
            LastRequest = request;
            return TaskIdToReturn;
        }
    }
}
