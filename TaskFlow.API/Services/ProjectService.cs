using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Data;
using TaskFlow.API.DTOs;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public class ProjectService
{
    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db) => _db = db;

    // Devuelve todos los proyectos del usuario autenticado
    public async Task<List<ProjectDto>> GetAllAsync(int userId)
    {
        return await _db.Projects
            .Where(p => p.UserId == userId)
            .Select(p => new ProjectDto(p.Id, p.Name, p.Description, p.CreatedAt))
            .ToListAsync();
    }

    // Crea un proyecto nuevo y sus 3 columnas Kanban por defecto
    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, int userId)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            UserId = userId,
            Columns = new List<BoardColumn>
            {
                new BoardColumn { Name = "Pendiente",    Order = 0 },
                new BoardColumn { Name = "En progreso",  Order = 1 },
                new BoardColumn { Name = "Hecho",        Order = 2 }
            }
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return new ProjectDto(project.Id, project.Name, project.Description, project.CreatedAt);
    }

    // Borra un proyecto — solo si pertenece al usuario autenticado
    public async Task<bool> DeleteAsync(int projectId, int userId)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project is null)
            return false;

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return true;
    }
}