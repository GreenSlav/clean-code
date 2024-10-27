using MarkdownProcessor.Classes;
using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;

namespace MarkdownProcessor.Structs.Tags;

public struct HeaderTag: ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    public TokenType TokenType { get; }
    public TokenType[] TagsCanBeInside { get; }

    //  По сути можем сделать статическим, ведь параллелить навряд ли будем
    // Да и тем более его все же наврн лучше сделать статическим, чтоб другие header'ы могли
    // тоже его состояние отслеживать
    public bool IsOpened { get; set; }

    public HeaderTag()
    {
        Symbol = "#";
        IsPaired = false;
        TokenType = TokenType.Header;
        TagsCanBeInside = [TokenType.Text, TokenType.Italics, TokenType.Link, TokenType.Bold];
    }

    public void ValidateInsideTokens(Token token, string sourceString)
    {
        
    }

    public bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols)
    {
        if (index < sourceString.Length && sourceString[index] == '#') // Пробел после решетки обязателен,
            // чтобы header сработал
        {
            specialSymbols.Add(new SpecialSymbol { Type = TokenType.Header, Index = index, TagLength = 1, IsPairedTag = false, IsClosingTag = false});
            IsOpened = true;
            SpecialSymbolUtils.IsOpenedHeader = true;
            //++index;
            
            return true;
        }
        
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
        if (index < sourceString.Length && sourceString[index] == '\n' && IsOpened)
        {
            if (sourceString[index] == '\n')
            {
                specialSymbols.Add(new SpecialSymbol
                {
                    Type = TokenType.Header, Index = index - 1, TagLength = 1, IsPairedTag = false, IsClosingTag = true
                });
                IsOpened = false;
                SpecialSymbolUtils.IsOpenedHeader = false;
                //++index;

                return true;
            }
        }
        
        return false;
    }

    public bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol, List<SpecialSymbol> specialSymbols)
    {
        bool spaceAfterSharp = (openingSymbol.Index + 1) < sourceString.Length && sourceString[openingSymbol.Index + 1] == ' ';
        bool firstTagIsOpening = openingSymbol.IsClosingTag == false;
        bool lastTagIsClosing = closingSymbol.IsClosingTag;
        bool isLongEnough = closingSymbol.Index - openingSymbol.Index > 1;

        return spaceAfterSharp && firstTagIsOpening && lastTagIsClosing && isLongEnough;
    }

    public void ResetParameters()
    {
        IsOpened = false;
        SpecialSymbolUtils.IsOpenedHeader = false;
    }
}