using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace LinePutScript;

//TODO: Refactor and rename

/// <summary>
/// 子类 LinePutScript最基础的类
/// </summary>
public class Sub : ISub, ICloneable, IEnumerable, IComparable<ISub>, IEquatable<ISub>
{
    /// <summary>
    /// 创建一个子类
    /// </summary>
    public Sub()
    {
        Name = "";
        RawInfo = new SetObject();
    }

    /// <summary>
    /// 通过lpsSub文本创建一个子类
    /// </summary>
    /// <param name="lpsSub">lpsSub文本</param>
    public Sub(string lpsSub)
    {
        var st = lpsSub.Split(new[] { '#' }, 2);
        Name = st[0];
        this.RawInfo = new SetObject();
        if (st.Length > 1)
            RawInfo.SetString(st[1]);
    }

    /// <summary>
    /// 加载 通过lps文本创建一个子类
    /// </summary>
    /// <param name="lpsSub">lps文本</param>
    public virtual void Load(string lpsSub)
    {
        var st = lpsSub.Split(new[] { '#' }, 2);
        Name = st[0];
        RawInfo = new SetObject();
        if (st.Length > 1)
            RawInfo.SetString(st[1]);
    }

    /// <summary>
    /// 通过名字和信息创建新的Sub
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">信息 (正常版本)</param>
    public Sub(string name, string info)
    {
        Name = name;
        this.RawInfo = new SetObject();
        this.RawInfo.SetString(Sub.TextReplace(info));
    }

    /// <summary>
    /// 通过名字和信息创建新的Sub
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">信息 (SetObject)</param>
    public Sub(string name, SetObject info)
    {
        Name = name;
        this.RawInfo = info;
    }

    /// <summary>
    /// 通过名字和信息创建新的Sub
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">信息 (正常版本)</param>
    public void Load(string name, string info)
    {
        Name = name;
        this.RawInfo = new SetObject();
        this.RawInfo.SetString(Sub.TextReplace(info));
    }

    /// <summary>
    /// 通过名字和信息创建新的Sub
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="info">多个信息 (正常版本)</param>
    public Sub(string name, params string[] info)
    {
        Name = name;
        StringBuilder sb = new StringBuilder();
        foreach (string s in info)
        {
            sb.Append(Sub.TextReplace(s));
            sb.Append(',');
        }

        this.RawInfo = new SetObject();
        this.RawInfo.SetString(sb.ToString().TrimEnd(','));
    }

    /// <summary>
    /// 通过Sub创建新的Sub
    /// </summary>
    /// <param name="sub">其他Sub</param>
    public Sub(ISub sub)
    {
        Name = sub.Name;
        RawInfo = (SetObject)sub.Info;
    }

    /// <summary>
    /// 将其他Sub内容拷贝到本Sub
    /// </summary>
    /// <param name="sub">其他Sub</param>
    public void Set(ISub sub)
    {
        Name = sub.Name;
        Info = (SetObject)sub.Info;
    }

    /// <summary>
    /// 名称 没有唯一性
    /// </summary>
    public string Name { get; set; }

    [Obsolete($"use {nameof(Id)} instead.")]
    public long ID
    {
        get => this.Id;
        set => this.Id = value;
    }

    /// <summary>
    /// ID 根据Name生成 没有唯一性
    /// </summary>
    public long Id
    {
        get => long.TryParse(Name, out long i) ? i : GetHashCode(Name);
        set => Name = value.ToString();
    }

    /// <summary>
    /// 信息 (去除关键字的文本)
    /// </summary>
    public SetObject RawInfo { get; set; }

    string ISub.info
    {
        get => RawInfo;
        set => RawInfo = value;
    }

    ICloneable ISub.infoCloneable => RawInfo;

    IComparable ISub.infoComparable => RawInfo;

    /// <summary>
    /// 信息 (正常)
    /// </summary>
    public string Info
    {
        get => TextDeReplace(RawInfo.GetString());
        set => RawInfo.SetString(TextReplace(value));
    }

    /// <summary>
    /// 获得Info的String结构
    /// </summary>
    public StringStructure Infos
    {
        get
        {
            infos ??= new StringStructure((x) => RawInfo.SetString(x), () => RawInfo.GetString());
            return infos;
        }
    }

    private StringStructure? infos;

