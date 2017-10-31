﻿using System;

namespace Krista.FM.Common.Consolidation.Calculations.Helpers
{
    public class Numbers
    {
        public static object Add(object a, object b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            a = ConvertIfString(a);
            b = ConvertIfString(b);

            TypeCode typeCodeA = Type.GetTypeCode(a.GetType());
            TypeCode typeCodeB = Type.GetTypeCode(b.GetType());

            switch (typeCodeA)
            {
                case TypeCode.Boolean:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'bool'");
                        case TypeCode.Byte: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.SByte: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Int16: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.UInt16: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Int32: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.UInt32: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Int64: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Single: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Double: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
                    }

                    break;
                case TypeCode.Byte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'byte' and 'bool'");
                        case TypeCode.Byte: return (byte)a + (byte)b;
                        case TypeCode.SByte: return (byte)a + (sbyte)b;
                        case TypeCode.Int16: return (byte)a + (short)b;
                        case TypeCode.UInt16: return (byte)a + (ushort)b;
                        case TypeCode.Int32: return (byte)a + (int)b;
                        case TypeCode.UInt32: return (byte)a + (uint)b;
                        case TypeCode.Int64: return (byte)a + (long)b;
                        case TypeCode.UInt64: return (byte)a + (ulong)b;
                        case TypeCode.Single: return (byte)a + (float)b;
                        case TypeCode.Double: return (byte)a + (double)b;
                        case TypeCode.Decimal: return (byte)a + (decimal)b;
                    }

                    break;
                case TypeCode.SByte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'sbyte' and 'bool'");
                        case TypeCode.Byte: return (sbyte)a + (byte)b;
                        case TypeCode.SByte: return (sbyte)a + (sbyte)b;
                        case TypeCode.Int16: return (sbyte)a + (short)b;
                        case TypeCode.UInt16: return (sbyte)a + (ushort)b;
                        case TypeCode.Int32: return (sbyte)a + (int)b;
                        case TypeCode.UInt32: return (sbyte)a + (uint)b;
                        case TypeCode.Int64: return (sbyte)a + (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'sbyte' and 'ulong'");
                        case TypeCode.Single: return (sbyte)a + (float)b;
                        case TypeCode.Double: return (sbyte)a + (double)b;
                        case TypeCode.Decimal: return (sbyte)a + (decimal)b;
                    }

                    break;

                case TypeCode.Int16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'short' and 'bool'");
                        case TypeCode.Byte: return (short)a + (byte)b;
                        case TypeCode.SByte: return (short)a + (sbyte)b;
                        case TypeCode.Int16: return (short)a + (short)b;
                        case TypeCode.UInt16: return (short)a + (ushort)b;
                        case TypeCode.Int32: return (short)a + (int)b;
                        case TypeCode.UInt32: return (short)a + (uint)b;
                        case TypeCode.Int64: return (short)a + (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'short' and 'ulong'");
                        case TypeCode.Single: return (short)a + (float)b;
                        case TypeCode.Double: return (short)a + (double)b;
                        case TypeCode.Decimal: return (short)a + (decimal)b;
                    }

                    break;

                case TypeCode.UInt16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ushort' and 'bool'");
                        case TypeCode.Byte: return (ushort)a + (byte)b;
                        case TypeCode.SByte: return (ushort)a + (sbyte)b;
                        case TypeCode.Int16: return (ushort)a + (short)b;
                        case TypeCode.UInt16: return (ushort)a + (ushort)b;
                        case TypeCode.Int32: return (ushort)a + (int)b;
                        case TypeCode.UInt32: return (ushort)a + (uint)b;
                        case TypeCode.Int64: return (ushort)a + (long)b;
                        case TypeCode.UInt64: return (ushort)a + (ulong)b;
                        case TypeCode.Single: return (ushort)a + (float)b;
                        case TypeCode.Double: return (ushort)a + (double)b;
                        case TypeCode.Decimal: return (ushort)a + (decimal)b;
                    }

                    break;

                case TypeCode.Int32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'int' and 'bool'");
                        case TypeCode.Byte: return (int)a + (byte)b;
                        case TypeCode.SByte: return (int)a + (sbyte)b;
                        case TypeCode.Int16: return (int)a + (short)b;
                        case TypeCode.UInt16: return (int)a + (ushort)b;
                        case TypeCode.Int32: return (int)a + (int)b;
                        case TypeCode.UInt32: return (int)a + (uint)b;
                        case TypeCode.Int64: return (int)a + (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'int' and 'ulong'");
                        case TypeCode.Single: return (int)a + (float)b;
                        case TypeCode.Double: return (int)a + (double)b;
                        case TypeCode.Decimal: return (int)a + (decimal)b;
                    }

                    break;

