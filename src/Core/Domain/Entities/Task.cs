using Domain.Common;
using TaskStatus = Domain.Common.Enums.TaskStatus;


namespace Domain.Models
{
    public class Task : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }

        public string CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public string TeamId { get; set; }
        public Team Team { get; set; }
    } 
} 