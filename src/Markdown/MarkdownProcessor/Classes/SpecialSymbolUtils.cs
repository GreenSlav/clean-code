using System.Text;
using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;
using MarkdownProcessor.Structs;

namespace MarkdownProcessor.Classes;

public static class SpecialSymbolUtils
{
    public static bool IsOpenedHeader { get; set; }
    
    public static bool CheckForUnclosedHeaderTag(string sourceString, List<SpecialSymbol> specialSymbols)
    {
        // Надо будет по красивее сделать, а то повторение кода
        // Этот if предусматривает случай, когда header был открыт,
        // но последним символом в исходной строке был какой-то специальный символ
        // и надо бы header закрыть, чтобы превратить его в токен
        if (IsOpenedHeader)
        {
            specialSymbols.Add(new SpecialSymbol { Type = TokenType.Header, Index = sourceString.Length - 1, TagLength = 1, IsPairedTag = false, IsClosingTag = true });
            IsOpenedHeader = false;
            
            return true;
        }

        return false;
    }
    
    public static bool IsWithinOneWord(string src, SpecialSymbol openingTag, SpecialSymbol closingTag)
    {
        bool containsSpace = src.Substring(openingTag.Index + openingTag.TagLength,
                closingTag.Index - openingTag.Index - openingTag.TagLength)
            .Any(c => char.IsWhiteSpace(c));

        // Вот ЭТО ВЫРАЖЕНИЕ ПЕРЕСМОТРЕТЬ, хотя вроде правильное
        return !(containsSpace &&
                 openingTag.Index - 1 >= 0 &&
                 closingTag.Index + closingTag.TagLength < src.Length &&
                 char.IsLetter(src[openingTag.Index - 1]) &&
                 char.IsLetter(src[closingTag.Index + closingTag.TagLength]));
    }
    
    public static bool IsNextSymbolIsSpecial(string sourceString, int currentIndex)
    {
        if (currentIndex < sourceString.Length - 1)
        {
            return sourceString[currentIndex + 1] == '#' || sourceString[currentIndex + 1] == '_';
        }

        if (currentIndex < sourceString.Length - 2)
        {
            return sourceString.Substring(currentIndex + 1, 2) == "__";
        }

        return false;
    }
    
    public static bool IsEscaped(string str, int index)
    {
        // Проверяем, есть ли перед текущим символом экранирующий символ
        if (index > 0 && str[index - 1] == '\\')
        {
            // Если символ экранирован, проверяем, является ли этот экранирующий символ
            // также экранированным (например, двойной слэш \\).
            int backslashCount = 0;
            index--;  // Начинаем проверять символы до текущего
            while (index >= 0 && str[index] == '\\')
            {
                backslashCount++;
                index--;
            }

            // Если количество экранирующих слэшей нечетное, значит символ экранирован
            return backslashCount % 2 == 1;
        }

        return false;
    }
    
    public static bool IsOpeningSymbol(SpecialSymbol symbol, List<SpecialSymbol> stack)
    {
        // Определяем, открывающий ли это символ, в зависимости от контекста
        // Например, можно проверять, что нет открытой пары для символа этого типа в стеке
        if (!symbol.IsPairedTag)
        {
            return !symbol.IsClosingTag;
        }
        
        bool openedTagBefore = false;

        for (int i = stack.Count - 1; i >= 0; i--)
        {
            if (stack[i].Type == symbol.Type)
            {
                openedTagBefore = true;
            }

            break;
        }
        
        // Проверяем, что стек может быть нулевым, до этого нигде этот открывающийся стек не встречался
        // Тогда он точно открывающий
        return  !openedTagBefore;
    }
    
    public static string GetEscapedText(string textToken)
    {
        var sb = new StringBuilder();
        
        // Здесь будем удалять лишние пробелы и обрабатывать экранирование
        for (int i = 0; i < textToken.Length; i++)
        {
            if (i > 0 && textToken[i - 1] == ' ' && textToken[i] == ' ')
            {
                continue;
            }

            if (textToken[i] == '\\')
            {
                int escapedCounter = 0;

                while (i < textToken.Length && textToken[i] == '\\')
                {
                    ++escapedCounter;
                    ++i;
                }
                
                // Если следующий символ специальный, то не добавляем символ экранирования
                
                // Так, тут по сути escapedCounter % 2 == 1 всегда будет давать ложь,
                // тк мы еще в самом первом цикле проверяли перед добавлением специального символа
                // является ли он escaped
                
                // Просто символы парами экранируют друг друга
                if (escapedCounter % 2 == 0)
                {
                    sb.Append(textToken.Substring(i - escapedCounter, escapedCounter / 2));
                }
                // Нечетное колво экранирующих символов
                else
                {
                    // Если следующий после экранирующих специальный, то его нужно удалить, а другие парами сопоставить
                    if (IsNextSymbolIsSpecial(textToken, i - 1))
                    {
                        sb.Append(textToken.Substring(i - escapedCounter, escapedCounter / 2));
                    }
                    else
                    {
                        // В противном случае просто пары удалим и один нечетный оставим
                        sb.Append(textToken.Substring(i - escapedCounter, escapedCounter / 2 + 1));
                    }
                }
            }

            // Проверяем, что не вышли за границы
            if (i < textToken.Length)
            {
                sb.Append(textToken[i]);
            }
        }

        return sb.ToString();
    }

