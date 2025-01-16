import React, { useEffect, useState } from 'react';
import styled from 'styled-components';

const EditorContainer = styled.div`
    display: flex;
    height: 95vh;
    width: 95vw;
    border: 2px solid #152bee;
    box-shadow: 1px 1px 15px #152bee;
    overflow: hidden;
    border-radius: 10px;
    margin: auto; /* Центрируем страницу */
`;

const EditorPanel = styled.div<{ width: number }>`
    height: 100%;
    width: ${({ width }) => width}%;
    transition: width 0.2s ease;
`;

const InsideEditorPanel = styled.div`
    height: 100%;
    width: 100%;
    overflow: auto;
    overflow-x: hidden;
    overflow-y: hidden;
    padding: 20px;
`;

const InputPanel = styled.textarea`
    width: 100%;
    height: 100%;
    letter-spacing: 1px;
    border: none;
    outline: none;
    background: none;
    color: inherit;
    font: inherit;
    resize: none;
    overflow: auto;
    padding: 15px;

    /* Кастомная прокрутка */
    &::-webkit-scrollbar {
        width: 8px;
    }

    &::-webkit-scrollbar-track {
        background: none;
    }

    &::-webkit-scrollbar-thumb {
        background-color: #ccc;
        border-radius: 10px;
    }
`;

const OutputPanel = styled.div`
    overflow: auto;
    background-color: #242424;
    word-wrap: break-word;
    overflow-wrap: break-word;
    white-space: pre-wrap;
    padding: 20px;
    height: 100%;

    /* Кастомная прокрутка */
    &::-webkit-scrollbar {
        width: 8px;
    }

    &::-webkit-scrollbar-track {
        background: none;
    }

    &::-webkit-scrollbar-thumb {
        background-color: #ccc;
        border-radius: 10px;
    }
`;

const Divider = styled.div`
    width: 5px;
    cursor: col-resize;
    background-color: #ccc;
    height: 100%;
    transition: background-color 0.2s;

    &:hover {
        background-color: #14b7a6;
    }
`;

const MarkdownEditor: React.FC = () => {
    const [markdownText, setMarkdownText] = useState('');
    const [htmlOutput, setHtmlOutput] = useState('');
    const [dividerX, setDividerX] = useState(50);
    const [isDragging, setIsDragging] = useState(false);
    const backendUrl = "http://localhost:5001"; // Сервер Blazor

    useEffect(() => {
        const loadBlazor = async () => {
            try {
                console.log(backendUrl);
                await import(`${backendUrl}/api/Blazor/resource/blazor.webassembly.js`);
                await window.Blazor.start({
                    loadBootResource: (type, name, defaultUri, integrity) => {
                        return `${backendUrl}/api/Blazor/resource/${name}`;
                    }
                }).then(() => {
                    console.log('Blazor WebAssembly initialized');
                }).catch(error => {
                    console.error('Blazor WebAssembly initialization error:', error);
                });
            } catch (error) {
                console.error('Blazor WebAssembly load error:', error);
            }
        };

        loadBlazor();
    }, []);

    const processMarkdown = async () => {
        try {
            if (window.processor) {
                const parsedHtml = await window.blazorInterop.parseMarkdown(markdownText);
                setHtmlOutput(parsedHtml);
            } else {
                console.error('Blazor WebAssembly not loaded');
            }
        } catch (error) {
            console.error('Markdown parsing error:', error);
        }
    };

    useEffect(() => {
        processMarkdown();
    }, [markdownText]);

    const handleMouseDown = () => {
        setIsDragging(true);
        document.body.style.userSelect = 'none';
    };

    const handleMouseMove = (e: React.MouseEvent) => {
        if (isDragging) {
            const newDividerX = (e.clientX / window.innerWidth) * 100;
            setDividerX(Math.min(80, Math.max(20, newDividerX))); // Ограничиваем минимальный и максимальный размер
        }
    };

    const handleMouseUp = () => {
        setIsDragging(false);
        document.body.style.userSelect = 'auto';
    };

    return (
        <EditorContainer onMouseMove={handleMouseMove} onMouseUp={handleMouseUp}>
            <EditorPanel width={dividerX}>
                <InsideEditorPanel>
                    <InputPanel
                        value={markdownText}
                        onChange={(e) => setMarkdownText(e.target.value)}
                        placeholder="Enter your markdown..."
                    />
                </InsideEditorPanel>
            </EditorPanel>
            <Divider onMouseDown={handleMouseDown} />
            <EditorPanel width={100 - dividerX}>
                <OutputPanel dangerouslySetInnerHTML={{ __html: htmlOutput }} />
            </EditorPanel>
        </EditorContainer>
    );
};

export default MarkdownEditor;