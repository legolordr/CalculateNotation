namespace CalculateNotation;
using System.Text;
using Alphabet;
public class Calculate
{
    protected int Bases;
    protected string NumberOne;
    protected string NumberTwo;
    protected StringBuilder Result;
    protected StringBuilder Explanation; // Новое поле для пояснений
    public string ResultString => Result.ToString();
    public string ExplanationString => Explanation.ToString(); // Свойство для получения пояснений

    protected Calculate(StringBuilder result, string numberOne, string numberTwo, int bases, StringBuilder explanation = null)
    {
        Result = result;
        NumberOne = numberOne;
        NumberTwo = numberTwo;
        Bases = bases;
        Explanation = explanation ?? new StringBuilder();
    }

    public static Calculate Create(StringBuilder result, string numberOne, string numberTwo, int bases)
    {
        return new Calculate(result, numberOne, numberTwo, bases);
    }
    
    

    protected Calculate Normalize()
    {
        Explanation.AppendLine("📏 Этап 1: Выравнивание чисел");
        Explanation.AppendLine($"   Исходное первое число: {NumberOne}");
        Explanation.AppendLine($"   Исходное второе число: {NumberTwo}");
        
        NumberOne = NumberOne.Replace(',', '.');
        NumberTwo = NumberTwo.Replace(',', '.');
    
        // Добавляем .0 если нет дробной части
        if (!NumberOne.Contains('.')) 
        {
            NumberOne += ".0";
            Explanation.AppendLine($"   Добавили .0 к первому числу: {NumberOne}");
        }
        if (!NumberTwo.Contains('.')) 
        {
            NumberTwo += ".0";
            Explanation.AppendLine($"   Добавили .0 ко второму числу: {NumberTwo}");
        }
    
        string[] partsOne = NumberOne.Split('.');
        string[] partsTwo = NumberTwo.Split('.');
    
        string integerOne = partsOne[0];
        string fractionalOne = partsOne[1];
        string integerTwo = partsTwo[0];
        string fractionalTwo = partsTwo[1];
    
        // Выравниваем целые части (добавляем нули слева)
        int maxIntegerLength = Math.Max(integerOne.Length, integerTwo.Length);
        integerOne = integerOne.PadLeft(maxIntegerLength, '0');
        integerTwo = integerTwo.PadLeft(maxIntegerLength, '0');
    
        // Выравниваем дробные части (добавляем нули справа)
        int maxFractionalLength = Math.Max(fractionalOne.Length, fractionalTwo.Length);
        fractionalOne = fractionalOne.PadRight(maxFractionalLength, '0');
        fractionalTwo = fractionalTwo.PadRight(maxFractionalLength, '0');
    
        Explanation.AppendLine($"   Выравниваем целые части до {maxIntegerLength} знаков:");
        Explanation.AppendLine($"     Первое число: {integerOne}");
        Explanation.AppendLine($"     Второе число: {integerTwo}");
        Explanation.AppendLine($"   Выравниваем дробные части до {maxFractionalLength} знаков:");
        Explanation.AppendLine($"     Первое число: {fractionalOne}");
        Explanation.AppendLine($"     Второе число: {fractionalTwo}");
    
        // Собираем обратно
        NumberOne = integerOne + "." + fractionalOne;
        NumberTwo = integerTwo + "." + fractionalTwo;
    
        Explanation.AppendLine($"   Итоговые выравненные числа:");
        Explanation.AppendLine($"     Первое число: {NumberOne}");
        Explanation.AppendLine($"     Второе число: {NumberTwo}");
        Explanation.AppendLine();

        return new Calculate(Result, NumberOne, NumberTwo, Bases, Explanation);
    }

    private int PlaceDot(string number)
    {
        int placeDot = 0;
        number = AddDot(number);
        placeDot = number.IndexOf('.'); 
        return placeDot;
    }
    
    private string AddDot(string number)
    {
        if (!(number.Contains('.'))) number += ".0";
        return number;
    }
    
