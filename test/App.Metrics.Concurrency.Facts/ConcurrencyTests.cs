﻿using System.Collections.Generic;
using System.Threading;
using App.Metrics.Concurrency.Internal;
using FluentAssertions;
using Xunit;

namespace App.Metrics.Concurrency.Facts
{
    public class ConcurrencyTests
    {
        [Theory]
        [
            InlineData(1000000, 16),
            InlineData(1000000, 8),
            InlineData(1000000, 4),
            InlineData(1000000, 2),
            InlineData(1000000, 1),
        ]
        public void Concurrency_AtomicLong_IsCorrectWithConcurrency(long total, int threadCount)
        {
            ConcurrencyTest<AtomicLong, long>(total, threadCount);
        }

        [Theory]
        [
            InlineData(1000000, 16),
            InlineData(1000000, 8),
            InlineData(1000000, 4),
            InlineData(1000000, 2),
            InlineData(1000000, 1),
        ]
        public void int_array_is_correct_with_concurrency(int total, int threadCount)
        {
            var array = new AtomicIntArray(100);
            var thread = new List<Thread>();

            for (var i = 0; i < threadCount; i++)
            {
                thread.Add(new Thread(() =>
                {
                    int index = 0;
                    for (long j = 0; j < total; j++)
                    {
                        array.Increment(index++);
                        if (index == array.Length)
                        {
                            index = 0;
                        }
                    }
                }));
            }

            thread.ForEach(t => t.Start());
            thread.ForEach(t => t.Join());

            long sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array.GetValue(i);
            }

            sum.Should().Be(total * threadCount);
        }

        [Theory]
        [
            InlineData(1000000, 16),
            InlineData(1000000, 8),
            InlineData(1000000, 4),
            InlineData(1000000, 2),
            InlineData(1000000, 1),
        ]
        public void int_is_correct_with_concurrency(int total, int threadCount)
        {
            ConcurrencyTest<AtomicInteger, int>(total, threadCount);
        }

        [Theory]
        [
            InlineData(1000000, 16),
            InlineData(1000000, 8),
            InlineData(1000000, 4),
            InlineData(1000000, 2),
            InlineData(1000000, 1),
        ]
        public void long_array_is_correct_with_concurrency(int total, int threadCount)
        {
            var array = new AtomicLongArray(100);
            var thread = new List<Thread>();

            for (var i = 0; i < threadCount; i++)
            {
                thread.Add(new Thread(() =>
                {
                    int index = 0;
                    for (long j = 0; j < total; j++)
                    {
                        array.Increment(index++);
                        if (index == array.Length)
                        {
                            index = 0;
                        }
                    }
                }));
            }

            thread.ForEach(t => t.Start());
            thread.ForEach(t => t.Join());

            long sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array.GetValue(i);
            }

            sum.Should().Be(total * threadCount);
        }

        [Theory]
        [
            InlineData(1000000, 16),
            InlineData(1000000, 8),
            InlineData(1000000, 4),
            InlineData(1000000, 2),
            InlineData(1000000, 1),
        ]
        public void padded_long_is_correct_with_concurrency(long total, int threadCount)
        {
            ConcurrencyTest<PaddedAtomicLong, long>(total, threadCount);
        }

        [Theory]
        [
            InlineData(1000000, 16),
            InlineData(1000000, 8),
            InlineData(1000000, 4),
            InlineData(1000000, 2),
            InlineData(1000000, 1),
        ]
        public void striped_long_adder_is_correct_with_concurrency(long total, int threadCount)
        {
            ConcurrencyTest<StripedLongAdder, long>(total, threadCount);
        }

        [Theory]
        [
            InlineData(1000000, 16),
            InlineData(1000000, 8),
            InlineData(1000000, 4),
            InlineData(1000000, 2),
            InlineData(1000000, 1),
        ]
        public void thread_local_long_adder_is_correct_with_concurrency(long total, int threadCount)
        {
            ConcurrencyTest<ThreadLocalLongAdder, long>(total, threadCount);
        }

        private static void ConcurrencyTest<T, TU>(long total, int threadCount) where T : IValueAdder<TU>, new()
        {
            var value = new T();
            var thread = new List<Thread>();

            for (var i = 0; i < threadCount; i++)
            {
                thread.Add(new Thread(() =>
                {
                    for (long j = 0; j < total; j++)
                    {
                        value.Increment();
                    }
                }));
            }

            thread.ForEach(t => t.Start());
            thread.ForEach(t => t.Join());

            var result = value.GetValue();
            if (result is int)
            {
                ((object)value.GetValue()).Should().Be((int)total * threadCount);
            }
            else
            {
                ((object)value.GetValue()).Should().Be(total * threadCount);
            }
        }
    }
}