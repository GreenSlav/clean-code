using System.Diagnostics;
using System.Text;
using Markdown;
using Markdown.Classes;
using Markdown.Interfaces;
using Markdown.Structs;
using Markdown.Structs.Tags;

namespace MarkdownDraft;

class Program
{
    public static bool IsUnderlinesWithinNumbers(string sourceString, SpecialSymbol openingSymbol,
        SpecialSymbol closingSymbol)
    {
        bool containsAllNumbers = sourceString.Substring(openingSymbol.Index + openingSymbol.TagLength, 
                closingSymbol.Index - openingSymbol.Index - openingSymbol.TagLength)
            .All(c => char.IsDigit(c));

        bool leftGood = openingSymbol.Index - 1 < 0 || sourceString[openingSymbol.Index - 1] == ' ';
        bool rightGood = closingSymbol.Index + closingSymbol.TagLength >= sourceString.Length || 
                         sourceString[closingSymbol.Index + closingSymbol.TagLength] == ' ';

        // Случаи когда выделяем целое число, которое не входит ни в какое слово
        if (containsAllNumbers && leftGood && rightGood)
            return true;
        
        // К этому моменту если выражение даст true, значит среди цифр точно есть буквы и 
        // нужно вернуть false 
        bool containsAnyNumbers = sourceString.Substring(openingSymbol.Index + openingSymbol.TagLength, 
                closingSymbol.Index - openingSymbol.Index - openingSymbol.TagLength)
            .Any(c => char.IsDigit(c));

        if (containsAnyNumbers)
            return false;
            
        // Но может быть ситуация: "_ццц_ц1"
        // Остается найти в текущем слове цифры
        int leftIndex = openingSymbol.Index - 1;
        // Наткнулись на пробел идя слева от слова
        bool encounteredSpaceFromLeft = false;
        int rightIndex = closingSymbol.Index + closingSymbol.TagLength;
        // Наткнулись на пробел идя справа от слова
        bool encounteredSpaceFromRight = false;
        
        // В слове, подверженном выделению, есть цифры, выделять в нем не стоит
        bool wordWithinNumbers = false;
        
        while ((!encounteredSpaceFromLeft || !encounteredSpaceFromRight) && (leftIndex >= 0 || rightIndex < sourceString.Length) )
        {
            if (leftIndex >= 0)
            {
                if (char.IsDigit(sourceString[leftIndex]))
                    wordWithinNumbers = true;
                encounteredSpaceFromLeft = sourceString[leftIndex] == ' ';
            }
            
            if (rightIndex < sourceString.Length)
            {
                if (char.IsDigit(sourceString[rightIndex]))
                    wordWithinNumbers = true;
                encounteredSpaceFromRight = sourceString[rightIndex] == ' ';
            }
            
            --leftIndex;
            ++rightIndex;
        }


        return !wordWithinNumbers;
    }
    
