using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Shinobytes.Core.BitPixLib
{
    public class BitPixUtilities
    {
        public static long FromBits(byte[] bits)
        {
            if (bits.Length % 8 != 0) throw new InvalidOperationException("The input byte array `bits` has to be divisible by 8.");
            return FromBytes(ToBytes(bits));
        }

        public static long FromBytes(byte[] bytes)
        {
            if (bytes.Length != 8) throw new InvalidOperationException("The input `bytes` has to contain 8 bytes.");

            return CombineBytes(bytes);
        }

        public static byte SetBit(byte targetByte, int bitNumber, bool bitValue)
        {
            int outputByte = targetByte;
            if (bitValue)
            {
                outputByte |= (1 << (bitNumber));
            }
            else
            {
                outputByte &= ~(1 << (bitNumber));
            }
            return (byte)outputByte;
        }

        public static bool GetBit(byte b, int num)
        {
            return (b & (1 << num)) != 0;
        }

        public static byte[] ToBits(byte[] bytes, int bitsPerByte = 8)
        {
            byte[] bits = new byte[bytes.Length * bitsPerByte];
            var bitIndex = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                for (var j = 0; j < bitsPerByte; j++)
                {
                    var bit = GetBit(bytes[i], j) ? 1 : 0;

                    bits[bitIndex++] = (byte)(bit);
                }

            }
            return bits;
        }

        public static byte[] ToBits(long value)
        {
            return ToBits(ToBytes(value), 8);
        }

        public static byte[] ToBits(byte b, int bitsPerByte = 8)
        {
            return ToBits(new[] { b }, bitsPerByte);
        }

        public static byte[] ToBytes(byte[] bits)
        {
            if (bits.Length % 8 != 0) throw new InvalidOperationException("The input byte array `bits` has to be divisible by 8.");
            var bytes = new byte[bits.Length / 8];
            var bitIndex = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    bytes[i] = SetBit(bytes[i], j, bits[bitIndex++] == 1);
                }
            }
            return bytes;
        }

        public static byte[] ToBytes(long value)
        {
            //var size = sizeof(long);
            //byte[] output = new byte[size];
            //for (int i = size - 1; i >= 0; i--)
            //{
            //    output[i] = (byte)(value & 0xFF);
            //    value >>= size;
            //}
            //return output;
            return BitConverter.GetBytes(value);
        }

        public static byte[] ToBytes(int value)
        {
            //var size = sizeof(int);
            //byte[] output = new byte[size];
            //for (int i = size - 1; i >= 0; i--)
            //{
            //    output[i] = (byte)(value & 0xFF);
            //    value >>= size;
            //}
            //return output;
            return BitConverter.GetBytes(value);
        }

        public static byte[] ToBytes(short value)
        {
            //var size = sizeof(short);
            //byte[] output = new byte[size];
            //for (int i = size - 1; i >= 0; i--)
            //{
            //    output[i] = (byte)(value & 0xFF);
            //    value >>= size;
            //}
            //return output;
            return BitConverter.GetBytes(value);
        }

        public static long CombineBytes(byte[] bytes)
        {
            long output = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                output <<= bytes.Length;
                output |= (byte)(bytes[i] & 0xFF);
            }
            return output;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberOfBitCombinations(int layers)
        {
            return (int)Math.Pow(2, layers);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CombineBits(byte[] bits)
        {
            return CombineBytes(ToBytes(bits));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] BitsFromInteger(int i)
        {
            return ToBits(ToBytes(i), 8);
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BitEquals(byte[] b1, byte[] b2)
        {
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }
    }
}