    /// <summary>
    /// 信息 (int)
    /// </summary>
    public int InfoToInt
    {
        get => RawInfo.GetInteger();
        set => RawInfo.SetInteger(value);
    }

    /// <summary>
    /// 信息 (int64)
    /// </summary>
    public long InfoToInt64
    {
        get => RawInfo.GetInteger64();
        set => RawInfo.SetInteger64(value);
    }

    /// <summary>
    /// 信息 (double)
    /// </summary>
    public double InfoToDouble
    {
        get => RawInfo.GetDouble();
        set => RawInfo.SetDouble(value);
    }

    /// <summary>
    /// 信息 (bool)
    /// </summary>
    public bool InfoToBoolean
    {
        get => RawInfo.GetBoolean();
        set => RawInfo.SetBoolean(value);
    }


    /// <summary>
    /// 返回循环访问 Info集合 的枚举数。
    /// </summary>
    /// <returns>用于 Info集合 的枚举数</returns>
    public IEnumerator GetEnumerator()
    {
        return GetInfos().GetEnumerator();
    }

    /// <summary>
    /// 返回一个 Info集合 的第一个string。
    /// </summary>
    /// <returns>要返回的第一个string</returns>
    public string? First()
    {
        var subs = GetInfos();
        return subs.Length == 0 ? null : subs[0];
    }

    /// <summary>
    /// 返回一个 Info集合 的最后一个string。
    /// </summary>
    /// <returns>要返回的最后一个string</returns>
    public string? Last()
    {
        var subs = GetInfos();
        return subs.Length == 0 ? null : subs[subs.Length - 1];
    }

    /// <summary>
    /// 退回Info的反转义文本 (正常显示)
    /// </summary>
    /// <returns>info的反转义文本 (正常显示)</returns>
    public string GetInfo()
    {
        return Info;
    }

    /// <summary>
    /// 退回Info集合的转义文本集合 (正常显示)
    /// </summary>
    /// <returns>info的转义文本集合 (正常显示)</returns>
    public string[] GetInfos()
    {
        var sts = RawInfo.GetString().Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        for (var i = 0; i < sts.Length; i++)
            sts[i] = TextDeReplace(sts[i]);
        return sts;
    }

    /// <summary>
    /// 将当前Sub转换成文本格式 (info已经被转义/去除关键字)
    /// </summary>
    /// <returns>Sub的文本格式 (info已经被转义/去除关键字)</returns>
    public override string ToString()
    {
        var str = RawInfo.GetStoreString();
        if (str == "")
            return Name + ":|";
        return Name + '#' + str + ":|";
    }

    #region Interface

    /// <summary>
    /// 获得该Sub的哈希代码
    /// </summary>
    /// <returns>32位哈希代码</returns>
    public override int GetHashCode() => (int)GetLongHashCode();

    /// <summary>
    /// 获得该Sub的长哈希代码
    /// </summary>
    /// <returns>64位哈希代码</returns>
    public virtual long GetLongHashCode() => GetHashCode(Name) * 2 + GetHashCode(RawInfo.GetStoreString()) * 3;

    /// <summary>
    /// 确认对象是否等于当前对象
    /// </summary>
    /// <param name="obj">Subs</param>
    /// <returns></returns>
    public bool Equals(ISub? obj)
    {
        return obj?.GetLongHashCode() == GetLongHashCode();
    }

    /// <summary>
    /// 将当前sub与另一个sub进行比较,并退回一个整数指示在排序位置中是位于另一个对象之前之后还是相同位置
    /// </summary>
    /// <param name="other">另一个sub</param>
    /// <returns>值小于零时排在 other 之前 值等于零时出现在相同排序位置 值大于零则排在 other 之后</returns>
    public int CompareTo(ISub? other)
    {
        if (other == null)
            return int.MaxValue;
        var comp = string.Compare(Name, other.Name, StringComparison.InvariantCulture);
        return comp != 0 ? comp : string.Compare(Info, other.Info, StringComparison.InvariantCulture);
    }

    /// <summary>
    /// 克隆一个Sub
    /// </summary>
    /// <returns>相同的sub</returns>
    public virtual object Clone()
    {
        return new Sub(this);
    }

    #endregion

    #region IGetOBject

    /// <inheritdoc/>
    dynamic ISetObject.Value
    {
        get => RawInfo.Value;
        set => RawInfo.Value = value;
    }

    /// <inheritdoc/>
    public string GetStoreString() => RawInfo.GetStoreString();

