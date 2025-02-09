using MarkdownProcessor.Classes;
using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;

namespace MarkdownProcessor.Structs.Tags;

public struct BoldTag: ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    public TokenType TokenType { get; }
    public TokenType[] TagsCanBeInside { get; }
    public bool IsOpened { get; set; }


    public BoldTag()
    {
        Symbol = "__";
        IsPaired = true;
        TokenType = TokenType.Bold;
        TagsCanBeInside = [TokenType.Text, TokenType.Italics, TokenType.Link];
    }

    public void ValidateInsideTokens(Token token, string sourceString)
    {
        
    }

    public bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols)
    {
        if (index < sourceString.Length - 1 && sourceString.Substring(index, 2) == "__")
        {
            // Даем шанс прочитать этот символ курсиву
            if (IsOpened && index < sourceString.Length - 2 && sourceString.Substring(index, 3) == "___")
                return false;
            
            
            specialSymbols.Add(new SpecialSymbol { Type = TokenType.Bold, Index = index, TagLength = 2, IsPairedTag = true });
            IsOpened = !IsOpened;
            return true;
        }

        return false;
    }

    public bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol, List<SpecialSymbol> specialSymbolsStack)
    {
        var symbolOpen = openingSymbol;
        var symbolClose = closingSymbol;
        var openingSymbolIndex = specialSymbolsStack.FindIndex(x => x.Index == symbolOpen.Index);
        var closingSymbolIndex = specialSymbolsStack.FindIndex(x => x.Index == symbolClose.Index);
        bool unClosedItalicsBetween = specialSymbolsStack
            .Where((ss, i) => i > openingSymbolIndex && i < closingSymbolIndex && ss.Type == TokenType.Italics).Any();
        // Если все условия выполнены, выходим из цикла, запоминаем 
        // символов надо удалить из стека, чтоб добраться до этого символ,
        // удаляем из стека эти символы, и готово - у нас есть правильный токен
        // Создаем его
        
        // openSymbolsStack[j] - открывающий
        // symbol - закрывающий
        // После открывающего и перед закрывающим нет пробела
        // Проверяем что если и есть внутри 
        // Левое безсполезное
        //bool noCrossingItalicsOnTheLeft = openingSymbolIndex <= 0 || (specialSymbolsStack[openingSymbolIndex - 1].Type != TokenType.Italics && specialSymbolsStack[openingSymbolIndex - 1].IsClosingTag);
        //bool noCrossingItalicsOnTheRight = closingSymbolIndex >= specialSymbolsStack.Count - 1 || (specialSymbolsStack[closingSymbolIndex + 1].Type != TokenType.Italics && !specialSymbolsStack[openingSymbolIndex + 1].IsClosingTag);
        
        bool noSpareSpaces =
            sourceString[openingSymbol.Index + openingSymbol.TagLength] != ' ' &&
            sourceString[closingSymbol.Index - 1] != ' ';

        bool distanceBetweenStartAndEndMoreThanZero =
            closingSymbol.Index - openingSymbol.Index > closingSymbol.TagLength;

        bool isWithinOneWord = SpecialSymbolUtils.IsWithinOneWord(sourceString, openingSymbol, closingSymbol);


        bool isDigitAmongWord = SpecialSymbolUtils.IsDigitAmongWord(sourceString, openingSymbol, closingSymbol);

        return noSpareSpaces && distanceBetweenStartAndEndMoreThanZero
                             && isWithinOneWord
                             && !isDigitAmongWord;
        //&& (unClosedItalicsBetween && noCrossingItalicsOnTheRight || !unClosedItalicsBetween);
    }

    public void ResetParameters()
    {
        IsOpened = false;
    }
}