// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NCrypt
    {
        internal static unsafe ErrorCode NCryptEncrypt(SafeNCryptKeyHandle hKey, ReadOnlySpan<byte> input, int cbInput, void* pPaddingInfo, Span<byte> output, int cbOutput, out int cbResult, AsymmetricPaddingMode dwFlags)
        {
            fixed (byte* pbInput = &MemoryMarshal.GetReference(input))
            fixed (byte* pbOutput = &MemoryMarshal.GetReference(output))
            fixed (int* pcbResult = &cbResult)
            {
                return NCryptEncrypt(hKey, pbInput, cbInput, pPaddingInfo, pbOutput, cbOutput, pcbResult, dwFlags);
            }
        }

        [GeneratedDllImport(Interop.Libraries.NCrypt, CharSet = CharSet.Unicode)]
        private static unsafe partial ErrorCode NCryptEncrypt(SafeNCryptKeyHandle hKey, byte* pbInput, int cbInput, void* pPaddingInfo, byte* pbOutput, int cbOutput, int* pcbResult, AsymmetricPaddingMode dwFlags);

        internal static unsafe ErrorCode NCryptDecrypt(SafeNCryptKeyHandle hKey, ReadOnlySpan<byte> input, int cbInput, void* pPaddingInfo, Span<byte> output, int cbOutput, out int cbResult, AsymmetricPaddingMode dwFlags)
        {
            fixed (byte* pbInput = &MemoryMarshal.GetReference(input))
            fixed (byte* pbOutput = &MemoryMarshal.GetReference(output))
            fixed (int* pcbResult = &cbResult)
            {
                return NCryptDecrypt(hKey, pbInput, cbInput, pPaddingInfo, pbOutput, cbOutput, pcbResult, dwFlags);
            }
        }

        [GeneratedDllImport(Interop.Libraries.NCrypt, CharSet = CharSet.Unicode)]
        private static unsafe partial ErrorCode NCryptDecrypt(SafeNCryptKeyHandle hKey, byte* pbInput, int cbInput, void* pPaddingInfo, byte* pbOutput, int cbOutput, int* pcbResult, AsymmetricPaddingMode dwFlags);
    }
}
