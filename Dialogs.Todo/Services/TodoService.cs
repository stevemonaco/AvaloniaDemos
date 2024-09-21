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
    private JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IEnumerable<TodoModel>? DeserializeTodos(string jsonContent)
    {
        return JsonSerializer.Deserialize<IEnumerable<TodoModel>>(jsonContent, _options);
    }

    public string SerializeTodos(IEnumerable<TodoModel> items)
    {
        return JsonSerializer.Serialize(items, _options);
    }
}
