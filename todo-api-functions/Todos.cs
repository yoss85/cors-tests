using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

public class Todos
{
    ITodoRepository _todoRepository = new TodoRepository();

    [FunctionName(nameof(GetAllTodos))]
    public async Task<IActionResult> GetAllTodos([HttpTrigger(Microsoft.Azure.WebJobs.Extensions.Http.AuthorizationLevel.Anonymous,
        "get", Route = "todo")] HttpRequest req) => new OkObjectResult(await _todoRepository.GetAll());

    [FunctionName(nameof(UpdateTodo))]
    public async Task<IActionResult> UpdateTodo([HttpTrigger(Microsoft.Azure.WebJobs.Extensions.Http.AuthorizationLevel.Anonymous,
       "put",
       Route = "todo")] HttpRequest req)
       {
            using var sr = new StreamReader(req.Body);
            var content = await sr.ReadToEndAsync();
            TodoModel todoToUpdate = JsonConvert.DeserializeObject<TodoModel>(content);
            if (await _todoRepository.Update(todoToUpdate))
            {
                return new NoContentResult();
            }
            return new NotFoundResult();
       }

    [FunctionName(nameof(DeleteTodo))]
    public async Task<IActionResult> DeleteTodo([HttpTrigger(Microsoft.Azure.WebJobs.Extensions.Http.AuthorizationLevel.Anonymous,
        "delete", Route = "todo/{todoId:int}")] HttpRequest req, int todoId)
        {
            return (await _todoRepository.Delete(todoId)) == true ? new NoContentResult() : new NotFoundResult();
        }

    [FunctionName(nameof(CreateTodo))]
    public async Task<IActionResult> CreateTodo([HttpTrigger(Microsoft.Azure.WebJobs.Extensions.Http.AuthorizationLevel.Anonymous,
        "post", "options",
        Route = "todo")] HttpRequest req)
        {
            if (string.Compare(req.Method, "options", true) == 0)
            {
                return new OkResult();
            }

            using var sr = new StreamReader(req.Body);
            var content = await sr.ReadToEndAsync();
            string location = string.Empty;
            var entityToAdd = JsonConvert.DeserializeObject<TodoModel>(content);
            var result = await _todoRepository.Add(entityToAdd);
            return new CreatedResult(location, result);
        }
}


public interface ITodoRepository
{
    Task<IEnumerable<TodoModel>> GetAll();
    Task<TodoModel> Get(int id);
    Task<bool> Update(TodoModel entity);
    Task<bool> Delete(int entityId);
    Task<TodoModel> Add(TodoModel entity);
}
public class TodoRepository : ITodoRepository
{
    static List<TodoModel> _todos = new List<TodoModel>
    {
        new TodoModel(1, "Todo - 1", "First Todo", false, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow + TimeSpan.FromDays(5))
    };
    public Task<TodoModel> Add(TodoModel entity)
    {
        var id = _todos.OrderBy(x => x.Id).Select(x => x.Id).Last();
        var entityToAdd = entity with { Id = id + 1, CreatedOn = DateTimeOffset.UtcNow};
        _todos.Add(entityToAdd);
        return Task.FromResult(entityToAdd);
    }

    public Task<bool> Delete(int entityId)
    {
        var existingTodo = _todos.FirstOrDefault(x => x.Id == entityId);
        if (existingTodo == null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(_todos.Remove(existingTodo));
    }

    public Task<TodoModel> Get(int id)
        => Task.FromResult(_todos.FirstOrDefault(x => x.Id == id));

    public Task<IEnumerable<TodoModel>> GetAll()
        => Task.FromResult(_todos.AsEnumerable());

    public Task<bool> Update(TodoModel entity)
    {
        var todoToUpdate = _todos.FirstOrDefault(t => t.Id == entity.Id);
        if (todoToUpdate is null) return Task.FromResult(false);
        _todos.Remove(todoToUpdate);
        _todos.Add(entity);
        return Task.FromResult(true);
    }

}

public record TodoModel(int Id, string Name, string Description, bool Completed, DateTimeOffset CreatedOn, DateTimeOffset DueDateTime)
{
    public TodoModel()
    :this(0, string.Empty, string.Empty, false, DateTimeOffset.MinValue, DateTimeOffset.MinValue) {}
}