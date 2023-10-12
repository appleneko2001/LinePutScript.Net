namespace LinePutScript.Extensions;

public static class ArrayExtensions
{
    public static T[] ToArray<T>(this T? v)
    {
        return v == null ? Array.Empty<T>() : new[] { v };
    }
}