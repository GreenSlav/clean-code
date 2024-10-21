using MarkdownProcessor.Classes;
using MarkdownProcessor.Enums;
using MarkdownProcessor.Structs;

namespace MarkdownProcessor.Interfaces;

public interface ITag: IResetTag
{
    string Symbol { get; }
    bool IsPaired { get; }
     
    TokenType TokenType { get; }
    // Токены, которые по вложенности могут лежать внутри текущего тега
    TokenType[] TagsCanBeInside { get; }
    // То, как выглядит тег
    string[] Pattern { get; }
    
    // ref нужен для изменения индекса в случае идентификации какого-то тега,
    // чтоб перепрыгнуть символы с найденным тегом, чтоб потом их опять не проходить
    void ValidateInsideTokens(Token token, string sourceString);
    bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols);
    bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol);
}