namespace PicoNet.Contracts.Extensions;

public static class PaginationHelper
{
    public static string EncodeCursor(Guid id) =>
        Convert.ToBase64String(id.ToByteArray());

    // Decoding (when reading the cursor)
    public static Guid? DecodeCursor(string? cursor)
    {
        if (string.IsNullOrEmpty(cursor)) return null;
        try
        {
            var bytes = Convert.FromBase64String(cursor);
            return new Guid(bytes);
        }
        catch { return null; }
    }
}