                case TypeCode.UInt32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'unit' and 'bool'");
                        case TypeCode.Byte: return (uint)a + (byte)b;
                        case TypeCode.SByte: return (uint)a + (sbyte)b;
                        case TypeCode.Int16: return (uint)a + (short)b;
                        case TypeCode.UInt16: return (uint)a + (ushort)b;
                        case TypeCode.Int32: return (uint)a + (int)b;
                        case TypeCode.UInt32: return (uint)a + (uint)b;
                        case TypeCode.Int64: return (uint)a + (long)b;
                        case TypeCode.UInt64: return (uint)a + (ulong)b;
                        case TypeCode.Single: return (uint)a + (float)b;
                        case TypeCode.Double: return (uint)a + (double)b;
                        case TypeCode.Decimal: return (uint)a + (decimal)b;
                    }

                    break;

                case TypeCode.Int64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'long' and 'bool'");
                        case TypeCode.Byte: return (long)a + (byte)b;
                        case TypeCode.SByte: return (long)a + (sbyte)b;
                        case TypeCode.Int16: return (long)a + (short)b;
                        case TypeCode.UInt16: return (long)a + (ushort)b;
                        case TypeCode.Int32: return (long)a + (int)b;
                        case TypeCode.UInt32: return (long)a + (uint)b;
                        case TypeCode.Int64: return (long)a + (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'long' and 'ulong'");
                        case TypeCode.Single: return (long)a + (float)b;
                        case TypeCode.Double: return (long)a + (double)b;
                        case TypeCode.Decimal: return (long)a + (decimal)b;
                    }

                    break;

                case TypeCode.UInt64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'bool'");
                        case TypeCode.Byte: return (ulong)a + (byte)b;
                        case TypeCode.SByte: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'sbyte'");
                        case TypeCode.Int16: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'short'");
                        case TypeCode.UInt16: return (ulong)a + (ushort)b;
                        case TypeCode.Int32: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'int'");
                        case TypeCode.UInt32: return (ulong)a + (uint)b;
                        case TypeCode.Int64: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'ulong'");
                        case TypeCode.UInt64: return (ulong)a + (ulong)b;
                        case TypeCode.Single: return (ulong)a + (float)b;
                        case TypeCode.Double: return (ulong)a + (double)b;
                        case TypeCode.Decimal: return (ulong)a + (decimal)b;
                    }

                    break;

                case TypeCode.Single:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'float' and 'bool'");
                        case TypeCode.Byte: return (float)a + (byte)b;
                        case TypeCode.SByte: return (float)a + (sbyte)b;
                        case TypeCode.Int16: return (float)a + (short)b;
                        case TypeCode.UInt16: return (float)a + (ushort)b;
                        case TypeCode.Int32: return (float)a + (int)b;
                        case TypeCode.UInt32: return (float)a + (uint)b;
                        case TypeCode.Int64: return (float)a + (long)b;
                        case TypeCode.UInt64: return (float)a + (ulong)b;
                        case TypeCode.Single: return (float)a + (float)b;
                        case TypeCode.Double: return (float)a + (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'float' and 'decimal'");
                    }

                    break;

                case TypeCode.Double:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'double' and 'bool'");
                        case TypeCode.Byte: return (double)a + (byte)b;
                        case TypeCode.SByte: return (double)a + (sbyte)b;
                        case TypeCode.Int16: return (double)a + (short)b;
                        case TypeCode.UInt16: return (double)a + (ushort)b;
                        case TypeCode.Int32: return (double)a + (int)b;
                        case TypeCode.UInt32: return (double)a + (uint)b;
                        case TypeCode.Int64: return (double)a + (long)b;
                        case TypeCode.UInt64: return (double)a + (ulong)b;
                        case TypeCode.Single: return (double)a + (float)b;
                        case TypeCode.Double: return (double)a + (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'double' and 'decimal'");
                    }

                    break;

                case TypeCode.Decimal:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'decimal' and 'bool'");
                        case TypeCode.Byte: return (decimal)a + (byte)b;
                        case TypeCode.SByte: return (decimal)a + (sbyte)b;
                        case TypeCode.Int16: return (decimal)a + (short)b;
                        case TypeCode.UInt16: return (decimal)a + (ushort)b;
                        case TypeCode.Int32: return (decimal)a + (int)b;
                        case TypeCode.UInt32: return (decimal)a + (uint)b;
                        case TypeCode.Int64: return (decimal)a + (long)b;
                        case TypeCode.UInt64: return (decimal)a + (ulong)b;
                        case TypeCode.Single: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'decimal' and 'float'");
                        case TypeCode.Double: throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'decimal' and 'double'");
                        case TypeCode.Decimal: return (decimal)a + (decimal)b;
                    }

                    break;
            }

            return null;
        }

        public static object Subtract(object a, object b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            a = ConvertIfString(a);
            b = ConvertIfString(b);

            TypeCode typeCodeA = Type.GetTypeCode(a.GetType());
            TypeCode typeCodeB = Type.GetTypeCode(b.GetType());

            switch (typeCodeA)
            {
                case TypeCode.Boolean:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'bool'");
                        case TypeCode.Byte: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.SByte: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Int16: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.UInt16: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Int32: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.UInt32: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Int64: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Single: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Double: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
                    }

                    break;
                case TypeCode.Byte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'byte' and 'bool'");
                        case TypeCode.SByte: return (byte)a - (sbyte)b;
                        case TypeCode.Int16: return (byte)a - (short)b;
                        case TypeCode.UInt16: return (byte)a - (ushort)b;
                        case TypeCode.Int32: return (byte)a - (int)b;
                        case TypeCode.UInt32: return (byte)a - (uint)b;
                        case TypeCode.Int64: return (byte)a - (long)b;
                        case TypeCode.UInt64: return (byte)a - (ulong)b;
                        case TypeCode.Single: return (byte)a - (float)b;
                        case TypeCode.Double: return (byte)a - (double)b;
                        case TypeCode.Decimal: return (byte)a - (decimal)b;
                    }

                    break;
                case TypeCode.SByte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'sbyte' and 'bool'");
                        case TypeCode.SByte: return (sbyte)a - (sbyte)b;
                        case TypeCode.Int16: return (sbyte)a - (short)b;
                        case TypeCode.UInt16: return (sbyte)a - (ushort)b;
                        case TypeCode.Int32: return (sbyte)a - (int)b;
                        case TypeCode.UInt32: return (sbyte)a - (uint)b;
                        case TypeCode.Int64: return (sbyte)a - (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'sbyte' and 'ulong'");
                        case TypeCode.Single: return (sbyte)a - (float)b;
                        case TypeCode.Double: return (sbyte)a - (double)b;
                        case TypeCode.Decimal: return (sbyte)a - (decimal)b;
                    }

                    break;

                case TypeCode.Int16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'short' and 'bool'");
                        case TypeCode.SByte: return (short)a - (sbyte)b;
                        case TypeCode.Int16: return (short)a - (short)b;
                        case TypeCode.UInt16: return (short)a - (ushort)b;
                        case TypeCode.Int32: return (short)a - (int)b;
                        case TypeCode.UInt32: return (short)a - (uint)b;
                        case TypeCode.Int64: return (short)a - (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'short' and 'ulong'");
                        case TypeCode.Single: return (short)a - (float)b;
                        case TypeCode.Double: return (short)a - (double)b;
                        case TypeCode.Decimal: return (short)a - (decimal)b;
                    }

                    break;

                case TypeCode.UInt16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ushort' and 'bool'");
                        case TypeCode.SByte: return (ushort)a - (sbyte)b;
                        case TypeCode.Int16: return (ushort)a - (short)b;
                        case TypeCode.UInt16: return (ushort)a - (ushort)b;
                        case TypeCode.Int32: return (ushort)a - (int)b;
                        case TypeCode.UInt32: return (ushort)a - (uint)b;
                        case TypeCode.Int64: return (ushort)a - (long)b;
                        case TypeCode.UInt64: return (ushort)a - (ulong)b;
                        case TypeCode.Single: return (ushort)a - (float)b;
                        case TypeCode.Double: return (ushort)a - (double)b;
                        case TypeCode.Decimal: return (ushort)a - (decimal)b;
                    }

                    break;

                case TypeCode.Int32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'int' and 'bool'");
                        case TypeCode.SByte: return (int)a - (sbyte)b;
                        case TypeCode.Int16: return (int)a - (short)b;
                        case TypeCode.UInt16: return (int)a - (ushort)b;
                        case TypeCode.Int32: return (int)a - (int)b;
                        case TypeCode.UInt32: return (int)a - (uint)b;
                        case TypeCode.Int64: return (int)a - (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'int' and 'ulong'");
                        case TypeCode.Single: return (int)a - (float)b;
                        case TypeCode.Double: return (int)a - (double)b;
                        case TypeCode.Decimal: return (int)a - (decimal)b;
                    }

                    break;

                case TypeCode.UInt32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'uint' and 'bool'");
                        case TypeCode.SByte: return (uint)a - (sbyte)b;
                        case TypeCode.Int16: return (uint)a - (short)b;
                        case TypeCode.UInt16: return (uint)a - (ushort)b;
                        case TypeCode.Int32: return (uint)a - (int)b;
                        case TypeCode.UInt32: return (uint)a - (uint)b;
                        case TypeCode.Int64: return (uint)a - (long)b;
                        case TypeCode.UInt64: return (uint)a - (ulong)b;
                        case TypeCode.Single: return (uint)a - (float)b;
                        case TypeCode.Double: return (uint)a - (double)b;
                        case TypeCode.Decimal: return (uint)a - (decimal)b;
                    }

                    break;

                case TypeCode.Int64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'long' and 'bool'");
                        case TypeCode.SByte: return (long)a - (sbyte)b;
                        case TypeCode.Int16: return (long)a - (short)b;
                        case TypeCode.UInt16: return (long)a - (ushort)b;
                        case TypeCode.Int32: return (long)a - (int)b;
                        case TypeCode.UInt32: return (long)a - (uint)b;
                        case TypeCode.Int64: return (long)a - (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'long' and 'ulong'");
                        case TypeCode.Single: return (long)a - (float)b;
                        case TypeCode.Double: return (long)a - (double)b;
                        case TypeCode.Decimal: return (long)a - (decimal)b;
                    }

                    break;

                case TypeCode.UInt64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'bool'");
                        case TypeCode.SByte: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'double'");
                        case TypeCode.Int16: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'short'");
                        case TypeCode.UInt16: return (ulong)a - (ushort)b;
                        case TypeCode.Int32: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'int'");
                        case TypeCode.UInt32: return (ulong)a - (uint)b;
                        case TypeCode.Int64: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'long'");
                        case TypeCode.UInt64: return (ulong)a - (ulong)b;
                        case TypeCode.Single: return (ulong)a - (float)b;
                        case TypeCode.Double: return (ulong)a - (double)b;
                        case TypeCode.Decimal: return (ulong)a - (decimal)b;
                    }

                    break;

                case TypeCode.Single:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'float' and 'bool'");
                        case TypeCode.SByte: return (float)a - (sbyte)b;
                        case TypeCode.Int16: return (float)a - (short)b;
                        case TypeCode.UInt16: return (float)a - (ushort)b;
                        case TypeCode.Int32: return (float)a - (int)b;
                        case TypeCode.UInt32: return (float)a - (uint)b;
                        case TypeCode.Int64: return (float)a - (long)b;
                        case TypeCode.UInt64: return (float)a - (ulong)b;
                        case TypeCode.Single: return (float)a - (float)b;
                        case TypeCode.Double: return (float)a - (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'float' and 'decimal'");
                    }

                    break;

                case TypeCode.Double:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'double' and 'bool'");
                        case TypeCode.SByte: return (double)a - (sbyte)b;
                        case TypeCode.Int16: return (double)a - (short)b;
                        case TypeCode.UInt16: return (double)a - (ushort)b;
                        case TypeCode.Int32: return (double)a - (int)b;
                        case TypeCode.UInt32: return (double)a - (uint)b;
                        case TypeCode.Int64: return (double)a - (long)b;
                        case TypeCode.UInt64: return (double)a - (ulong)b;
                        case TypeCode.Single: return (double)a - (float)b;
                        case TypeCode.Double: return (double)a - (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'double' and 'decimal'");
                    }

                    break;

                case TypeCode.Decimal:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'decimal' and 'bool'");
                        case TypeCode.SByte: return (decimal)a - (sbyte)b;
                        case TypeCode.Int16: return (decimal)a - (short)b;
                        case TypeCode.UInt16: return (decimal)a - (ushort)b;
                        case TypeCode.Int32: return (decimal)a - (int)b;
                        case TypeCode.UInt32: return (decimal)a - (uint)b;
                        case TypeCode.Int64: return (decimal)a - (long)b;
                        case TypeCode.UInt64: return (decimal)a - (ulong)b;
                        case TypeCode.Single: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'decimal' and 'float'");
                        case TypeCode.Double: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'decimal' and 'double'");
                        case TypeCode.Decimal: return (decimal)a - (decimal)b;
                    }

                    break;
            }

            return null;
        }
        
        public static object Multiply(object a, object b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            a = ConvertIfString(a);
            b = ConvertIfString(b);

            TypeCode typeCodeA = Type.GetTypeCode(a.GetType());
            TypeCode typeCodeB = Type.GetTypeCode(b.GetType());

            switch (typeCodeA)
            {
                case TypeCode.Byte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'byte' and 'bool'");
                        case TypeCode.SByte: return (byte)a * (sbyte)b;
                        case TypeCode.Int16: return (byte)a * (short)b;
                        case TypeCode.UInt16: return (byte)a * (ushort)b;
                        case TypeCode.Int32: return (byte)a * (int)b;
                        case TypeCode.UInt32: return (byte)a * (uint)b;
                        case TypeCode.Int64: return (byte)a * (long)b;
                        case TypeCode.UInt64: return (byte)a * (ulong)b;
                        case TypeCode.Single: return (byte)a * (float)b;
                        case TypeCode.Double: return (byte)a * (double)b;
                        case TypeCode.Decimal: return (byte)a * (decimal)b;
                    }

                    break;
                case TypeCode.SByte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'sbyte' and 'bool'");
                        case TypeCode.SByte: return (sbyte)a * (sbyte)b;
                        case TypeCode.Int16: return (sbyte)a * (short)b;
                        case TypeCode.UInt16: return (sbyte)a * (ushort)b;
                        case TypeCode.Int32: return (sbyte)a * (int)b;
                        case TypeCode.UInt32: return (sbyte)a * (uint)b;
                        case TypeCode.Int64: return (sbyte)a * (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'sbyte' and 'ulong'");
                        case TypeCode.Single: return (sbyte)a * (float)b;
                        case TypeCode.Double: return (sbyte)a * (double)b;
                        case TypeCode.Decimal: return (sbyte)a * (decimal)b;
                    }

                    break;

                case TypeCode.Int16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'short' and 'bool'");
                        case TypeCode.SByte: return (short)a * (sbyte)b;
                        case TypeCode.Int16: return (short)a * (short)b;
                        case TypeCode.UInt16: return (short)a * (ushort)b;
                        case TypeCode.Int32: return (short)a * (int)b;
                        case TypeCode.UInt32: return (short)a * (uint)b;
                        case TypeCode.Int64: return (short)a * (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'short' and 'ulong'");
                        case TypeCode.Single: return (short)a * (float)b;
                        case TypeCode.Double: return (short)a * (double)b;
                        case TypeCode.Decimal: return (short)a * (decimal)b;
                    }

                    break;

                case TypeCode.UInt16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ushort' and 'bool'");
                        case TypeCode.SByte: return (ushort)a * (sbyte)b;
                        case TypeCode.Int16: return (ushort)a * (short)b;
                        case TypeCode.UInt16: return (ushort)a * (ushort)b;
                        case TypeCode.Int32: return (ushort)a * (int)b;
                        case TypeCode.UInt32: return (ushort)a * (uint)b;
                        case TypeCode.Int64: return (ushort)a * (long)b;
                        case TypeCode.UInt64: return (ushort)a * (ulong)b;
                        case TypeCode.Single: return (ushort)a * (float)b;
                        case TypeCode.Double: return (ushort)a * (double)b;
                        case TypeCode.Decimal: return (ushort)a * (decimal)b;
                    }

                    break;

                case TypeCode.Int32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'int' and 'bool'");
                        case TypeCode.SByte: return (int)a * (sbyte)b;
                        case TypeCode.Int16: return (int)a * (short)b;
                        case TypeCode.UInt16: return (int)a * (ushort)b;
                        case TypeCode.Int32: return (int)a * (int)b;
                        case TypeCode.UInt32: return (int)a * (uint)b;
                        case TypeCode.Int64: return (int)a * (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'int' and 'ulong'");
                        case TypeCode.Single: return (int)a * (float)b;
                        case TypeCode.Double: return (int)a * (double)b;
                        case TypeCode.Decimal: return (int)a * (decimal)b;
                    }

                    break;

                case TypeCode.UInt32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'uint' and 'bool'");
                        case TypeCode.SByte: return (uint)a * (sbyte)b;
                        case TypeCode.Int16: return (uint)a * (short)b;
                        case TypeCode.UInt16: return (uint)a * (ushort)b;
                        case TypeCode.Int32: return (uint)a * (int)b;
                        case TypeCode.UInt32: return (uint)a * (uint)b;
                        case TypeCode.Int64: return (uint)a * (long)b;
                        case TypeCode.UInt64: return (uint)a * (ulong)b;
                        case TypeCode.Single: return (uint)a * (float)b;
                        case TypeCode.Double: return (uint)a * (double)b;
                        case TypeCode.Decimal: return (uint)a * (decimal)b;
                    }

                    break;

                case TypeCode.Int64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'long' and 'bool'");
                        case TypeCode.SByte: return (long)a * (sbyte)b;
                        case TypeCode.Int16: return (long)a * (short)b;
                        case TypeCode.UInt16: return (long)a * (ushort)b;
                        case TypeCode.Int32: return (long)a * (int)b;
                        case TypeCode.UInt32: return (long)a * (uint)b;
                        case TypeCode.Int64: return (long)a * (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'long' and 'ulong'");
                        case TypeCode.Single: return (long)a * (float)b;
                        case TypeCode.Double: return (long)a * (double)b;
                        case TypeCode.Decimal: return (long)a * (decimal)b;
                    }

                    break;

                case TypeCode.UInt64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'bool'");
                        case TypeCode.SByte: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'sbyte'");
                        case TypeCode.Int16: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'short'");
                        case TypeCode.UInt16: return (ulong)a * (ushort)b;
                        case TypeCode.Int32: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'int'");
                        case TypeCode.UInt32: return (ulong)a * (uint)b;
                        case TypeCode.Int64: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'long'");
                        case TypeCode.UInt64: return (ulong)a * (ulong)b;
                        case TypeCode.Single: return (ulong)a * (float)b;
                        case TypeCode.Double: return (ulong)a * (double)b;
                        case TypeCode.Decimal: return (ulong)a * (decimal)b;
                    }

                    break;

                case TypeCode.Single:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'float' and 'bool'");
                        case TypeCode.SByte: return (float)a * (sbyte)b;
                        case TypeCode.Int16: return (float)a * (short)b;
                        case TypeCode.UInt16: return (float)a * (ushort)b;
                        case TypeCode.Int32: return (float)a * (int)b;
                        case TypeCode.UInt32: return (float)a * (uint)b;
                        case TypeCode.Int64: return (float)a * (long)b;
                        case TypeCode.UInt64: return (float)a * (ulong)b;
                        case TypeCode.Single: return (float)a * (float)b;
                        case TypeCode.Double: return (float)a * (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'float' and 'decimal'");
                    }

                    break;

                case TypeCode.Double:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'double' and 'bool'");
                        case TypeCode.SByte: return (double)a * (sbyte)b;
                        case TypeCode.Int16: return (double)a * (short)b;
                        case TypeCode.UInt16: return (double)a * (ushort)b;
                        case TypeCode.Int32: return (double)a * (int)b;
                        case TypeCode.UInt32: return (double)a * (uint)b;
                        case TypeCode.Int64: return (double)a * (long)b;
                        case TypeCode.UInt64: return (double)a * (ulong)b;
                        case TypeCode.Single: return (double)a * (float)b;
                        case TypeCode.Double: return (double)a * (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'double' and 'decimal'");
                    }

                    break;

                case TypeCode.Decimal:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'decimal' and 'bool'");
                        case TypeCode.SByte: return (decimal)a * (sbyte)b;
                        case TypeCode.Int16: return (decimal)a * (short)b;
                        case TypeCode.UInt16: return (decimal)a * (ushort)b;
                        case TypeCode.Int32: return (decimal)a * (int)b;
                        case TypeCode.UInt32: return (decimal)a * (uint)b;
                        case TypeCode.Int64: return (decimal)a * (long)b;
                        case TypeCode.UInt64: return (decimal)a * (ulong)b;
                        case TypeCode.Single: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'decimal' and 'float'");
                        case TypeCode.Double: throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'decimal' and 'double'");
                        case TypeCode.Decimal: return (decimal)a * (decimal)b;
                    }

                    break;
            }

            return null;
        }
        
        public static object Divide(object a, object b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            a = ConvertIfString(a);
            b = ConvertIfString(b);

            TypeCode typeCodeA = Type.GetTypeCode(a.GetType());
            TypeCode typeCodeB = Type.GetTypeCode(b.GetType());

            switch (typeCodeA)
            {
                case TypeCode.Byte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'byte' and 'bool'");
                        case TypeCode.SByte: return (byte)a / (sbyte)b;
                        case TypeCode.Int16: return (byte)a / (short)b;
                        case TypeCode.UInt16: return (byte)a / (ushort)b;
                        case TypeCode.Int32: return (byte)a / (int)b;
                        case TypeCode.UInt32: return (byte)a / (uint)b;
                        case TypeCode.Int64: return (byte)a / (long)b;
                        case TypeCode.UInt64: return (byte)a / (ulong)b;
                        case TypeCode.Single: return (byte)a / (float)b;
                        case TypeCode.Double: return (byte)a / (double)b;
                        case TypeCode.Decimal: return (byte)a / (decimal)b;
                    }

                    break;
                case TypeCode.SByte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'sbyte' and 'bool'");
                        case TypeCode.SByte: return (sbyte)a / (sbyte)b;
                        case TypeCode.Int16: return (sbyte)a / (short)b;
                        case TypeCode.UInt16: return (sbyte)a / (ushort)b;
                        case TypeCode.Int32: return (sbyte)a / (int)b;
                        case TypeCode.UInt32: return (sbyte)a / (uint)b;
                        case TypeCode.Int64: return (sbyte)a / (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'sbyte' and 'ulong'");
                        case TypeCode.Single: return (sbyte)a / (float)b;
                        case TypeCode.Double: return (sbyte)a / (double)b;
                        case TypeCode.Decimal: return (sbyte)a / (decimal)b;
                    }

                    break;

                case TypeCode.Int16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'short' and 'bool'");
                        case TypeCode.SByte: return (short)a / (sbyte)b;
                        case TypeCode.Int16: return (short)a / (short)b;
                        case TypeCode.UInt16: return (short)a / (ushort)b;
                        case TypeCode.Int32: return (short)a / (int)b;
                        case TypeCode.UInt32: return (short)a / (uint)b;
                        case TypeCode.Int64: return (short)a / (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'short' and 'ulong'");
                        case TypeCode.Single: return (short)a / (float)b;
                        case TypeCode.Double: return (short)a / (double)b;
                        case TypeCode.Decimal: return (short)a / (decimal)b;
                    }

                    break;

                case TypeCode.UInt16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ushort' and 'bool'");
                        case TypeCode.SByte: return (ushort)a / (sbyte)b;
                        case TypeCode.Int16: return (ushort)a / (short)b;
                        case TypeCode.UInt16: return (ushort)a / (ushort)b;
                        case TypeCode.Int32: return (ushort)a / (int)b;
                        case TypeCode.UInt32: return (ushort)a / (uint)b;
                        case TypeCode.Int64: return (ushort)a / (long)b;
                        case TypeCode.UInt64: return (ushort)a / (ulong)b;
                        case TypeCode.Single: return (ushort)a / (float)b;
                        case TypeCode.Double: return (ushort)a / (double)b;
                        case TypeCode.Decimal: return (ushort)a / (decimal)b;
                    }

                    break;

                case TypeCode.Int32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'int' and 'bool'");
                        case TypeCode.SByte: return (int)a / (sbyte)b;
                        case TypeCode.Int16: return (int)a / (short)b;
                        case TypeCode.UInt16: return (int)a / (ushort)b;
                        case TypeCode.Int32: return (int)a / (int)b;
                        case TypeCode.UInt32: return (int)a / (uint)b;
                        case TypeCode.Int64: return (int)a / (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'int' and 'ulong'");
                        case TypeCode.Single: return (int)a / (float)b;
                        case TypeCode.Double: return (int)a / (double)b;
                        case TypeCode.Decimal: return (int)a / (decimal)b;
                    }

                    break;

                case TypeCode.UInt32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'uint' and 'bool'");
                        case TypeCode.SByte: return (uint)a / (sbyte)b;
                        case TypeCode.Int16: return (uint)a / (short)b;
                        case TypeCode.UInt16: return (uint)a / (ushort)b;
                        case TypeCode.Int32: return (uint)a / (int)b;
                        case TypeCode.UInt32: return (uint)a / (uint)b;
                        case TypeCode.Int64: return (uint)a / (long)b;
                        case TypeCode.UInt64: return (uint)a / (ulong)b;
                        case TypeCode.Single: return (uint)a / (float)b;
                        case TypeCode.Double: return (uint)a / (double)b;
                        case TypeCode.Decimal: return (uint)a / (decimal)b;
                    }

                    break;

                case TypeCode.Int64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'long' and 'bool'");
                        case TypeCode.SByte: return (long)a / (sbyte)b;
                        case TypeCode.Int16: return (long)a / (short)b;
                        case TypeCode.UInt16: return (long)a / (ushort)b;
                        case TypeCode.Int32: return (long)a / (int)b;
                        case TypeCode.UInt32: return (long)a / (uint)b;
                        case TypeCode.Int64: return (long)a / (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'long' and 'ulong'");
                        case TypeCode.Single: return (long)a / (float)b;
                        case TypeCode.Double: return (long)a / (double)b;
                        case TypeCode.Decimal: return (long)a / (decimal)b;
                    }

                    break;

                case TypeCode.UInt64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'bool'");
                        case TypeCode.SByte: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ulong' and 'sbyte'");
                        case TypeCode.Int16: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ulong' and 'short'");
                        case TypeCode.UInt16: return (ulong)a / (ushort)b;
                        case TypeCode.Int32: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ulong' and 'int'");
                        case TypeCode.UInt32: return (ulong)a / (uint)b;
                        case TypeCode.Int64: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ulong' and 'long'");
                        case TypeCode.UInt64: return (ulong)a / (ulong)b;
                        case TypeCode.Single: return (ulong)a / (float)b;
                        case TypeCode.Double: return (ulong)a / (double)b;
                        case TypeCode.Decimal: return (ulong)a / (decimal)b;
                    }

                    break;

                case TypeCode.Single:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'float' and 'bool'");
                        case TypeCode.SByte: return (float)a / (sbyte)b;
                        case TypeCode.Int16: return (float)a / (short)b;
                        case TypeCode.UInt16: return (float)a / (ushort)b;
                        case TypeCode.Int32: return (float)a / (int)b;
                        case TypeCode.UInt32: return (float)a / (uint)b;
                        case TypeCode.Int64: return (float)a / (long)b;
                        case TypeCode.UInt64: return (float)a / (ulong)b;
                        case TypeCode.Single: return (float)a / (float)b;
                        case TypeCode.Double: return (float)a / (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'float' and 'decimal'");
                    }

                    break;

                case TypeCode.Double:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'double' and 'bool'");
                        case TypeCode.SByte: return (double)a / (sbyte)b;
                        case TypeCode.Int16: return (double)a / (short)b;
                        case TypeCode.UInt16: return (double)a / (ushort)b;
                        case TypeCode.Int32: return (double)a / (int)b;
                        case TypeCode.UInt32: return (double)a / (uint)b;
                        case TypeCode.Int64: return (double)a / (long)b;
                        case TypeCode.UInt64: return (double)a / (ulong)b;
                        case TypeCode.Single: return (double)a / (float)b;
                        case TypeCode.Double: return (double)a / (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'double' and 'decimal'");
                    }

                    break;

                case TypeCode.Decimal:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'decimal' and 'bool'");
                        case TypeCode.SByte: return (decimal)a / (sbyte)b;
                        case TypeCode.Int16: return (decimal)a / (short)b;
                        case TypeCode.UInt16: return (decimal)a / (ushort)b;
                        case TypeCode.Int32: return (decimal)a / (int)b;
                        case TypeCode.UInt32: return (decimal)a / (uint)b;
                        case TypeCode.Int64: return (decimal)a / (long)b;
                        case TypeCode.UInt64: return (decimal)a / (ulong)b;
                        case TypeCode.Single: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'decimal' and 'float'");
                        case TypeCode.Double: throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'decimal' and 'double'");
                        case TypeCode.Decimal: return (decimal)a / (decimal)b;
                    }

                    break;
            }

            return null;
        }

        public static object Modulo(object a, object b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            a = ConvertIfString(a);
            b = ConvertIfString(b);

            TypeCode typeCodeA = Type.GetTypeCode(a.GetType());
            TypeCode typeCodeB = Type.GetTypeCode(b.GetType());

            switch (typeCodeA)
            {
                case TypeCode.Byte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'byte' and 'bool'");
                        case TypeCode.SByte: return (byte)a % (sbyte)b;
                        case TypeCode.Int16: return (byte)a % (short)b;
                        case TypeCode.UInt16: return (byte)a % (ushort)b;
                        case TypeCode.Int32: return (byte)a % (int)b;
                        case TypeCode.UInt32: return (byte)a % (uint)b;
                        case TypeCode.Int64: return (byte)a % (long)b;
                        case TypeCode.UInt64: return (byte)a % (ulong)b;
                        case TypeCode.Single: return (byte)a % (float)b;
                        case TypeCode.Double: return (byte)a % (double)b;
                        case TypeCode.Decimal: return (byte)a % (decimal)b;
                    }

                    break;
                case TypeCode.SByte:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'sbyte' and 'bool'");
                        case TypeCode.SByte: return (sbyte)a % (sbyte)b;
                        case TypeCode.Int16: return (sbyte)a % (short)b;
                        case TypeCode.UInt16: return (sbyte)a % (ushort)b;
                        case TypeCode.Int32: return (sbyte)a % (int)b;
                        case TypeCode.UInt32: return (sbyte)a % (uint)b;
                        case TypeCode.Int64: return (sbyte)a % (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'sbyte' and 'ulong'");
                        case TypeCode.Single: return (sbyte)a % (float)b;
                        case TypeCode.Double: return (sbyte)a % (double)b;
                        case TypeCode.Decimal: return (sbyte)a % (decimal)b;
                    }

                    break;

                case TypeCode.Int16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'short' and 'bool'");
                        case TypeCode.SByte: return (short)a % (sbyte)b;
                        case TypeCode.Int16: return (short)a % (short)b;
                        case TypeCode.UInt16: return (short)a % (ushort)b;
                        case TypeCode.Int32: return (short)a % (int)b;
                        case TypeCode.UInt32: return (short)a % (uint)b;
                        case TypeCode.Int64: return (short)a % (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'short' and 'ulong'");
                        case TypeCode.Single: return (short)a % (float)b;
                        case TypeCode.Double: return (short)a % (double)b;
                        case TypeCode.Decimal: return (short)a % (decimal)b;
                    }

                    break;

                case TypeCode.UInt16:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ushort' and 'bool'");
                        case TypeCode.SByte: return (ushort)a % (sbyte)b;
                        case TypeCode.Int16: return (ushort)a % (short)b;
                        case TypeCode.UInt16: return (ushort)a % (ushort)b;
                        case TypeCode.Int32: return (ushort)a % (int)b;
                        case TypeCode.UInt32: return (ushort)a % (uint)b;
                        case TypeCode.Int64: return (ushort)a % (long)b;
                        case TypeCode.UInt64: return (ushort)a % (ulong)b;
                        case TypeCode.Single: return (ushort)a % (float)b;
                        case TypeCode.Double: return (ushort)a % (double)b;
                        case TypeCode.Decimal: return (ushort)a % (decimal)b;
                    }

                    break;

                case TypeCode.Int32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'int' and 'bool'");
                        case TypeCode.SByte: return (int)a % (sbyte)b;
                        case TypeCode.Int16: return (int)a % (short)b;
                        case TypeCode.UInt16: return (int)a % (ushort)b;
                        case TypeCode.Int32: return (int)a % (int)b;
                        case TypeCode.UInt32: return (int)a % (uint)b;
                        case TypeCode.Int64: return (int)a % (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'int' and 'ulong'");
                        case TypeCode.Single: return (int)a % (float)b;
                        case TypeCode.Double: return (int)a % (double)b;
                        case TypeCode.Decimal: return (int)a % (decimal)b;
                    }

                    break;

                case TypeCode.UInt32:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'uint' and 'bool'");
                        case TypeCode.SByte: return (uint)a % (sbyte)b;
                        case TypeCode.Int16: return (uint)a % (short)b;
                        case TypeCode.UInt16: return (uint)a % (ushort)b;
                        case TypeCode.Int32: return (uint)a % (int)b;
                        case TypeCode.UInt32: return (uint)a % (uint)b;
                        case TypeCode.Int64: return (uint)a % (long)b;
                        case TypeCode.UInt64: return (uint)a % (ulong)b;
                        case TypeCode.Single: return (uint)a % (float)b;
                        case TypeCode.Double: return (uint)a % (double)b;
                        case TypeCode.Decimal: return (uint)a % (decimal)b;
                    }

                    break;

                case TypeCode.Int64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'long' and 'bool'");
                        case TypeCode.SByte: return (long)a % (sbyte)b;
                        case TypeCode.Int16: return (long)a % (short)b;
                        case TypeCode.UInt16: return (long)a % (ushort)b;
                        case TypeCode.Int32: return (long)a % (int)b;
                        case TypeCode.UInt32: return (long)a % (uint)b;
                        case TypeCode.Int64: return (long)a % (long)b;
                        case TypeCode.UInt64: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'long' and 'ulong'");
                        case TypeCode.Single: return (long)a % (float)b;
                        case TypeCode.Double: return (long)a % (double)b;
                        case TypeCode.Decimal: return (long)a % (decimal)b;
                    }

                    break;

                case TypeCode.UInt64:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'bool'");
                        case TypeCode.SByte: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'sbyte'");
                        case TypeCode.Int16: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'short'");
                        case TypeCode.UInt16: return (ulong)a % (ushort)b;
                        case TypeCode.Int32: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'int'");
                        case TypeCode.UInt32: return (ulong)a % (uint)b;
                        case TypeCode.Int64: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'long'");
                        case TypeCode.UInt64: return (ulong)a % (ulong)b;
                        case TypeCode.Single: return (ulong)a % (float)b;
                        case TypeCode.Double: return (ulong)a % (double)b;
                        case TypeCode.Decimal: return (ulong)a % (decimal)b;
                    }

                    break;

                case TypeCode.Single:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'float' and 'bool'");
                        case TypeCode.SByte: return (float)a % (sbyte)b;
                        case TypeCode.Int16: return (float)a % (short)b;
                        case TypeCode.UInt16: return (float)a % (ushort)b;
                        case TypeCode.Int32: return (float)a % (int)b;
                        case TypeCode.UInt32: return (float)a % (uint)b;
                        case TypeCode.Int64: return (float)a % (long)b;
                        case TypeCode.UInt64: return (float)a % (ulong)b;
                        case TypeCode.Single: return (float)a % (float)b;
                        case TypeCode.Double: return (float)a % (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'float' and 'decimal'");
                    }

                    break;

                case TypeCode.Double:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'double' and 'bool'");
                        case TypeCode.SByte: return (double)a % (sbyte)b;
                        case TypeCode.Int16: return (double)a % (short)b;
                        case TypeCode.UInt16: return (double)a % (ushort)b;
                        case TypeCode.Int32: return (double)a % (int)b;
                        case TypeCode.UInt32: return (double)a % (uint)b;
                        case TypeCode.Int64: return (double)a % (long)b;
                        case TypeCode.UInt64: return (double)a % (ulong)b;
                        case TypeCode.Single: return (double)a % (float)b;
                        case TypeCode.Double: return (double)a % (double)b;
                        case TypeCode.Decimal: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'double' and 'decimal'");
                    }

                    break;

                case TypeCode.Decimal:
                    switch (typeCodeB)
                    {
                        case TypeCode.Boolean: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'decimal' and 'bool'");
                        case TypeCode.SByte: return (decimal)a % (sbyte)b;
                        case TypeCode.Int16: return (decimal)a % (short)b;
                        case TypeCode.UInt16: return (decimal)a % (ushort)b;
                        case TypeCode.Int32: return (decimal)a % (int)b;
                        case TypeCode.UInt32: return (decimal)a % (uint)b;
                        case TypeCode.Int64: return (decimal)a % (long)b;
                        case TypeCode.UInt64: return (decimal)a % (ulong)b;
                        case TypeCode.Single: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'decimal' and 'float'");
                        case TypeCode.Double: throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'decimal' and 'decimal'");
                        case TypeCode.Decimal: return (decimal)a % (decimal)b;
                    }

                    break;
            }

            return null;
        }
        
        public static object Max(object a, object b)
        {
            a = ConvertIfString(a);
            b = ConvertIfString(b);

            if (a == null && b == null)
            {
                return null;
            }

            if (a == null)
            {
                return b;
            }

            if (b == null)
            {
                return a;
            }

            TypeCode typeCodeA = Type.GetTypeCode(a.GetType());

            switch (typeCodeA)
            {
                case TypeCode.Byte:
                    return Math.Max((byte)a, Convert.ToByte(b));
                case TypeCode.SByte:
                    return Math.Max((sbyte)a, Convert.ToSByte(b));
                case TypeCode.Int16:
                    return Math.Max((short)a, Convert.ToInt16(b));
                case TypeCode.UInt16:
                    return Math.Max((ushort)a, Convert.ToUInt16(b));
                case TypeCode.Int32:
                    return Math.Max((int)a, Convert.ToInt32(b));
                case TypeCode.UInt32:
                    return Math.Max((uint)a, Convert.ToUInt32(b));
                case TypeCode.Int64:
                    return Math.Max((long)a, Convert.ToInt64(b));
                case TypeCode.UInt64:
                    return Math.Max((ulong)a, Convert.ToUInt64(b));
                case TypeCode.Single:
                    return Math.Max((float)a, Convert.ToSingle(b));
                case TypeCode.Double:
                    return Math.Max((double)a, Convert.ToDouble(b));
                case TypeCode.Decimal:
                    return Math.Max((decimal)a, Convert.ToDecimal(b));
            }

            return null;
        }
        
        public static object Min(object a, object b)
        {
            a = ConvertIfString(a);
            b = ConvertIfString(b);

            if (a == null && b == null)
            {
                return null;
            }

            if (a == null)
            {
                return b;
            }

            if (b == null)
            {
                return a;
            }

            TypeCode typeCodeA = Type.GetTypeCode(a.GetType());

            switch (typeCodeA)
            {
                case TypeCode.Byte:
                    return Math.Min((byte)a, Convert.ToByte(b));
                case TypeCode.SByte:
                    return Math.Min((sbyte)a, Convert.ToSByte(b));
                case TypeCode.Int16:
                    return Math.Min((short)a, Convert.ToInt16(b));
                case TypeCode.UInt16:
                    return Math.Min((ushort)a, Convert.ToUInt16(b));
                case TypeCode.Int32:
                    return Math.Min((int)a, Convert.ToInt32(b));
                case TypeCode.UInt32:
                    return Math.Min((uint)a, Convert.ToUInt32(b));
                case TypeCode.Int64:
                    return Math.Min((long)a, Convert.ToInt64(b));
                case TypeCode.UInt64:
                    return Math.Min((ulong)a, Convert.ToUInt64(b));
                case TypeCode.Single:
                    return Math.Min((float)a, Convert.ToSingle(b));
                case TypeCode.Double:
                    return Math.Min((double)a, Convert.ToDouble(b));
                case TypeCode.Decimal:
                    return Math.Min((decimal)a, Convert.ToDecimal(b));
            }

            return null;
        }

        private static object ConvertIfString(object s)
        {
            if (s is string || s is char)
            {
                return Decimal.Parse(s.ToString());
            }

            return s;
        }
    }
}