    static void Main(string[] args)
    {
        // TODO: Сделать так, чтоб внутри одинарных не работали двойные для этого в enum'е TokenType я специально иерархически типы токенов выстроил. Сделаем отдельный метод ValidateTokens или чет типо того ( СДЕЛАНО )
        //
        // TODO: Пробелы между словами всегда должны быть одинарными 
        // if (str[i] == ' ' && i > 0 && str[i-1] == ' ', тогда скипаем текущие символ
        // 
        // TODO: Также надо предусмотреть, что я же не просто так указал в структуре SpecialSymbol свойство Length ( СДЕЛАНО )
        //
        // TODO: Когда дойдем до рендера, убирать специальные символы (например "__") будем ориентуруясь на то, парный ли тег (IsPairedTag) и какова длина специального символа (TagLength)
        //
        // TODO: Сделать так, чтобы теги при экранировании через "\" не превращались в теги. В случае "\_text\_" должно получиться после рендера "_text_", поэтому просто проигнорировать символы экранирования не получится, нужно будет их убрать в отдельном методе, который будет отвечать за фильтрацию текста, то есть туда же нужно будет добавить решение проблемы с пробелами.
        
        string str1 = "__dadawdaw _wdwadawd_ wdadawdawdawd__";
        string str2 = "_dadawdaw __wdwadawd__ wdadawdawdawd_";
        string str3 = "#__dadawdaw _wdwadawd_ wdadawdawdawd__     \n wdadawda";
        string str4 = "__text_text___";
        string str5 = "_italic_";
        string str6 = "__bold__";
        string str7 = "This is _italic_ text.";
        string str8 = "__bold _italic_ text__";
        string str9 = "__bold text_ with _italic_ text__";
        string str10 = "This is _italic text";
        string str11 = "#dwadadawd";
        string str12 = "# Заголовок первого уровня\n\nЭто пример текста, который будет использоваться для тестирования парсера Markdown. Здесь мы можем использовать __жирный текст__, чтобы выделить важные слова, и _курсив_, чтобы сделать акцент на других аспектах.\n\n## Заголовок второго уровня\n\nВ этом разделе мы будем обсуждать различные аспекты парсинга. Например, мы можем смешивать __жирный__ и _курсивный_ текст, чтобы увидеть, как парсер справляется с разными форматами.\n\n### Заголовок третьего уровня\n\n1. Первый элемент списка\n2. Второй элемент списка с _курсивом_\n3. Третий элемент списка с __жирным текстом__ и _курсивом_\n\nТеперь давайте посмотрим на более сложные примеры:\n\nЭто текст с __жирным__ и _курсивом_, а также # заголовком. Следующий параграф будет содержать даже больше форматов.\n\n__Важно:__ __жирный текст__ должен быть правильно обработан, а _курсив_ не должен конфликтовать с __жирным__.\n\nЕсли вы прочитали этот текст, вы можете заметить, что # заголовок не должен быть затенен другим текстом. Например, __жирный текст__, который находится _между курсивом_ и заголовком.\n\n# Заключение\n\nМы надеемся, что этот текст помог вам протестировать ваш парсер. Убедитесь, что все форматы правильно обрабатываются. Если __жирный текст__ стоит в конце строки, а _курсив_ в начале, это не должно вызывать ошибок. # Заголовок должен оставаться отдельным и не смешиваться с другими форматами.\n\nТаким образом, тестируя производительность вашего метода, вы можете увидеть, как он справляется с обработкой длинного текста. Не забудьте протестировать его на различных вводных данных!";
        string str13 = " __text__  _wdada_ ";
        string str14 = "# Заголовок первого уровня\n\nЭто пример длинного текста для тестирования парсера Markdown. В этом тексте мы используем _курсив_ и __жирный текст__ для проверки, как работает обработка таких тегов.\n\nТестирование парсера важно для того, чтобы убедиться, что все теги правильно интерпретируются. Например, _курсивный текст_ помогает выделять слова в предложениях, а __жирный текст__ используется для акцентирования на важных фразах.\n\n# Вложенные элементы\n\nКроме этого, необходимо проверять, как работает парсер с _вложенными_ тегами. Например, вот так: __жирный _и курсив_ в одном предложении__.\n\nТакже стоит протестировать парсер на больших объемах текста, чтобы убедиться, что __он не замедляется__ при обработке длинных строк. Оптимизация работы парсера очень важна, так как это напрямую влияет на производительность программы.\n\n# Тестирование производительности\n\nВот пример длинного текста с большим количеством тегов для проверки производительности:\n\n__Это жирный текст__, а вот _курсивный текст_, который используется для различных тестов. Продолжаем добавлять больше текста, чтобы создать нагрузку на парсер. Проверяем, как _курсив_ и __жирный__ текст взаимодействуют друг с другом.\n\nТеперь давайте добавим еще больше текста, чтобы убедиться, что парсер справляется с обработкой длинных строк. Мы будем добавлять теги _курсива_ и __жирного текста__, чтобы увидеть, как они работают вместе.\n\n__Жирный текст__ должен обрабатываться правильно, как и _курсивный текст_. Это важно, потому что парсер должен работать с множеством символов и тегов одновременно. Важно убедиться, что программа не начинает __замедляться__ или _падать_ на больших данных.\n\n# Заключение\n\nПарсеры Markdown используются в самых разных проектах, от генерации веб-страниц до редактирования текстов в блогах. Тестирование парсера на различных входных данных позволяет убедиться в его стабильности и производительности. __Жирный текст__ и _курсивный текст_ помогают создавать более выразительный контент, и важно, чтобы парсер корректно обрабатывал эти элементы. Это ссылка на [Google](https://www.google.com \"Тройные кавычки\") ";
        string str15 =
            "# Header";

        

        string test = "Это ссылка на [Google](https://www.google.com \"Тройные кавычки\")";

        var stopwatch1 = new Stopwatch();
        stopwatch1.Start();

        
        
        
        var ital = new ItalicsTag();
        var bold = new BoldTag();
        var link = new Link();
        var header = new HeaderTag();
        var main = new MainTag();
        var list = new List<ITag>();
        list.Add(bold);
        list.Add(ital);
        list.Add(link);
        list.Add(header);
        list.Add(main);
        var mdProc = new MdProcessor(new StringParser(list), new ConsoleMdRenderer());
        var resu = stopwatch1.Elapsed;

        Console.WriteLine(str14.Length);
        mdProc.ParseAndRender(str14);
        stopwatch1.Stop();
        Console.WriteLine(resu);

        
        return;
        Console.WriteLine("dwjdnawndawndiawidan");
        return;
        /*string test = "\\\\\\_wdada\\_      \\_wdada\\_";
        Console.WriteLine(GetEscapedText(test));
        return;*/
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        
        var result = Parse(test);
        var rendered = Render(result, test);
        Console.WriteLine(rendered);
        

        stopwatch.Stop();
        var res = stopwatch.Elapsed;
        
        
        Console.WriteLine(res);
        Console.WriteLine(test.Length);
        
        
        //Thread.Sleep(10000);
        
        foreach (var VARIABLE in result)
        {
            Console.WriteLine($"{VARIABLE.Type}: {VARIABLE.StartIndex} - {VARIABLE.EndIndex}");
            var arr1 = VARIABLE.InsideTokens;
            if (arr1 != null)
            {
                foreach (var token in VARIABLE.InsideTokens)
                {
                    Console.WriteLine($"\t{token.Type}: {token.StartIndex} - {token.EndIndex}");
                    var arr2 = token.InsideTokens;
                    if (arr2 != null)
                    {
                        foreach (var tkn in token.InsideTokens)
                        {
                            Console.WriteLine($"\t\t{tkn.Type}: {tkn.StartIndex} - {tkn.EndIndex}");
                        }
                    }
                }
            }

            Console.WriteLine("=================================================");
        }
        //Thread.Sleep(10000);
    }
    
