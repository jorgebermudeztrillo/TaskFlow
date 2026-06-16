using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Data;
using TaskFlow.API.DTOs;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public class TaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db) => _db = db;

    // Devuelve el tablero completo — columnas con sus tareas anidadas
    // Primero verifica que el proyecto pertenece al usuario
    public async Task<List<BoardColumnWithTasksDto>> GetBoardAsync(int projectId, int userId)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        // Si el proyecto no existe o no es del usuario devuelve lista vacía
        if (project is null)
            return new List<BoardColumnWithTasksDto>();

        // Carga las columnas ordenadas con sus tareas ordenadas dentro
        return await _db.BoardColumns
            .Where(c => c.ProjectId == projectId)
            .OrderBy(c => c.Order)
            .Select(c => new BoardColumnWithTasksDto(
                c.Id,
                c.Name,
                // Por cada columna, carga sus tareas ordenadas por Order
                c.Tasks
                    .OrderBy(t => t.Order)
                    .Select(t => new TaskDto(t.Id, t.Title, t.Description, t.Order, t.BoardColumnId))
                    .ToList()
            ))
            .ToListAsync();
    }

    // Crea una tarea nueva dentro de una columna
    public async Task<TaskDto?> CreateAsync(CreateTaskDto dto, int userId)
    {
        // Include hace un JOIN con Project para poder verificar el UserId
        // Así nos aseguramos que la columna pertenece al usuario autenticado
        var column = await _db.BoardColumns
            .Include(c => c.Project)
            .FirstOrDefaultAsync(c => c.Id == dto.BoardColumnId && c.Project.UserId == userId);

        if (column is null)
            return null;

        // Calcula el Order máximo actual para poner la tarea al final
        // El ?? 0 significa "si no hay tareas, empieza desde 0"
        var maxOrder = await _db.TaskItems
            .Where(t => t.BoardColumnId == dto.BoardColumnId)
            .MaxAsync(t => (int?)t.Order) ?? 0;

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            BoardColumnId = dto.BoardColumnId,
            Order = maxOrder + 1
        };

        _db.TaskItems.Add(task);
        await _db.SaveChangesAsync();

        return new TaskDto(task.Id, task.Title, task.Description, task.Order, task.BoardColumnId);
    }

    // Mueve una tarea a otra columna cambiando su BoardColumnId y Order
    public async Task<bool> MoveAsync(int taskId, MoveTaskDto dto, int userId)
    {
        // ThenInclude hace un JOIN de dos niveles: TaskItem → BoardColumn → Project
        // Necesitamos llegar hasta Project para verificar el UserId
        var task = await _db.TaskItems
            .Include(t => t.BoardColumn)
            .ThenInclude(c => c.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.BoardColumn.Project.UserId == userId);

        if (task is null)
            return false;

        // Mover la tarea es simplemente cambiar estos dos valores
        task.BoardColumnId = dto.BoardColumnId;
        task.Order = dto.Order;

        await _db.SaveChangesAsync();
        return true;
    }

    // Borra una tarea verificando que pertenece al usuario
    public async Task<bool> DeleteAsync(int taskId, int userId)
    {
        var task = await _db.TaskItems
            .Include(t => t.BoardColumn)
            .ThenInclude(c => c.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.BoardColumn.Project.UserId == userId);

        if (task is null)
            return false;

        _db.TaskItems.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }
}