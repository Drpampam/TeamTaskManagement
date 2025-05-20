public class TeamCreateDto
{
    public string Name { get; set; } = string.Empty;
}

public class TeamMemberDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class TeamDto
{
    public Guid Id { get; set; }            // Unique identifier of the team
    public string Name { get; set; }        // Team name
    public DateTime CreatedAt { get; set; } // (Optional) Timestamp of when the team was created
}
