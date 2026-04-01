using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdsTk.Domain.Entities;
using PdsTk.Infrastructure.Data;

namespace PdsTk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TicketsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> ObtenerTodos()
    {
        var tickets = await _context.Tickets.ToListAsync();
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> ObtenerPorId(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket is null)
            return NotFound();

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<Ticket>> Crear(Ticket ticket)
    {
        ticket.FechaCreacion = DateTime.UtcNow;
        ticket.Estado = "Pendiente";

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerPorId), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
    {
        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket is null)
            return NotFound();

        ticket.Estado = nuevoEstado;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}