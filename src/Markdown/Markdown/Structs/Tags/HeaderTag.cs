using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Structs;

public struct HeaderTag: ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    
    public bool IsActuallyTag { get; }

    public TokenType TokenType { get; }

    public HeaderTag()
    {
        Symbol = "#";
        IsPaired = false;
        IsActuallyTag = true;
        TokenType = TokenType.Header;
    }
    
    public bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols, ref bool isOpenedHeader)
    {
        if (sourceString[index] == '#') // Пробел после решетки обязателен,
            // чтобы header сработал
        {
            specialSymbols.Add(new SpecialSymbol { Type = TokenType.Header, Index = index, TagLength = 1, IsPairedTag = false, IsClosingTag = false});
            isOpenedHeader = true;
            ++index;
            
            return true;
        }
        
        return false;
    }

    public bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol)
    {
        bool spaceAfterSharp = (openingSymbol.Index + 1) < sourceString.Length && sourceString[openingSymbol.Index + 1] == ' ';
        bool firstTagIsOpening = openingSymbol.IsClosingTag == false;
        bool lastTagIsClosing = closingSymbol.IsClosingTag;
        bool isLongEnough = closingSymbol.Index - openingSymbol.Index > 1;

        return spaceAfterSharp && firstTagIsOpening && lastTagIsClosing && isLongEnough;
    }
}