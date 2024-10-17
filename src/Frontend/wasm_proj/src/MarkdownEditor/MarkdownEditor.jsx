import { useEffect, useState } from 'react';

const MarkdownEditor = () => {
    const [markdownText, setMarkdownText] = useState('');
    const [htmlOutput, setHtmlOutput] = useState('');
    const [wasmInstance, setWasmInstance] = useState(null);  // Сохраняем инстанс WebAssembly

    useEffect(() => {
        const loadWasm = async () => {
            // Загрузка WebAssembly только один раз при первом рендере
            const response = await fetch('path/to/your/markdownprocessor.wasm');
            const bytes = await response.arrayBuffer();
            const module = await WebAssembly.instantiate(bytes);
            setWasmInstance(module.instance.exports);  // Сохраняем инстанс для переиспользования
        };

        loadWasm();
    }, []);

    useEffect(() => {
        if (wasmInstance) {
            // Вызываем функцию парсинга, если WebAssembly модуль загружен
            const parsedHtml = wasmInstance.ParseMarkdown(markdownText);
            setHtmlOutput(parsedHtml);
        }
    }, [markdownText, wasmInstance]);  // Обновляем только если изменился текст или был загружен wasm

    return (
        <div>
            <textarea
                value={markdownText}
                onChange={(e) => setMarkdownText(e.target.value)}
                placeholder="Введите ваш Markdown"
            />
            <div dangerouslySetInnerHTML={{ __html: htmlOutput }} />
        </div>
    );
};

export default MarkdownEditor;