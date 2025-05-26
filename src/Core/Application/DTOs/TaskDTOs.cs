public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public Guid AssignedToUserId { get; set; }
}

public class TaskUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public Guid AssignedToUserId { get; set; }
}

public class TaskStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid AssignedToUserId { get; set; }
    public string AssignedToUsername { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid TeamId { get; set; }
}
