using Markdown.Classes;
using Markdown.Enums;
using Markdown.Structs;

namespace Markdown.Interfaces;

public interface ITag
{
     string Symbol { get; }
     bool IsPaired { get; }
     
     // Чтоб отличать действительно теги, от вспомогательных символов, например '\n' или конец строки
     // P.S. Походу это свойство все-таки не понадобится
     bool IsActuallyTag { get; }
     TokenType TokenType { get; }
     
     // ref нужен для изменения индекса в случае идентификации какого-то тега,
     // чтоб перепргынуть символы с найденным тегом, чтоб потом их опять не проходить
    bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols, ref bool isOpenedHeader);
    bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol);
}