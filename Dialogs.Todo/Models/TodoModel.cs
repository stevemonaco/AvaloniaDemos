namespace Dialogs.Models;

public enum TodoPriority { Low, Medium, High, Urgent }
public record TodoModel(string Activity, bool IsCompleted, TodoPriority Priority);
