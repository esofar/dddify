namespace Dddify.Domain;

public interface IStronglyTypedId<TValue>
{
    TValue Value { get; }
}