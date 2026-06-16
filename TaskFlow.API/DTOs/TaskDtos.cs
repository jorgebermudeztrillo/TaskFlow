namespace TaskFlow.API.DTOs
{




    public record CreateTaskDto(string Title, string Description, int BoardColumnId);
    public record UpdateTaksDto(string Title, string Description);
    public record MoveTaskDto(int BoardColumnId,int Order);
    public record TaskDto(int Id, string Title, string Description, int Order, int BoardColumnId);
    public record BoardColumnWithTasksDto(int Id, string Name, List<TaskDto> Tasks);

   
}
