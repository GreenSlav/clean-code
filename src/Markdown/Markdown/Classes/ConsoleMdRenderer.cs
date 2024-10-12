using System.Text;
using Markdown.Interfaces;
using Markdown;
using Markdown.Enums;
using Markdown.Structs;

namespace Markdown.Classes;

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

    private static string RenderToken(Token token, string input)
    {
        var sb = new StringBuilder();
     
        int length = token.EndIndex - token.StartIndex - token.TagLength * (token.IsPairedTag ? 2 : 1) + 1;
        
        string content = input.Substring(token.StartIndex + token.TagLength , length);

        switch (token.Type)
        {
            case TokenType.Header:
                sb.Append("<h1>");
                sb.Append(RenderInsideTokens(token, input)); 
                sb.Append("</h1>");
                break;
            case TokenType.Bold:
                sb.Append("<strong>");
                sb.Append(RenderInsideTokens(token, input)); 
                sb.Append("</strong>");
                break;
            case TokenType.Italics:
                sb.Append("<em>");
                sb.Append(RenderInsideTokens(token, input));
                sb.Append("</em>");
                break;
            case TokenType.Text:
                sb.Append(SpecialSymbolUtils.GetEscapedText(content));
                break;
        }
        
        return sb.ToString();
    }

    private static string RenderInsideTokens(Token token, string input)
    {
        var sb = new StringBuilder();

        foreach (var innerToken in token.InsideTokens)
        {
            sb.Append(RenderToken(innerToken, input));
        }

        return sb.ToString();
    }
}