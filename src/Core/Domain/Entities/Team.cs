using Domain.Common;

namespace Domain.Models
{
    public class Team : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
} 