    public static List<Token> Parse(string str)
    {
        var listOfSpecialSymbols = new List<SpecialSymbol>();
        var openSymbolsStack = new List<SpecialSymbol>();
        // Строка с обработанным экранированием и пробелами
        var sb = new StringBuilder();
        var mainToken = new Token()
        {
            Type = TokenType.Main,
            StartIndex = 0,
            EndIndex = str.Length - 1,
            IsPairedTag = true,
            TagLength = 0,
            InsideTokens = new List<Token>()
        }; 
        
        
        bool isOpenedHeader = false;

        for (int i = 0; i < str.Length; i++)
        {
            
            // Если перед тегом стоит четное кол-во экранирований, то пропускаем этот тег
            if (IsEscaped(str, i))
            {
                continue;
            }
            
            if (str[i] == '#') // Пробел после решетки обязателен,
            // чтобы header сработал
            {
                listOfSpecialSymbols.Add(new SpecialSymbol { Type = TokenType.Header, Index = i, TagLength = 1, IsPairedTag = false, IsClosingTag = false});
                isOpenedHeader = true;
                ++i;
            }
            
            // i > 0 потому что будем считать, что перенос на новую строку
            // будет считаться концом header'а
            // а header будет кончаться перед переносом на новую строку
            // хотя не, во избежании ситуации "#_text_\n" лучше оставлю последний символ
            // header'а на самом переносе, а то потом проблемы с поиском вложенных могут возникнуть:
            // if (token.StartIndex > startIndex && token.StartIndex < endIndex)
            // Внимание на строгое сравнение
            // А нет, походу придется сделать все-таки до новой строки,
            // ведь может возникнуть ситуация "# text", где header должен работать, хотя 
            // переноса на новую строку (aka закрывающего тега) не было
            // Поэтому теперь закр. тегом будет считать символ до переноса на новую строку
            // и конец строки, если header был открыт
            // Проверка индекс нужно, стоб понять, где-то выше при инкрементировании не улетели ли мы за пределы
            if (i < str.Length && (str[i] == '\n' || i == str.Length - 1) && isOpenedHeader)
            {
                var ch = str[i];
                if (str[i] == '\n')
                {
                    listOfSpecialSymbols.Add(new SpecialSymbol { Type = TokenType.Header, Index = i - 1, TagLength = 1, IsPairedTag = false, IsClosingTag = true});
                    isOpenedHeader = false;
                    continue;
                }
                
                /*if (i == str.Length - 1)
                {
                    listOfSpecialSymbols.Add(new SpecialSymbol { Type = TokenType.Header, Index = i, TagLength = 1, IsPairedTag = false, IsClosingTag = true });
                    isOpenedHeader = false;
                }*/
            }

            /*if (isBoldOpened && i < str.Length - 3 && str.Substring(i, 2) == "___")
            {
                
            }*/
            
            if (i < str.Length - 1 && str.Substring(i, 2) == "__")
            {
                listOfSpecialSymbols.Add(new SpecialSymbol { Type = TokenType.Bold, Index = i, TagLength = 2, IsPairedTag = true });
                i += 2;
            }

            if (i < str.Length && str[i] == '_')
            {
                listOfSpecialSymbols.Add(new SpecialSymbol { Type = TokenType.Italics, Index = i, TagLength = 1, IsPairedTag = true });
                ++i;
            }

            // Надо будет по красивее сделать, а то повторение кода
            // Этот if предусматривает случай, когда header был открыт,
            // но последним символом в исходной строке был какой-то специальный символ
            // и надо бы header закрыть, чтобы превратить его в токен
            if (i >= str.Length - 1 && isOpenedHeader)
            {
                listOfSpecialSymbols.Add(new SpecialSymbol { Type = TokenType.Header, Index = str.Length - 1, TagLength = 1, IsPairedTag = false, IsClosingTag = true });
                isOpenedHeader = false;
            }
        }
        
        
        
        for (int i = 0; i < listOfSpecialSymbols.Count; i++)
        {
            var symbol = listOfSpecialSymbols[i];

            // Если это открывающий символ
            if (IsOpeningSymbol(symbol, openSymbolsStack))
            {
                openSymbolsStack.Add(symbol);
            }
            else
            {
                // Здесь мы пытаемся найти закрывающий символ
                // По сути если мы попали в это ветвление, то открывающий тег точно есть
                // по этому ищем его и удаляем вложенность в закрывающемся теге
                
                // По сути если внутри пары тегов, с которыми мы тут работаем, если
                // и были какие-то теги, то они уже убрались из стека
                // по этому с чистой душой можем делать .Pop()
                // Но может сложиться ситуация: "__text_ text_text_"

                // Пришел инициализировать здесь, а то внизу там при инициализации токена мозги делают
                SpecialSymbol openingSymbol = new SpecialSymbol();
                // Переменная снизу даст понять, что пара тегов не сошлась, токен создавать не надо
                // и позволит перейти к следующей итерации, может там найдется пара корректных тегов
                bool tagIsNeedToBeSkipped = false;
                
                for (int j = openSymbolsStack.Count - 1; j >= 0; j--)
                {
                    if (openSymbolsStack[j].Type == symbol.Type)
                    {
                        // Тут будет проверяться корректность потенциального токена перед его созданием
                        // Типо пробел после header'а, его отсутствие после открывающего "_" и тд
                        if (symbol.Type == TokenType.Italics || symbol.Type == TokenType.Bold)
                        {
                            // Если все условия выполнены, выходим из цикла, запоминаем 
                            // символов надо удалить из стека, чтоб добраться до этого символ,
                            // удаляем из стека эти символы, и готово - у нас есть правильный токен
                            // Создаем его
                            
                            // openSymbolsStack[j] - открывающий
                            // symbol - закрывающий
                            // После открывающего и перед закрывающим нет пробела

                            bool noSpareSpaces =
                                str[openSymbolsStack[j].Index + openSymbolsStack[j].TagLength] != ' ' &&
                                str[symbol.Index - 1] != ' ';

                            bool distanceBetweenStartAndEndMoreThanZero =
                                symbol.Index - openSymbolsStack[j].Index > symbol.TagLength;

                            bool isWithinOneWord = IsWithinOneWord(str, openSymbolsStack[j], symbol);
                            
                            bool containsNumbers = str.Substring(openSymbolsStack[j].Index + openSymbolsStack[j].TagLength, 
                                    symbol.Index - openSymbolsStack[j].Index - openSymbolsStack[j].TagLength)
                                .All(c => char.IsDigit(c));
                            
                            if (noSpareSpaces && distanceBetweenStartAndEndMoreThanZero &&
                                !containsNumbers && isWithinOneWord)
                            {
                                openingSymbol = openSymbolsStack[j];
                                openSymbolsStack.RemoveAt(j);
                                break;
                            }
                            else
                            {
                                // Теги не прошли условия
                                // Удаляем самый первый тег, потому что может со следующим повезет
                                // Ситуация: ["_", "_", "_"]
                                // Тег 0 и 1 не подошли, может тогда 1 и 2 подойдут?
                                openSymbolsStack.RemoveAt(j);
                                openSymbolsStack.Add(symbol);
                                tagIsNeedToBeSkipped = true;
                                break; 
                            }
                            
                        }
                        if (symbol.Type == TokenType.Header)
                        {
                            bool spaceAfterSharp = (openSymbolsStack[j].Index + 1) < str.Length && str[openSymbolsStack[j].Index + 1] == ' ';
                            bool firstTagIsOpening = openSymbolsStack[j].IsClosingTag == false;
                            bool lastTagIsClosing = symbol.IsClosingTag;

                            if (spaceAfterSharp && firstTagIsOpening && lastTagIsClosing)
                            {
                                openingSymbol = openSymbolsStack[j];
                                openSymbolsStack.RemoveAt(j);
                                break; 
                            }
                            else
                            {
                                openSymbolsStack.RemoveAt(j);
                                openSymbolsStack.Add(symbol);
                                tagIsNeedToBeSkipped = true;
                                break; 
                            }
                        }
                        // Удаляем все элементы на пути к открывающему тегу
                        openSymbolsStack.RemoveAt(openSymbolsStack.Count - 1);
                    }
                }
                    
                if (tagIsNeedToBeSkipped)
                    continue;

                
                var newToken = new Token
                {
                    StartIndex = openingSymbol.Index,
                    EndIndex = symbol.Index + symbol.TagLength - 1,
                    Type = symbol.Type,
                    // Вот эти два свойства ниже понадобятся нам когда будем очищать
                    // Контент токенов от самих тегов
                    TagLength = openingSymbol.TagLength,
                    IsPairedTag = openingSymbol.IsPairedTag,
                    InsideTokens = ExtractInsideTokens(openingSymbol.Index, symbol.Index, mainToken.InsideTokens)
                };

                // Остается одна проблема
                // Вложенные в другие теги теги все равно будут находиться в tokens
                // Поэтому лучше их как-то удалить,
                // сделаю это в методе ExtractInsideTokens
                mainToken.InsideTokens.Add(newToken);
            }
        }

        RemoveInvalidTokens(mainToken);
        FillTokensListsWithTextTokens(mainToken);
        
        return mainToken.InsideTokens;
    }

