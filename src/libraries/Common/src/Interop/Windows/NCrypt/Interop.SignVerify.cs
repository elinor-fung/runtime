// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NCrypt
    {
        internal static unsafe ErrorCode NCryptSignHash(SafeNCryptKeyHandle hKey, void* pPaddingInfo, ReadOnlySpan<byte> hashValue, int cbHashValue, Span<byte> signature, int cbSignature, out int cbResult, AsymmetricPaddingMode dwFlags)
        {
            fixed (byte* pbHashValue = &MemoryMarshal.GetReference(hashValue))
            fixed (byte* pbSignature = &MemoryMarshal.GetReference(signature))
            fixed (int* pcbResult = &cbResult)
            {
                return NCryptSignHash(hKey, pPaddingInfo, pbHashValue, cbHashValue, pbSignature, cbSignature, pcbResult, dwFlags);
            }
        }

        [GeneratedDllImport(Libraries.NCrypt, CharSet = CharSet.Unicode)]
        private static unsafe partial ErrorCode NCryptSignHash(SafeNCryptKeyHandle hKey, void* pPaddingInfo, byte* pbHashValue, int cbHashValue, byte* pbSignature, int cbSignature, int* pcbResult, AsymmetricPaddingMode dwFlags);

        internal static unsafe ErrorCode NCryptVerifySignature(SafeNCryptKeyHandle hKey, void* pPaddingInfo, ReadOnlySpan<byte> hashValue, int cbHashValue, ReadOnlySpan<byte> signature, int cbSignature, AsymmetricPaddingMode dwFlags)
        {
            fixed (byte* pbHashValue = &MemoryMarshal.GetReference(hashValue))
            fixed (byte* pbSignature = &MemoryMarshal.GetReference(signature))
            {
                return NCryptVerifySignature(hKey, pPaddingInfo, pbHashValue, cbHashValue, pbSignature, cbSignature, dwFlags);
            }
        }

        [GeneratedDllImport(Libraries.NCrypt, CharSet = CharSet.Unicode)]
        private static unsafe partial ErrorCode NCryptVerifySignature(SafeNCryptKeyHandle hKey, void* pPaddingInfo, byte* pbHashValue, int cbHashValue, byte* pbSignature, int cbSignature, AsymmetricPaddingMode dwFlags);
    }
}
