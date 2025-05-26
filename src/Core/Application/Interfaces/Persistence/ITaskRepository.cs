using Application.Responses;

namespace Application.Interfaces.Persistence
{
    public interface ITaskRepository
    {
        Task<BaseResponse<List<TaskDto>>> GetTeamTasks(GetTaskDto dto);
    }
}
