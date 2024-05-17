using System.Text.Json.Serialization;

namespace PaginationExample;

public class Paginged<T> where T : class
{
    public Paginged() { }

    public Paginged(PaginationQuery pagination)
    {
        pagination.Fix();
        Page = pagination.Page!.Value;
        Size = pagination.Size!.Value;
    }

    public T[] Data { get; set; } = null!;
    public int Page { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }

    [JsonIgnore]
    public int PagesCount => Count / Size + (Count % Size == 0 ? 0 : 1);
}