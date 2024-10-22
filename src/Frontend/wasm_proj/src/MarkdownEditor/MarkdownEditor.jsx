
import classes from "./MarkdownEditor.module.css"
import { useEffect, useState } from 'react';

const MarkdownEditor = () => {
    const [markdownText, setMarkdownText] = useState('');
    const [htmlOutput, setHtmlOutput] = useState('');

    useEffect(() => {
        const loadBlazor = async () => {
            try {
                await import('http://localhost:8080/_framework/blazor.webassembly.js');

                // Настройка загрузки ресурсов Blazor
                await window.Blazor.start({
                    loadBootResource: (type, name, defaultUri, integrity) => {
                        return `http://localhost:8080/_framework/${name}`;
                    }
                }).then(() => {
                    console.log('Blazor WebAssembly инициализирован');
                }).catch(error => {
                    console.error('Ошибка инициализации Blazor WebAssembly:', error);
                });
            } catch (error) {
                console.error('Ошибка при загрузке Blazor WebAssembly:', error);
            }
        };

        loadBlazor();
    }, []);

    // Используем useEffect для вызова processMarkdown после обновления markdownText
    useEffect(() => {
        processMarkdown();
    }, [markdownText]);  // Этот useEffect сработает каждый раз, когда markdownText изменится

    const processMarkdown = async () => {
        try {
            if (window.processor) {
                const parsedHtml = await window.blazorInterop.parseMarkdown(markdownText);
                setHtmlOutput(parsedHtml);
            } else {
                console.error('Blazor WebAssembly не загружен');
            }
        } catch (error) {
            console.error('Ошибка при парсинге Markdown:', error);
        }
    };

    return (
        <div>
            <div dangerouslySetInnerHTML={{__html: htmlOutput}}/>
            <textarea
                className={classes.input}
                value={markdownText}
                onChange={(e) => setMarkdownText(e.target.value)}
                placeholder="Введите ваш Markdown"
            />
        </div>
    );
};

export default MarkdownEditor;