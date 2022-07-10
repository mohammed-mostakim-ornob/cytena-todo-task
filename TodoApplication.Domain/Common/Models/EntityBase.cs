namespace TodoApplication.Domain.Common.Models;

public class EntityBase<TKey>
{
    public TKey Id { get; }
}