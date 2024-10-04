﻿using MarkdownDraft;

public struct Token : IEquatable<Token>
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public TokenType Type;
    // Как будто бы Content бесполезен, если все равно есть InsideTokens, если только
    // для текстовых токенов
    private string _textContent;
    public string TextContent {
        get
        {
            return _textContent;
        }
        set
        {
            // Позволяем установить контент, если тип текстовый
            if (Type == TokenType.Text)
            {
                _textContent = value;
            }
        } 
    }
    
    // Внутри токена, например, заголовка может храниться и курсив, и жир,
    // поэтому нужно создать массив еще внутренних токенов.
    // Когда будем превращать все это дело в html просто сначала будем проходиться циклом
    // по основному списку, и на каждой итерации проверять, нет ли у текущего токена
    // вложенных токенов
    public List<Token> InsideTokens { get; set; }

    public bool Equals(Token other)
    {
        return Type == other.Type && StartIndex == other.StartIndex && EndIndex == other.EndIndex;
    }

    public override bool Equals(object? obj)
    {
        return obj is Token other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Type, StartIndex, EndIndex);
    }
}