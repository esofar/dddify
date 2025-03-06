namespace TodoListApp.Infrastructure.Data.EntityConfigurations;

public class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Text)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Priority)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.IsDone)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasData(
            new Todo(new Guid("08daa6de-fabb-4e57-86e0-386a66fe8ae7"), "Write project proposal 📃", TodoPriority.Low),
            new Todo(new Guid("08daa6df-0599-4227-8e79-ac83b22305f3"), "Schedule kick-off meeting ✅", TodoPriority.Medium),
            new Todo(new Guid("08daa6df-1457-4ff3-8080-a57e71d0d80c"), "Review research results 🤯", TodoPriority.High));
    }
}