    public static string GetEscapedText(string textToken)
    {
        var sb = new StringBuilder();
        
        // Здесь будем удалять лишние пробелы и обрабатывать экранирование
        for (int i = 0; i < textToken.Length; i++)
        {
            if (i > 0 && textToken[i - 1] == ' ' && textToken[i] == ' ')
            {
                continue;
            }

            if (textToken[i] == '\\')
            {
                int escapedCounter = 0;

                while (i < textToken.Length && textToken[i] == '\\')
                {
                    ++escapedCounter;
                    ++i;
                }
                
                // Если следующий символ специальный, то не добавляем символ экранирования
                
                // Так, тут по сути escapedCounter % 2 == 1 всегда будет давать ложь,
                // тк мы еще в самом первом цикле проверяли перед добавлением специального символа
                // является ли он escaped
                
                // Просто символы парами экранируют друг друга
                if (escapedCounter % 2 == 0)
                {
                    sb.Append(textToken.Substring(i - escapedCounter, escapedCounter / 2));
                }
                // Нечетное колво экранирующих символов
                else
                {
                    // Если следюущий после экранирующих специальный, то его нужно удалить, а другие парами сопоставить
                    if (IsNextSymbolIsSpecial(textToken, i - 1))
                    {
                        sb.Append(textToken.Substring(i - escapedCounter, escapedCounter / 2));
                    }
                    else
                    {
                        // В проивном случае просто пары удалим и один нечетный оставим
                        sb.Append(textToken.Substring(i - escapedCounter, escapedCounter / 2 + 1));

                    }
                }
            }

            if (i < textToken.Length)
            {
                sb.Append(textToken[i]);
            }
        }

        return sb.ToString();
    }

