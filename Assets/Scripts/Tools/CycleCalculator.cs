using System;

public static class CycleCalculator
{
    /// <summary>
    /// 循环加法
    /// </summary>
    /// <param name="current">当前值（自动规范化到[0,base)区间）</param>
    /// <param name="delta">变化量</param>
    /// <param name="base">进制基数（>=2）</param>
    public static int Add(int current, int delta, int @base)
    {
        ValidateBase(@base);
        current = Normalize(current, @base);
        return (current + delta) % @base < 0
            ? (current + delta) % @base + @base
            : (current + delta) % @base;
    }

    /// <summary>
    /// 循环减法
    /// </summary>
    public static int Subtract(int current, int delta, int @base)
    {
        return Add(current, -delta, @base);
    }

    /// <summary>
    /// 规范化输入值到合法区间
    /// </summary>
    private static int Normalize(int value, int @base)
    {
        value %= @base;
        return value < 0 ? value + @base : value;
    }

    private static void ValidateBase(int @base)
    {
        if (@base < 2)
            throw new ArgumentException("进制基数必须≥2", nameof(@base));
    }
}