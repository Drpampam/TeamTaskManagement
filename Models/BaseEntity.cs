namespace TeamTaskManagement.API.Models
{
    public class BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddDays(7);
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow.AddDays(7);
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
    }
}
