// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class BCrypt
    {
        internal static NTSTATUS BCryptHashData(SafeBCryptHashHandle hHash, ReadOnlySpan<byte> input, int cbInput, int dwFlags)
        {
            unsafe
            {
                fixed (byte* pbInput = &MemoryMarshal.GetReference(input))
                {
                    return BCryptHashData(hHash, pbInput, cbInput, dwFlags);
                }
            }
        }

        [GeneratedDllImport(Libraries.BCrypt, CharSet = CharSet.Unicode)]
        private static unsafe partial NTSTATUS BCryptHashData(SafeBCryptHashHandle hHash, byte* pbInput, int cbInput, int dwFlags);
    }
}
