using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;

namespace MarkdownProcessor.Structs.Tags;

// Необходимая абстракция
public struct MainTag: ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    public TokenType TokenType { get; }
    public TokenType[] TagsCanBeInside { get; }
    public bool IsOpened { get;  set; }

    public MainTag()
    {
        Symbol = "";
        IsPaired = true;
        TokenType = TokenType.Main;
        TagsCanBeInside = [TokenType.Text, TokenType.Italics, TokenType.Link, TokenType.Bold, TokenType.Header];
    }

    public void ValidateInsideTokens(Token token, string sourceString)
    {
        
    }

    public bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols)
    {
        return false;
    }

    public bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol)
    {
        return true;
    }

    public void ResetParameters()
    {
        IsOpened = false;
    }
}