    /// <inheritdoc/>
    public string GetString() => Info;

    /// <inheritdoc/>
    public long GetInteger64() => RawInfo.GetInteger64();

    /// <inheritdoc/>
    public int GetInteger() => RawInfo.GetInteger();

    /// <inheritdoc/>
    public double GetDouble() => RawInfo.GetDouble();

    /// <inheritdoc/>
    public float GetFloat() => RawInfo.GetFloat();

    /// <inheritdoc/>
    public DateTime GetDateTime() => RawInfo.GetDateTime();

    /// <inheritdoc/>
    public bool GetBoolean() => RawInfo.GetBoolean();

    /// <inheritdoc/>
    public void SetString(string value) => Info = value;

    /// <inheritdoc/>
    public void SetInteger(int value) => RawInfo.SetInteger(value);

    /// <inheritdoc/>
    public void SetInteger64(long value) => RawInfo.SetInteger64(value);

    /// <inheritdoc/>
    public void SetDouble(double value) => RawInfo.SetDouble(value);

    /// <inheritdoc/>
    public void SetFloat(float value) => RawInfo.SetFloat(value);

    /// <inheritdoc/>
    public void SetDateTime(DateTime value) => RawInfo.SetDateTime(value);

    /// <inheritdoc/>
    public void SetBoolean(bool value) => RawInfo.SetBoolean(value);

    /// <inheritdoc/>
    public int CompareTo(object? obj) => RawInfo.CompareTo(obj);

    /// <inheritdoc/>
    public bool Equals(ISetObject? other) => RawInfo.Equals(other);

    /// <inheritdoc/>
    public int CompareTo(ISetObject? other) => RawInfo.CompareTo(other);

    #endregion

    #region static Function

    /// <summary>
    /// 分割字符串
    /// </summary>
    /// <param name="text">需要分割的文本</param>
    /// <param name="separator">分割符号</param>
    /// <param name="count">分割次数 -1 为无限次数</param>
    /// <returns></returns>
    public static List<string> Split(string text, string separator, int count = -1)
    {
        var list = new List<string>();
        var lastText = text;
        for (var i = 0; i < count || count == -1; i++)
        {
            var iof = lastText?.IndexOf(separator) ?? throw new NullReferenceException();
            if (iof == -1)
            {
                break;
            }

            list.Add(lastText.Substring(0, iof));
            lastText = lastText.Substring(iof + separator.Length);
        }

        list.Add(lastText);
        return list;
    }

    /// <summary>
    /// 将文本进行反转义处理(成为正常显示的文本)
    /// </summary>
    /// <param name="regex">要反转义的文本</param>
    /// <returns>反转义后的文本 正常显示的文本</returns>
    public static string TextDeReplace(string regex)
    {
        if (regex == null)
            return "";
        regex = regex.Replace("/stop", ":|");
        regex = regex.Replace("/equ", "=");
        regex = regex.Replace("/tab", "\t");
        regex = regex.Replace("/n", "\n");
        regex = regex.Replace("/r", "\r");
        regex = regex.Replace("/id", "#");
        regex = regex.Replace("/com", ",");
        regex = regex.Replace("/!", "/");
        regex = regex.Replace("/|", "|");
        return regex;
    }

    /// <summary>
    /// 将文本进行转义处理(成为去除关键字的文本)
    /// </summary>
    /// <param name="regex">要转义的文本</param>
    /// <returns>转义后的文本 (去除关键字的文本)</returns>
    public static string TextReplace(string regex)
    {
        if (regex == null)
            return "";
        regex = regex.Replace("|", "/|");
        regex = regex.Replace("/", "/!");
        regex = regex.Replace(":|", "/stop");
        regex = regex.Replace("\t", "/tab");
        regex = regex.Replace("\n", "/n");
        regex = regex.Replace("\r", "/r");
        regex = regex.Replace("#", "/id");
        regex = regex.Replace(",", "/com");
        //Reptex = Reptex.Replace("=", "/equ");
        return regex;
    }

    /// <summary>
    /// 获取String的HashCode(MD5)
    /// </summary>
    /// <param name="text">String</param>
    /// <returns>HashCode</returns>
    public static long GetHashCode(string text)
    {
        using MD5 md5 = MD5.Create();
        return BitConverter.ToInt64(md5.ComputeHash(Encoding.UTF8.GetBytes(text)), 0);
    }

    #endregion
}