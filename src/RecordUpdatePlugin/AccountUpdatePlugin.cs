namespace RecordUpdatePlugin
{
    /// <summary>
    /// Plugin that automatically creates a task when an account record is updated.
    /// This plugin is triggered during account updates to ensure proper workflow and tracking.
    /// </summary>
    public class AccountUpdatePlugin
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// Initialize the plugin with a task service implementation.
        /// </summary>
        /// <param name="taskService">Service used to create tasks. Must not be null.</param>
        public AccountUpdatePlugin(ITaskService taskService)
        {
            if (taskService == null) throw new System.ArgumentNullException(nameof(taskService));
            _taskService = taskService;
        }

        /// <summary>
        /// Handle account update by automatically creating an associated follow-up task.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account being updated. Must not be null.</param>
        /// <param name="fieldName">The name of the field that was updated. Must not be null.</param>
        /// <param name="oldValue">The previous value of the field.</param>
        /// <param name="newValue">The new value of the field.</param>
        /// <returns>
        /// True if a field change was detected and a task was successfully created;
        /// false if no change was detected or task creation failed.
        /// </returns>
        public bool OnAccountUpdated(string accountId, string fieldName, object oldValue, object newValue)
        {
            // Validate required arguments to fail fast for incorrect usage.
            if (accountId == null) throw new System.ArgumentNullException(nameof(accountId));
            if (fieldName == null) throw new System.ArgumentNullException(nameof(fieldName));

            // If the old and new values are equal, there is no change to act upon.
            if (object.Equals(oldValue, newValue))
            {
                return false;
            }

            // Build a task creation request for the account update.
            // This task serves as a reminder to review or follow up on the account changes.
            var taskRequest = new TaskCreationRequest
            {
                AccountId = accountId,
                TaskTitle = $"Review Account Update: {fieldName}",
                Description = $"Review the update to field '{fieldName}' for account '{accountId}'. Previous value: '{oldValue}', New value: '{newValue}'.",
                Priority = "Medium"
            };

            // Delegate the task creation to the service and return its result.
            // The service will return the task id on success, or null on failure.
            var taskId = _taskService.CreateTask(taskRequest);
            return !string.IsNullOrEmpty(taskId);
        }
    }
}
