import React, { useEffect, useState } from 'react';
import classes from './MarkdownEditor.module.css';

const MarkdownEditor = () => {
    const [markdownText, setMarkdownText] = useState('');
    const [htmlOutput, setHtmlOutput] = useState('');
    const [dividerX, setDividerX] = useState(50);
    const [isDragging, setIsDragging] = useState(false);

    useEffect(() => {
        const loadBlazor = async () => {
            try {
                await import('http://localhost:5199/api/Blazor/resource/blazor.webassembly.js');
                await window.Blazor.start({
                    loadBootResource: (type, name, defaultUri, integrity) => {
                        return `http://localhost:5199/api/Blazor/resource/${name}`;
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
        // Откчлючение выделения
        document.body.style.userSelect = 'none';
    };

    const handleMouseMove = (e) => {
        if (isDragging) {
            const newDividerX = (e.clientX / window.innerWidth) * 100;
            setDividerX(newDividerX);
        }
    };

    const handleMouseUp = () => {
        setIsDragging(false);
        // Возвращаем вожможность выделять
        document.body.style.userSelect = 'auto';
    };

    return (
        <div className={classes.editor_container} onMouseMove={handleMouseMove} onMouseUp={handleMouseUp}>
            <div className={`${classes.editor_panel}`} style={{ width: `${dividerX}%` }}>
                <div className={`${classes.inside_editor_panel} ${classes.input_panel}`}>
                    <textarea
                        value={markdownText}
                        onChange={(e) => {
                            setMarkdownText(e.target.value);
                        }}
                        placeholder="Enter your markdown"
                    />
                </div>
            </div>
            <div className={classes.divider} onMouseDown={handleMouseDown}></div>
            <div className={`${classes.editor_panel} ${classes.output_panel}`} style={{width: `${100 - dividerX}%`}}>
                <div dangerouslySetInnerHTML={{ __html: htmlOutput }} />
            </div>
        </div>
    );
};

export default MarkdownEditor;