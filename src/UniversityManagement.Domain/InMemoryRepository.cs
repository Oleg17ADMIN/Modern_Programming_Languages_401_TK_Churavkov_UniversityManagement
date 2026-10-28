namespace UniversityManagement.Domain;

// <T> = "будь-який тип". Один клас підходить і для Student, і для Course.
// where T : class = generic constraint: T має бути посилальним типом (class).
public class InMemoryRepository<T> where T : class
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    // true, якщо елемент знайдено і видалено
    public bool Remove(T item) => _items.Remove(item);

    public IReadOnlyCollection<T> GetAll() => _items;
}