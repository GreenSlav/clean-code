namespace Markdown.Enums;

// Иерархия важна, ведь благодаря ей мы ориентуремся в правах тегов, например, ранг у Bold выше, 
// чем у Italics, поэтому Bold внутри Italics работать не будет
public enum TokenType
{
    Text,
    Italics,
    Bold, 
    Header,
    Main, // Вне какого-либо тега
}