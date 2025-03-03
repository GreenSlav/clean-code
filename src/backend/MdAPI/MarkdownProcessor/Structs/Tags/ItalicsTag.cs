using MarkdownProcessor.Classes;
using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;

namespace MarkdownProcessor.Structs.Tags;

public struct ItalicsTag: ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    public TokenType TokenType { get; }
    public TokenType[] TagsCanBeInside { get; }
    public bool IsOpened { get; set; }

    public ItalicsTag()
    {
        Symbol = "_";
        IsPaired = true;
        TokenType = TokenType.Italics;
        TagsCanBeInside = [TokenType.Text, TokenType.Link];
    }

    public void ValidateInsideTokens(Token token, string sourceString)
    {
        
    }

    public bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols)
    {
        if (index < sourceString.Length && sourceString[index] == '_')
        {
            specialSymbols.Add(new SpecialSymbol
                { Type = TokenType.Italics, Index = index, TagLength = 1, IsPairedTag = true });
            IsOpened = !IsOpened;            
            return true;
        }

        return false;
    }

    public bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol, List<SpecialSymbol> specialSymbols)
    {
        // Если все условия выполнены, выходим из цикла, запоминаем 
        // символов надо удалить из стека, чтоб добраться до этого символ,
        // удаляем из стека эти символы, и готово - у нас есть правильный токен
        // Создаем его
                            
        // openSymbolsStack[j] - открывающий
        // symbol - закрывающий
        // После открывающего и перед закрывающим нет пробела

        bool noSpareSpaces =
            sourceString[openingSymbol.Index + openingSymbol.TagLength] != ' ' &&
            sourceString[closingSymbol.Index - closingSymbol.TagLength] != ' ';

        bool distanceBetweenStartAndEndMoreThanZero =
            closingSymbol.Index - openingSymbol.Index > closingSymbol.TagLength;

        bool isWithinOneWord = SpecialSymbolUtils.IsWithinOneWord(sourceString, openingSymbol, closingSymbol);

        bool IsDigitAmongWord = SpecialSymbolUtils.IsDigitAmongWord(sourceString, openingSymbol, closingSymbol);

        return noSpareSpaces && distanceBetweenStartAndEndMoreThanZero &&
               isWithinOneWord && !IsDigitAmongWord;
    }

    public void ResetParameters()
    {
        IsOpened = false;
    }
}