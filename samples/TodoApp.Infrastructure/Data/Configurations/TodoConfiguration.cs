using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Infrastructure.Data.Configurations;

public sealed class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(Todo.TitleMaxLength);

        builder.Property(x => x.Description)
            .HasMaxLength(Todo.DescriptionMaxLength);

        builder.Property(x => x.Priority)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.HasIndex(x => new { x.Status, x.DueDate });
        builder.HasIndex(x => x.IsPinned);
    }
}
