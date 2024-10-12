using Markdown.Classes;
using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Structs;

public struct ItalicsTag: ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    
    public bool IsActuallyTag { get; }
    
    public TokenType TokenType { get; }

    public ItalicsTag()
    {
        Symbol = "_";
        IsPaired = true;
        IsActuallyTag = true;
        TokenType = TokenType.Italics;
    }

    public bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols,
        ref bool isOpenedHeader)
    {
        if (index < sourceString.Length && sourceString[index] == '_')
        {
            specialSymbols.Add(new SpecialSymbol
                { Type = TokenType.Italics, Index = index, TagLength = 1, IsPairedTag = true });
            ++index;

            return true;
        }

        return false;
    }

    public bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol)
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
}