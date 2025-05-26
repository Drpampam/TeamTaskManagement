using Application.Responses;

namespace Application.Interfaces
{
    public interface ITaskService
    {
        Task<BaseResponse<List<TaskDto>>> GetTasksAsync(GetTaskDto dto);
        Task<BaseResponse<TaskDto>> CreateTaskAsync(CreateTaskDto dto);
        Task<BaseResponse<bool>> UpdateTaskAsync(string taskId, TaskUpdateDto dto, string userId);
        Task<BaseResponse<bool>> DeleteTaskAsync(string taskId, string userId);
        Task<BaseResponse<bool>> UpdateTaskStatusAsync(string taskId, TaskStatusDto dto, string userId);
    }
}
