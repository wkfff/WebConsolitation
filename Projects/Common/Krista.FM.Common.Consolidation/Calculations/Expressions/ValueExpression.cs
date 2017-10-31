using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class ValueExpression : ConsRelationExpression
    {
        public ValueExpression()
        {
        }

        public ValueExpression(object value, ValueTypes type)
        {
            Value = value;
            Type = type;
        }

        public ValueExpression(object value)
        {
            switch (System.Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    Type = ValueTypes.Boolean;
                    break;

                case TypeCode.DateTime:
                    Type = ValueTypes.DateTime;
                    break;

                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    Type = ValueTypes.Float;
                    break;

                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    Type = ValueTypes.Integer;
                    break;

                case TypeCode.String:
                    Type = ValueTypes.String;
                    break;

                default:
                    throw new EvaluationException("This value could not be handled: " + value);
            }

            Value = value;
        }

        public ValueExpression(string value)
        {
            Value = value;
            Type = ValueTypes.String;
        }

        public ValueExpression(int value)
        {
            Value = value;
            Type = ValueTypes.Integer;
        }

        public ValueExpression(float value)
        {
            Value = value;
            Type = ValueTypes.Float;
        }

        public ValueExpression(DateTime value)
        {
            Value = value;
            Type = ValueTypes.DateTime;
        }

        public ValueExpression(bool value)
        {
            Value = value;
            Type = ValueTypes.Boolean;
        }

        [DataMember]
        [ProtoMember(1)]
        public ValueTypes Type { get; set; }

        [DataMember]
        public object Value { get; set; }

        [ProtoMember(2)]
        public string ValueSerialized
        {
            get
            {
                return Convert.ToString(Value);
            }

            set
            {
                switch (Type)
                {
                    case ValueTypes.Boolean:
                        Value = Boolean.Parse(value);
                        break;
                    case ValueTypes.DateTime:
                        Value = DateTime.Parse(value);
                        break;
                    case ValueTypes.Float:
                        Value = Decimal.Parse(value);
                        break;
                    case ValueTypes.Integer:
                        Value = Int32.Parse(value);
                        break;
                    case ValueTypes.String:
                        Value = value;
                        break;
                }
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
