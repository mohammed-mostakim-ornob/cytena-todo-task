using TodoApplication.Domain.Common.Repository;

namespace TodoApplication.Domain.Todo.Repositories;

public interface ITodoRepository : IGenericRepository<Domain.Todo.Models.Todo, long>
{ }