    public static bool IsNextSymbolIsSpecial(string sourceString, int currentIndex)
    {
        if (currentIndex < sourceString.Length - 1)
        {
            return sourceString[currentIndex + 1] == '#' || sourceString[currentIndex + 1] == '_';
        }

        if (currentIndex < sourceString.Length - 2)
        {
            return sourceString.Substring(currentIndex + 1, 2) == "__";
        }

        return false;
    }
    
    public static bool IsEscaped(string str, int index)
    {
        // Проверяем, есть ли перед текущим символом экранирующий символ
        if (index > 0 && str[index - 1] == '\\')
        {
            // Если символ экранирован, проверяем, является ли этот экранирующий символ
            // также экранированным (например, двойной слэш \\).
            int backslashCount = 0;
            index--;  // Начинаем проверять символы до текущего
            while (index >= 0 && str[index] == '\\')
            {
                backslashCount++;
                index--;
            }

            // Если количество экранирующих слэшей нечетное, значит символ экранирован
            return backslashCount % 2 == 1;
        }

        return false;
    }

    private static bool IsWithinOneWord(string src, SpecialSymbol openingTag, SpecialSymbol closingTag)
    {
        bool containsSpace = src.Substring(openingTag.Index + openingTag.TagLength,
                closingTag.Index - openingTag.Index - openingTag.TagLength)
            .Any(c => char.IsWhiteSpace(c));


        return !(containsSpace &&
               openingTag.Index - 1 >= 0 &&
               closingTag.Index + closingTag.TagLength < src.Length &&
               char.IsLetter(src[openingTag.Index - 1]) &&
               char.IsLetter(src[closingTag.Index + closingTag.TagLength]));
    }
    
