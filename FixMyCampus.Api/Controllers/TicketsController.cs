using FixMyCampus.Api.DTOs;
using FixMyCampus.Api.Models;
using FixMyCampus.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FixMyCampus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _service;

    public TicketsController(ITicketService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTicketDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Category) ||
            string.IsNullOrWhiteSpace(dto.Building) ||
            string.IsNullOrWhiteSpace(dto.Room) ||
            string.IsNullOrWhiteSpace(dto.Description))
        {
            return BadRequest(
                "Category, building, room and description are required.");
        }

        var ticket = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = ticket.Id },
            ticket);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? building,
        [FromQuery] TicketStatus? status)
    {
        var tickets = await _service.GetAllAsync(
            building,
            status);

        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await _service.GetByIdAsync(id);

        if (ticket == null)
            return NotFound("Ticket not found.");

        return Ok(ticket);
    }

    [HttpPut("{id:int}/assign")]
    public async Task<IActionResult> Assign(
        int id,
        AssignTicketDto dto)
    {
        try
        {
            var success = await _service.AssignAsync(
                id,
                dto);

            if (!success)
                return NotFound("Ticket not found.");

            return Ok(new
            {
                message = "Ticket assigned successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(
        int id,
        ChangeStatusDto dto)
    {
        var result = await _service.ChangeStatusAsync(
            id,
            dto);

        if (!result.Success)
        {
            if (result.Error == "Ticket not found.")
                return NotFound(result.Error);

            return BadRequest(result.Error);
        }

        return Ok(new
        {
            message = "Status updated successfully."
        });
    }
}