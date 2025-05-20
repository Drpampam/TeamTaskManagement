namespace TeamTaskManagement.API.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetTasksAsync(Guid teamId, string userId);
        Task<TaskDto> CreateTaskAsync(Guid teamId, string creatorId, TaskCreateDto dto);
        Task<bool> UpdateTaskAsync(Guid taskId, TaskUpdateDto dto, string userId);
        Task<bool> DeleteTaskAsync(Guid taskId, string userId);
        Task<bool> UpdateTaskStatusAsync(Guid taskId, TaskStatusDto dto, string userId);
    }
}
