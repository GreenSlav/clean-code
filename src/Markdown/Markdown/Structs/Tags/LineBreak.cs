using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Structs;

public struct LineBreak: ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    
    public bool IsActuallyTag { get; }

    public TokenType TokenType { get; }

    public LineBreak()
    {
        Symbol = "\n";
        IsPaired = false;
        IsActuallyTag = false;
        TokenType = TokenType.Header;
    }

    public bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols,
        ref bool isOpenedHeader)
    {
        // i > 0 потому что будем считать, что перенос на новую строку
        // будет считаться концом header'а
        // а header будет кончаться перед переносом на новую строку
        // хотя не, во избежании ситуации "#_text_\n" лучше оставлю последний символ
        // header'а на самом переносе, а то потом проблемы с поиском вложенных могут возникнуть:
        // if (token.StartIndex > startIndex && token.StartIndex < endIndex)
        // Внимание на строгое сравнение
        // А нет, походу придется сделать все-таки до новой строки,
        // ведь может возникнуть ситуация "# text", где header должен работать, хотя 
        // переноса на новую строку (aka закрывающего тега) не было
        // Поэтому теперь закр. тегом будет считать символ до переноса на новую строку
        // и конец строки, если header был открыт
        // Проверка индекс нужно, чтоб понять, где-то выше при инкрементировании не улетели ли мы за пределы
        if (index < sourceString.Length && sourceString[index] == '\n' && isOpenedHeader)
        {
            if (sourceString[index] == '\n')
            {
                specialSymbols.Add(new SpecialSymbol
                {
                    Type = TokenType.Header, Index = index - 1, TagLength = 1, IsPairedTag = false, IsClosingTag = true
                });
                isOpenedHeader = false;
                ++index;

                return true;
            }
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