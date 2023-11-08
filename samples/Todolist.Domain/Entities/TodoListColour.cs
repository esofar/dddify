using Dddify.Domain;

namespace Todolist.Domain.Entities;

public class TodoListColour : ValueObject
{
    public TodoListColour(string code)
    {
        Code = code;
    }

    public TodoListColour() { }

    public static TodoListColour? From(string code)
    {
        var colour = new TodoListColour { Code = code };

        if (!SupportedColours.Contains(colour))
        {
            return default;
        }

        return colour;
    }

    public static TodoListColour White => new("#FFFFFF");

    public static TodoListColour Red => new("#FF5733");

    public static TodoListColour Orange => new("#FFC300");

    public static TodoListColour Yellow => new("#FFFF66");

    public static TodoListColour Green => new("#CCFF99");

    public static TodoListColour Blue => new("#6666FF");

    public static TodoListColour Purple => new("#9966CC");

    public static TodoListColour Grey => new("#999999");

    public string Code { get; set; }

    public static implicit operator string(TodoListColour colour)
    {
        return colour.ToString();
    }

    public static explicit operator TodoListColour(string code)
    {
        return From(code);
    }

    public override string ToString()
    {
        return Code;
    }

    public static IEnumerable<TodoListColour> SupportedColours
    {
        get
        {
            yield return White;
            yield return Red;
            yield return Orange;
            yield return Yellow;
            yield return Green;
            yield return Blue;
            yield return Purple;
            yield return Grey;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return SupportedColours;
    }
}
