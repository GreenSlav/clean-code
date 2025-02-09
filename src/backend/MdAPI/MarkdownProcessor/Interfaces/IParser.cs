using MarkdownProcessor.Enums;
using MarkdownProcessor.Structs;

namespace MarkdownProcessor.Interfaces;

public interface IParser
{
    // Осторожно, следует добавлять в правильной последовательности
    List<ITag> TagsToParse { get; }
    
    // Различные алгоритмы парсинга, например, через стек или еще как-нибудь
    // В оригинальный List добавляем токены в процессе парсинга
    
    // parsedTags - теги, которые парсер будет учитывать
    List<Token> Parse(string textToBeMarkdown, List<ITag> parsedTags);
}