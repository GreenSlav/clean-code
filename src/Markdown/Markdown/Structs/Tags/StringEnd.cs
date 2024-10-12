using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Structs;

public struct StringEnd: ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    public bool IsActuallyTag { get; }
    public TokenType TokenType { get; }

    public StringEnd()
    {
        Symbol = "";
        IsPaired = false;
        IsActuallyTag = false;
        TokenType = TokenType.Header;
    }
    public bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols, ref bool isOpenedHeader)
    {
        // Надо будет по красивее сделать, а то повторение кода
        // Этот if предусматривает случай, когда header был открыт,
        // но последним символом в исходной строке был какой-то специальный символ
        // и надо бы header закрыть, чтобы превратить его в токен
        if (index >= sourceString.Length - 1 && isOpenedHeader)
        {
            specialSymbols.Add(new SpecialSymbol { Type = TokenType.Header, Index = sourceString.Length - 1, TagLength = 1, IsPairedTag = false, IsClosingTag = true });
            isOpenedHeader = false;
            
            return true;
        }
        
        return false;
    }
    
    public bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol)
    {
        bool spaceAfterSharp = (openingSymbol.Index + 1) < sourceString.Length && sourceString[openingSymbol.Index + 1] == ' ';
        bool firstTagIsOpening = openingSymbol.IsClosingTag == false;
        bool lastTagIsClosing = closingSymbol.IsClosingTag;

        return spaceAfterSharp && firstTagIsOpening && lastTagIsClosing;
    }
}