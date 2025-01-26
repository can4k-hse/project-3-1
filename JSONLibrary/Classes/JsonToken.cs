// Author: Alexander Yakovlev
// CreatedAt: 23 января 2025 г. 14:10:51
// Filename: JsonToken.cs
// Summary: Класс для определения единицы JSON кодирования токена
// Для больших деталей посетите https://www.json.org/json-en.html


namespace JSONLibrary.Classes;

public readonly struct JsonToken
{
    /// <summary>
    /// Тип JSON токена
    /// </summary>
    public enum TokenType
    {
        String,
        Integer,
        Double,
        Boolean,
        Null,
        Syntax,
        Undefined,
        Whitespace,
    }

    public TokenType Type { get; }
    public object? Value { get; }

    public JsonToken() { }

    public JsonToken(object? token)
    {
        Type = TokenType.Undefined;

        if (token is string)
        {
            Type = TokenType.String;
        }

        if (token is int)
        {
            Type = TokenType.Integer;
        }

        if (token is double)
        {
            Type = TokenType.Double;
        }

        if (token is bool)
        {
            Type = TokenType.Boolean;
        }

        if (token is null)
        {
            Type = TokenType.Null;
        }

        if (token is char c1 && JsonParser.JsonSyntax.Contains(c1))
        {
            Type = TokenType.Syntax;
        }

        if (token is char c2 && JsonParser.Whitespaces.Contains(c2))
        {
            Type = TokenType.Whitespace;
        }

        // Если не смогли определить тип токена
        if (Type == TokenType.Undefined)
        {
            throw new ArgumentException("invalid token");
        }

        Value = token;
    }

    /// <summary>
    /// Возвращает -1, если токен не является фигурной скобкой, 0, если является '{' и 1, если '}'. 
    /// </summary>
    public int IsFigureBracket
    {
        get
        {
            if (Type != TokenType.Syntax)
            {
                return -1;
            }

            if ((char)Value == '{')
            {
                return 0;
            }

            if ((char)Value == '}')
            {
                return 1;
            }

            return -1;
        }
    }

    /// <summary>
    /// Возвращает -1, если токен не является квадратной скобкой, 0, если является '[' и 1, если ']'. 
    /// </summary>
    public int IsSquareBracket
    {
        get
        {
            if (Type != TokenType.Syntax)
            {
                return -1;
            }

            if ((char)Value == '[')
            {
                return 0;
            }

            if ((char)Value == ']')
            {
                return 1;
            }

            return -1;
        }
    }

    /// <summary>
    /// Возвращает -1, если токен не является скобкой, 0, если является '{', 1, если '}'.
    /// 2, если является '[', 3, если является ']' 
    /// </summary>
    public int IsBracket()
    {
        if (IsFigureBracket != -1)
        {
            return IsFigureBracket;
        }

        if (IsSquareBracket != -1)
        {
            return 2 + IsSquareBracket;
        }

        return -1;
    }

    /// <summary>
    /// Приводит JsonToken к строковому представлению согласно типу токена
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override string ToString()
    {
        return Type switch
        {
            TokenType.String => (string) Value,
            TokenType.Boolean => ((bool)Value).ToString().ToLower(),
            TokenType.Double => ((double)Value).ToString(),
            TokenType.Syntax => ((char)Value).ToString(),
            TokenType.Integer => ((int)Value).ToString(),
            TokenType.Whitespace => ((char)Value).ToString(),
            TokenType.Null => "null",
            _ => throw new Exception("Undefined token")
        };
    }

    /// <summary>
    /// Возвращает строковое представление типа токена
    /// </summary>
    /// <returns></returns>
    public new string GetType()
    {
        return Type.ToString();
    }

    /// <summary>
    /// Является ли токен whitespace
    /// </summary>
    public bool IsWhitespace => Type == TokenType.Whitespace; 
    
    /// <summary>
    /// Является ли данный токен строкой
    /// </summary>
    public bool IsString => Type == TokenType.String;

    /// <summary>
    /// Является ли данный токен ':'
    /// </summary>
    public bool IsColon => Type == TokenType.Syntax && (char)Value == ':';

    /// <summary>
    /// Является ли данный токен ','
    /// </summary>
    public bool IsComma => Type == TokenType.Syntax && (char)Value == ',';

    /// <summary>
    /// Является ли данный токен примитивом (не синтаксисом или Undefined)
    /// </summary>
    public bool IsPrimitive =>
        Type is TokenType.Boolean or TokenType.Double or TokenType.Null or TokenType.String
            or TokenType.Integer;
}