    public static bool IsDigitAmongWord(string sourceString, SpecialSymbol openingSymbol,
        SpecialSymbol closingSymbol)
    {
        string subString = sourceString.Substring(openingSymbol.Index + openingSymbol.TagLength,
            closingSymbol.Index - openingSymbol.Index - openingSymbol.TagLength);
        
        bool containsAllNumbers = subString.All(c => char.IsDigit(c));

        // Проверяем, что наше выделение является числом и рядом нет никаких других символов
        bool leftIsSpaceOrEnd = openingSymbol.Index - 1 < 0 
                                || sourceString[openingSymbol.Index - 1] == ' ' 
                                || sourceString[openingSymbol.Index - 1] == '\n';
        bool rightIsSpaceOrEnd = closingSymbol.Index + closingSymbol.TagLength >= sourceString.Length 
                                 || sourceString[closingSymbol.Index + closingSymbol.TagLength] == ' ' 
                                 || sourceString[closingSymbol.Index + closingSymbol.TagLength] == '\n';

        // Случаи когда выделяем целое число, которое не входит ни в какое слово
        if (containsAllNumbers && leftIsSpaceOrEnd && rightIsSpaceOrEnd)
            return false;
        
        // К этому моменту если выражение даст true, значит среди цифр точно есть буквы и 
        // нужно вернуть false 
        bool containsAnyNumbers = subString.Any(c => char.IsDigit(c));

        bool numbersAmongWordInside = false;
        
        if (containsAnyNumbers)
        {
            for (int i = 0; i < subString.Length; i++)
            {
                if (char.IsDigit(subString[i]))
                {
                    // Если внутри тега и встречается цифры, то они должны быть отдельны от букв (слов)
                    if (!((i - 1 < 0 || subString[i - 1] == ' ' || subString[i - 1] == '\n' || char.IsDigit(subString[i - 1])) &&
                        i + 1 >= subString.Length || subString[i + 1] == ' ' || subString[i + 1] == '\n' || char.IsDigit(subString[i + 1])))
                        numbersAmongWordInside = true;
                    break;
                }
            }
        }
        
        if (numbersAmongWordInside)
            return true;
            
        // __React__:\n1)
        // Но может быть ситуация: "_ццц_ц1"
        // Остается найти в текущем слове цифры
        int leftIndex = openingSymbol.Index - 1;
        // Наткнулись на пробел идя слева от слова
        bool encounteredSpaceFromLeft = false;
        int rightIndex = closingSymbol.Index + closingSymbol.TagLength;
        // Наткнулись на пробел идя справа от слова
        bool encounteredSpaceFromRight = false;
        
        // В слове, подверженном выделению, есть цифры, выделять в нем не стоит
        bool wordWithinNumbers = false;
        
        while ((!encounteredSpaceFromLeft || !encounteredSpaceFromRight) && (leftIndex >= 0 || rightIndex < sourceString.Length) )
        {
            if (leftIndex >= 0)
            {
                if (char.IsDigit(sourceString[leftIndex]))
                    wordWithinNumbers = true;
                
                // Чтобы избежать изменения этого значения, когда мы уже до этого достигли пробел, чтоб не забыть это
                if (!encounteredSpaceFromLeft)
                    encounteredSpaceFromLeft = sourceString[leftIndex] == ' ' || sourceString[leftIndex] == '\n';
            }
            
            if (rightIndex < sourceString.Length)
            {
                if (char.IsDigit(sourceString[rightIndex]))
                    wordWithinNumbers = true;
                if (!encounteredSpaceFromRight)
                    encounteredSpaceFromRight = sourceString[rightIndex] == ' ' || sourceString[rightIndex] == '\n';
            }
            
            --leftIndex;
            ++rightIndex;
        }


        return wordWithinNumbers;
    }

    public static TokenType FirstItalicsOrBold(List<ITag> tags)
    {
        foreach (var tag in tags)
        {
            switch (tag.TokenType)
            {
                case TokenType.Bold:
                    return tag.TokenType;
                case TokenType.Italics:
                    return tag.TokenType;
            }
        }
        
        throw new Exception("Italics or Bold tag not found");
    }
    
    public static void SwapItalicsAndBold(List<ITag> tags)
    {
        var firstEncounter = FirstItalicsOrBold(tags);
        
        // Найдем индексы тегов с типом Italics и Bold
        int italicsIndex = -1;
        int boldIndex = -1;

        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i].TokenType == TokenType.Italics)
            {
                italicsIndex = i;
            }
            else if (tags[i].TokenType == TokenType.Bold)
            {
                boldIndex = i;
            }

            // Если оба индекса найдены, можем завершить поиск
            if (italicsIndex != -1 && boldIndex != -1)
            {
                break;
            }
        }

        // Если оба индекса найдены, меняем элементы местами
        if (italicsIndex != -1 && boldIndex != -1 && firstEncounter == TokenType.Italics)
        {
            (tags[italicsIndex], tags[boldIndex]) = (tags[boldIndex], tags[italicsIndex]);
        }
    }
}