    public Calculate Addition()
    {
        Explanation.AppendLine("➕ Начинаем сложение:");
        Explanation.AppendLine($"   Система счисления: {Bases}");
        Explanation.AppendLine();
        
        Normalize();
        
        Explanation.AppendLine("📝 Этап 2: Убираем точки для удобства вычислений");
        int placeDotOne = PlaceDot(NumberOne);
        int placeDotTwo = PlaceDot(NumberTwo);
        string numberOne = AddDot(NumberOne).Remove(placeDotOne, 1);
        string numberTwo = AddDot(NumberTwo).Remove(placeDotTwo, 1);
        
        Explanation.AppendLine($"   Первое число без точки: {numberOne}");
        Explanation.AppendLine($"   Второе число без точки: {numberTwo}");
        Explanation.AppendLine($"   Точка будет на позиции {placeDotOne} в результате");
        Explanation.AppendLine();
        
        Explanation.AppendLine("🧮 Этап 3: Сложение поразрядно (справа налево):");
        StringBuilder result = new StringBuilder();
        int countNextRank = 0;
        
        for (int i = numberOne.Length - 1; i >= 0; i--)
        {
            int digitOne = Alphabet.FullAlphabet.IndexOf(numberOne[i]);
            int digitTwo = Alphabet.FullAlphabet.IndexOf(numberTwo[i]);
            int numberInRank = digitOne + digitTwo + countNextRank;
            
            Explanation.Append($"   Разряд {numberOne.Length - i}: {numberOne[i]} + {numberTwo[i]}");
            if (countNextRank > 0) Explanation.Append($" + {countNextRank} (перенос)");
            Explanation.Append($" = {numberInRank} (в десятичной)");
            
            countNextRank = numberInRank / Bases;
            int countCurrentRank = numberInRank % Bases;
            
            Explanation.AppendLine($" → {numberInRank}/{Bases} = частное {countNextRank}, остаток {countCurrentRank}");
            Explanation.AppendLine($"     Записываем {Alphabet.FullAlphabet[countCurrentRank]} в результат, переносим {countNextRank}");
            
            result.Insert(0, Alphabet.FullAlphabet[countCurrentRank]);
        }
        
        if (countNextRank > 0) 
        {
            Explanation.AppendLine($"   После последнего разряда остался перенос {countNextRank}");
            Explanation.AppendLine($"   Добавляем {Alphabet.FullAlphabet[countNextRank]} в начало результата");
            result.Insert(0, Alphabet.FullAlphabet[countNextRank]);
        }
        
        Explanation.AppendLine();
        Explanation.AppendLine($"📌 Промежуточный результат без точки: {result}");
        
        // Вставляем точку
        result.Insert(placeDotOne, '.');
        Explanation.AppendLine($"   Вставляем точку на позицию {placeDotOne}");
        
        Explanation.AppendLine();
        Explanation.AppendLine($"🎯 Итоговый результат: {result}");
        
        return new Calculate(result, numberOne, numberTwo, Bases, Explanation);
    }
    
