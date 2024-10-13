using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Structs.Tags;

public struct HeaderTag: ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    public TokenType TokenType { get; }
    public TokenType[] TagsCanBeInside { get; }
    public string[] Pattern { get; }

    //  По сути можем сделать статическим, ведь параллелить навряд ли будем
    // Да и тем более его все же наврн лучше сделать статическим, чтоб другие header'ы могли
    // тоже его состояние отслеживать
    public static bool IsOpenedHeader { get; private set; }

    public HeaderTag()
    {
        Symbol = "#";
        IsPaired = false;
        TokenType = TokenType.Header;
        TagsCanBeInside = [TokenType.Text, TokenType.Italics, TokenType.Link, TokenType.Bold];
        Pattern = ["#", "\n"];
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
            IsOpenedHeader = true;
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
        if (index < sourceString.Length && sourceString[index] == '\n' && IsOpenedHeader)
        {
            if (sourceString[index] == '\n')
            {
                specialSymbols.Add(new SpecialSymbol
                {
                    Type = TokenType.Header, Index = index - 1, TagLength = 1, IsPairedTag = false, IsClosingTag = true
                });
                IsOpenedHeader = false;
                //++index;

                return true;
            }
        }
        
        return false;
    }

    public static bool CheckForUnclosedHeaderTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols)
    {
        // Надо будет по красивее сделать, а то повторение кода
        // Этот if предусматривает случай, когда header был открыт,
        // но последним символом в исходной строке был какой-то специальный символ
        // и надо бы header закрыть, чтобы превратить его в токен
        if (index >= sourceString.Length - 1 && IsOpenedHeader)
        {
            specialSymbols.Add(new SpecialSymbol { Type = TokenType.Header, Index = sourceString.Length - 1, TagLength = 1, IsPairedTag = false, IsClosingTag = true });
            IsOpenedHeader = false;
            
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