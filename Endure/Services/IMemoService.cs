using Endure.Models;

namespace Endure.Services;

public interface IMemoService
{
    Task<List<Card>?> GetTasksAsync();
}