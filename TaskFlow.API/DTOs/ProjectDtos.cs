namespace TaskFlow.API.DTOs
{


    public record CreateProjectDto(string Name, string Description);

    public record ProjectDto(int Id, string Name, string Description,DateTime CreatedAt);

    public class ProjectDtos
    {
    }
}
