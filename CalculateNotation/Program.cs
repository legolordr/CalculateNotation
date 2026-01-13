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
        Console.WriteLine("Вы хотите перевести число из одной системы счисления в другую" +
                                  "или выполнить арифметические операции?(t/k)");
        char action = Console.ReadKey().KeyChar;
        
        if (action == 't')
        {
            try
            {
                Console.WriteLine($"В качестве цифр используйте следующий набор:\n{Alphabet.Alphabet.FullAlphabet}");

                string sourceBase = ReadInput("Укажите из какой системы счисления вы хотите перевести:");
                string targetBase = ReadInput("Укажите в какую систему счисления вы хотите перевести:");
                string number = ReadInput("Укажите какое число вы хотите перевести:");

                // Создаем объект исходной системы
                NumberSystem sourceNumber = NumberSystemFactory.Create(sourceBase, number);

                // Конвертируем в целевую систему
                NumberSystem targetNumber = sourceNumber.ConvertTo(int.Parse(targetBase));

                Console.WriteLine($"Результат: {targetNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        else if (action == 'c')
        {
            try
            {
                string bases = ReadInput("Укажите в какой системы счисления будут производиться операции:");

                string numberOne = ReadInput("Укажите первое число:");
                new ArbitraryBaseNumber(int.Parse(bases), numberOne);

                string numberTwo = ReadInput("Укажите второе число:");
                new ArbitraryBaseNumber(int.Parse(bases), numberTwo);

                StringBuilder res = new StringBuilder();
                Console.WriteLine("Укажите какую операцию вы хотите выполнить?(+/*):");
                char actionInCalculate = Console.ReadKey().KeyChar;
                
                string result = actionInCalculate switch
                {
                    '+' => Calculate.Create(res, numberOne, numberTwo, int.Parse(bases)).Addition().ResultString,
                    '*' => Calculate.Create(res,numberOne,numberTwo,int.Parse(bases)).Multiplication().ResultString,
                    _ => throw new InvalidOperationException()
                };
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}