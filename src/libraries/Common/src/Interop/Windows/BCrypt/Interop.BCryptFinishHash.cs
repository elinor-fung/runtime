// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class BCrypt
    {
        internal static NTSTATUS BCryptFinishHash(SafeBCryptHashHandle hHash, Span<byte> output, int cbOutput, int dwFlags)
        {
            unsafe
            {
                fixed (byte* pbOutput = &MemoryMarshal.GetReference(output))
                {
                    return BCryptFinishHash(hHash, pbOutput, cbOutput, dwFlags);
                }
            }
        }

        [GeneratedDllImport(Libraries.BCrypt, CharSet = CharSet.Unicode)]
        private static unsafe partial NTSTATUS BCryptFinishHash(SafeBCryptHashHandle hHash, byte* pbOutput, int cbOutput, int dwFlags);
    }
}
