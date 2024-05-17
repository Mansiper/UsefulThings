namespace PaginationExample;

public class PaginationQuery
{
    public const int DefaultPage = 1;
    public const int DefaultSize = 50;
    public const int MinSize = 10;
    public const int MaxSize = 50;

    public int? Page { get; set; }
    public int? Size { get; set; }

    public void Fix()
    {
        if (Page is null or < 1)
            Page = DefaultPage;

        Size = Size switch
        {
            null or < MinSize => DefaultSize,
            > MaxSize => MaxSize,
            _ => Size
        };
    }
}