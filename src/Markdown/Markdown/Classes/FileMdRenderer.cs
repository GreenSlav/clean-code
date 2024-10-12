using Markdown.Interfaces;
using Markdown.Enums;
using Markdown.Structs;

namespace Markdown.Classes;

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