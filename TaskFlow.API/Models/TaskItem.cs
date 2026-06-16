namespace TaskFlow.API.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int BoardColumnId { get; set; }
    public BoardColumn BoardColumn { get; set; } = null!;
}
