namespace Program;
using System.Text;
using TranslateNotation;
using CalculateNotation;
class Program
{
    enum Operation
    {
        Calculate,
        Addition,
        Multiplication,
        Translate
    }
    public static string ReadInput(string prompt)
    {
        Console.WriteLine(prompt);
        return Console.ReadLine();
    }
    
    public static void Main()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("🧮 КАЛЬКУЛЯТОР СИСТЕМ СЧИСЛЕНИЯ");
        Console.WriteLine("========================================");
        Console.WriteLine();
        
        string action = ReadInput("Вы хотите перевести число из одной системы счисления в другую (translate) " +
                                  "или выполнить арифметические операции (calculate)?").ToLower();
        
        if (action == nameof(Operation.Translate).ToLower())
        {
            try
            {
                Console.WriteLine($"\n📚 В качестве цифр используйте следующий набор:\n{Alphabet.Alphabet.FullAlphabet}");

                string sourceBase = ReadInput("\nУкажите из какой системы счисления вы хотите перевести:");
                string targetBase = ReadInput("Укажите в какую систему счисления вы хотите перевести:");
                string number = ReadInput("Укажите какое число вы хотите перевести:");

                Console.WriteLine("\n🔍 Начинаем перевод...");
                Console.WriteLine("========================================");
                
                // Создаем объект исходной системы
                NumberSystem sourceNumber = NumberSystemFactory.Create(sourceBase, number);
                Console.WriteLine($"Исходное число: {sourceNumber} (основание {sourceBase})");

                // Конвертируем в целевую систему
                NumberSystem targetNumber = sourceNumber.ConvertTo(int.Parse(targetBase));
                
                Console.WriteLine("========================================");
                Console.WriteLine($"\n🎯 Результат: {targetNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка: {ex.Message}");
            }
        }
        else if (action == nameof(Operation.Calculate).ToLower())
        {
            try
            {
                Console.WriteLine($"\n📚 В качестве цифр используйте следующий набор:\n{Alphabet.Alphabet.FullAlphabet}");
                
                string bases = ReadInput("\nУкажите в какой системе счисления будут производиться операции:");
                int baseNumber = int.Parse(bases);

                string numberOne = ReadInput("Укажите первое число:");
                // Проверяем валидность чисел
                new ArbitraryBaseNumber(baseNumber, numberOne);

                string numberTwo = ReadInput("Укажите второе число:");
                new ArbitraryBaseNumber(baseNumber, numberTwo);

                StringBuilder res = new StringBuilder();
                string actionInCalculate = ReadInput("\nУкажите какую операцию вы хотите выполнить?\n+ для сложения\n* для умножения\n:");
                
                if  (actionInCalculate == "+") actionInCalculate = nameof(Operation.Addition);
                else if (actionInCalculate == "*") actionInCalculate = nameof(Operation.Multiplication);
                
                Console.WriteLine("\n🔍 Начинаем вычисления...");
                Console.WriteLine("========================================");
                
                Calculate calculator = Calculate.Create(res, numberOne, numberTwo, baseNumber);
                string result = "";
                
                if (actionInCalculate == nameof(Operation.Addition))
                {
                    calculator = calculator.Addition();
                    result = calculator.ResultString;
                    Console.WriteLine(calculator.ExplanationString);
                }
                else if (actionInCalculate == nameof(Operation.Multiplication))
                {
                    calculator = calculator.Multiplication();
                    result = calculator.ResultString;
                    Console.WriteLine(calculator.ExplanationString);
                }
                else
                {
                    throw new InvalidOperationException($"Неизвестная операция: {actionInCalculate}");
                }
                
                Console.WriteLine("========================================");
                Console.WriteLine($"\n🎯 Итоговый результат: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка: {ex.Message}");
            }
        }
        
        Console.WriteLine("\n========================================");
    }
}