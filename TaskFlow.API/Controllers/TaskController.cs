using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.API.DTOs;
using TaskFlow.API.Services;

namespace TaskFlow.API.Controllers;

// [Authorize] — todos los endpoints de este Controller requieren JWT válido
// Sin token Angular recibirá un 401 automáticamente
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService) => _taskService = taskService;

    // Extrae el Id del usuario del token JWT
    // ClaimTypes.NameIdentifier es el campo Id que metimos al generar el token en AuthService
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET api/task/board/{projectId}
    // Devuelve todas las columnas con sus tareas para un proyecto concreto
    [HttpGet("board/{projectId}")]
    public async Task<IActionResult> GetBoard(int projectId)
    {
        var board = await _taskService.GetBoardAsync(projectId, GetUserId());
        return Ok(board);
    }

    // POST api/task
    // Crea una tarea nueva dentro de la columna indicada en el DTO
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var task = await _taskService.CreateAsync(dto, GetUserId());

        // Si el Service devuelve null significa que la columna no existe
        // o no pertenece al usuario autenticado
        if (task is null)
            return BadRequest(new { message = "Columna no encontrada." });

        return Ok(task);
    }

    // PUT api/task/{id}/move
    // Mueve una tarea a otra columna — Angular manda el nuevo BoardColumnId y Order
    [HttpPut("{id}/move")]
    public async Task<IActionResult> Move(int id, MoveTaskDto dto)
    {
        var result = await _taskService.MoveAsync(id, dto, GetUserId());

        // Si devuelve false la tarea no existe o no pertenece al usuario
        if (!result)
            return NotFound(new { message = "Tarea no encontrada." });

        // NoContent = 204, significa éxito sin datos que devolver
        return NoContent();
    }

    // DELETE api/task/{id}
    // Borra una tarea verificando que pertenece al usuario autenticado
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _taskService.DeleteAsync(id, GetUserId());

        if (!result)
            return NotFound(new { message = "Tarea no encontrada." });

        return NoContent();
    }
}