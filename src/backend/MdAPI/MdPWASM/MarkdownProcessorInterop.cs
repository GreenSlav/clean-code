using MarkdownProcessor;
using MarkdownProcessor.Classes;
using MarkdownProcessor.Interfaces;
using MarkdownProcessor.Structs.Tags;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace MarkdownProcessorWasm
{
    public class MarkdownProcessorInterop
    {
        private MdProcessor _markdownProcessor;

        public MarkdownProcessorInterop()
        {
            // Инициализация MdProcessor
            var ital = new ItalicsTag();
            var bold = new BoldTag();
            var link = new Link();
            var header = new HeaderTag();
            var main = new MainTag();
            var list = new List<ITag> { bold, ital, link, header, main };

            _markdownProcessor = new MdProcessor(new StringParser(list), new StringMdRenderer());
        }
        
        [JSInvokable]
        public string ParseMarkdown(string markdownText)
        {
            return _markdownProcessor.ParseAndRender(markdownText);
        }
    }
}
