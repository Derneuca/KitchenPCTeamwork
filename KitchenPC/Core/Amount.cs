namespace KitchenPC
{
    using System;

    public class Amount : IEquatable<Amount>
    {
        public Amount(float size, Units unit)
        {
            this.SizeHigh = size;
            this.Unit = unit;
        }

        public Amount()
            : this(0, Units.Unit)
        {
        }

        public Amount(float? fromUnit, float toUnit, Units unit)
        {
            this.SizeLow = fromUnit;
            this.SizeHigh = toUnit;
            this.Unit = unit;
        }

        public Amount(Amount amount)
            : this(amount.SizeLow, amount.SizeHigh, amount.Unit)
        {
        }

        public float? SizeLow { get; set; }

        public float SizeHigh { get; set; }

        public Units Unit { get; set; }

        public static Amount operator *(Amount amount, float coefficient)
        {
            var result = new Amount(amount.SizeLow * coefficient, amount.SizeHigh * coefficient, amount.Unit);
            return result;
        }

        public static Amount operator /(Amount amount, float denominator)
        {
            var result = new Amount(amount.SizeLow / denominator, amount.SizeHigh / denominator, amount.Unit);
            return result;
        }

        public static Amount operator +(Amount amount, float addition)
        {
            var result = new Amount(amount.SizeLow + addition, amount.SizeHigh + addition, amount.Unit);
            return result;
        }

        public static Amount operator -(Amount amount, float subtrahend)
        {
            var result = new Amount(amount.SizeLow - subtrahend, amount.SizeHigh - subtrahend, amount.Unit);
            return result;
        }

        public static Amount operator +(Amount firstAmount, Amount secondAmount)
        {
            if (firstAmount.Unit == secondAmount.Unit)
            {
                if (firstAmount.SizeLow.HasValue && secondAmount.SizeLow.HasValue)
                {
                    var result = 
                        new Amount(
                            firstAmount.SizeLow + secondAmount.SizeLow, 
                            firstAmount.SizeHigh + secondAmount.SizeHigh, 
                            firstAmount.Unit);
                    return result;
                }

                if (firstAmount.SizeLow.HasValue)
                {
                    var result = 
                        new Amount(
                            firstAmount.SizeLow + secondAmount.SizeHigh, 
                            firstAmount.SizeHigh + secondAmount.SizeHigh, 
                            firstAmount.Unit);
                    return result;
                }

                if (secondAmount.SizeLow.HasValue)
                {
                    var result = 
                        new Amount(
                            firstAmount.SizeHigh + secondAmount.SizeLow, 
                            firstAmount.SizeHigh + secondAmount.SizeHigh, 
                            firstAmount.Unit);
                    return result;
                }
                
                var resultAmount = 
                    new Amount(
                        firstAmount.SizeHigh + secondAmount.SizeHigh, 
                        firstAmount.Unit);
                return resultAmount;
            }

            if (UnitConverter.CanConvert(firstAmount.Unit, secondAmount.Unit))
            {
                // TODO: Handle range + nonrange
                var newLow = 
                    secondAmount.SizeLow.HasValue 
                    ? (float?)UnitConverter.Convert(secondAmount.SizeLow.Value, secondAmount.Unit, firstAmount.Unit) 
                    : null;
                var newHigh = firstAmount.SizeHigh + UnitConverter.Convert(secondAmount.SizeHigh, secondAmount.Unit, firstAmount.Unit);
                var result = new Amount(newLow, newHigh, firstAmount.Unit);
                return result;
            }

            throw new IncompatibleAmountException();
        }

        public static bool operator ==(Amount firstAmount, Amount secondAmount)
        {
            return firstAmount.Equals(secondAmount);
        }

        public static bool operator !=(Amount firstAmount, Amount secondAmount)
        {
            return firstAmount.Equals(secondAmount);
        }

        /// <summary>Attempts to find a more suitable unit for this amount.</summary>
        public static Amount Normalize(Amount amount, float multiplier)
        {
            var result = new Amount(amount) * multiplier;
            float lowSize = result.SizeLow.GetValueOrDefault();
            float highSize = result.SizeHigh;

            if (KitchenPC.Unit.GetConvertionType(result.Unit) == UnitType.Weight)
            {
                if (result.Unit == Units.Ounce &&
                    lowSize % 16 + highSize % 16 == 0)
                {
                    result /= 16;
                    result.Unit = Units.Pound;
                }
            }

            if (KitchenPC.Unit.GetConvertionType(result.Unit) == UnitType.Volume)
            {
                // If teaspoons, convert to Tlb (3tsp in 1Tbl)
                if (result.Unit == Units.Teaspoon &&
                    lowSize % 3 + highSize % 3 == 0)
                {
                    result /= 3;
                    result.Unit = Units.Tablespoon;
                }

                // If Fl Oz, convert to cup (8 fl oz in 1 cup)
                if (result.Unit == Units.FluidOunce &&
                    lowSize % 8 + highSize % 8 == 0)
                {
                    result /= 8;
                    result.Unit = Units.Cup;
                }

                // If pints, convert to quarts (2 pints in a quart)
                if (result.Unit == Units.Pint &&
                    lowSize % 2 + highSize % 2 == 0)
                {
                    result /= 2;
                    result.Unit = Units.Quart;
                }

                // If quarts, convert to gallons (4 quarts in a gallon)
                if (result.Unit == Units.Quart &&
                    lowSize % 4 + highSize % 4 == 0)
                {
                    result /= 4;
                    result.Unit = Units.Gallon;
                }
            }

            return result;
        }

        public Amount Round(int decimalPlaces)
        {
            var result = new Amount(this);
            result.SizeLow =
                this.SizeLow.HasValue
                ? (float?)Math.Round(this.SizeLow.Value, decimalPlaces)
                : null;
            result.SizeHigh = (float)Math.Round(this.SizeHigh, decimalPlaces);
            return result;
        }

        public Amount RoundUp(float nearestMultiple)
        {
            var result = new Amount(this);
            result.SizeLow =
                this.SizeLow.HasValue
                ? (float?)(Math.Ceiling(this.SizeLow.Value / nearestMultiple) * nearestMultiple)
                : null;
            result.SizeHigh = (float)Math.Ceiling(this.SizeHigh / nearestMultiple) * nearestMultiple;
            return result;
        }

        public string ToString(string unit)
        {
            string highSize;
            string lowSize;

            if (KitchenPC.Unit.GetConvertionType(this.Unit) == UnitType.Weight)
            {
                // Render in decimal
                highSize = Math.Round(this.SizeHigh, 2).ToString();
                lowSize = this.SizeLow.HasValue ? Math.Round(this.SizeLow.Value, 2).ToString() : null;
            }
            else
            {
                // Render in fractions
                highSize = Fractions.FromDecimal((decimal)this.SizeHigh);
                lowSize = this.SizeLow.HasValue ? Fractions.FromDecimal((decimal)this.SizeLow.Value) : null;
            }

            var amount =
                lowSize != null
                ? string.Format("{0} - {1}", lowSize, highSize)
                : highSize;
            return string.Format("{0} {1}", amount, unit).Trim();
        }

        public override string ToString()
        {
            return this.ToString(
                this.SizeLow.HasValue || this.SizeHigh > 1
                ? KitchenPC.Unit.GetPlural(this.Unit) 
                : KitchenPC.Unit.GetSingular(this.Unit));
        }

        public override bool Equals(object obj)
        {
            var other = obj as Amount;
            if (object.ReferenceEquals(other, null) ||
                object.ReferenceEquals(this, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return 
                other.SizeLow == this.SizeLow && 
                other.SizeHigh == this.SizeHigh && 
                other.Unit == this.Unit;
        }

        public override int GetHashCode()
        {
            return this.SizeLow.GetHashCode() ^ this.SizeHigh.GetHashCode();
        }
    }
}