using Dialogs.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace Dialogs.Services;
public interface ITodoService
{
    IEnumerable<TodoModel>? DeserializeTodos(string jsonContent);
    string SerializeTodos(IEnumerable<TodoModel> items);
}

public class TodoService : ITodoService
{
    public IEnumerable<TodoModel>? DeserializeTodos(string jsonContent)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<IEnumerable<TodoModel>>(jsonContent, jsonOptions);
    }

    public string SerializeTodos(IEnumerable<TodoModel> items)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Serialize(items, jsonOptions);
    }
}
