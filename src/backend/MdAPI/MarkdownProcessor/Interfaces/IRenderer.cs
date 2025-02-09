using MarkdownProcessor.Structs;

namespace MarkdownProcessor.Interfaces;

public interface IRenderer
{
    string RenderMarkdown(List<Token> tokens, string sourceString);
}