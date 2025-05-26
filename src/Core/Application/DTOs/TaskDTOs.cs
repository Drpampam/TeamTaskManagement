public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string AssignedToUserId { get; set; }
}

public class TaskUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string AssignedToUserId { get; set; }
}

public class TaskStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class TaskDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AssignedToUserId { get; set; }
    public string AssignedToUsername { get; set; }
    public string CreatedByUserId { get; set; }
    public string TeamId { get; set; }

    public void MapFrom(Domain.Models.Task task)
    {
        Id = task.Id;
        Title = task.Title;
        Description = task.Description;
        DueDate = task.DueDate;
        Status = task.Status.ToString();
        CreatedAt = task.CreatedAt;
        AssignedToUserId = task.AssignedToUserId;
        AssignedToUsername = task.AssignedToUser?.Username ?? string.Empty;
        CreatedByUserId = task.CreatedByUserId;
        TeamId = task.TeamId;
    }
}

public class GetTaskDto
{
    public string TeamId { get; set; }
    public string UserId { get; set; }
}

public record CreateTaskDto(string teamId, TaskCreateDto createTaskDto);


