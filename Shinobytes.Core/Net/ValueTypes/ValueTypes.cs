/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

namespace Shinobytes.Core.Net.ValueTypes
{
    public class I8 : SerializableValueType<I8, sbyte> { }
    public class I16 : SerializableValueType<I16, short> { }
    public class I32 : SerializableValueType<I32, int> { }
    public class I64 : SerializableValueType<I64, long> { }

    public class U8 : SerializableValueType<U8, byte> { }
    public class U16 : SerializableValueType<U16, ushort> { }
    public class U32 : SerializableValueType<U32, uint> { }
    public class U64 : SerializableValueType<U64, ulong> { }

    public class F32 : SerializableValueType<F32, float> { }
    public class F64 : SerializableValueType<F64, float> { }
    public class Str : SerializableValueType<Str, string> { }   
}