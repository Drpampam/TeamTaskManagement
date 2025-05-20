namespace TeamTaskManagement.API.Models
{
    public class Task
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }

        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public Guid TeamId { get; set; }
        public Team Team { get; set; }
    }

    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
} 