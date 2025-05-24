using TeamTaskManagement.API.Response;

namespace TeamTaskManagement.API.Interfaces
{
    public interface ITaskService
    {
        Task<BaseResponse<List<TaskDto>>> GetTasksAsync(string teamId, string userId);
        Task<BaseResponse<TaskDto>> CreateTaskAsync(string teamId, string creatorId, TaskCreateDto dto);
        Task<BaseResponse<bool>> UpdateTaskAsync(string taskId, TaskUpdateDto dto, string userId);
        Task<BaseResponse<bool>> DeleteTaskAsync(string taskId, string userId);
        Task<BaseResponse<bool>> UpdateTaskStatusAsync(string taskId, TaskStatusDto dto, string userId);
    }
}
