using System.Text;
using MarkdownProcessor;
using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;
using MarkdownProcessor.Structs;

namespace MarkdownProcessor.Classes;

public class ConsoleMdRenderer: IRenderer
{
    public string RenderMarkdown(List<Token> tokens, string sourceString)
    {
        var sb = new StringBuilder();
        sb.Append("<div>");
        int currentIndex = 0;

        foreach (var token in tokens)
        {
            sb.Append(RenderToken(token, sourceString));
        }
        
        sb.Append("</div>");
        return sb.ToString();
    }

    private static string RenderToken(Token token, string sourceString)
    {
        var sb = new StringBuilder();
     
        int length = token.EndIndex - token.StartIndex - token.TagLength * (token.IsPairedTag ? 2 : 1) + 1;
        
        string content = sourceString.Substring(token.StartIndex + token.TagLength , length);

        switch (token.Type)
        {
            case TokenType.Header:
                sb.Append("<h1>");
                sb.Append(RenderInsideTokens(token, sourceString)); 
                sb.Append("</h1>");
                break;
            case TokenType.Bold:
                sb.Append("<strong>");
                sb.Append(RenderInsideTokens(token, sourceString)); 
                sb.Append("</strong>");
                break;
            case TokenType.Italics:
                sb.Append("<em>");
                sb.Append(RenderInsideTokens(token, sourceString));
                sb.Append("</em>");
                break;
            case TokenType.Link:
                sb.Append(RenderLink(token, sourceString));
                break;
            case TokenType.Text:
                sb.Append(SpecialSymbolUtils.GetEscapedText(content));
                break;
        }
        
        return sb.ToString();
    }

    private static string RenderInsideTokens(Token token, string sourceString)
    {
        var sb = new StringBuilder();

        foreach (var innerToken in token.InsideTokens)
        {
            sb.Append(RenderToken(innerToken, sourceString));
        }

        return sb.ToString();
    }

    public static string RenderLink(Token token, string sourceString)
    {
        int bracketIndex = sourceString.IndexOf("](", token.StartIndex, StringComparison.Ordinal);

        // Чтоб разделить ссылку и title
        string[] linkAndTitle = sourceString
            .Substring(bracketIndex + 2, token.EndIndex - (bracketIndex + 2))
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        StringBuilder wordLink = new StringBuilder();
        
        // Проверка внутренних токенов ссылки, хотим убедиться, что если там есть какой-то тег,
        // то он находится между двумя квадратными скобками
        // В этом цикле мы собрали первую часть тега - слово, которое будет являться ссылкой
        for (int i = 0; i < token.InsideTokens.Count; i++)
        {
            var insideToken = token.InsideTokens[i];

            // Если у какого-то внутреннего токена начало там, где сама ссылка (https://example.com)
            // уже должна быть, то такой токен переделываем в текстовый токен, если он еще нет
            if (insideToken.Type != TokenType.Text
                && (insideToken.EndIndex > bracketIndex + 1
                || insideToken.StartIndex > bracketIndex + 1))
            {
                var tempToken = new Token
                {
                    StartIndex = insideToken.StartIndex,
                    EndIndex = insideToken.EndIndex,
                    Type = TokenType.Text,
                    IsPairedTag = false,
                    TagLength = 0,
                };
                
                token.InsideTokens[i] = tempToken;
            }
            
            insideToken = token.InsideTokens[i];
            
            // Это токены, которые не переступали изначально за "]("
            if (insideToken.EndIndex < bracketIndex)
            {
                wordLink.Append(RenderToken(insideToken, sourceString));
            }
            else
            {
                var tokenToRender = insideToken;
                tokenToRender.EndIndex = (tokenToRender.EndIndex > bracketIndex) ? bracketIndex - 1 : tokenToRender.EndIndex;
                
                wordLink.Append(RenderToken(tokenToRender, sourceString));
            }
        }

        return $"<a href=\"{linkAndTitle[0]}\" title={(linkAndTitle.Length > 1 ? string.Join(" ", linkAndTitle[1..]) : "\"" + string.Empty + "\"")}>{wordLink}</a>";
    }
}