    private static bool IsOpeningSymbol(SpecialSymbol symbol, List<SpecialSymbol> stack)
    {
        // Определяем, открывающий ли это символ, в зависимости от контекста
        // Например, можно проверять, что нет открытой пары для символа этого типа в стеке
        bool openedTagBefore = false;

        for (int i = stack.Count - 1; i >= 0; i--)
        {
            if (stack[i].Type == symbol.Type)
            {
                openedTagBefore = true;
            }

            break;
        }
        
        // Проверяем, что стек может быть нулевым, до этого нигде этот открывающийся стек не встречался
        // Тогда он точно открывающий
        return  !openedTagBefore;
    }

    private static List<Token> ExtractInsideTokens(int startIndex, int endIndex, List<Token> tokens)
    {
        // Извлекаем вложенные токены, которые лежат внутри текущего токена
        List<Token> resultToReturn = new List<Token>();
        // Мы получили в переменную выше токены, которые нужно засунуть
        // в InsideTokens
        // Теперь нужно их удалить из главного массива tokens
        foreach (var token in tokens.ToList())
        {
            // Сделал <=, чтоб корректно обрабатывать вложенность header'ов, которые ведут до конца строки
            // Пример "# header _ada_"
            if (token.StartIndex > startIndex && token.StartIndex <= endIndex)
            {
                resultToReturn.Add(token);
                tokens.Remove(token);
            }
        }

        return resultToReturn;
    }

