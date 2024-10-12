using Markdown.Structs;

namespace Markdown.Interfaces;

public interface IRenderer
{
    string RenderMarkdown(List<Token> tokens, string sourceString);
}