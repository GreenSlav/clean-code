using Markdown.Interfaces;
using Markdown.Structs;

namespace Markdown;

public class MdProcessor
{
    private List<Token> _tokens;
    // Решил вынести поля ниже именно в этот класс, тк посчитал, что именно он должен
    // основным пайплайном md процессинга, включающего парсинг и рендер
    private IRenderer _renderer; 
    private IParser _parser;
    
    public MdProcessor(IParser parser, IRenderer renderer)
    {
        _renderer = renderer;
        _parser = parser;
    }

    // Строка textToParse - это либо путь к файлу, либо строка для парсинга в качестве md
    /// <summary>
    /// Парсит и рендерит MD формат в виде HTML с учетом параметров, переданных в конструктор данного класса
    /// </summary>
    /// <param name="textToParse">Текст, который подвергнется парсингу</param>
    /// <returns>Возвращает результат конвертации MD формата в HTML</returns>
    public string ParseAndRender(string textToParse)
    {
        // Очищаем список токенов перед парсингом и рендером
        _tokens = _parser.Parse(textToParse, _parser.TagsToParse);
        string resultOfRender = _renderer.RenderMarkdown(_tokens, textToParse);

        return resultOfRender;
    }
}