    // В этом методе удалим токены, которые не могут рендериться в текущем токене
    // Пример: _text__text__text_ => <em>text__text__text</em>
    // По сути самый длинный рекусривный вызов здесь может быть длиной в 3, 
    // тк иначе даже если будет __a __текст, который может еще суб-токену содержать__ a__,
    // то такой суб-массив просто удалиться, и не будет обрабатываться
    // NOTE: По сути здесь никогда не может быть nullRefEx,
    // тк как любому токену присуждается минимум пустой список
    // САМОЕ ГЛАВНОЕ - ВЫЗЫВАТЬ ЭТОТ МЕТОД ПЕРЕД FillTokensListsWithTextTokens
    private static void RemoveInvalidTokens(Token token)
    {
        for (int i = token.InsideTokens.Count - 1; i >= 0; i--)
        {
            var insideToken = token.InsideTokens[i];
            
            if (insideToken.Type >= token.Type)
            {
                token.InsideTokens.RemoveAt(i); // Удаляем токен по индексу
            }
            else
            {
                // Рекурсивный вызов для вложенных токенов
                RemoveInvalidTokens(insideToken);
            }
        }
    }

    // В этом месте заполним пустые символы, не занятые никакими токенами токенами текста
    // Пример: __text_text___ = Bold { Text, Italics },
    // потому что без него будет: Bold { Italics }
    private static void FillTokensListsWithTextTokens(Token token)
    {
        foreach (var tokenInside in token.InsideTokens)
        {
            FillTokensListsWithTextTokens(tokenInside);
        }

        int textStartIndex = token.StartIndex + token.TagLength;
        
        for (int i = 0; i < token.InsideTokens.Count; i++)
        {
            var insideToken = token.InsideTokens[i];
        
            if (insideToken.StartIndex > textStartIndex)
            {
                token.InsideTokens.Insert(i, new Token()
                {
                    StartIndex = textStartIndex,
                    EndIndex = insideToken.StartIndex - 1,
                    Type = TokenType.Text,
                    IsPairedTag = false,
                    TagLength = 0,
                });
                ++i; // Перепрыгиваем только что созданный токен
            }

            textStartIndex = insideToken.EndIndex + 1;
        }
    
        // Обработка расстояния между последним токеном внутри и правой границей
        if (token.EndIndex - ((token.IsPairedTag) ? token.TagLength : 0) >= textStartIndex)
        {
            int endIndex = token.EndIndex - ((token.IsPairedTag) ? token.TagLength : 0);

            token.InsideTokens.Add(new Token()
            {
                StartIndex = textStartIndex + (token.Type == TokenType.Header && textStartIndex < endIndex ? 1: 0),
                EndIndex = endIndex,
                Type = TokenType.Text,
                IsPairedTag = false,
                TagLength = 0,
            });
        }
    }

    public static string Render(List<Token> tokens, string input)
    {
        var sb = new StringBuilder();
        int currentIndex = 0;

        foreach (var token in tokens)
        {
            sb.Append(RenderToken(token, input));
        }
        
        return sb.ToString();
    }

    private static string RenderToken(Token token, string input)
    {
        var sb = new StringBuilder();
     
        int length = token.EndIndex - token.StartIndex - token.TagLength * (token.IsPairedTag ? 2 : 1) + 1;
        
        string content = input.Substring(token.StartIndex + token.TagLength, length);

        switch (token.Type)
        {
            case TokenType.Header:
                sb.Append("<h1>");
                sb.Append(RenderInsideTokens(token, input)); 
                sb.Append("</h1>");
                break;
            case TokenType.Bold:
                sb.Append("<strong>");
                sb.Append(RenderInsideTokens(token, input)); 
                sb.Append("</strong>");
                break;
            case TokenType.Italics:
                sb.Append("<em>");
                sb.Append(RenderInsideTokens(token, input));
                sb.Append("</em>");
                break;
            case TokenType.Text:
                sb.Append(GetEscapedText(content));
                break;
        }

        return sb.ToString();
    }

    private static string RenderInsideTokens(Token token, string input)
    {
        var sb = new StringBuilder();

        foreach (var innerToken in token.InsideTokens)
        {
            sb.Append(RenderToken(innerToken, input));
        }

        return sb.ToString();
    }
}