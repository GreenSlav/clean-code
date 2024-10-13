using System.Text;
using Markdown.Interfaces;
using Markdown.Structs;
using Markdown.Enums;
using Markdown.Structs.Tags;

namespace Markdown.Classes;

public class StringParser: IParser
{
    public List<ITag> TagsToParse { get; }

    public StringParser(List<ITag> tagsToParse)
    {
        TagsToParse = tagsToParse;
    }
    
    // В контексте данного класса строка textToBeMarkdown - строка, которую нужно превратить в html
    public List<Token> Parse(string textToBeMarkdown, List<ITag> tagsToParse)
    {
        var listOfSpecialSymbols = new List<SpecialSymbol>();
        var openSymbolsStack = new List<SpecialSymbol>();
        // Главный токен, то что будет содержать в себе весь контент строки
        var mainToken = new Token()
        {
            Type = TokenType.Main,
            StartIndex = 0,
            EndIndex = textToBeMarkdown.Length - 1,
            IsPairedTag = true,
            TagLength = 0,
            InsideTokens = new List<Token>()
        }; 
        
        for (int i = 0; i < textToBeMarkdown.Length; i++)
        {
            // Если перед тегом стоит четное кол-во экранирований, то пропускаем этот тег
            if (SpecialSymbolUtils.IsEscaped(textToBeMarkdown, i))
            {
                continue;
            }

            var ch = textToBeMarkdown[i];
            foreach (var tag in tagsToParse)
            {
                // Пытаемся проверить текущий символ на специальность, если текущий индекс еще не занят
                // каким-то специальным символом
                if (listOfSpecialSymbols.Count == 0 
                    || (listOfSpecialSymbols.Count > 0 
                        && listOfSpecialSymbols[^1].Index + listOfSpecialSymbols[^1].TagLength - 1 < i))
                    tag.CheckSymbolForTag(textToBeMarkdown, ref i, listOfSpecialSymbols);
            }
            
            // Проверяем, не осталось ли незакрытого заголовка
            HeaderTag.CheckForUnclosedHeaderTag(textToBeMarkdown, ref i, listOfSpecialSymbols);
        }
        
        
        for (int i = 0; i < listOfSpecialSymbols.Count; i++)
        {
            var symbol = listOfSpecialSymbols[i];

            // Если это открывающий символ
            if (SpecialSymbolUtils.IsOpeningSymbol(symbol, openSymbolsStack))
            {
                openSymbolsStack.Add(symbol);
            }
            else
            {
                // Здесь мы пытаемся найти закрывающий символ
                // По сути если мы попали в это ветвление, то открывающий тег точно есть
                // по этому ищем его и удаляем вложенность в закрывающемся теге

                // Пришел инициализировать здесь, а то внизу там при инициализации токена мозги делают
                SpecialSymbol openingSymbol = new SpecialSymbol();
                // Переменная снизу даст понять, что пара тегов не сошлась, токен создавать не надо
                // и позволит перейти к следующей итерации, может там найдется пара корректных тегов
                bool tagIsNeedToBeSkipped = false;
                
                for (int j = openSymbolsStack.Count - 1; j >= 0; j--)
                {
                    if (openSymbolsStack[j].Type == symbol.Type)
                    {
                        bool exitCurrentLoop = false;
                        
                        foreach (var tag in tagsToParse)
                        {
                            if (tag.TokenType == symbol.Type)
                            {
                                // Тут будет проверяться корректность потенциального токена перед его созданием
                                // Типо пробел после header'а, его отсутствие после открывающего "_" и тд
                                bool isValidPair = tag.ValidatePairOfTags(textToBeMarkdown, openSymbolsStack[j], symbol);

                                if (isValidPair)
                                {
                                    openingSymbol = openSymbolsStack[j];
                                    openSymbolsStack.RemoveAt(j);
                                }
                                else
                                {
                                    // Теги не прошли условия
                                    // Удаляем самый первый тег, потому что может со следующим повезет
                                    // Ситуация: ["_", "_", "_"]
                                    // Тег 0 и 1 не подошли, может тогда 1 и 2 подойдут?
                                    openSymbolsStack.RemoveAt(j);
                                    openSymbolsStack.Add(symbol);
                                    tagIsNeedToBeSkipped = true;
                                }
                                
                                exitCurrentLoop = true;
                                break;
                            }
                        }

                        if (exitCurrentLoop)
                            break;
                        
                        // Удаляем все элементы на пути к открывающему тегу
                        openSymbolsStack.RemoveAt(openSymbolsStack.Count - 1);
                    }
                }
                    
                if (tagIsNeedToBeSkipped)
                    continue;

                
                var newToken = new Token
                {
                    StartIndex = openingSymbol.Index,
                    EndIndex = symbol.Index + symbol.TagLength - 1,
                    Type = symbol.Type,
                    // Вот эти два свойства ниже понадобятся нам когда будем очищать
                    // Контент токенов от самих тегов
                    TagLength = openingSymbol.TagLength,
                    IsPairedTag = openingSymbol.IsPairedTag,
                    InsideTokens =
                        TokenUtils.ExtractInsideTokens(openingSymbol.Index, symbol.Index, mainToken.InsideTokens),
                };

                TokenUtils.DefineTag(symbol.Type, tagsToParse)?.ValidateInsideTokens(newToken, textToBeMarkdown);


                // Остается одна проблема
                // Вложенные в другие теги теги все равно будут находиться в tokens
                // Поэтому лучше их как-то удалить,
                // сделаю это в методе ExtractInsideTokens
                mainToken.InsideTokens.Add(newToken);
            }
        }

        TokenUtils.RemoveInvalidTokens(mainToken, tagsToParse);
        TokenUtils.FillTokensListsWithTextTokens(mainToken);
        
        return mainToken.InsideTokens;
    }
}