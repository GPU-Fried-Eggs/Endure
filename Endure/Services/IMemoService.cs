using Endure.Models;

namespace Endure.Services;

public interface IMemoService
{
    Task<List<Memo>?> GetMemoAsync();

    Task SaveMemoAsync(Memo memo, bool isNew);

    Task DeleteMemoAsync(Guid id);
}