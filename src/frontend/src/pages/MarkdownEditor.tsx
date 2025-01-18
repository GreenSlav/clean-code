import React, {useEffect, useState, useRef} from 'react';
import styled from 'styled-components';
import {useNavigate} from 'react-router-dom';

const EditorContainer = styled.div`
    display: flex;
    flex-direction: column;
    height: 100vh;
    box-sizing: border-box;
    overflow: hidden;
`;

const Toolbar = styled.div`
    width: 100%;
    display: flex;
    padding: 10px;
    background: #0b0d0e;
    box-sizing: border-box;
    justify-content: space-between;
    align-items: center;
`;

const Dropdown = styled.div`
    position: relative;
    display: inline-block;
`;

const DropdownButton = styled.button`
    background: transparent;
    color: white;
    border: none;
    position: relative;
    padding: 8px 12px;
    font-size: 1rem;
    border-radius: 5px;
    cursor: pointer;
    transition: color 0.3s ease;

    &::after {
        content: "";
        position: absolute;
        left: 0;
        bottom: 0;
        width: 0%;
        height: 2px;
        background: #14b7a6;
        transition: width 0.3s ease-in-out;
    }

    &:hover {
        color: #14b7a6;
    }

    &:hover::after {
        width: 100%;
    }
`;

const DropdownContent = styled.div`
    display: none;
    position: absolute;
    background-color: #1c1c1c;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    border-radius: 5px;
    min-width: 160px;
    top: 100%;
    left: 0;
    z-index: 100;
    flex-direction: column;
    padding: 8px;

    &.open {
        display: flex;
    }
`;

const DropdownItem = styled.button`
    background: none;
    border: none;
    color: #919191;
    text-align: left;
    padding: 8px;
    width: 100%;
    cursor: pointer;
    transition: background 0.2s;

    &:hover {
        background: #0b0d0e;
        color: #5eead4;
        border-radius: 5px;
    }
`;

const DashboardButton = styled(DropdownButton)`
    color: white;

    &:hover {
        color: #14b7a6;
    }
`;

const EditorWrapper = styled.div`
    display: flex;
    flex: 1;
    overflow: hidden;
`;

const EditorPanel = styled.div<{ width: number }>`
    height: 100%;
    width: ${({ width }) => width}%;
    transition: width 0.2s ease;
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
`;

const OutputPanel = styled.div`
    overflow: auto;
    background-color: #242424;
    word-wrap: break-word;
    white-space: pre-wrap;
    padding: 20px;
    height: 100%;
`;

const Divider = styled.div`
    width: 5px;
    cursor: col-resize;
    background-color: #ccc;
    height: 100%;

    &:hover {
        background-color: #14b7a6;
    }
`;

const MarkdownEditor: React.FC = () => {
    const [markdownText, setMarkdownText] = useState('');
    const [htmlOutput, setHtmlOutput] = useState('');
    const [isDropdownOpen, setDropdownOpen] = useState(false);
    const navigate = useNavigate();
    const dropdownRef = useRef<HTMLDivElement>(null);
    const [dividerX, setDividerX] = useState(50);
    const [isDragging, setIsDragging] = useState(false);
    const backendUrl = "http://localhost:5001"; // Blazor WebAssembly сервер

    const toggleDropdown = () => {
        setDropdownOpen((prev) => !prev);
    };

    const closeDropdown = () => {
        setDropdownOpen(false);
    };

    // Закрытие меню при клике вне него
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                closeDropdown();
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    useEffect(() => {
        const loadBlazor = async () => {
            try {
                await import(`${backendUrl}/api/blazor/resource/blazor.webassembly.js`);
                await window.Blazor.start({
                    loadBootResource: (type, name, defaultUri, integrity) => {
                        return `${backendUrl}/api/Blazor/resource/${name}`;
                    }
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
            setDividerX(Math.min(80, Math.max(20, newDividerX)));
        }
    };

    const handleMouseUp = () => {
        setIsDragging(false);
        document.body.style.userSelect = 'auto';
    };

    const insertText = (text: string) => {
        const selectionStart = (document.activeElement as HTMLTextAreaElement).selectionStart;
        const selectionEnd = (document.activeElement as HTMLTextAreaElement).selectionEnd;
        setMarkdownText((prev) => prev.substring(0, selectionStart) + text + prev.substring(selectionEnd));
    };

    const handleSaveMarkdown = () => {
        // const blob = new Blob([markdownText], { type: 'text/markdown' });
        // const a = document.createElement('a');
        // a.href = URL.createObjectURL(blob);
        // a.download = 'document.md';
        // document.body.appendChild(a);
        // a.click();
        // document.body.removeChild(a);
        closeDropdown(); // Закрываем меню после клика
    };

    const handleExportHTML = () => {
        // const blob = new Blob([htmlOutput], { type: 'text/html' });
        // const a = document.createElement('a');
        // a.href = URL.createObjectURL(blob);
        // a.download = 'document.html';
        // document.body.appendChild(a);
        // a.click();
        // document.body.removeChild(a);
        closeDropdown(); // Закрываем меню после клика
    };

    const handleUploadMarkdown = (e: React.ChangeEvent<HTMLInputElement>) => {
        // const file = e.target.files?.[0];
        // if (!file) return;
        // const reader = new FileReader();
        // reader.onload = (event) => {
        //     setMarkdownText(event.target?.result as string);
        // };
        // reader.readAsText(file);
        closeDropdown();
    };

    const goToDashboard = () => {
        navigate('/dashboard');
    };

    return (
        <EditorContainer>
            <Toolbar>
                {/* Меню "Файл" */}
                <Dropdown ref={dropdownRef}>
                    <DropdownButton onClick={toggleDropdown}>File</DropdownButton>
                    <DropdownContent className={isDropdownOpen ? 'open' : ''}>
                        <DropdownItem onClick={handleSaveMarkdown}>Save</DropdownItem>
                        <DropdownItem onClick={handleExportHTML}>Export to HTML</DropdownItem>
                        <DropdownItem>
                            Upload Markdown
                            <input type="file" accept=".md" onChange={handleUploadMarkdown} style={{display: 'none'}}/>
                        </DropdownItem>
                        <DropdownItem onClick={closeDropdown}>Access settings</DropdownItem>
                    </DropdownContent>
                </Dropdown>

                {/* Кнопка возврата в Dashboard */}
                <DashboardButton onClick={goToDashboard}>Dashboard</DashboardButton>
            </Toolbar>
            <EditorWrapper onMouseMove={handleMouseMove} onMouseUp={handleMouseUp}>
                <EditorPanel width={dividerX}>
                    <InputPanel value={markdownText} onChange={(e) => setMarkdownText(e.target.value)}
                                placeholder="Enter Markdown..."/>
                </EditorPanel>
                <Divider onMouseDown={handleMouseDown}/>
                <EditorPanel width={100 - dividerX}>
                    <OutputPanel dangerouslySetInnerHTML={{__html: htmlOutput}}/>
                </EditorPanel>
            </EditorWrapper>
        </EditorContainer>
    );
};

export default MarkdownEditor;