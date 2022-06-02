namespace MyCompany.MyProject.Application.Todos.Queries;

internal record PriorityLevelDto
{
    public int Value { get; set; }

    public string Name { get; set; } = default!;
}
