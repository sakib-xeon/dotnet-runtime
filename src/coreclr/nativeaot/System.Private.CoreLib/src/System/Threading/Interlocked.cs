// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace System.Threading
{
    public static partial class Interlocked
    {
        #region CompareExchange

        [Intrinsic]
        public static int CompareExchange(ref int location1, int value, int comparand)
        {
#if TARGET_X86 || TARGET_AMD64 || TARGET_ARM64 || TARGET_RISCV64
            return CompareExchange(ref location1, value, comparand); // Must expand intrinsic
#else
            if (Unsafe.IsNullRef(ref location1))
                ThrowHelper.ThrowNullReferenceException();
            return RuntimeImports.InterlockedCompareExchange(ref location1, value, comparand);
#endif
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CompareExchange(ref long location1, long value, long comparand)
        {
#if TARGET_AMD64 || TARGET_ARM64 || TARGET_RISCV64
            return CompareExchange(ref location1, value, comparand); // Must expand intrinsic
#else
            if (Unsafe.IsNullRef(ref location1))
                ThrowHelper.ThrowNullReferenceException();
            return RuntimeImports.InterlockedCompareExchange(ref location1, value, comparand);
#endif
        }

        [Intrinsic]
        [return: NotNullIfNotNull(nameof(location1))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CompareExchange<T>(ref T location1, T value, T comparand) where T : class?
        {
            return Unsafe.As<T>(CompareExchange(ref Unsafe.As<T, object?>(ref location1), value, comparand));
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNullIfNotNull(nameof(location1))]
        public static object? CompareExchange(ref object? location1, object? value, object? comparand)
        {
            if (Unsafe.IsNullRef(ref location1))
                ThrowHelper.ThrowNullReferenceException();
            return RuntimeImports.InterlockedCompareExchange(ref location1, value, comparand);
        }

        #endregion

        #region Exchange

        [Intrinsic]
        public static int Exchange(ref int location1, int value)
        {
#if TARGET_X86 || TARGET_AMD64 || TARGET_ARM64 || TARGET_RISCV64
            return Exchange(ref location1, value); // Must expand intrinsic
#else
            int oldValue;

            do
            {
                oldValue = location1;
            } while (CompareExchange(ref location1, value, oldValue) != oldValue);

            return oldValue;
#endif
        }

        [Intrinsic]
        public static long Exchange(ref long location1, long value)
        {
#if TARGET_AMD64 || TARGET_ARM64 || TARGET_RISCV64
            return Exchange(ref location1, value); // Must expand intrinsic
#else
            long oldValue;

            do
            {
                oldValue = location1;
            } while (CompareExchange(ref location1, value, oldValue) != oldValue);

            return oldValue;
#endif
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNullIfNotNull(nameof(location1))]
        public static T Exchange<T>([NotNullIfNotNull(nameof(value))] ref T location1, T value) where T : class?
        {
            if (Unsafe.IsNullRef(ref location1))
                ThrowHelper.ThrowNullReferenceException();
            return Unsafe.As<T>(RuntimeImports.InterlockedExchange(ref Unsafe.As<T, object?>(ref location1), value));
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNullIfNotNull(nameof(location1))]
        public static object? Exchange([NotNullIfNotNull(nameof(value))] ref object? location1, object? value)
        {
            if (Unsafe.IsNullRef(ref location1))
                ThrowHelper.ThrowNullReferenceException();
            return RuntimeImports.InterlockedExchange(ref location1, value);
        }

        #endregion

        #region Increment

        [Intrinsic]
        public static int Increment(ref int location)
        {
            return ExchangeAdd(ref location, 1) + 1;
        }

        [Intrinsic]
        public static long Increment(ref long location)
        {
            return ExchangeAdd(ref location, 1) + 1;
        }

        #endregion

        #region Decrement

        [Intrinsic]
        public static int Decrement(ref int location)
        {
            return ExchangeAdd(ref location, -1) - 1;
        }

        [Intrinsic]
        public static long Decrement(ref long location)
        {
            return ExchangeAdd(ref location, -1) - 1;
        }

        #endregion

        #region Add

        [Intrinsic]
        public static int Add(ref int location1, int value)
        {
            return ExchangeAdd(ref location1, value) + value;
        }

        [Intrinsic]
        public static long Add(ref long location1, long value)
        {
            return ExchangeAdd(ref location1, value) + value;
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ExchangeAdd(ref int location1, int value)
        {
            int oldValue;

            do
            {
                oldValue = location1;
            } while (CompareExchange(ref location1, oldValue + value, oldValue) != oldValue);

            return oldValue;
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ExchangeAdd(ref long location1, long value)
        {
            long oldValue;

            do
            {
                oldValue = location1;
            } while (CompareExchange(ref location1, oldValue + value, oldValue) != oldValue);

            return oldValue;
        }

        #endregion

        #region Read
        public static long Read(ref long location)
        {
            return CompareExchange(ref location, 0, 0);
        }
        #endregion

        public static void MemoryBarrierProcessWide()
        {
            RuntimeImports.RhFlushProcessWriteBuffers();
        }
    }
}
