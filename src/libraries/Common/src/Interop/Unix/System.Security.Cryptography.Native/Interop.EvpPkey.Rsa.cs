// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [GeneratedDllImport(Libraries.CryptoNative)]
        private static partial SafeEvpPKeyHandle CryptoNative_RsaGenerateKey(int keySize);

        internal static SafeEvpPKeyHandle RsaGenerateKey(int keySize)
        {
            SafeEvpPKeyHandle pkey = CryptoNative_RsaGenerateKey(keySize);

            if (pkey.IsInvalid)
            {
                pkey.Dispose();
                throw CreateOpenSslCryptographicException();
            }

            return pkey;
        }

        [GeneratedDllImport(Libraries.CryptoNative)]
        private static unsafe partial int CryptoNative_RsaDecrypt(
            SafeEvpPKeyHandle pkey,
            byte* source,
            int sourceLength,
            RSAEncryptionPaddingMode paddingMode,
            IntPtr digestAlgorithm,
            byte* destination,
            int destinationLength);

        internal static int RsaDecrypt(
            SafeEvpPKeyHandle pkey,
            ReadOnlySpan<byte> source,
            RSAEncryptionPaddingMode paddingMode,
            IntPtr digestAlgorithm,
            Span<byte> destination)
        {
            int written;
            unsafe
            {
                fixed (byte* sourcePtr = &MemoryMarshal.GetReference(source))
                fixed (byte* destinationPtr = &MemoryMarshal.GetReference(destination))
                {
                    written = CryptoNative_RsaDecrypt(
                        pkey,
                        sourcePtr,
                        source.Length,
                        paddingMode,
                        digestAlgorithm,
                        destinationPtr,
                        destination.Length);
                }
            }

            if (written < 0)
            {
                Debug.Assert(written == -1);
                throw CreateOpenSslCryptographicException();
            }

            return written;
        }

        [GeneratedDllImport(Libraries.CryptoNative)]
        private static unsafe partial int CryptoNative_RsaEncrypt(
            SafeEvpPKeyHandle pkey,
            byte* source,
            int sourceLength,
            RSAEncryptionPaddingMode paddingMode,
            IntPtr digestAlgorithm,
            byte* destination,
            int destinationLength);

        internal static int RsaEncrypt(
            SafeEvpPKeyHandle pkey,
            ReadOnlySpan<byte> source,
            RSAEncryptionPaddingMode paddingMode,
            IntPtr digestAlgorithm,
            Span<byte> destination)
        {
            int written;
            unsafe
            {
                fixed (byte* sourcePtr = &MemoryMarshal.GetReference(source))
                fixed (byte* destinationPtr = &MemoryMarshal.GetReference(destination))
                {
                    written = CryptoNative_RsaEncrypt(
                        pkey,
                        sourcePtr,
                        source.Length,
                        paddingMode,
                        digestAlgorithm,
                        destinationPtr,
                        destination.Length);
                }
            }

            if (written < 0)
            {
                Debug.Assert(written == -1);
                throw CreateOpenSslCryptographicException();
            }

            return written;
        }

        [GeneratedDllImport(Libraries.CryptoNative)]
        private static unsafe partial int CryptoNative_RsaSignHash(
            SafeEvpPKeyHandle pkey,
            RSASignaturePaddingMode paddingMode,
            IntPtr digestAlgorithm,
            byte* hash,
            int hashLength,
            byte* destination,
            int destinationLength);

        internal static int RsaSignHash(
            SafeEvpPKeyHandle pkey,
            RSASignaturePaddingMode paddingMode,
            IntPtr digestAlgorithm,
            ReadOnlySpan<byte> hash,
            Span<byte> destination)
        {
            int written;
            unsafe
            {
                fixed (byte* hashPtr = &MemoryMarshal.GetReference(hash))
                fixed (byte* destinationPtr = &MemoryMarshal.GetReference(destination))
                {
                    written = CryptoNative_RsaSignHash(
                        pkey,
                        paddingMode,
                        digestAlgorithm,
                        hashPtr,
                        hash.Length,
                        destinationPtr,
                        destination.Length);
                }
            }

            if (written < 0)
            {
                Debug.Assert(written == -1);
                throw CreateOpenSslCryptographicException();
            }

            return written;
        }

        [GeneratedDllImport(Libraries.CryptoNative)]
        private static unsafe partial int CryptoNative_RsaVerifyHash(
            SafeEvpPKeyHandle pkey,
            RSASignaturePaddingMode paddingMode,
            IntPtr digestAlgorithm,
            byte* hash,
            int hashLength,
            byte* signature,
            int signatureLength);

        internal static bool RsaVerifyHash(
            SafeEvpPKeyHandle pkey,
            RSASignaturePaddingMode paddingMode,
            IntPtr digestAlgorithm,
            ReadOnlySpan<byte> hash,
            ReadOnlySpan<byte> signature)
        {
            int ret;
            unsafe
            {
                fixed (byte* hashPtr = &MemoryMarshal.GetReference(hash))
                fixed (byte* signaturePtr = &MemoryMarshal.GetReference(signature))
                {
                    ret = CryptoNative_RsaVerifyHash(
                        pkey,
                        paddingMode,
                        digestAlgorithm,
                        hashPtr,
                        hash.Length,
                        signaturePtr,
                        signature.Length);
                }
            }

            if (ret == 1)
            {
                return true;
            }

            if (ret == 0)
            {
                return false;
            }

            Debug.Assert(ret == -1);
            throw CreateOpenSslCryptographicException();
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPkeyGetRsa")]
        internal static partial SafeRsaHandle EvpPkeyGetRsa(SafeEvpPKeyHandle pkey);

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPkeySetRsa")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool EvpPkeySetRsa(SafeEvpPKeyHandle pkey, SafeRsaHandle rsa);

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPkeySetRsa")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool EvpPkeySetRsa(SafeEvpPKeyHandle pkey, IntPtr rsa);
    }
}
