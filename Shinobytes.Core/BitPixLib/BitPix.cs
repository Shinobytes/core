using System;
using System.Runtime.CompilerServices;

namespace Shinobytes.Core.BitPixLib
{
    public class BitPix
    {
        private static readonly Random RandomGenerator = new Random();

        protected BitPix(int width, int height, int colorCount, byte[][] layers)
        {
            if (layers == null || layers.Length == 0) throw new ArgumentException("`layers` cannot be empty. At least 1 layer is required.");
            Width = width;
            Height = height;
            ColorCount = colorCount;
            Layers = layers;
        }

        protected BitPix(int width, int height, int colorCount, params long[] layers)
            : this(width, height, colorCount, LayerFromLongs(layers)) { }


        public int Width { get; }

        public int Height { get; }

        public int ColorCount { get; }

        // layers are BYTE and not BIT
        // so the size will always be: Layers[LAYER_COUNT][BYTE_COUNT], where BYTE_COUNT = PIXEL_COUNT / 8
        public byte[][] Layers { get; }


        public override string ToString()
        {
            return ToString(this);
        }

        public static BitPix FromLong(long l)
        {
            return new BitPix(8, 8, 2, l);
        }

        public static BitPix FromLongs(long[] l)
        {
            return new BitPix(8, 8, BitPixUtilities.NumberOfBitCombinations(l.Length), l.Length);
        }

        public static BitPix CreateEmpty(int colorLayers)
        {
            return new BitPix(8, 8, BitPixUtilities.NumberOfBitCombinations(colorLayers), new long[colorLayers]);
        }

        public static BitPix CreateEmpty(int size, int colorLayers)
        {
            if (size % 2 != 0) throw new ArgumentException("The `size` MUST be divisble by 2.");
            return CreateEmpty(size, size, colorLayers);
        }

        public static BitPix CreateEmpty(int width, int height, int colorLayers)
        {
            if (width * height % 2 != 0) throw new ArgumentException("The `width * height` MUST be divisble by 2.");
            var numOfColors = BitPixUtilities.NumberOfBitCombinations(colorLayers);
            var layers = new byte[colorLayers][];
            for (var i = 0; i < layers.GetLength(0); i++)
            {
                // layers[i] = new byte[((width * height) / 8) * colorLayers];
                layers[i] = new byte[(width * height) / 8];
            }
            return new BitPix(width, height, numOfColors, layers);
        }

        public static BitPix CreateRandom(int size, int colorLayers)
        {
            var insignia = CreateEmpty(size, colorLayers);

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var colorIndex = (byte)RandomGenerator.Next(0, insignia.ColorCount);
                    insignia.SetColorIndexAt(x, y, colorIndex);
                }
            }

