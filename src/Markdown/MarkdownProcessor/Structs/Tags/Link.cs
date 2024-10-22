using MarkdownProcessor.Classes;
using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;

namespace MarkdownProcessor.Structs.Tags;

public struct Link : ITag
{
    public string Symbol { get; }
    public bool IsPaired { get; }
    public TokenType TokenType { get; }
    public TokenType[] TagsCanBeInside { get; }
    public bool IsOpened { get; set; }

    // Здесь отслеживаем как в данный момент выглядит символ
    // Пример: "[](", "["
    public string CurrentString { get; private set; }

    public Link()
    {
        Symbol = "[]()";
        IsPaired = true;
        TokenType = TokenType.Link;
        TagsCanBeInside = [TokenType.Text, TokenType.Italics, TokenType.Bold];
        CurrentString = string.Empty;
    }

    public void ValidateInsideTokens(Token token, string sourceString)
    {
        // К данному этапу тег валидацию прошел
        // и внутри токенов могут быть только токены !текста
        // Если внутри и есть теги, то они не должны выходить за пределы []

        for (int i = token.InsideTokens.Count - 1; i >= 0; i--)
        {
            var insideToken = token.InsideTokens[i];
            
            bool insideTagsBeforeGoLink = token.StartIndex < insideToken.StartIndex 
                                          && insideToken.EndIndex < sourceString.IndexOf("](", token.StartIndex, StringComparison.Ordinal);

            if (!insideTagsBeforeGoLink)
            {
                token.InsideTokens.RemoveAt(i);
            }
        }
    }
    

    public bool CheckSymbolForTag(string sourceString, ref int index, List<SpecialSymbol> specialSymbols)
    {
        if (index < sourceString.Length)
        {
            switch (sourceString[index])
            {
                // [ []() )
                case '[':
                    if (!IsOpened)
                    {
                        specialSymbols.Add(new SpecialSymbol
                        {
                            Type = TokenType.Link, Index = index, TagLength = 1, IsPairedTag = true,
                            IsClosingTag = false
                        });
                        IsOpened = true;
                        //++index;
                    }
                    else
                    {
                        // Меняем начало ссылки, ведь после "[" встретили еще один "[", теперь он
                        // открывающий для ссылки
                        for (int i = specialSymbols.Count - 1; i >= 0; i--)
                        {
                            if (specialSymbols[i].Type == TokenType.Link && !specialSymbols[i].IsClosingTag)
                            {
                                specialSymbols.RemoveAt(i);
                                specialSymbols.Add(new SpecialSymbol
                                {
                                    Type = TokenType.Link, Index = index, TagLength = 1, IsPairedTag = true,
                                    IsClosingTag = false
                                });
                                //++index;
                                break;
                            }
                        }
                    }

                    CurrentString = "[";
                    return true;
                    break;

                case ']':
                    if (!IsOpened) // Ссылочный тег даже не открыт, идем дальше
                    {
                        //++index;
                    }
                    else
                    {
                        if (CurrentString == "[") // Все хорошо, ищем дальше закрывающий
                        {
                            //++index;
                            CurrentString = "[]";
                            return true;
                        }

                        // По сути тут никакого else не может быть
                    }

                    break;

                case '(':
                    if (!IsOpened) // Ссылочный тег даже не открыт, идем дальше
                    {
                        //++index;
                    }
                    else
                    {
                        if (CurrentString == "[]") // Все хорошо, ищем дальше закрывающий
                        {
                            //++index;
                            CurrentString = "[](";
                            return true;
                        }

                        // По сути тут никакого else не может быть
                    }

                    break;

                case ')':
                    if (IsOpened)
                    {
                        if (CurrentString == "[](") // Все хорошо, ищем дальше закрывающий
                        {
                            CurrentString = "";
                            specialSymbols.Add(new SpecialSymbol
                            {
                                Type = TokenType.Link, Index = index, TagLength = 1, IsPairedTag = true,
                                IsClosingTag = true
                            });
                            IsOpened = false;
                            //++index;
                            return true;
                        }

                        // По сути тут никакого else не может быть

                    }

                    break;
            }
        }

        return false;
    }

    public bool ValidatePairOfTags(string sourceString, in SpecialSymbol openingSymbol, in SpecialSymbol closingSymbol)
    {
        string subString = sourceString.Substring(openingSymbol.Index, closingSymbol.Index - openingSymbol.Index + 1);

        bool openingAndClosing = openingSymbol.IsClosingTag == false && closingSymbol.IsClosingTag;
        // Пока на всякий случай оставлю, но по сути это предусматривается выше на этапе добавление спецсимвола
        //bool hasNeededAllBrackets = subString.IndexOf()
        // Лень стало, потом если че сделаю

        // Проверка, что подстрока "](" существует
        int bracketIndex = subString.IndexOf("](", StringComparison.Ordinal);
        bool containsBracketSequence = bracketIndex != -1;

        // Проверка, что расстояние от openingSymbol.Index до "](" больше нуля
        bool distanceFromOpeningMoreThanZero = containsBracketSequence && bracketIndex - 1 > 0;
        
       // Проверка, что расстояние от closingSymbol.Index до "](" больше нуля
        bool distanceFromClosingMoreThanZero =
            containsBracketSequence && 1 < (subString.Length - 1 - (bracketIndex + 1));

        // Чтоб не уйти за границы
        if (distanceFromOpeningMoreThanZero && distanceFromClosingMoreThanZero)
        {
            // Проверка, что действительно есть слово и ссылка, а не тупо пробелы
            bool notWhiteSpaces = subString
                                      .Substring(0, bracketIndex - 1)
                                      .All(c => char.IsWhiteSpace(c)) 
                                  || subString
                                      .Substring(bracketIndex + 2, subString.Length - 1 - (bracketIndex + 2))
                                      .All(c => char.IsWhiteSpace(c));
            
            // Нужно проверить, что если и есть Title в ссылке, то он помечен кавычками правильно
            string[] linkAndTitle = subString.Substring(bracketIndex + 2, subString.Length - 1 - (bracketIndex + 2)).Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            bool isValidTitle = true;
            if (linkAndTitle.Length > 1)
            {
                string titleString = string.Join(" ", linkAndTitle[1..]);
                // Проверяем, что если title и еть, то есть linkAndTitle.Length > 1, то он окружен кавычками
                isValidTitle = titleString
                    .Count(ch => ch == '"') == 2
                               && titleString
                                .Where((ch, i) => ch == '"' && (i == 0 || i == titleString.Length - 1))
                                .Count() == 2;
            }
            
            // Возвращаем общее условие валидации
            return openingAndClosing && containsBracketSequence 
                                     && distanceFromOpeningMoreThanZero 
                                     && distanceFromClosingMoreThanZero
                                     && !notWhiteSpaces
                                     && !SpecialSymbolUtils.IsEscaped(sourceString, bracketIndex)
                                     && !SpecialSymbolUtils.IsEscaped(sourceString, bracketIndex + 1)
                                     && isValidTitle;
                                
        }
        
        return false;
    }

    public void ResetParameters()
    {
        IsOpened = false;
        CurrentString = String.Empty;
    }
}