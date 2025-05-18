using System.Threading;
using UnityEngine;
using Random = System.Random;

/// <summary>
///     NET 6 开始才有 Random.Shared 可以用
///     <see href="https://stackoverflow.com/questions/3049467/is-c-sharp-random-number-generator-thread-safe" />
/// </summary>
public static class ThreadSafeRandom
{
    private static readonly Random GlobalRandom = new();
    private static readonly ThreadLocal<Random> LocalRandom = new(() =>
    {
        lock (GlobalRandom)
        {
            return new Random(GlobalRandom.Next());
        }
    });

    public static int Next() => LocalRandom.Value.Next();

    public static int Next(int max) => LocalRandom.Value.Next(max);

    public static int Next(int min, int max) => LocalRandom.Value.Next(min, max);

    public static float NextSingle() => (float)LocalRandom.Value.NextDouble();

    public static float NextSingle(float max) => NextSingle(0f, max);

    public static float NextSingle(float min, float max) => Mathf.Lerp(min, max, NextSingle());
}