using MarkdownProcessor.Enums;
using MarkdownProcessor.Interfaces;
using MarkdownProcessor.Structs;

namespace MarkdownProcessor.Classes;

public class FileParser: IParser
{
    // В контексте данного класса строка textToBeMarkdown - путь к файлу .md, который нужно запарсить
    public List<ITag> TagsToParse { get; }

    public List<Token> Parse(string textToBeMarkdown, List<ITag> parsedTags)
    {
        throw new NotImplementedException();
    }
}