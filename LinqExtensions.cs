using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class LinqExtensions
{
    public static int MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        // ReSharper disable once PossibleMultipleEnumeration
        if (source.Any())
        {
            // ReSharper disable once PossibleMultipleEnumeration
            return source.Max(selector);
        }
        else
        {
            return 0;
        }
    }

    public static long MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        // ReSharper disable once PossibleMultipleEnumeration
        if (source.Any())
        {
            // ReSharper disable once PossibleMultipleEnumeration
            return source.Max(selector);
        }
        else
        {
            return 0;
        }
    }

    public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource item)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source is IList<TSource> sourceList)
        {
            return sourceList.IndexOf(item);
        }

        return IndexOf(source, i => Equals(i, item));
    }

    public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        int index = 0;
        foreach (TSource item in source)
        {
            if (predicate(item))
            {
                return index;
            }

            index += 1;
        }

        return -1;
    }

    /// <summary>
    ///     <see href="https://stackoverflow.com/questions/648196/random-row-from-linq-to-sql/648240#648240" />
    /// </summary>
    public static TSource RandomOrDefault<TSource>(this IEnumerable<TSource> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        TSource current = default(TSource);
        int count = 0;
        foreach (TSource element in source)
        {
            count++;
            if (ThreadSafeRandom.Next(count) == 0)
            {
                current = element;
            }
        }

        return count == 0 ? default(TSource) : current;
    }

    /// <summary>
    ///     <see href="https://stackoverflow.com/questions/648196/random-row-from-linq-to-sql/648240#648240" />
    /// </summary>
    public static TSource RandomOrDefault<TSource>(this IEnumerable<TSource> source, Random random)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (random is null)
        {
            throw new ArgumentNullException(nameof(random));
        }

        TSource current = default(TSource);
        int count = 0;
        foreach (TSource element in source)
        {
            count++;
            if (random.Next(count) == 0)
            {
                current = element;
            }
        }

        return count == 0 ? default(TSource) : current;
    }

    /// <summary>
    ///     <see href="https://stackoverflow.com/questions/648196/random-row-from-linq-to-sql/648240#648240" />
    /// </summary>
    public static TSource Random<TSource>(this IEnumerable<TSource> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        TSource current = default(TSource);
        int count = 0;
        foreach (TSource element in source)
        {
            count++;
            if (ThreadSafeRandom.Next(count) == 0)
            {
                current = element;
            }
        }

        if (count == 0)
        {
            throw new InvalidOperationException("Sequence was empty");
        }

        return current;
    }

    /// <summary>
    ///     <see href="https://stackoverflow.com/questions/648196/random-row-from-linq-to-sql/648240#648240" />
    /// </summary>
    public static TSource Random<TSource>(this IEnumerable<TSource> source, Random random)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (random is null)
        {
            throw new ArgumentNullException(nameof(random));
        }

        TSource current = default(TSource);
        int count = 0;
        foreach (TSource element in source)
        {
            count++;
            if (random.Next(count) == 0)
            {
                current = element;
            }
        }

        if (count == 0)
        {
            throw new InvalidOperationException("Sequence was empty");
        }

        return current;
    }

    /// <summary>
    ///  渐进式的随机选择 新元素的选择概率为 元素权重 / 总权重
    /// 
    ///  假设集合有 A B C, 权重为 AW BW CW
    ///  首先判定是否选中A, 选中概率为 AW / AW, 此时 之前元素的权重为 = null, 总权重为 AW 所以新元素的概率是 AW / AW
    ///  其次判定是否选中B, 选中概率为 BW / (AW + BW), 此时 之前元素的权重为 AW, 总权重为 AW + BW 所以新元素的概率是 BW / (AW + BW)
    ///  其次判定是否选中C, 选中概率为 CW / (AW + BW + CW), 此时 之前元素的权重为 AW + BW, 总权重为 AW + BW + CW 所以新元素的概率是 CW / (AW + BW + CW)
    ///
    ///  算法逆向来看 等同于先判定选 C 还是 AB, 然后再判定选 A 还是 B, 渐进式的方式等同于先判定了选择 A 还是 B, 然后再判定选择 C 还是 AB
    ///  如果有新元素出现 就等于在判定选择 D 还是 ABC, 再有新元素 就等于在选择 E 还是 ABCD
    /// </summary>
    public static TSource RandomOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> weightSelector)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (weightSelector is null)
        {
            throw new ArgumentNullException(nameof(weightSelector));
        }

        TSource current = default(TSource);
        double totalWeight = 0.0;
        foreach (TSource element in source)
        {
            double weight = weightSelector(element);
            totalWeight += weight;
            float weightPercent = Mathf.InverseLerp(0, (float)totalWeight, (float)weight);
            if (ThreadSafeRandom.NextSingle() < weightPercent)
            {
                current = element;
            }
        }

        return current;
    }

    /// <summary>
    ///  渐进式的随机选择 新元素的选择概率为 元素权重 / 总权重
    /// 
    ///  假设集合有 A B C, 权重为 AW BW CW
    ///  首先判定是否选中A, 选中概率为 AW / AW, 此时 之前元素的权重为 = null, 总权重为 AW 所以新元素的概率是 AW / AW
    ///  其次判定是否选中B, 选中概率为 BW / (AW + BW), 此时 之前元素的权重为 AW, 总权重为 AW + BW 所以新元素的概率是 BW / (AW + BW)
    ///  其次判定是否选中C, 选中概率为 CW / (AW + BW + CW), 此时 之前元素的权重为 AW + BW, 总权重为 AW + BW + CW 所以新元素的概率是 CW / (AW + BW + CW)
    ///
    ///  算法逆向来看 等同于先判定选 C 还是 AB, 然后再判定选 A 还是 B, 渐进式的方式等同于先判定了选择 A 还是 B, 然后再判定选择 C 还是 AB
    ///  如果有新元素出现 就等于在判定选择 D 还是 ABC, 再有新元素 就等于在选择 E 还是 ABCD
    /// </summary>
    public static TSource RandomOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> weightSelector, Random random)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (weightSelector is null)
        {
            throw new ArgumentNullException(nameof(weightSelector));
        }

        if (random is null)
        {
            throw new ArgumentNullException(nameof(random));
        }

        TSource current = default(TSource);
        double totalWeight = 0.0;
        foreach (TSource element in source)
        {
            double weight = weightSelector(element);
            totalWeight += weight;
            float weightPercent = Mathf.InverseLerp(0, (float)totalWeight, (float)weight);
            if (random.NextDouble() < weightPercent)
            {
                current = element;
            }
        }

        return current;
    }

    /// <summary>
    ///  渐进式的随机选择 新元素的选择概率为 元素权重 / 总权重
    /// 
    ///  假设集合有 A B C, 权重为 AW BW CW
    ///  首先判定是否选中A, 选中概率为 AW / AW, 此时 之前元素的权重为 = null, 总权重为 AW 所以新元素的概率是 AW / AW
    ///  其次判定是否选中B, 选中概率为 BW / (AW + BW), 此时 之前元素的权重为 AW, 总权重为 AW + BW 所以新元素的概率是 BW / (AW + BW)
    ///  其次判定是否选中C, 选中概率为 CW / (AW + BW + CW), 此时 之前元素的权重为 AW + BW, 总权重为 AW + BW + CW 所以新元素的概率是 CW / (AW + BW + CW)
    ///
    ///  算法逆向来看 等同于先判定选 C 还是 AB, 然后再判定选 A 还是 B, 渐进式的方式等同于先判定了选择 A 还是 B, 然后再判定选择 C 还是 AB
    ///  如果有新元素出现 就等于在判定选择 D 还是 ABC, 再有新元素 就等于在选择 E 还是 ABCD
    /// </summary>
    public static TSource Random<TSource>(this IEnumerable<TSource> source, Func<TSource, double> weightSelector)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (weightSelector is null)
        {
            throw new ArgumentNullException(nameof(weightSelector));
        }

        TSource current = default(TSource);
        bool isAnySelected = false;
        double totalWeight = 0.0;
        foreach (TSource element in source)
        {
            double weight = weightSelector(element);
            totalWeight += weight;
            float weightPercent = Mathf.InverseLerp(0, (float)totalWeight, (float)weight);
            if (ThreadSafeRandom.NextSingle() < weightPercent)
            {
                current = element;
                isAnySelected = true;
            }
        }

        if (!isAnySelected)
        {
            throw new InvalidOperationException("Sequence was empty");
        }

        return current;
    }

    /// <summary>
    ///  渐进式的随机选择 新元素的选择概率为 元素权重 / 总权重
    /// 
    ///  假设集合有 A B C, 权重为 AW BW CW
    ///  首先判定是否选中A, 选中概率为 AW / AW, 此时 之前元素的权重为 = null, 总权重为 AW 所以新元素的概率是 AW / AW
    ///  其次判定是否选中B, 选中概率为 BW / (AW + BW), 此时 之前元素的权重为 AW, 总权重为 AW + BW 所以新元素的概率是 BW / (AW + BW)
    ///  其次判定是否选中C, 选中概率为 CW / (AW + BW + CW), 此时 之前元素的权重为 AW + BW, 总权重为 AW + BW + CW 所以新元素的概率是 CW / (AW + BW + CW)
    ///
    ///  算法逆向来看 等同于先判定选 C 还是 AB, 然后再判定选 A 还是 B, 渐进式的方式等同于先判定了选择 A 还是 B, 然后再判定选择 C 还是 AB
    ///  如果有新元素出现 就等于在判定选择 D 还是 ABC, 再有新元素 就等于在选择 E 还是 ABCD
    /// </summary>
    public static TSource Random<TSource>(this IEnumerable<TSource> source, Func<TSource, double> weightSelector, Random random)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (weightSelector is null)
        {
            throw new ArgumentNullException(nameof(weightSelector));
        }

        if (random is null)
        {
            throw new ArgumentNullException(nameof(random));
        }

        TSource current = default(TSource);
        bool isAnySelected = false;
        double totalWeight = 0.0;
        foreach (TSource element in source)
        {
            double weight = weightSelector(element);
            totalWeight += weight;
            float weightPercent = Mathf.InverseLerp(0, (float)totalWeight, (float)weight);
            if (random.NextDouble() < weightPercent)
            {
                current = element;
                isAnySelected = true;
            }
        }

        if (!isAnySelected)
        {
            throw new InvalidOperationException("Sequence was empty");
        }

        return current;
    }
}