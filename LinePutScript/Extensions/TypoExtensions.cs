using LinePutScript.Dictionary;

namespace LinePutScript.Extensions;

/// <summary>
/// This extension is used for Redirecting API with incorrect naming to the refactored API.
/// </summary>
public static class TypoExtensions
{
    public static ISub[] SeachALL<T>(this Line<T> line, string value) where T : IList<ISub>, new()
    {
        return line.SearchAll(value);
    }

    public static ISub? Seach<T>(this Line<T> line, string value) where T : IList<ISub>, new()
    {
        return line.Search(value);
    }

    public static ISub FindorAdd<T>(this Line<T> line, string subName) where T : IList<ISub>, new()
    {
        return line.FindOrAdd(subName);
    }

    public static void AddorReplaceSub<T>(this Line_D<T> dict, ISub newSub) where T : IDictionary<string, ISub>, new()
    {
        dict.AddOrReplaceSub(newSub);
    }
    //LPS_D<T> : ILPS where T : IDictionary<string, ILine>, new()
    //where Y : ILine, new()
    public static ILine FindorAddLine<T>(this LPS_D<T> dict, string lineName)
        where T : IDictionary<string, ILine>, new()
    {
        return dict.FindOrAddLine(lineName);
    }
}