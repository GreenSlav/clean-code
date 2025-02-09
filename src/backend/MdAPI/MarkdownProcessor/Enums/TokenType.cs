namespace MarkdownProcessor.Enums;

// Иерархия важна, ведь благодаря ей мы ориентуремся в правах тегов, например, ранг у Bold выше, 
// чем у Italics, поэтому Bold внутри Italics работать не будет

// Иерархия уже не так важна будет, появилась ссылка и все испортила
public enum TokenType
{
    Text,
    Italics,
    Bold, 
    Link,
    Header,
    Main, // Вне какого-либо тега
}