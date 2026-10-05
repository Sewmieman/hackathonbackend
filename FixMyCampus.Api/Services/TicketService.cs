using FixMyCampus.Api.Data;
using FixMyCampus.Api.DTOs;
using FixMyCampus.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCampus.Api.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Ticket> CreateAsync(CreateTicketDto dto)
    {
        var ticket = new Ticket
        {
            Category = dto.Category,
            Building = dto.Building,
            Room = dto.Room,
            Description = dto.Description,
            ReporterName = dto.ReporterName,
            Status = TicketStatus.New
        };

        _db.Tickets.Add(ticket);

        await _db.SaveChangesAsync();

        return ticket;
    }

    public async Task<List<Ticket>> GetAllAsync(
        string? building,
        TicketStatus? status)
    {
        var query = _db.Tickets
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(building))
        {
            query = query.Where(x =>
                x.Building == building);
        }

        if (status.HasValue)
        {
            query = query.Where(x =>
                x.Status == status.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _db.Tickets
            .Include(x => x.History)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> AssignAsync(
        int id,
        AssignTicketDto dto)
    {
        var ticket = await _db.Tickets
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ticket == null)
            return false;

        if (ticket.Status != TicketStatus.New)
            throw new InvalidOperationException(
                "Only New tickets can be assigned.");

        if (string.IsNullOrWhiteSpace(dto.TechnicianName))
            throw new ArgumentException(
                "Technician name is required.");

        ticket.TechnicianName = dto.TechnicianName;

        var history = new TicketStatusHistory
        {
            TicketId = ticket.Id,
            FromStatus = TicketStatus.New,
            ToStatus = TicketStatus.Assigned,
            ChangedBy = "Admin"
        };

        ticket.Status = TicketStatus.Assigned;

        _db.TicketStatusHistories.Add(history);

        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<(bool Success, string? Error)> ChangeStatusAsync(
        int id,
        ChangeStatusDto dto)
    {
        var ticket = await _db.Tickets
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ticket == null)
            return (false, "Ticket not found.");

        var current = ticket.Status;
        var requested = dto.Status;

        bool validTransition =
            (current == TicketStatus.Assigned &&
             requested == TicketStatus.InProgress)
            ||
            (current == TicketStatus.InProgress &&
             requested == TicketStatus.Resolved);

        if (!validTransition)
        {
            return (
                false,
                $"Invalid status transition: {current} → {requested}."
            );
        }

        ticket.Status = requested;

        var history = new TicketStatusHistory
        {
            TicketId = ticket.Id,
            FromStatus = current,
            ToStatus = requested,
            ChangedBy = dto.ChangedBy
        };

        _db.TicketStatusHistories.Add(history);

        await _db.SaveChangesAsync();

        return (true, null);
    }
}