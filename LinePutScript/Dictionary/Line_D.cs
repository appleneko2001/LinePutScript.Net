using System.Globalization;
using System.Text;
using LinePutScript.Extensions;

namespace LinePutScript.Dictionary;

/// <summary>
/// 通过字典类型的行, Name不会重复
/// </summary>
public class Line_D<T> : Sub, ILine where T : IDictionary<string, ISub>, new()
{
    /// <summary>
    /// 创建一行
    /// </summary>
    public Line_D()
    {
    }

    /// <summary>
    /// 通过lpsLine文本创建一行
    /// </summary>
    /// <param name="lpsLine">lpsSub文本</param>
    public Line_D(string lpsLine)
    {
        Load(lpsLine);
    }

    /// <summary>
    /// 通过其他Line创建新的Line
    /// </summary>
    /// <param name="line">其他line</param>
    public Line_D(ILine line)
    {
        Name = line.Name;
        Info = (SetObject)line.Info;
        RawText = line.RawText;
        AddRange(line.ToArray());
    }

    /// <summary>
    /// 通过名字和信息创建新的Line
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">信息 (正常)</param>
    /// <param name="text">文本 在末尾没有结束行号的文本 (正常)</param>
    /// <param name="subs">子类集合</param>
    public Line_D(string name, string info, string text = "", params ISub[] subs) : base(name, info)
    {
        Text = text;
        AddRange(subs);
    }

    /// <summary>
    /// 通过名字和信息创建新的Line
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">信息 (正常)</param>
    /// <param name="text">文本 在末尾没有结束行号的文本 (正常)</param>
    /// <param name="subs">子类集合</param>
    public Line_D(string name, string info, IEnumerable<ISub> subs, string text = "") : base(name, info)
    {
        Text = text;
        AddRange(subs);
    }

    /// <summary>
    /// 子项目
    /// </summary>
    public T Subs { get; set; } = new T();

    [Obsolete($"use {nameof(RawText)} instead.")]
    public string text
    {
        get => RawText;
        set => RawText = value;
    }

    /// <summary>
    /// 文本 在末尾没有结束行号的文本 (去除关键字的文本)
    /// </summary>
    public string RawText { get; set; } = "";

    /// <summary>
    /// 文本 在末尾没有结束行号的文本 (正常)
    /// </summary>
    public string Text
    {
        get => TextDeReplace(RawText);
        set => RawText = TextReplace(value);
    }

    /// <summary>
    /// 获得Text的String结构
    /// </summary>
    public StringStructure Texts
    {
        get
        {
            texts ??= new StringStructure((x) => RawText = x, () => RawText);
            return texts;
        }
    }

    StringStructure? texts;

    /// <summary>
    /// 文本 (int)
    /// </summary>
    public int TextToInt
    {
        get => int.TryParse(RawText, out int i) ? i : 0;
        set => RawText = value.ToString();
    }

    /// <summary>
    /// 文本 (int64)
    /// </summary>
    public long TextToInt64
    {
        get => long.TryParse(RawText, out long i) ? i : 0;
        set => RawText = value.ToString();
    }

