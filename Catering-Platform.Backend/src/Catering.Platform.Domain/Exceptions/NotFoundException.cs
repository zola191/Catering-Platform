namespace Catering.Platform.Domain.Exceptions;

public class NotFoundException : Exception
{
    public string EntityName { get; }
    public Guid? EntityId { get; }

    protected NotFoundException(string entityName, Guid? entityId, string message)
        : base(message)
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public static NotFoundException For<TEntity>(Guid id)
        where TEntity : class
    {
        var entityName = typeof(TEntity).Name;
        var message = $"Entity '{entityName}' with ID '{id}' was not found.";
        return new NotFoundException(entityName, id, message);
    }
}
