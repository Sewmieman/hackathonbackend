using System.ComponentModel.DataAnnotations;
using FixMyCampus.Api.Models;

namespace FixMyCampus.Api.DTOs;

public record CreateTicketDto
(
    [Required]
    string Category,

    [Required]
    string Building,

    [Required]
    string Room,

    [Required]
    [MinLength(10)]
    string Description,

    [Required]
    string ReporterName
);

public record AssignTicketDto(
    [Required]
    string TechnicianName
);

public record ChangeStatusDto(
    TicketStatus Status,
    [Required]
    string ChangedBy
);

public record TicketResponseDto(
    int Id,
    string Category,
    string Building,
    string Room,
    string Description,
    string? TechnicianName,
    TicketStatus Status,
    string ReporterName,
    DateTime CreatedAt
);