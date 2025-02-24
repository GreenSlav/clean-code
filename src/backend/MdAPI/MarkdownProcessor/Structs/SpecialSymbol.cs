using MarkdownProcessor.Enums;

namespace MarkdownProcessor.Structs;

public struct SpecialSymbol
{
    public TokenType Type { get; set; }
    public int Index { get; set; }
    
    // Длина специального символа
    public bool IsPairedTag { get; set; }
    // Длина специального символа
    public int TagLength { get; set; }
    // По большей части понадобится только для непарных тегов
    public bool IsClosingTag { get; set; }
    //public bool IsEscaped { get; set; }
}