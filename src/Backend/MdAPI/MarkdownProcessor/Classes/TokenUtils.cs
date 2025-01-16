using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;
using MarkdownProcessor.Structs;

namespace MarkdownProcessor.Classes;

public static class TokenUtils
{
    public static TokenType[] DefineAllowedInsideTokens(Token token, List<ITag> tags)
    {
        var defineAllowedInsideTokens = tags.FirstOrDefault(tag => tag.TokenType == token.Type)?.TagsCanBeInside;
        if (defineAllowedInsideTokens != null)
            return defineAllowedInsideTokens;

        return [];
    }
    
    public static ITag? DefineTag(TokenType tokenType, List<ITag> tags)
    {
        return tags.FirstOrDefault(tag => tag.TokenType == tokenType);
    }
    
    public static List<Token> ExtractInsideTokens(int startIndex, int endIndex, List<Token> tokens)
    {
        // Извлекаем вложенные токены, которые лежат внутри текущего токена
        List<Token> resultToReturn = new List<Token>();
        // Мы получили в переменную выше токены, которые нужно засунуть
        // в InsideTokens
        // Теперь нужно их удалить из главного массива tokens
        foreach (var token in tokens.ToList())
        {
            // Сделал <=, чтоб корректно обрабатывать вложенность header'ов, которые ведут до конца строки
            // Пример "# header _ada_"
            if (token.StartIndex > startIndex && token.StartIndex <= endIndex)
            {
                resultToReturn.Add(token);
                tokens.Remove(token);
            }
        }

        return resultToReturn;
    }
    
    // В этом методе удалим токены, которые не могут рендериться в текущем токене
    // Пример: _text__text__text_ => <em>text__text__text</em>
    // По сути самый длинный рекусривный вызов здесь может быть длиной в 3, 
    // тк иначе даже если будет __a __текст, который может еще суб-токену содержать__ a__,
    // то такой суб-массив просто удалиться, и не будет обрабатываться
    // NOTE: По сути здесь никогда не может быть nullRefEx,
    // тк как любому токену присуждается минимум пустой список
    // САМОЕ ГЛАВНОЕ - ВЫЗЫВАТЬ ЭТОТ МЕТОД ПЕРЕД FillTokensListsWithTextTokens
    public static void RemoveInvalidTokens(Token token, List<ITag> activeTags)
    {
        var allowedTagsInside = DefineAllowedInsideTokens(token, activeTags);
        
        for (int i = token.InsideTokens.Count - 1; i >= 0; i--)
        {
            var insideToken = token.InsideTokens[i];

            bool tagsIsAllowed = false;
            
            foreach (var allowedTag in allowedTagsInside)
            {
                if (insideToken.Type == allowedTag)
                {
                    tagsIsAllowed = true;
                }
            }
            
            if (!tagsIsAllowed)
            {
                token.InsideTokens.RemoveAt(i); // Удаляем токен по индексу
            }
            else
            {
                // Рекурсивный вызов для вложенных токенов
                RemoveInvalidTokens(insideToken, activeTags);
            }
        }
    }
    
    // В этом месте заполним пустые символы, не занятые никакими токенами токенами текста
    // Пример: __text_text___ = Bold { Text, Italics },
    // потому что без него будет: Bold { Italics }
    public static void FillTokensListsWithTextTokens(Token token)
    {
        foreach (var tokenInside in token.InsideTokens)
        {
            FillTokensListsWithTextTokens(tokenInside);
        }

        int textStartIndex = token.StartIndex + token.TagLength  + (token.Type == TokenType.Header ? 1: 0);
        
        for (int i = 0; i < token.InsideTokens.Count; i++)
        {
            var insideToken = token.InsideTokens[i];
        
            if (insideToken.StartIndex > textStartIndex)
            {
                token.InsideTokens.Insert(i, new Token()
                {
                    StartIndex = textStartIndex,
                    EndIndex = insideToken.StartIndex - 1,
                    Type = TokenType.Text,
                    IsPairedTag = false,
                    TagLength = 0,
                });

                ++i; // Перепрыгиваем только что созданный токен
            }

            textStartIndex = insideToken.EndIndex + 1;
        }
    
        // Обработка расстояния между последним токеном внутри и правой границей
        if (token.EndIndex - ((token.IsPairedTag) ? token.TagLength : 0) >= textStartIndex)
        {
            int endIndex = token.EndIndex - ((token.IsPairedTag) ? token.TagLength : 0);

            token.InsideTokens.Add(new Token()
            {
                StartIndex = textStartIndex,
                EndIndex = endIndex,
                Type = TokenType.Text,
                IsPairedTag = false,
                TagLength = 0,
            });
        }
    }
}