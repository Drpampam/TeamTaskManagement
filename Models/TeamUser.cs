using System;

namespace TeamTaskManagement.API.Models
{
    public class TeamUser : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public string TeamId { get; set; }
        public Team Team { get; set; }

        public TeamRole Role { get; set; } = TeamRole.Member;
    }

    public enum TeamRole
    {
        Member,
        Admin
    }
} 