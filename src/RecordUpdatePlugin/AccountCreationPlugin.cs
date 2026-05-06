namespace RecordUpdatePlugin
{
    /// <summary>
    /// Represents a task that should be created in the system.
    /// </summary>
    public class TaskCreationRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the account for which the task is created.
        /// </summary>
        public string? AccountId { get; set; }

        /// <summary>
        /// Gets or sets the title of the task.
        /// </summary>
        public string? TaskTitle { get; set; }

        /// <summary>
        /// Gets or sets the description of the task.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the priority level of the task (e.g., "Low", "Medium", "High").
        /// </summary>
        public string? Priority { get; set; }
    }

    /// <summary>
    /// Service interface for creating and managing tasks.
    /// Implementations should persist task records to the underlying data store.
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Create a new task based on the provided request.
        /// </summary>
        /// <param name="request">The task creation request containing account id, title, description, and priority.</param>
        /// <returns>The unique identifier of the created task; null if creation failed.</returns>
        string CreateTask(TaskCreationRequest request);
    }

    /// <summary>
    /// Plugin that automatically creates a task when an account is created.
    /// This plugin is triggered during account creation to ensure proper onboarding workflow.
    /// </summary>
    public class AccountCreationPlugin
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// Initialize the plugin with a task service implementation.
        /// </summary>
        /// <param name="taskService">Service used to create tasks. Must not be null.</param>
        public AccountCreationPlugin(ITaskService taskService)
        {
            if (taskService == null) throw new System.ArgumentNullException(nameof(taskService));
            _taskService = taskService;
        }

        /// <summary>
        /// Handle account creation by automatically creating an associated setup/onboarding task.
        /// </summary>
        /// <param name="accountId">The unique identifier of the newly created account. Must not be null.</param>
        /// <param name="accountName">The name of the account. Used in task description. Must not be null.</param>
        /// <returns>
        /// True if the account was created and a task was successfully created;
        /// false if task creation failed.
        /// </returns>
        public bool OnAccountCreated(string accountId, string accountName)
        {
            // Validate required arguments to fail fast for incorrect usage.
            if (accountId == null) throw new System.ArgumentNullException(nameof(accountId));
            if (accountName == null) throw new System.ArgumentNullException(nameof(accountName));

            // Build a task creation request for the new account.
            // This task serves as a reminder to complete account setup and onboarding steps.
            var taskRequest = new TaskCreationRequest
            {
                AccountId = accountId,
                TaskTitle = $"Setup Account: {accountName}",
                Description = $"Complete the onboarding and setup process for the new account '{accountName}'.",
                Priority = "High"
            };

            // Delegate the task creation to the service and return its result.
            // The service will return the task id on success, or null on failure.
            var taskId = _taskService.CreateTask(taskRequest);
            return !string.IsNullOrEmpty(taskId);
        }
    }
}
