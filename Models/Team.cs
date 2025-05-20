using System;
using System.Collections.Generic;

namespace TeamTaskManagement.API.Models
{
    public class Team
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
} 