// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class BCrypt
    {
        internal static NTSTATUS BCryptCreateHash(SafeBCryptAlgorithmHandle hAlgorithm, out SafeBCryptHashHandle phHash, IntPtr pbHashObject, int cbHashObject, ReadOnlySpan<byte> secret, int cbSecret, BCryptCreateHashFlags dwFlags)
        {
            unsafe
            {
                fixed (byte* pbSecret = &MemoryMarshal.GetReference(secret))
                {
                    return BCryptCreateHash(hAlgorithm, out phHash, pbHashObject, cbHashObject, pbSecret, cbSecret, dwFlags);
                }
            }
        }

        [GeneratedDllImport(Libraries.BCrypt, CharSet = CharSet.Unicode)]
        private static unsafe partial NTSTATUS BCryptCreateHash(SafeBCryptAlgorithmHandle hAlgorithm, out SafeBCryptHashHandle phHash, IntPtr pbHashObject, int cbHashObject, byte* pbSecret, int cbSecret, BCryptCreateHashFlags dwFlags);

        [Flags]
        internal enum BCryptCreateHashFlags : int
        {
            None = 0x00000000,
            BCRYPT_HASH_REUSABLE_FLAG = 0x00000020,
        }
    }
}