            return insignia;
        }

        public static string ToString(BitPix bitpix)
        {
            var lowestValueNum = (byte)'0';
            var returnString = "";
            for (var y = 0; y < bitpix.Height; y++)
            {
                for (var x = 0; x < bitpix.Width; x++)
                {
                    var index = bitpix.GetColorIndexAt(x, y);
                    returnString += (char)(lowestValueNum + index);
                }
            }
            return returnString;
        }

        public static BitPix FromString(string bitpixString)
        {
            if (string.IsNullOrEmpty(bitpixString)) throw new ArgumentNullException(nameof(bitpixString));
            // no reason to go above 8 layers as it wouldnt really be suitable as a bitpix
            // 8 layers means 1 byte per pixel. (full grayscale)
            const int maximumSuitableLayers = 8;

            var widthHeight = (int)Math.Sqrt(bitpixString.Length);
            var layers = 1;
            var lowestValueNum = (byte)'0';
            var highestValueNum = (byte)0;
            // 1. find highest value to determine the layer count
            //    if the highest value is 0, then 1 layer is used.
            for (var i = 0; i < bitpixString.Length; i++)
            {
                var value = (byte)bitpixString[i];
                if (value > highestValueNum)
                    highestValueNum = value;
            }

            var highestValue = highestValueNum - lowestValueNum;
            for (var layer = 1; layer <= maximumSuitableLayers; layer++)
            {
                var lastCount = BitPixUtilities.NumberOfBitCombinations(layer - 1);
                var thisCount = BitPixUtilities.NumberOfBitCombinations(layer);
                if (highestValue >= lastCount && highestValue < thisCount)
                {
                    layers = layer;
                    break;
                }
            }

            // 2. loop through each char of the string and convert it to the number it is.
            var pix = BitPix.CreateEmpty(widthHeight, layers);

            for (var y = 0; y < widthHeight; y++)
            {
                for (var x = 0; x < widthHeight; x++)
                {
                    var charValue = (byte)bitpixString[y * widthHeight + x];
                    var value = (byte)(charValue - lowestValueNum);
                    pix.SetColorIndexAt(x, y, value);
                }
            }

            return pix;
        }

        public static BitPix FromBytes(int width, int height, int layers, byte[] data, int index = 0)
        {
            var pix = BitPix.CreateEmpty(width, height, layers);
            for (var i = 0; i < pix.Layers.GetLength(0); i++)
            {
                for (var j = 0; j < pix.Layers[i].Length; j++)
                {
                    pix.Layers[i][j] = data[index++];
                }
            }
            return pix;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="BitPix"/> by using raw bytes, including the necessary header.
        ///     The first 5 bytes of the `data` - byte array is the header. (Width, Height, LayerCount)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BitPix FromEncodedBytes(byte[] data)
        {
            var index = 0;
            var width = BitConverter.ToInt16(data, 0); // data[index++] | data[index++] << 8;
            var height = BitConverter.ToInt16(data, 2); // data[index++] | data[index++] << 8;
            index += 4;

            var layers = data[index++];
            return FromBytes(width, height, layers, data, index);
        }

        /// <summary>
        ///     Creates an array of raw bytes, including the necessary header.
        ///     The first 5 bytes of the byte array is the header. (Width, Height, LayerCount)
        /// </summary>
        /// <param name="bitpix"></param>
        /// <returns></returns>
        public static byte[] ToEncodedBytes(BitPix bitpix)
        {
            var resultIndex = 0;
            var w = BitPixUtilities.ToBytes((short)bitpix.Width);
            var h = BitPixUtilities.ToBytes((short)bitpix.Height);
            var l = (byte)bitpix.Layers.GetLength(0);

            var result = new byte[2 + 2 + 1 + bitpix.GetTotalByteCount()]; //  w + h + l + img
            result[resultIndex++] = w[0];
            result[resultIndex++] = w[1];
            result[resultIndex++] = h[0];
            result[resultIndex++] = h[1];
            result[resultIndex++] = l;

            var layerCount = bitpix.Layers.GetLength(0);
            for (var i = 0; i < layerCount; i++)
            {
                var layerContentSize = bitpix.Layers[i].Length;
                for (var j = 0; j < layerContentSize; j++)
                {
                    result[resultIndex++] = bitpix.Layers[i][j];
                }
            }

            return result;
        }

        public byte[] ToEncodedBytes()
        {
            return ToEncodedBytes(this);
        }

        public T[] GetPixels<T>(T[] colorPalette)
        {
            if (ColorCount > colorPalette.Length) throw new IndexOutOfRangeException("The provided `colorPalette` is too small. " + ColorCount + " colors were expected ");
            var pixels = new T[Width * Height];
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    pixels[GetBitIndex(x, y)] = colorPalette[GetColorIndexAt(x, y)];
                }
            }
            return pixels;
        }

        public void SetPixels(byte[] colorPaletteIndices)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    SetColorIndexAt(x, y, colorPaletteIndices[GetBitIndex(x, y)]);
                }
            }
        }

        public byte GetColorIndexAt(int x, int y)
        {
            var index = GetBitIndex(x, y);

            var bits = new byte[Layers.Length];

            for (var i = 0; i < Layers.Length; i++)
            {
                bits[i] = BitPixUtilities.ToBits(Layers[i])[index];
            }

            return (byte)GetColorIndexFromBits(bits);
        }

        public int GetColorIndexFromBits(byte[] colorBits)
        {
            for (var i = 0; i < ColorCount; i++)
            {
                var bits = BitPixUtilities.ToBits((byte)i, colorBits.Length);
                if (BitPixUtilities.BitEquals(colorBits, bits)) return i;
            }
            return -1;
        }

        public byte[] GetColorBits(byte colorIndex)
        {
            return BitPixUtilities.ToBits(colorIndex, Layers.Length);
        }

        public void SetColorIndexAt(int x, int y, byte colorIndex)
        {
            var bits = GetColorBits(colorIndex);
            for (var i = 0; i < bits.Length; i++)
            {
                SetColorBitAt(x, y, bits[i] == 1, i);
            }
        }

        public byte GetColorBitAt(int x, int y, int layer)
        {
            var l = Layers[layer];
            var idx = GetBitIndex(x, y);
            var bits = BitPixUtilities.ToBits(l);
            return bits[idx];
        }

        public void SetColorBitAt(int x, int y, bool value, int layer)
        {
            var l = Layers[layer];
            var idx = GetBitIndex(x, y);
            var bits = BitPixUtilities.ToBits(l);

            bits[idx] = (byte)(value ? 1 : 0);

            Layers[layer] = BitPixUtilities.ToBytes(bits);
        }

        public float GetPixelsPerByteCount()
        {
            var bitsPerPixel = Layers.GetLength(0);

            return 8f / bitsPerPixel;
            // / Layers.GetLength(0)
        }

        public int GetTotalByteCount()
        {
            return Layers.GetLength(0) * (Width * Height) / 8;
        }

        private int GetBitIndex(int x, int y)
        {
            return y * Width + x;
        }


        private static byte[][] LayerFromLongs(params long[] layers)
        {
            if (layers == null || layers.Length == 0) throw new ArgumentException("`layers` cannot be empty. At least 1 layer is required.");
            var result = new byte[layers.Length][];
            for (var i = 0; i < layers.Length; i++)
            {
                result[i] = BitPixUtilities.ToBytes(layers[i]);
            }
            return result;
        }


        public BitPix Scale(int widthScale, int heightScale)
        {
            var empty = CreateEmpty(widthScale * Width, heightScale * Height, Layers.Length);

            for (var y = 0; y < Height; y++)
            {
                for (var ys = 0; ys < heightScale; ys++)
                {
                    for (var x = 0; x < Width; x++)
                    {
                        var c = this.GetColorIndexAt(x, y);
                        for (var xs = 0; xs < widthScale; xs++)
                        {
                            empty.SetColorIndexAt(y + ys, x + xs, c);
                        }
                    }
                }
            }
            return empty;
        }

    }
}