    /// <summary>
    /// 文本 (double)
    /// </summary>
    public double TextToDouble
    {
        get => double.TryParse(RawText, out double i) ? i : 0;
        set => RawText = value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 注释 ///为注释
    /// </summary>
    public string Comments { get; set; } = "";

    /// <summary>
    /// 获取Sub数量
    /// </summary>
    public int Count => Subs.Count;

    /// <summary>
    /// 是否只读
    /// </summary>
    public bool IsReadOnly => ((ICollection<ISub>)Subs).IsReadOnly;

    /// <summary>
    /// 通过引索修改Line中Sub内容(错误:字典没有引索)
    /// </summary>
    /// <param name="index">要获得或设置的引索</param>
    /// <returns>引索指定的Sub</returns>
    [Obsolete("错误:字典没有引索")]
    public ISub this[int index]
    {
        get => throw new ArrayTypeMismatchException();
        set => throw new ArrayTypeMismatchException();
    }

    /// <summary>
    /// 将指定的Sub添加到Subs列表的末尾
    /// </summary>
    /// <param name="item">要添加的Sub</param>
    public void Add(ISub item)
    {
        Subs[item.Name] = item;
    }

    /// <summary>
    /// 若无相同名称(Name)的Sub,则将指定的Sub添加到Subs列表的末尾
    /// 若有,则替换成要添加的Sub
    /// </summary>
    /// <param name="newSub">要添加的Sub</param>
    public void AddOrReplaceSub(ISub newSub)
    {
        Subs[newSub.Name] = newSub;
    }

    /// <summary>
    /// 将指定Sub的元素添加到Subs的末尾
    /// </summary>
    /// <param name="newSubs">要添加的多个Sub</param>
    public void AddRange(IEnumerable<ISub> newSubs)
    {
        foreach (var newSub in newSubs)
        {
            Add(newSub);
        }
    }

    /// <summary>
    /// 将指定的Sub添加到Subs列表的末尾
    /// </summary>
    /// <param name="newSub">要添加的Sub</param>
    public void AddSub(ISub newSub)
    {
        Subs[newSub.Name] = newSub;
    }

    /// <summary>
    /// 移除Line中所有的Sub
    /// </summary>
    public void Clear() => Subs.Clear();

    /// <summary>
    /// 返回一个值,该值指示指定的字段是否出现在Subs的Sub的名字
    /// </summary>
    /// <param name="value">字段</param>
    /// <returns>如果在Line集合中找到符合的名字,则为True; 否则为false</returns>
    public bool Contains(string value)
    {
        return Subs.ContainsKey(value);
    }

    /// <summary>
    /// 确定某Sub是否在Line集合中
    /// </summary>
    /// <param name="item">要在Line集合中定位的Sub</param>
    /// <returns>如果在Line集合中找到sub,则为True; 否则为false</returns>
    public bool Contains(ISub item)
    {
        return Subs.ContainsKey(item.Name);
    }

    /// <summary>
    /// 将整个array复制到Line的Subs
    /// </summary>
    /// <param name="array">复制到Subs的Sub列表</param>
    /// <param name="arrayIndex">从零开始的引索,从引索处开始复制</param>
    public void CopyTo(ISub[] array, int arrayIndex)
    {
        for (var i = arrayIndex; i < array.Length; i++)
            Subs[array[i].Name] = array[i];
    }

    /// <summary>
    /// 搜索与指定名称,并返回Line或整个Subs中的第一个匹配元素
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <returns>如果找到相同名称的第一个sub,则为该sub; 否则为null</returns>
    public ISub? Find(string subName)
    {
        return Subs.TryGetValue(subName, out var sub) ? sub : default;
    }

    /// <summary>
    /// 搜索与指定名称,并返回整个Assemblage中的第一个匹配元素
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="subinfo">用于定义匹配的信息 (去除关键字的文本)</param>
    /// <returns>如果找到相同名称和信息的第一个Line,则为该Line; 否则为null</returns>
    public ISub? Find(string subName, string subinfo)
    {
        if (!Subs.TryGetValue(subName, out var v))
            return default;
        
        return v.GetString().Equals(subinfo) ? v : default;
    }

    /// <summary>
    /// 匹配拥有相同名称的Line或sub的所有元素(注意:在字典中,信息是唯一的)
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <returns>如果找到相同名称的sub,其中所有元素均与指定谓词定义的条件匹配,则为该数组; 否则为一个空的Array</returns>
    [Obsolete("注意:在字典中,信息是唯一的")]
    public ISub[] FindAll(string subName)
    {
        return !Subs.TryGetValue(subName, out var v) ? Array.Empty<ISub>() : new[] { v };
    }

    /// <summary>
    /// 匹配拥有相同名称和信息的Line或sub的所有元素(注意:在字典中,信息是唯一的)
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="subInfo">用于定义匹配的信息 (去除关键字的文本)</param>
    /// <returns>如果找到相同名称和信息的sub,其中所有元素均与指定谓词定义的条件匹配,则为该数组; 否则为一个空的Array</returns>
    [Obsolete("注意:在字典中,信息是唯一的")]
    public ISub[] FindAll(string subName, string subInfo)
    {
        return Find(subName, subInfo).ToArray();
    }

    /// <summary>
    /// 匹配拥有相同信息的Line或sub的所有元素(注意:在字典中,信息是唯一的)
    /// </summary>
    /// <param name="subInfo">用于定义匹配的信息 (去除关键字的文本)</param>
    /// <returns>如果找到相同信息的sub,其中所有元素均与指定谓词定义的条件匹配,则为该数组; 否则为一个空的Array</returns>
    [Obsolete("注意:在字典中,信息是唯一的")]
    public ISub[] FindAllInfo(string subInfo)
    {
        return FindInfo(subInfo).ToArray();
    }

    /// <summary>
    /// 搜索与指定信息,并返回整个Assemblage中的第一个匹配元素
    /// </summary>
    /// <param name="subInfo">用于定义匹配的信息 (去除关键字的文本)</param>
    /// <returns>如果找到相同信息的第一个Line,则为该Line; 否则为null</returns>
    public ISub? FindInfo(string subInfo)
    {
        var v = Subs.Values
            .FirstOrDefault(x => x.GetString().Equals(subInfo));
        return v ?? default;
    }

    /// <summary>
    /// 搜索与指定名称,并返回Line或整个Subs中的第一个匹配元素;若未找到,则新建并添加相同名称的Sub,并且返回这个Sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <returns>如果找到相同名称的第一个sub,则为该sub; 否则为新建的相同名称sub</returns>
    public ISub FindOrAdd(string subName)
    {
        var sub = Find(subName);
        if (sub != null)
            return sub;
        
        sub = new Sub();
        sub.Name = subName;
        AddSub(sub);
        return sub;

    }

    /// <summary>
    /// 返回一个 Subs 的第一个元素。
    /// </summary>
    /// <returns>要返回的第一个元素</returns>
    public new ISub First()
    {
        return Subs.Values.First();
    }

    /// <summary>
    /// 返回一个 Subs 的最后一个元素。
    /// </summary>
    /// <returns>要返回的最后一个元素</returns>
    public new ISub Last()
    {
        return Subs.Values.Last();
    }

    #region GETER

    /// <summary>
    /// 搜索与指定名称,并返回Line或整个Subs中的第一个匹配元素;若未找到,则新建并添加相同名称的Sub,并且返回这个Sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <returns>如果找到相同名称的第一个sub,则为该sub; 否则为新建的相同名称sub</returns>
    public ISub this[string subName]
    {
        get => FindOrAdd(subName);
        set => AddOrReplaceSub(value);
    }

    /// <summary>
    /// 获得bool属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="defaultValue">It will be used as result, if the named object not exist</param>
    /// <returns>如果找到相同名称的sub,则为True; 否则为false</returns>
    public bool GetBool(string subName, bool defaultValue = false) =>
        Find(subName)?.GetBoolean() ?? defaultValue;

    /// <summary>
    /// 设置bool属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="value">
    /// 值
    /// </param>
    public void SetBool(string subName, bool value)
    {
        FindOrAdd(subName).SetBoolean(value);
    }

    /// <summary>
    /// 获得int属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="defaultValue">如果没找到返回的默认值</param>
    /// <returns>
    /// 如果找到相同名称的sub,返回sub中储存的int值
    /// 如果没找到,则返回默认值
    /// </returns>
    public int GetInt(string subName, int defaultValue = default)
    {
        var sub = Find(subName);
        return sub?.InfoToInt ?? defaultValue;
    }

    /// <summary>
    /// 设置int属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="value">储存进sub的int值</param>
    public void SetInt(string subName, int value) => FindOrAdd(subName).InfoToInt = value;

    /// <summary>
    /// 获得long属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="defaultValue">如果没找到返回的默认值</param>
    /// <returns>
    /// 如果找到相同名称的sub,返回sub中储存的long值
    /// 如果没找到,则返回默认值
    /// </returns>
    public long GetInt64(string subName, long defaultValue = default)
    {
        var sub = Find(subName);
        return sub?.InfoToInt64 ?? defaultValue;
    }

    /// <summary>
    /// 设置long属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="value">储存进sub的long值</param>
    public void SetInt64(string subName, long value) => FindOrAdd(subName).InfoToInt64 = value;

    /// <summary>
    /// 获得double(long)属性的sub 通过转换long获得更精确的小数,小数位最大保留9位
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="defaultValue">如果没找到返回的默认值</param>
    /// <returns>
    /// 如果找到相同名称的sub,返回sub中储存的double(long)值
    /// 如果没找到,则返回默认值
    /// </returns>
    public float GetFloat(string subName, float defaultValue = default)
    {
        var sub = Find(subName);
        return sub?.GetFloat() ?? defaultValue;
    }

    /// <summary>
    /// 设置double(long)属性的sub 通过转换long获得更精确的小数,小数位最大保留9位
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="value">储存进sub的double(long)值</param>
    public void SetFloat(string subName, float value) => FindOrAdd(subName).SetFloat(value);

    /// <summary>
    /// 获得DateTime属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="defaultValue">如果没找到返回的默认值</param>
    /// <returns>
    /// 如果找到相同名称的sub,返回sub中储存的DateTime值
    /// 如果没找到,则返回默认值
    /// </returns>
    public DateTime GetDateTime(string subName, DateTime defaultValue = default)
    {
        var sub = Find(subName);
        return sub?.GetDateTime() ?? defaultValue;
    }

    /// <summary>
    /// 设置DateTime属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="value">储存进sub的DateTime值</param>
    public void SetDateTime(string subName, DateTime value) => FindOrAdd(subName).SetDateTime(value);

    /// <summary>
    /// 获得String属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="defaultValue">如果没找到返回的默认值</param>
    /// <returns>
    /// 如果找到相同名称的sub,返回sub中储存的String值
    /// 如果没找到,则返回默认值
    /// </returns>
    public string? GetString(string subName, string? defaultValue = default)
    {
        var sub = Find(subName);
        return sub == null ? defaultValue : sub.Info;
    }

    /// <summary>
    /// 设置String属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="value">储存进sub的String值</param>
    public void SetString(string subName, string? value) => FindOrAdd(subName).Info = value ?? "";

    /// <summary>
    /// 获得double属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="defaultValue">如果没找到返回的默认值</param>
    /// <returns>
    /// 如果找到相同名称的sub,返回sub中储存的double值
    /// 如果没找到,则返回默认值
    /// </returns>
    public double GetDouble(string subName, double defaultValue = default)
    {
        var sub = Find(subName);
        return sub?.InfoToDouble ?? defaultValue;
    }

    /// <summary>
    /// 设置double属性的sub
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <param name="value">储存进sub的double值</param>
    public void SetDouble(string subName, double value) => FindOrAdd(subName).InfoToDouble = value;

    /// <summary>
    /// 通过名字和信息创建新的Line
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">信息 (正常)</param>
    /// <param name="text">文本 在末尾没有结束行号的文本 (正常)</param>
    /// <param name="subs">子类集合</param>
    public void Load(string name, string info, string text = "", params ISub[] subs)
    {
        Load(name, info);
        Text = text;
        AddRange(subs);
    }

    /// <summary>
    /// 通过名字和信息创建新的Line
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">信息 (正常)</param>
    /// <param name="text">文本 在末尾没有结束行号的文本 (正常)</param>
    /// <param name="subs">子类集合</param>
    public void Load(string name, string info, IEnumerable<ISub> subs, string text = "")
    {
        Load(name, info);
        Text = text;
        AddRange(subs);
    }

    /// <summary>
    /// 将其他Line内容拷贝到本Line
    /// </summary>
    /// <param name="line">其他line</param>
    public void Load(ILine line)
    {
        Name = line.Name;
        RawInfo = (SetObject)line.infoCloneable.Clone();

        RawText = line.RawText;
        AddRange(line.ToArray());
    }

    /// <summary>
    /// 退回Text的反转义文本 (正常显示)
    /// </summary>
    /// <returns>Text的反转义文本 (正常显示)</returns>
    public string GetText()
    {
        return Text;
    }

    /// <summary>
    /// 将指定的Sub添加到指定索引处(失效:字典没有顺序)
    /// </summary>
    /// <param name="index">应插入 Sub 的从零开始的索引(失效)</param>
    /// <param name="newSub">要添加的Sub</param>
    [Obsolete("失效:字典没有顺序")]
    public void InsertSub(int index, ISub newSub)
    {
        Add(newSub);
    }

    /// <summary>
    /// 将指定Sub的元素添加指定索引处(失效:字典没有顺序)
    /// </summary>
    /// <param name="index">应插入 Sub 的从零开始的索引</param>
    /// <param name="newSubs">要添加的多个Sub</param>
    [Obsolete("失效:字典没有顺序")]
    public void InsertRange(int index, IEnumerable<ISub> newSubs)
    {
        AddRange(newSubs);
    }

    /// <summary>
    /// 从Subs中移除特定名称
    /// </summary>
    /// <param name="subName">要从Subs中删除的Sub的名称</param>
    /// <returns>如果成功移除了Sub,则为 true; 否则为 false</returns>
    public bool Remove(string subName)
    {
        return Subs.Remove(subName);
    }

    /// <summary>
    /// 从Subs中移除特定名称的所有元素(失效:字典为单一性)
    /// </summary>
    /// <param name="subName">要从Subs中删除的Sub的名称</param>
    [Obsolete("失效:字典没有顺序")]
    public void RemoveAll(string subName)
    {
        Subs.Remove(subName);
    }

    /// <summary>
    /// 搜索全部相似名称的Sub的所有元素
    /// </summary>
    /// <param name="value">%字段%</param>
    /// <returns>如果找到相似名称的Sub,则为数组; 否则为一个空的Array</returns>
    public ISub[] SearchAll(string value) => Subs.Values
        .Where(su => su.Name.Contains(value)).ToArray();

    /// <summary>
    /// 搜索字段是否出现在Line名称,并返回整个Subs中的第一个匹配元素
    /// </summary>
    /// <param name="value">%字段%</param>
    /// <returns>如果找到相似名称的第一个Sub,则为该Sub; 否则为null</returns>
    public ISub? Search(string value)
    {
        return Subs.Values.FirstOrDefault(x => x.Name.Contains(value));
    }

    /// <summary>
    /// 搜索相同名称的Sub,并返回整个Subs中第一个匹配的sub从零开始的索引(错误:字典没有引索)
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <returns>如果找到相同名称的sub的第一个元素,则为该元素的从零开始的索引; 否则为 -1</returns>
    [Obsolete("错误:字典没有引索")]
    public int IndexOf(string subName)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 搜索相同名称的Sub,并返回整个Subs中第一个匹配的Sub从零开始的索引(错误:字典没有引索)
    /// </summary>
    /// <param name="item">用于定义匹配的Sub</param>
    /// <returns>如果找到相同名称的Sub的第一个元素,则为该元素的从零开始的索引; 否则为 -1</returns>
    [Obsolete("错误:字典没有引索")]
    public int IndexOf(ISub item)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 搜索相同名称的Sub,并返回整个Sub中全部匹配的sub从零开始的索引(错误:字典没有引索)
    /// </summary>
    /// <param name="subName">用于定义匹配的名称</param>
    /// <returns>如果找到相同名称的sub的元素,则为该元素的从零开始的索引组; 否则为空的Array</returns>
    [Obsolete("错误:字典没有引索")]
    public int[] IndexsOf(string subName)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 将当前Line转换成文本格式 (info已经被转义/去除关键字)
    /// </summary>
    /// <returns>Line的文本格式 (info已经被转义/去除关键字)</returns>
    public void ToString(StringBuilder str)
    {
        str.Append('\n' + Name);
        var infostorestr = RawInfo.GetStoreString();
        if (infostorestr != "")
            str.Append('#' + infostorestr);
        if (str.Length != 0)
            str.Append(":|");
        foreach (ISub su in Subs.Values)
            str.Append(su.ToString());
        str.Append(RawText);
        if (Comments != "")
        {
            str.Append("///");
            str.Append(Comments);
        }
    }

    /// <summary>
    /// 将当前Line转换成文本格式 (info已经被转义/去除关键字)
    /// </summary>
    /// <returns>Line的文本格式 (info已经被转义/去除关键字)</returns>
    public override string ToString()
    {
        var str = new StringBuilder(Name);
        var infostorestr = RawInfo.GetStoreString();
        if (infostorestr != "")
            str.Append('#' + infostorestr);
        if (str.Length != 0)
            str.Append(":|");
        foreach (ISub su in Subs.Values)
            str.Append(su.ToString());
        str.Append(RawText);
        if (Comments != "")
        {
            str.Append("///");
            str.Append(Comments);
        }

        return str.ToString();
    }

    /// <summary>
    /// 返回一个新List,包含所有Subs
    /// </summary>
    /// <returns>所有储存的Subs</returns>
    public List<ISub> ToList()
    {
        return Subs.Values.ToList();
    }

    /// <summary>
    /// 加载 通过lps文本创建一个子类
    /// </summary>
    /// <param name="lps">lps文本</param>
    public override void Load(string lps)
    {
        var sts = Split(lps, "///", 2).ToArray();
        if (sts.Length == 2)
            Comments = sts[1];
        sts = Split(sts[0], ":|").ToArray();
        var st = sts[0].Split(new[] { '#' }, 2); //第一个
        Name = st[0];
        RawInfo = new SetObject();
        if (st.Length > 1)
            RawInfo.SetString(st[1]); //lpstext都是转义后(无关键字)

        RawText = sts.Last(); //最后一个

        for (var i = 1; i < sts.Length - 1; i++)
        {
            Add(new Sub(sts[i]));
        }
    }

    /// <summary>
    /// 获得该Line的长哈希代码
    /// </summary>
    /// <returns>64位哈希代码</returns>
    public override long GetLongHashCode()
    {
        return GetHashCode(Name) * 2 + 
               GetHashCode(RawInfo) * 3 + 
               GetHashCode(RawText) * 4 + 
               Subs.Values.Sum(su => su.GetLongHashCode());
    }

    /// <summary>
    /// 获得该Line的哈希代码
    /// </summary>
    /// <returns>32位哈希代码</returns>
    public override int GetHashCode() => (int)GetLongHashCode();


    /// <summary>
    /// 将指定的Sub添加到指定索引处 (失效:字典没有顺序)
    /// </summary>
    /// <param name="index">应插入 Sub 的从零开始的索引(失效)</param>
    /// <param name="item">要添加的Sub</param>
    [Obsolete("失效:字典没有顺序")]
    public void Insert(int index, ISub item)
    {
        Add(item);
    }

    /// <summary>
    /// 从Subs中移除特定引索的Sub (错误:字典没有顺序)
    /// </summary>
    /// <param name="index">要删除Sub的引索</param>
    [Obsolete("错误:字典没有顺序")]
    public void RemoveAt(int index)
    {
        throw new ArrayTypeMismatchException();
    }

    /// <summary>
    /// 从Subs中移除特定对象的第一个匹配项
    /// </summary>
    /// <param name="item">要从Subs中删除的Sub</param>
    /// <returns>如果成功移除了Sub,则为 true; 否则为 false</returns>
    public bool Remove(ISub item)
    {
        return Subs.Remove(item.Name);
    }

    /// <summary>
    /// 返回循环访问 Subs 的枚举数。
    /// </summary>
    /// <returns>用于 Subs 的枚举数</returns>
    public new IEnumerator<ISub> GetEnumerator()
    {
        return Subs.Values.GetEnumerator();
    }

    /// <summary>
    /// 将当前line与另一个line进行比较,并退回一个整数指示在排序位置中是位于另一个对象之前之后还是相同位置
    /// </summary>
    /// <param name="other">另一个line</param>
    /// <returns>值小于零时排在 other 之前 值等于零时出现在相同排序位置 值大于零则排在 other 之后</returns>
    public int CompareTo(ILine? other)
    {
        if (other == null)
            return int.MaxValue;
        var comp = string.Compare(Name, other.Name, StringComparison.InvariantCulture);
        if (comp != 0)
            return comp;
        comp = other.infoComparable.CompareTo(RawInfo);
        return comp != 0 ? comp : string
            .Compare(ToString(), other?.ToString(), StringComparison.InvariantCulture);
    }

    /// <summary>
    /// 将当前line与另一个line进行比较, 判断是否内容相同
    /// </summary>
    /// <param name="other">另一个line</param>
    /// <returns>如果为True则内容相同</returns>
    public bool Equals(ILine? other) => CompareTo(other) == 0;

    #endregion

    #region GOBJ

    /// <summary>
    /// 获取或设置 String 属性的sub
    /// </summary>
    /// <param name="subName">(gstr)用于定义匹配的名称</param>
    /// <returns>获取或设置对 String 属性的Sub</returns>
    public string? this[gstr subName]
    {
        get => GetString((string)subName);
        set => SetString((string)subName, value);
    }

    /// <summary>
    /// 获取或设置 Bool 属性的sub
    /// </summary>
    /// <param name="subName">(gbol)用于定义匹配的名称</param>
    /// <returns>获取或设置对 bool 属性的Sub</returns>
    public bool this[gbol subName]
    {
        get => GetBool((string)subName);
        set => SetBool((string)subName, value);
    }

    /// <summary>
    /// 获取或设置 Int 属性的sub
    /// </summary>
    /// <param name="subName">(gint)用于定义匹配的名称</param>
    /// <returns>获取或设置对 int 属性的Sub</returns>
    public int this[gint subName]
    {
        get => GetInt((string)subName);
        set => SetInt((string)subName, value);
    }

    /// <summary>
    /// 获取或设置 Long 属性的sub
    /// </summary>
    /// <param name="subName">(gi64)用于定义匹配的名称</param>
    /// <returns>获取或设置对 long 属性的Sub</returns>
    public long this[gi64 subName]
    {
        get => GetInt64((string)subName);
        set => SetInt64((string)subName, value);
    }

    /// <summary>
    /// 获取或设置 Double 属性的sub
    /// </summary>
    /// <param name="subName">(gdbe)用于定义匹配的名称</param>
    /// <returns>获取或设置对 double 属性的Sub</returns>
    public double this[gdbe subName]
    {
        get => GetDouble((string)subName);
        set => SetDouble((string)subName, value);
    }

    /// <summary>
    /// 获取或设置 Double(long) 属性的sub  通过转换long获得更精确的小数,小数位最大保留9位
    /// </summary>
    /// <param name="subName">(gflt)用于定义匹配的名称</param>
    /// <returns>获取或设置对 double 属性的Sub</returns>
    public float this[gflt subName]
    {
        get => GetFloat((string)subName);
        set => SetFloat((string)subName, value);
    }

    /// <summary>
    /// 获取或设置 DateTime 属性的sub
    /// </summary>
    /// <param name="subName">(gdbe)用于定义匹配的名称</param>
    /// <returns>获取或设置对 double 属性的Sub</returns>
    public DateTime this[gdat subName]
    {
        get => GetDateTime((string)subName);
        set => SetDateTime((string)subName, value);
    }

    #endregion
}

/// <summary>
/// 通过字典类型的行, Name不会重复 
/// </summary>
public class Line_D : Line_D<Dictionary<string, ISub>>
{
    /// <summary>
    /// 创建一行
    /// </summary>
    public Line_D()
    {
    }

    /// <summary>
    /// 通过lpsLine文本创建一行
    /// </summary>
    /// <param name="lpsLine">lpsSub文本</param>
    public Line_D(string lpsLine) : base(lpsLine)
    {
    }

    /// <summary>
    /// 通过其他Line创建新的Line
    /// </summary>
    /// <param name="line">其他line</param>
    public Line_D(ILine line) : base(line)
    {
    }

    /// <summary>
    /// 通过名字和信息创建新的Line
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">信息 (正常)</param>
    /// <param name="text">文本 在末尾没有结束行号的文本 (正常)</param>
    /// <param name="subs">子类集合</param>
    public Line_D(string name, string info, string text = "", params ISub[] subs) : base(name, info, text, subs)
    {
    }


    /// <summary>
    /// 通过名字和信息创建新的Line
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">信息 (正常)</param>
    /// <param name="text">文本 在末尾没有结束行号的文本 (正常)</param>
    /// <param name="subs">子类集合</param>
    public Line_D(string name, string info, IEnumerable<ISub> subs, string text = "") : base(name, info, subs, text)
    {
    }
}