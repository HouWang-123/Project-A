using System;

public class BitMap
{
    // 每个uint元素包含的位数
    private const int BitsPerElement = 32;
    private readonly uint[] bits;
    // 位图的总位数
    private readonly int size;

    public BitMap(int size)
    {
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "容量不能小于等于0。");
        this.size = size;
        // 计算需要的uint元素数量，向上取整
        int elementCount = (size + BitsPerElement - 1) / BitsPerElement;
        bits = new uint[elementCount];
    }

    // 设置指定索引处的位为1
    public void Set(int index)
    {
        if (index < 0 || index >= size) throw new ArgumentOutOfRangeException(nameof(index), "索引越界。");
        int elementIndex = index / BitsPerElement;
        int bitIndex = index % BitsPerElement;
        // 按位或和左移设置位
        bits[elementIndex] |= (1u << bitIndex);
    }

    // 设置指定索引处的位为0
    public void Clear(int index)
    {
        if (index < 0 || index >= size) throw new ArgumentOutOfRangeException(nameof(index), "索引越界。");
        int elementIndex = index / BitsPerElement;
        int bitIndex = index % BitsPerElement;
        // 按位与和非清除位
        bits[elementIndex] &= ~(1u << bitIndex);
    }

    // 获取指定索引处的位的值
    public bool Get(int index)
    {
        if (index < 0 || index >= size) throw new ArgumentOutOfRangeException(nameof(index), "索引越界。");
        int elementIndex = index / BitsPerElement;
        int bitIndex = index % BitsPerElement;
        // 按位与检查位是否被设置
        return (bits[elementIndex] & (1u << bitIndex)) != 0;
    }

    // 返回位图的位数
    public int GetSize()
    {
        return size;
    }

    // 找到并设置第一个0比特位
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

