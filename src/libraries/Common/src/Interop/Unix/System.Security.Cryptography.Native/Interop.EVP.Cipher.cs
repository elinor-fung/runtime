// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCreate2")]
        internal static partial SafeEvpCipherCtxHandle EvpCipherCreate(
            IntPtr cipher,
            ref byte key,
            int keyLength,
            int effectivekeyLength,
            ref byte iv,
            int enc);

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCreatePartial")]
        internal static partial SafeEvpCipherCtxHandle EvpCipherCreatePartial(
            IntPtr cipher);

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetKeyAndIV")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool EvpCipherSetKeyAndIV(
            SafeEvpCipherCtxHandle ctx,
            byte* key,
            byte* iv,
            EvpCipherDirection direction);

        internal static void EvpCipherSetKeyAndIV(
            SafeEvpCipherCtxHandle ctx,
            ReadOnlySpan<byte> key,
            ReadOnlySpan<byte> iv,
            EvpCipherDirection direction)
        {
            unsafe
            {
                fixed (byte* keyPtr = &MemoryMarshal.GetReference(key))
                fixed (byte* ivPtr = &MemoryMarshal.GetReference(iv))
                {
                    if (!EvpCipherSetKeyAndIV(
                        ctx,
                        keyPtr,
                        ivPtr,
                        direction))
                    {
                        throw CreateOpenSslCryptographicException();
                    }
                }
            }
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetGcmNonceLength")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool CryptoNative_EvpCipherSetGcmNonceLength(
            SafeEvpCipherCtxHandle ctx, int nonceLength);

        internal static void EvpCipherSetGcmNonceLength(SafeEvpCipherCtxHandle ctx, int nonceLength)
        {
            if (!CryptoNative_EvpCipherSetGcmNonceLength(ctx, nonceLength))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetCcmNonceLength")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool CryptoNative_EvpCipherSetCcmNonceLength(
            SafeEvpCipherCtxHandle ctx, int nonceLength);

        internal static void EvpCipherSetCcmNonceLength(SafeEvpCipherCtxHandle ctx, int nonceLength)
        {
            if (!CryptoNative_EvpCipherSetCcmNonceLength(ctx, nonceLength))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherDestroy")]
        internal static extern void EvpCipherDestroy(IntPtr ctx);

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherReset")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool EvpCipherReset(SafeEvpCipherCtxHandle ctx);

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCtxSetPadding")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool EvpCipherCtxSetPadding(SafeEvpCipherCtxHandle x, int padding);

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherUpdate")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool EvpCipherUpdate(
            SafeEvpCipherCtxHandle ctx,
            byte* output,
            out int outl,
            byte* input,
            int inl);

        internal static bool EvpCipherUpdate(
            SafeEvpCipherCtxHandle ctx,
            Span<byte> output,
            out int bytesWritten,
            ReadOnlySpan<byte> input)
        {
            unsafe
            {
                fixed (byte* outputPtr = &MemoryMarshal.GetReference(output))
                fixed (byte* inputPtr = &MemoryMarshal.GetReference(input))
                {
                    return EvpCipherUpdate(
                        ctx,
                        outputPtr,
                        out bytesWritten,
                        inputPtr,
                        input.Length);
                }
            }
        }

        internal static void EvpCipherSetInputLength(SafeEvpCipherCtxHandle ctx, int inputLength)
        {
            unsafe
            {
                fixed (byte* nullRef = &MemoryMarshal.GetReference(Span<byte>.Empty))
                {
                    if (!EvpCipherUpdate(ctx, nullRef, out _, nullRef, inputLength))
                    {
                        throw CreateOpenSslCryptographicException();
                    }
                }
            }
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherFinalEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool EvpCipherFinalEx(
            SafeEvpCipherCtxHandle ctx,
            byte* outm,
            out int outl);

        internal static bool EvpCipherFinalEx(
            SafeEvpCipherCtxHandle ctx,
            Span<byte> output,
            out int bytesWritten)
        {
            unsafe
            {
                fixed (byte* outputPtr = &MemoryMarshal.GetReference(output))
                {
                    return EvpCipherFinalEx(ctx, outputPtr, out bytesWritten);
                }
            }
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherGetGcmTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool EvpCipherGetGcmTag(
            SafeEvpCipherCtxHandle ctx,
            byte* tag,
            int tagLength);

        internal static void EvpCipherGetGcmTag(SafeEvpCipherCtxHandle ctx, Span<byte> tag)
        {
            unsafe
            {
                fixed (byte* tagPtr = &MemoryMarshal.GetReference(tag))
                {
                    if (!EvpCipherGetGcmTag(ctx, tagPtr, tag.Length))
                    {
                        throw CreateOpenSslCryptographicException();
                    }
                }
            }
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherGetAeadTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool EvpCipherGetAeadTag(
            SafeEvpCipherCtxHandle ctx,
            byte* tag,
            int tagLength);

        internal static void EvpCipherGetAeadTag(SafeEvpCipherCtxHandle ctx, Span<byte> tag)
        {
            unsafe
            {
                fixed (byte* tagPtr = &MemoryMarshal.GetReference(tag))
                {
                    if (!EvpCipherGetAeadTag(ctx, tagPtr, tag.Length))
                    {
                        throw CreateOpenSslCryptographicException();
                    }
                }
            }
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetGcmTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool EvpCipherSetGcmTag(
            SafeEvpCipherCtxHandle ctx,
            byte* tag,
            int tagLength);

        internal static void EvpCipherSetGcmTag(SafeEvpCipherCtxHandle ctx, ReadOnlySpan<byte> tag)
        {
            unsafe
            {
                fixed (byte* tagPtr = &MemoryMarshal.GetReference(tag))
                {
                    if (!EvpCipherSetGcmTag(ctx, tagPtr, tag.Length))
                    {
                        throw CreateOpenSslCryptographicException();
                    }
                }
            }
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetAeadTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool EvpCipherSetAeadTag(
            SafeEvpCipherCtxHandle ctx,
            byte* tag,
            int tagLength);

        internal static void EvpCipherSetAeadTag(SafeEvpCipherCtxHandle ctx, ReadOnlySpan<byte> tag)
        {
            unsafe
            {
                fixed (byte* tagPtr = &MemoryMarshal.GetReference(tag))
                {
                    if (!EvpCipherSetAeadTag(ctx, tagPtr, tag.Length))
                    {
                        throw CreateOpenSslCryptographicException();
                    }
                }
            }
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherGetCcmTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool EvpCipherGetCcmTag(
            SafeEvpCipherCtxHandle ctx,
            byte* tag,
            int tagLength);

        internal static void EvpCipherGetCcmTag(SafeEvpCipherCtxHandle ctx, Span<byte> tag)
        {
            unsafe
            {
                fixed (byte* tagPtr = &MemoryMarshal.GetReference(tag))
                {
                    if (!EvpCipherGetCcmTag(ctx, tagPtr, tag.Length))
                    {
                        throw CreateOpenSslCryptographicException();
                    }
                }
            }
        }

        [GeneratedDllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetCcmTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool EvpCipherSetCcmTag(
            SafeEvpCipherCtxHandle ctx,
            byte* tag,
            int tagLength);

        internal static void EvpCipherSetCcmTag(SafeEvpCipherCtxHandle ctx, ReadOnlySpan<byte> tag)
        {
            unsafe
            {
                fixed (byte* tagPtr = &MemoryMarshal.GetReference(tag))
                {
                    if (!EvpCipherSetCcmTag(ctx, tagPtr, tag.Length))
                    {
                        throw CreateOpenSslCryptographicException();
                    }
                }
            }
        }

        internal static void EvpCipherSetCcmTagLength(SafeEvpCipherCtxHandle ctx, int tagLength)
        {
            unsafe
            {
                fixed (byte* nullRef = &MemoryMarshal.GetReference(Span<byte>.Empty))
                {
                    if (!EvpCipherSetCcmTag(ctx, nullRef, tagLength))
                    {
                        throw CreateOpenSslCryptographicException();
                    }
                }
            }
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Ecb")]
        internal static extern IntPtr EvpAes128Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Cbc")]
        internal static extern IntPtr EvpAes128Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Gcm")]
        internal static extern IntPtr EvpAes128Gcm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Cfb8")]
        internal static extern IntPtr EvpAes128Cfb8();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Cfb128")]
        internal static extern IntPtr EvpAes128Cfb128();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Ccm")]
        internal static extern IntPtr EvpAes128Ccm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Ecb")]
        internal static extern IntPtr EvpAes192Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Cbc")]
        internal static extern IntPtr EvpAes192Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Gcm")]
        internal static extern IntPtr EvpAes192Gcm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Cfb8")]
        internal static extern IntPtr EvpAes192Cfb8();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Cfb128")]
        internal static extern IntPtr EvpAes192Cfb128();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Ccm")]
        internal static extern IntPtr EvpAes192Ccm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Ecb")]
        internal static extern IntPtr EvpAes256Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Cbc")]
        internal static extern IntPtr EvpAes256Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Gcm")]
        internal static extern IntPtr EvpAes256Gcm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Cfb128")]
        internal static extern IntPtr EvpAes256Cfb128();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Cfb8")]
        internal static extern IntPtr EvpAes256Cfb8();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Ccm")]
        internal static extern IntPtr EvpAes256Ccm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDesCbc")]
        internal static extern IntPtr EvpDesCbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDesEcb")]
        internal static extern IntPtr EvpDesEcb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDesCfb8")]
        internal static extern IntPtr EvpDesCfb8();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Cbc")]
        internal static extern IntPtr EvpDes3Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Ecb")]
        internal static extern IntPtr EvpDes3Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Cfb8")]
        internal static extern IntPtr EvpDes3Cfb8();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Cfb64")]
        internal static extern IntPtr EvpDes3Cfb64();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpRC2Cbc")]
        internal static extern IntPtr EvpRC2Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpRC2Ecb")]
        internal static extern IntPtr EvpRC2Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpChaCha20Poly1305")]
        internal static extern IntPtr EvpChaCha20Poly1305();

        internal enum EvpCipherDirection : int
        {
            NoChange = -1,
            Decrypt = 0,
            Encrypt = 1,
        }
    }
}
