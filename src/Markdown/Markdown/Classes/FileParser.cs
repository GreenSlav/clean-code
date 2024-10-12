using Markdown.Interfaces;
using Markdown.Enums;
using Markdown.Structs;

namespace Markdown.Classes;

public class FileParser: IParser
{
    // В контексте данного класса строка textToBeMarkdown - путь к файлу .md, который нужно запарсить
    public List<ITag> TagsToParse { get; }

    public List<Token> Parse(string textToBeMarkdown, List<ITag> parsedTags)
    {
        throw new NotImplementedException();
    }
}