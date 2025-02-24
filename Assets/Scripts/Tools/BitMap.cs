using System;

public class BitMap
{
    // 每个uint元素包含的位数
    private const int BitsPerElement = 32;
    private uint[] bits;
    // 位图的总位数
    private int size;
    // 扩容的比例
    private readonly int resizeRatio = 2;

    public BitMap(int size)
    {
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "容量不能小于等于0。");
        this.size = size;
        // 计算需要的uint元素数量，向上取整
        int elementCount = (size + BitsPerElement - 1) / BitsPerElement;
        bits = new uint[elementCount];
    }

    private void Expansion(int currentSize)
    {
        while (currentSize >= size)
        {
            size *= resizeRatio;
        }
        // 计算需要的uint元素数量，向上取整
        int elementCount = (size + BitsPerElement - 1) / BitsPerElement;
        uint[] temp = new uint[elementCount];
        // 拷贝原有数组
        for (int i = 0; i < size; ++i)
        {
            temp[i] = bits[i];
        }
        bits = temp;
    }

    /// <summary>
    /// 设置指定索引处的位为1
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引越界异常</exception>
    public void Set(int index)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "索引越界。");
        if (index >= size) Expansion(index);
        int elementIndex = index / BitsPerElement;
        int bitIndex = index % BitsPerElement;
        // 按位或和左移设置位
        bits[elementIndex] |= (1u << bitIndex);
    }

    /// <summary>
    /// 设置指定索引处的位为0
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引越界异常</exception>
    public void Clear(int index)
    {
        if (index < 0 || index >= size) throw new ArgumentOutOfRangeException(nameof(index), "索引越界。");
        int elementIndex = index / BitsPerElement;
        int bitIndex = index % BitsPerElement;
        // 按位与和非清除位
        bits[elementIndex] &= ~(1u << bitIndex);
    }

    /// <summary>
    /// 获取指定索引处的位的值
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">索引越界异常</exception>
    public bool Get(int index)
    {
        if (index < 0 || index >= size) throw new ArgumentOutOfRangeException(nameof(index), "索引越界。");
        int elementIndex = index / BitsPerElement;
        int bitIndex = index % BitsPerElement;
        // 按位与检查位是否被设置
        return (bits[elementIndex] & (1u << bitIndex)) != 0;
    }

    /// <summary>
    /// 返回位图的位数
    /// </summary>
    /// <returns>位数</returns>
    public int GetSize()
    {
        return size;
    }

    /// <summary>
    /// 找到并设置第一个0比特位
    /// </summary>
    /// <param name="zeroBitIndex">出参：第一个零比特位的索引</param>
    /// <returns>是否找到</returns>
    public bool SetFirstZero(out int zeroBitIndex)
    {
        for (int i = 0; i < bits.Length; ++i)
        {
            int start = i * BitsPerElement;
            int remainingBits = size - start;
            if (remainingBits <= 0) break;

            int bitsInElement = Math.Min(BitsPerElement, remainingBits);
            uint mask = (bitsInElement == BitsPerElement) ? 0xFFFFFFFFu : (1u << bitsInElement) - 1u;
            uint current = bits[i] & mask;
            uint zeroBitMask = ~current & mask;

            if (zeroBitMask != 0)
            {
                int bitPos = TrailingZeroCount(zeroBitMask);
                zeroBitIndex = start + bitPos;
                bits[i] |= (1u << bitPos);
                return true;
            }
        }
        zeroBitIndex = -1;
        return false; // 遍历完所有元素都没有找到0比特位，返回false
    }
    /// <summary>
    /// 统计0比特位个数
    /// </summary>
    /// <param name="value">数据</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">参数错误</exception>
    private int TrailingZeroCount(uint value)
    {
        if (value == 0) throw new ArgumentException("值不能为0.", nameof(value));
        int count = 0;
        while ((value & 1) == 0)
        {
            value >>= 1;
            count++;
        }
        return count;
    }
}

