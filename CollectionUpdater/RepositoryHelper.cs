namespace CollectionUpdater;

public static class RepositoryHelper
{
    public static void UpdateCollectionField<T>(List<T> collection,
        IReadOnlyCollection<T> newData,
        Action<T, T>? update = null,
        Action<T>? delete = null) where T : Entity
    {
        if (collection.Count == 0 && newData.Count == 0)
            return;

        var existing = collection.Select(e => e.Id).ToList();
        var loaded = newData.Select(e => e.Id).ToList();

        if (delete is not null)
            foreach (var item in collection.Where(e => !loaded.Contains(e.Id)))
                delete(item);
        collection.RemoveAll(e => !loaded.Contains(e.Id));

        if (update is not null)
            foreach (var item in collection)
                update(item, newData.First(e => e.Id == item.Id));

        collection.AddRange(
            newData.Where(e => !existing.Contains(e.Id))
                .Select(e =>
                {
                    e.Id = default;
                    return e;
                }));
    }
}