    public Calculate Multiplication()
    {
        Explanation.AppendLine("✖️ Начинаем умножение:");
        Explanation.AppendLine($"   Система счисления: {Bases}");
        Explanation.AppendLine();
        
        Normalize();
        
        Explanation.AppendLine("📝 Этап 2: Подготовка чисел:");
        int placeDotOne = PlaceDot(NumberOne);
        int placeDotTwo = PlaceDot(NumberTwo);
        string numberOne = AddDot(NumberOne).Remove(placeDotOne, 1);
        string numberTwo = AddDot(NumberTwo).Remove(placeDotTwo, 1);
        
        Explanation.AppendLine($"   Первое число без точки: {numberOne}");
        Explanation.AppendLine($"   Второе число без точки: {numberTwo}");
        
        // кол - во дробных знаков в результате
        int fractionalDigitsOne = numberOne.Length - placeDotOne;
        int fractionalDigitsTwo = numberTwo.Length - placeDotTwo;
        int totalFractionalDigits = fractionalDigitsOne + fractionalDigitsTwo;
        
        Explanation.AppendLine($"   Дробных цифр в первом числе: {fractionalDigitsOne}");
        Explanation.AppendLine($"   Дробных цифр во втором числе: {fractionalDigitsTwo}");
        Explanation.AppendLine($"   Всего дробных цифр в результате: {totalFractionalDigits}");
        Explanation.AppendLine();
        
        Explanation.AppendLine("🧮 Этап 3: Умножение в столбик:");
        
        // массив для хранения промежуточных результатов
        int maxLength = numberOne.Length + numberTwo.Length;
        int[] intermediate = new int[maxLength];
        
        // умножаем
        for (int i = numberOne.Length - 1; i >= 0; i--)
        {
            int digitOne = Alphabet.FullAlphabet.IndexOf(numberOne[i]);
            Explanation.AppendLine($"   Умножаем на цифру {numberOne[i]} (значение {digitOne}):");
            
            for (int j = numberTwo.Length - 1; j >= 0; j--)
            {
                int digitTwo = Alphabet.FullAlphabet.IndexOf(numberTwo[j]);
                int product = digitOne * digitTwo;
                
                Explanation.Append($"     {digitOne} × {numberTwo[j]} (значение {digitTwo}) = {product}");
                
                int position = i + j + 1;
                int sum = intermediate[position] + product;
                
                intermediate[position] = sum % Bases;
                intermediate[position - 1] += sum / Bases;
                
                Explanation.AppendLine($" → записываем {sum % Bases} в позицию {position}, перенос {sum / Bases} в позицию {position-1}");
            }
        }
        
        Explanation.AppendLine();
        Explanation.AppendLine("📊 Промежуточный массив:");
        for (int i = 0; i < intermediate.Length; i++)
        {
            Explanation.AppendLine($"   Позиция {i}: {intermediate[i]} → цифра {Alphabet.FullAlphabet[intermediate[i]]}");
        }
        
        Explanation.AppendLine();
        Explanation.AppendLine("🔢 Этап 4: Преобразуем массив в строку:");
        StringBuilder result = new StringBuilder();
        
        for (int i = maxLength - 1; i >= 0; i--)
        {
            result.Insert(0, Alphabet.FullAlphabet[intermediate[i]]);
        }
        
        Explanation.AppendLine($"   Получили: {result}");
        
        // убираем ведущие нули
        Explanation.AppendLine();
        Explanation.AppendLine("🧹 Этап 5: Убираем лишние нули:");
        while (result.Length > 1 && result[0] == Alphabet.FullAlphabet[0])
        {
            Explanation.AppendLine($"   Убираем ведущий ноль: {result} → {result.ToString().Substring(1)}");
            result.Remove(0, 1);
        }
        
        // вставляем точку
        if (totalFractionalDigits > 0)
        {
            Explanation.AppendLine();
            Explanation.AppendLine("📍 Этап 6: Ставим точку:");
            int dotPosition = result.Length - totalFractionalDigits;
            
            if (dotPosition <= 0)
            {
                Explanation.AppendLine($"   Нужно добавить нули перед числом (результат меньше 1)");
                result.Insert(0, new string(Alphabet.FullAlphabet[0], -dotPosition + 1));
                dotPosition = 1;
                Explanation.AppendLine($"   После добавления нулей: {result}");
            }
            
            result.Insert(dotPosition, '.');
            Explanation.AppendLine($"   Вставляем точку на позицию {dotPosition}");
        }
        
        // убираем хвостовые нули после точки
        if (result.ToString().Contains('.'))
        {
            Explanation.AppendLine();
            Explanation.AppendLine("🧹 Этап 7: Убираем нули после точки:");
            while (result.Length > 1 && result[result.Length - 1] == Alphabet.FullAlphabet[0])
            {
                Explanation.AppendLine($"   Убираем нуль в конце: {result} → {result.ToString().Substring(0, result.Length - 1)}");
                result.Remove(result.Length - 1, 1);
            }
            // если после точки ничего не осталось, убираем точку
            if (result[result.Length - 1] == '.')
            {
                Explanation.AppendLine($"   После точки ничего нет, убираем точку: {result} → {result.ToString().Substring(0, result.Length - 1)}");
                result.Remove(result.Length - 1, 1);
            }
        }
        
        // если результат пустой, возвращаем 0
        if (result.Length == 0)
        {
            Explanation.AppendLine($"   Результат пустой, записываем 0");
            result.Append(Alphabet.FullAlphabet[0]);
        }
        
        Explanation.AppendLine();
        Explanation.AppendLine($"🎯 Итоговый результат: {result}");
        
        return new Calculate(result, numberOne, numberTwo, Bases, Explanation);
    }
}