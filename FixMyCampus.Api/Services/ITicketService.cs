using FixMyCampus.Api.DTOs;
using FixMyCampus.Api.Models;

namespace FixMyCampus.Api.Services;

public interface ITicketService
{
    Task<Ticket> CreateAsync(CreateTicketDto dto);

    Task<List<Ticket>> GetAllAsync(
        string? building,
        TicketStatus? status);

    Task<Ticket?> GetByIdAsync(int id);

    Task<bool> AssignAsync(int id, AssignTicketDto dto);

    Task<(bool Success, string? Error)> ChangeStatusAsync(
        int id,
        ChangeStatusDto dto);
}