namespace FixMyCampus.Api.Models;

public enum TicketStatus
{
    New,
    Assigned,
    InProgress,
    Resolved
}

public class Ticket
{
    public int Id { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Building { get; set; } = string.Empty;

    public string Room { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? TechnicianName { get; set; }

    public TicketStatus Status { get; set; } = TicketStatus.New;

    public string ReporterName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TicketStatusHistory> History { get; set; } = new List<TicketStatusHistory>();
}