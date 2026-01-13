namespace TranslateNotation;
using System.Globalization;
using System.Text;
using Alphabet;

public abstract class NumberSystem
{
    public int Base { get; }
    public string Value { get;}
    
    protected NumberSystem(int numberBase, string value)
    {
        Base = numberBase;
        Value = value;
    }
    
    public abstract DecimalNumber ToDecimal();
    public abstract NumberSystem ConvertTo(int targetBase);
    
    protected void Validate()
    {
        if (Base < 2 || Base > Alphabet.FullAlphabet.Length)
            throw new ArgumentException($"Основание системы должно быть от 2 до {Alphabet.FullAlphabet.Length}");
        bool firstIteration = true; 
        foreach (char digit in Value)
        {
            if (firstIteration is false)
            {
                if (digit == '-') throw new ArgumentException(" - может стоять только в первом разряде");
            }
            if (digit != '-' && digit != '.' && Alphabet.FullAlphabet.IndexOf(digit) >= Base)
                throw new ArgumentException($"Цифра '{digit}' недопустима для системы с основанием {Base}");
            firstIteration = false;
        }
    }
    
    public override string ToString() => Value;
}
public class DecimalNumber : NumberSystem
{
    public DecimalNumber(string value) : base(10, value) { }
    
    public override DecimalNumber ToDecimal() => this;
    
    public override NumberSystem ConvertTo(int targetBase)
    {
        if (targetBase == 10) return this;
        
        string[] parts = Value.Replace(',', '.').Split('.');
        string integerPart = parts[0];
        string fractionalPart = parts.Length > 1 ? parts[1] : "0";
        
        bool isNegative = integerPart.StartsWith("-");
        if (isNegative) integerPart = integerPart.Substring(1);
        
        // Конвертация целой части
        StringBuilder integerResult = new StringBuilder();
        long integerValue = long.Parse(integerPart);
        
        if (integerValue == 0)
        {
            integerResult.Append('0');
        }
        else
        {
            while (integerValue > 0)
            {
                int remainder = (int)(integerValue % targetBase);
                integerResult.Insert(0, Alphabet.FullAlphabet[remainder]);
                integerValue /= targetBase;
            }
        }
        
        // Конвертация дробной части
        StringBuilder fractionalResult = new StringBuilder();
        double fractionalValue = double.Parse("0." + fractionalPart, CultureInfo.InvariantCulture);
        
        for (int i = 0; i < 10 && fractionalValue > 0; i++) // Ограничение знаков
        {
            fractionalValue *= targetBase;
            int digit = (int)fractionalValue;
            fractionalResult.Append(Alphabet.FullAlphabet[digit]);
            fractionalValue -= digit;
        }
        
        string result = isNegative ? "-" : "";
        result += integerResult.ToString();
        //result - число в конечной СС
        if (fractionalResult.Length > 0)
        {
            result += "." + fractionalResult.ToString();
        }
        
        return new ArbitraryBaseNumber(targetBase, result);//Возваращает объект класса любой СС с TB и result
    }
}
public class ArbitraryBaseNumber : NumberSystem
{
    public ArbitraryBaseNumber(int numberBase, string value) : base(numberBase, value)
    {
        Validate();
    }
    
    public override DecimalNumber ToDecimal()
    {
        double result = 0;
        string normalizedValue = Value.Replace(',', '.');
        bool isNegative = normalizedValue.StartsWith("-");
        
        if (isNegative) normalizedValue = normalizedValue.Substring(1);
        
        string[] parts = normalizedValue.Split('.');
        string integerPart = parts[0];
        string fractionalPart = parts.Length > 1 ? parts[1] : "";
        
        // Конвертация целой части
        for (int i = 0; i < integerPart.Length; i++)
        {
            char digit = integerPart[i];
            int digitValue = Alphabet.FullAlphabet.IndexOf(digit);
            double power = Math.Pow(Base, integerPart.Length - 1 - i);
            result += digitValue * power;
        }
        
        // Конвертация дробной части
        for (int i = 0; i < fractionalPart.Length; i++)
        {
            char digit = fractionalPart[i];
            int digitValue = Alphabet.FullAlphabet.IndexOf(digit);
            double power = Math.Pow(Base, -(i + 1));
            result += digitValue * power;
        }
        
        if (isNegative) result = -result;
        
        return new DecimalNumber(result.ToString(CultureInfo.InvariantCulture));
    }
    
    public override NumberSystem ConvertTo(int targetBase)
    {
        if (targetBase == Base) return this;
        
        DecimalNumber decimalValue = ToDecimal();
        return decimalValue.ConvertTo(targetBase);
    }
}
public static class NumberSystemFactory
{
    public static NumberSystem Create(int numberBase, string value)
    {
        if (numberBase == 10)
            return new DecimalNumber(value);
        return new ArbitraryBaseNumber(numberBase, value);
    }
    
    public static NumberSystem Create(string numberBase, string value)
    {
        return Create(int.Parse(numberBase), value);
    }
}