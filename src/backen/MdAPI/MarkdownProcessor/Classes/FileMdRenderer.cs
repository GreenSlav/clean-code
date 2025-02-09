using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;
using MarkdownProcessor.Structs;

namespace MarkdownProcessor.Classes;

public class FileMdRenderer: IRenderer
{
    public readonly string PathToFileToWrite;
    
    public FileMdRenderer(string pathToFileToWrite)
    {
        PathToFileToWrite = pathToFileToWrite;
    }
    
    public string RenderMarkdown(List<Token> tokens, string sourceString)
    {   // Здесь как-то записываем html в файлик по директории PathToFile
        throw new NotImplementedException();
    }
}