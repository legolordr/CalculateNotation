namespace CalculateNotation;
using System.Text;
using Alphabet;
public class Calculate
{
    protected int Bases;
    protected string NumberOne;
    protected string NumberTwo;
    protected StringBuilder Result;
    public string ResultString => Result.ToString();

    protected Calculate(StringBuilder result, string numberOne, string numberTwo, int bases)
    {
        Result = result;
        NumberOne = numberOne;
        NumberTwo = numberTwo;
        Bases = bases;
    }

    public static Calculate Create(StringBuilder result, string numberOne, string numberTwo, int bases)
    {
        return new Calculate(result, numberOne, numberTwo, bases);
    }
    
    


    protected Calculate Normalize()
    {
        NumberOne = NumberOne.Replace(',', '.');
        NumberTwo = NumberTwo.Replace(',', '.');
    
        // Добавляем .0 если нет дробной части
        if (!NumberOne.Contains('.')) NumberOne += ".0";
        if (!NumberTwo.Contains('.')) NumberTwo += ".0";
    
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
    
        // Собираем обратно
        NumberOne = integerOne + "." + fractionalOne;
        NumberTwo = integerTwo + "." + fractionalTwo;
    
        return new Calculate(Result, NumberOne, NumberTwo, Bases);

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
        Normalize();
        int countNextRank = 0;
        StringBuilder result = new StringBuilder();
        int placeDotOne = PlaceDot(NumberOne);
        int placeDotTwo = PlaceDot(NumberTwo);
        string numberOne = AddDot(NumberOne).Remove(placeDotOne, 1);
        string numberTwo = AddDot(NumberTwo).Remove(placeDotTwo, 1);
        for (int i = numberOne.Length - 1; i >= 0; i--)
        {
            int digitOne = Alphabet.FullAlphabet.IndexOf(numberOne[i]);
            int digitTwo = Alphabet.FullAlphabet.IndexOf(numberTwo[i]);
            int numberInRank = digitOne + digitTwo + countNextRank;
            countNextRank = numberInRank / Bases;
            int countCurrentRank = numberInRank % Bases;
            result.Insert(0, Alphabet.FullAlphabet[countCurrentRank]);
        }
        if (countNextRank > 0) result.Insert(0, Alphabet.FullAlphabet[countNextRank]);
        result.Insert(placeDotOne,'.');
        return new Calculate(result, numberOne, numberTwo, Bases);
    }
    
    public Calculate Multiplication()
{
    Normalize();
    StringBuilder result = new StringBuilder();
    int placeDotOne = PlaceDot(NumberOne);
    int placeDotTwo = PlaceDot(NumberTwo);
    string numberOne = AddDot(NumberOne).Remove(placeDotOne, 1);
    string numberTwo = AddDot(NumberTwo).Remove(placeDotTwo, 1);
    
    // кол - во дробных знаков в результате
    int fractionalDigitsOne = numberOne.Length - placeDotOne;
    int fractionalDigitsTwo = numberTwo.Length - placeDotTwo;
    int totalFractionalDigits = fractionalDigitsOne + fractionalDigitsTwo;
    
    // массив для хранения промежуточных результатов
    int maxLength = numberOne.Length + numberTwo.Length;
    int[] intermediate = new int[maxLength];
    
    // умножаем
    for (int i = numberOne.Length - 1; i >= 0; i--)
    {
        int digitOne = Alphabet.FullAlphabet.IndexOf(numberOne[i]);
        
        for (int j = numberTwo.Length - 1; j >= 0; j--)
        {
            int digitTwo = Alphabet.FullAlphabet.IndexOf(numberTwo[j]);
            int product = digitOne * digitTwo;
            
            int position = i + j + 1;
            int sum = intermediate[position] + product;
            
            intermediate[position] = sum % Bases;
            intermediate[position - 1] += sum / Bases;
        }
    }
    
    // преобразуем массив в строку
    for (int i = maxLength - 1; i >= 0; i--)
    {
        result.Insert(0, Alphabet.FullAlphabet[intermediate[i]]);
    }
    
    // убираем ведущие нули
    while (result.Length > 1 && result[0] == Alphabet.FullAlphabet[0])
    {
        result.Remove(0, 1);
    }
    
    // вставляем точку
    if (totalFractionalDigits > 0)
    {
        int dotPosition = result.Length - totalFractionalDigits;
        
        if (dotPosition <= 0)
        {
            // если нужно добавить нули перед числом (для дробей меньше 1)
            result.Insert(0, new string(Alphabet.FullAlphabet[0], -dotPosition + 1));
            dotPosition = 1;
        }
        
        result.Insert(dotPosition, '.');
    }
    
    // убираем хвостовые нули после точки
    if (result.ToString().Contains('.'))
    {
        while (result.Length > 1 && result[result.Length - 1] == Alphabet.FullAlphabet[0])
        {
            result.Remove(result.Length - 1, 1);
        }
        // если после точки ничего не осталось, убираем точку
        if (result[result.Length - 1] == '.')
        {
            result.Remove(result.Length - 1, 1);
        }
    }
    
    // если результат пустой, возвращаем 0
    if (result.Length == 0)
    {
        result.Append(Alphabet.FullAlphabet[0]);
    }
    
    return new Calculate(result, numberOne, numberTwo, Bases);
}
}