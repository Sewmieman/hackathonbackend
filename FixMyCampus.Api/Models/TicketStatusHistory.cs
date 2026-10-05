using System.Text.Json.Serialization;

namespace FixMyCampus.Api.Models;

public class TicketStatusHistory
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public TicketStatus FromStatus { get; set; }

    public TicketStatus ToStatus { get; set; }

    public DateTime ChangedAt { get; set; }

    public string ChangedBy { get; set; } = string.Empty;

    [JsonIgnore]
    public Ticket Ticket { get; set; } = null!;
}