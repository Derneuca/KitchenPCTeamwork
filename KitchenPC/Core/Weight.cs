namespace KitchenPC
{
    using System;

    public class Weight : IComparable, IFormattable, IComparable<int>, IEquatable<int>
    {
        public Weight()
            : this(0)
        {
        }

        public Weight(int grams)
        {
            this.Value = grams;
        }

        public int Value { get; set; }

        public static implicit operator Weight(int grams)
        {
            return new Weight(grams);
        }

        public static implicit operator int(Weight weight)
        {
            if ((object)weight == null)
            {
                return 0;
            }
            else
            {
                return weight.Value;
            }
        }

        public static bool operator ==(Weight firstWeight, Weight secondWeight)
        {
            if (ReferenceEquals(firstWeight, secondWeight))
            {
                return true;
            }

            if (ReferenceEquals(firstWeight, null) ||
                ReferenceEquals(secondWeight, null))
            {
                return false;
            }

            return firstWeight.Value == secondWeight.Value;
        }

        public static bool operator !=(Weight firstWeight, Weight secondWeight)
        {
            return !(firstWeight == secondWeight);
        }

        public int CompareTo(object obj)
        {
            if (obj is Weight)
            {
                return this.Value.CompareTo(((Weight)obj).Value);
            }
            else
            {
                return this.Value.CompareTo(obj);
            }
        }

        public int CompareTo(int other)
        {
            return this.Value.CompareTo(other);
        }

        public bool Equals(int other)
        {
            return this.Value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is int)
            {
                return this.Value == (int)obj;
            }
            else if (obj is Weight)
            {
                return this == (Weight)obj;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0:f} g.", this.Value);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public new bool Equals(object firstWeight, object secondWeight)
        {
            if (ReferenceEquals(firstWeight, secondWeight))
            {
                return true;
            }

            if (ReferenceEquals(firstWeight, null) ||
                ReferenceEquals(secondWeight, null))
            {
                return false;
            }

            return firstWeight.Equals(secondWeight);
        }
    }
}