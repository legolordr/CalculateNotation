namespace TranslateNotation;
using System.Globalization;
using System.Text;
using Alphabet;

public abstract class NumberSystem
{
    public int Base { get; protected set; }
    public string Value { get; protected set; }
    protected StringBuilder Explanation { get; set; } // Добавляем поле для пояснений
    
    protected NumberSystem(int numberBase, string value)
    {
        Base = numberBase;
        Value = value;
        Explanation = new StringBuilder();
    }
    
    public abstract DecimalNumber ToDecimal();
    public abstract NumberSystem ConvertTo(int targetBase);
    
    protected virtual void Validate()
    {
        if (Base < 2 || Base > Alphabet.FullAlphabet.Length)
            throw new ArgumentException($"Основание системы должно быть от 2 до {Alphabet.FullAlphabet.Length}");
            
        foreach (char digit in Value)
        {
            if (digit != '-' && digit != '.' && Alphabet.FullAlphabet.IndexOf(digit) >= Base)
                throw new ArgumentException($"Цифра '{digit}' недопустима для системы с основанием {Base}");
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
        
        Console.WriteLine($"\n🔀 Перевод из десятичной системы в систему с основанием {targetBase}");
        Console.WriteLine($"Исходное число: {Value}");
        Console.WriteLine();
        
        string[] parts = Value.Replace(',', '.').Split('.');
        string integerPart = parts[0];
        string fractionalPart = parts.Length > 1 ? parts[1] : "0";
        
        bool isNegative = integerPart.StartsWith("-");
        if (isNegative) integerPart = integerPart.Substring(1);
        
        Console.WriteLine("1. 📝 Разделяем число на целую и дробную части:");
        Console.WriteLine($"   Целая часть: {integerPart}");
        Console.WriteLine($"   Дробная часть: {fractionalPart}");
        if (isNegative) Console.WriteLine($"   Число отрицательное, знак минус запомним");
        Console.WriteLine();
        
        // Конвертация целой части
        Console.WriteLine("2. 🔢 Конвертируем целую часть (делением на основание):");
        StringBuilder integerResult = new StringBuilder();
        long integerValue = long.Parse(integerPart);
        
        if (integerValue == 0)
        {
            integerResult.Append('0');
            Console.WriteLine($"   Число равно 0, записываем '0'");
        }
        else
        {
            int step = 1;
            while (integerValue > 0)
            {
                int remainder = (int)(integerValue % targetBase);
                string digit = Alphabet.FullAlphabet[remainder].ToString();
                integerResult.Insert(0, digit);
                
                Console.WriteLine($"   Шаг {step}: {integerValue} ÷ {targetBase} = {integerValue / targetBase} (целое), остаток {remainder}");
                Console.WriteLine($"       Остаток {remainder} → цифра '{digit}', добавляем в начало");
                Console.WriteLine($"       Текущий результат: {integerResult}");
                
                integerValue /= targetBase;
                step++;
            }
        }
        Console.WriteLine($"   Итог целой части: {integerResult}");
        Console.WriteLine();
        
        // Конвертация дробной части
        Console.WriteLine("3. 🔄 Конвертируем дробную часть (умножением на основание):");
        StringBuilder fractionalResult = new StringBuilder();
        double fractionalValue = double.Parse("0." + fractionalPart, CultureInfo.InvariantCulture);
        
        if (fractionalValue > 0)
        {
            Console.WriteLine($"   Начальная дробная часть: 0.{fractionalPart} = {fractionalValue}");
            
            for (int i = 0; i < 10 && fractionalValue > 0; i++) // Ограничение знаков
            {
                double before = fractionalValue;
                fractionalValue *= targetBase;
                int digit = (int)fractionalValue;
                string digitChar = Alphabet.FullAlphabet[digit].ToString();
                fractionalResult.Append(digitChar);
                fractionalValue -= digit;
                
                Console.WriteLine($"   Шаг {i+1}: {before:F6} × {targetBase} = {before * targetBase:F6}");
                Console.WriteLine($"       Целая часть: {digit} → цифра '{digitChar}', добавляем в конец");
                Console.WriteLine($"       Остаток: {fractionalValue:F6}");
                Console.WriteLine($"       Текущий результат: {fractionalResult}");
            }
        }
        else
        {
            Console.WriteLine($"   Дробная часть равна 0, пропускаем");
        }
        
        string result = isNegative ? "-" : "";
        result += integerResult.ToString();
        
        Console.WriteLine();
        Console.WriteLine("4. 🎯 Собираем окончательный результат:");
        
        if (fractionalResult.Length > 0)
        {
            result += "." + fractionalResult.ToString();
            Console.WriteLine($"   Целая часть: {integerResult}");
            Console.WriteLine($"   Дробная часть: {fractionalResult}");
            Console.WriteLine($"   Собираем: {integerResult}.{fractionalResult}");
        }
        else
        {
            Console.WriteLine($"   Только целая часть: {integerResult}");
        }
        
        if (isNegative)
        {
            Console.WriteLine($"   Добавляем знак минус: -{integerResult}");
        }
        
        Console.WriteLine($"   Итог: {result}");
        
        return new ArbitraryBaseNumber(targetBase, result);
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
        Console.WriteLine($"\n🔢 Перевод в десятичную систему из системы с основанием {Base}");
        Console.WriteLine($"Исходное число: {Value}");
        Console.WriteLine();
        
        double result = 0;
        string normalizedValue = Value.Replace(',', '.');
        bool isNegative = normalizedValue.StartsWith("-");
        
        if (isNegative) 
        {
            normalizedValue = normalizedValue.Substring(1);
            Console.WriteLine($"Число отрицательное, убираем минус для вычислений");
        }
        
        string[] parts = normalizedValue.Split('.');
        string integerPart = parts[0];
        string fractionalPart = parts.Length > 1 ? parts[1] : "";
        
        Console.WriteLine("1. 📝 Разделяем число на целую и дробную части:");
        Console.WriteLine($"   Целая часть: {integerPart}");
        if (!string.IsNullOrEmpty(fractionalPart)) 
            Console.WriteLine($"   Дробная часть: {fractionalPart}");
        Console.WriteLine();
        
        // Конвертация целой части
        Console.WriteLine("2. 🔢 Конвертируем целую часть:");
        for (int i = 0; i < integerPart.Length; i++)
        {
            char digit = integerPart[i];
            int digitValue = Alphabet.FullAlphabet.IndexOf(digit);
            double power = Math.Pow(Base, integerPart.Length - 1 - i);
            double contribution = digitValue * power;
            result += contribution;
            
            Console.WriteLine($"   Разряд {i+1}: цифра '{digit}' = {digitValue}");
            Console.WriteLine($"       Позиция: {integerPart.Length - 1 - i}, вес: {Base}^{integerPart.Length - 1 - i} = {power}");
            Console.WriteLine($"       Вклад: {digitValue} × {power} = {contribution}");
            Console.WriteLine($"       Сумма: {result}");
        }
        
        // Конвертация дробной части
        if (!string.IsNullOrEmpty(fractionalPart))
        {
            Console.WriteLine();
            Console.WriteLine("3. 🔄 Конвертируем дробную часть:");
            for (int i = 0; i < fractionalPart.Length; i++)
            {
                char digit = fractionalPart[i];
                int digitValue = Alphabet.FullAlphabet.IndexOf(digit);
                double power = Math.Pow(Base, -(i + 1));
                double contribution = digitValue * power;
                result += contribution;
                
                Console.WriteLine($"   Разряд {i+1} после точки: цифра '{digit}' = {digitValue}");
                Console.WriteLine($"       Позиция: -{i+1}, вес: {Base}^-{i+1} = {power:F6}");
                Console.WriteLine($"       Вклад: {digitValue} × {power:F6} = {contribution:F6}");
                Console.WriteLine($"       Сумма: {result:F6}");
            }
        }
        
        if (isNegative) 
        {
            result = -result;
            Console.WriteLine($"\nДобавляем знак минус: -{Math.Abs(result)}");
        }
        
        Console.WriteLine($"\n🎯 Итоговое десятичное число: {result}");
        
        return new DecimalNumber(result.ToString(CultureInfo.InvariantCulture));
    }
    
    public override NumberSystem ConvertTo(int targetBase)
    {
        if (targetBase == Base) return this;
        
        Console.WriteLine($"\n🔀 Перевод из системы {Base} в систему {targetBase}");
        Console.WriteLine($"Исходное число: {Value}");
        Console.WriteLine();
        
        Console.WriteLine("📊 Стратегия: сначала переведем в десятичную систему,");
        Console.WriteLine("              а затем из десятичной в целевую");
        Console.WriteLine();
        
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
        else
            return new ArbitraryBaseNumber(numberBase, value);
    }
    
    public static NumberSystem Create(string numberBase, string value)
    {
        return Create(int.Parse(numberBase), value);
    }
}