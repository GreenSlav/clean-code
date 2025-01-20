import React, {useEffect, useState, useRef} from 'react';
import styled, {keyframes} from 'styled-components';
import {useNavigate} from 'react-router-dom';
import ProtectedPage from "./ProtectedPage.tsx";
import DocumentForm from "../components/DocumentForm.tsx";


const spin = keyframes`
    from {
        transform: rotate(0deg);
    }
    to {
        transform: rotate(360deg);
    }
`;

const fadeIn = keyframes`
    from {
        opacity: 0;
        transform: translate(-50%, -10px);
    }
    to {
        opacity: 1;
        transform: translate(-50%, 0px);
    }
`;

const fadeInDropdown = keyframes`
    from {
        opacity: 0;
        transform: translateY(-10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
`;

const fadeOutDropdown = keyframes`
    from {
        opacity: 1;
        transform: translateY(0);
    }
    to {
        opacity: 0;
        transform: translateY(-10px);
    }
`;

const LoadingWrapper = styled.div`
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    width: 100vw;
    background-color: #0b0d0e;
    color: #fafafa;
`;

const Spinner = styled.div`
    width: 50px;
    height: 50px;
    border: 5px solid #0d9488;
    border-top: 5px solid #14b7a6;
    border-radius: 50%;
    animation: ${spin} 1s linear infinite;
`;

const LoadingText = styled.p`
    margin-top: 1rem;
    font-size: 1.5rem;
    color: #fafafa;
`;

const MessageWrapper = styled.div`
    position: fixed;
    top: 20px;
    left: 50%;
    transform: translateX(-50%);
    background-color: #e63946;
    color: #ffffff;
    padding: 1rem 1.5rem;
    border-radius: 8px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    font-size: 1rem;
    text-align: center;
    z-index: 1000;
    animation: ${fadeIn} 0.5s ease-out;
`;

// Кнопка крестика для закрытия
const CloseButton = styled.button`
    background: none;
    border: none;
    color: white;
    font-size: 1.2rem;
    cursor: pointer;
    margin-left: 10px;
    outline: none;
    transition: color 0.3s;

    &:hover {
        color: #ffcccc;
    }
`;

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

const DropdownContent = styled.div<{ $isOpen: boolean }>`
    position: absolute;
    background-color: #1c1c1c;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    border-radius: 5px;
    min-width: 160px;
    top: 100%;
    left: 0;
    z-index: 100;
    flex-direction: column;
    overflow: hidden;
    //padding: 8px;
    opacity: ${({$isOpen}) => ($isOpen ? 1 : 0)};
    transform: ${({$isOpen}) => ($isOpen ? "translateY(0)" : "translateY(-10px)")};
    animation: ${({$isOpen}) => ($isOpen ? fadeInDropdown : fadeOutDropdown)} 0.3s ease-out;
    pointer-events: ${({$isOpen}) => ($isOpen ? "auto" : "none")}; /* Блокируем клики, когда меню скрыто */
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
        background: #181818;
        color: #5eead4;
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

// const EditorPanel = styled.div<{ width: number }>`
//     height: 100%;
//     width: ${({ width }) => width}%;
//     transition: width 0.2s ease;
// `;

const EditorPanel = styled.div.attrs<{ width: number }>(props => ({
    style: {
        width: `${props.width}%`,
    },
}))`
    height: 100%;
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
    box-sizing: border-box;
`;

const OutputPanel = styled.div`
    overflow: auto;
    background-color: #242424;
    word-wrap: break-word;
    white-space: pre-wrap;
    padding: 20px;
    height: 100%;
    box-sizing: border-box;
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

const NewDocumentEditor: React.FC = () => {
    const [markdownText, setMarkdownText] = useState('');
    const [htmlOutput, setHtmlOutput] = useState('');
    const [isDropdownOpen, setDropdownOpen] = useState(false);
    const navigate = useNavigate();
    const dropdownRef = useRef<HTMLDivElement>(null);
    const [dividerX, setDividerX] = useState(50);
    const [isDragging, setIsDragging] = useState(false);
    const backendUrl = "http://localhost:5001"; // Blazor WebAssembly сервер
    const [isLoading, setIsLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState('');
    const [isVisible, setIsVisible] = useState(false); // Управляет отображением
    const [isFormVisible, setIsFormVisible] = useState(false); // Управляем формой


    const saveNewDocument = async (title: string, isPrivate: boolean) => {
        try {
            const response = await fetch("http://localhost:5001/api/documents/create", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                body: JSON.stringify({
                    title: title,
                    content: markdownText, // Используем текст из редактора
                    isPrivate: isPrivate,
                }),
            });

            const data = await response.json();
            if (!response.ok) {
                throw new Error(data.message || "Failed to create document");
            }

            //alert("Document created successfully!");
            navigate(`/documents/${data.documentId}`);
            window.location.reload();
        } catch (error) {
            //alert("Error: " + error.message);
            console.log(error);
        }
    };

    const toggleDropdown = () => {
        if (!isDropdownOpen) {
            setIsVisible(true); // Сначала показываем
            setTimeout(() => setDropdownOpen(true), 10); // Запускаем анимацию
        } else {
            setDropdownOpen(false);
            setTimeout(() => setIsVisible(false), 300); // Ждём завершения анимации и скрываем
        }
    };

    const closeDropdown = () => {
        setDropdownOpen(false);
    };

    // Функция закрытия сообщения
    const handleCloseError = () => {
        setErrorMessage('');
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

    // Загрузка Blazor
    useEffect(() => {
        const loadBlazor = async () => {
            try {
                // Проверяем, был ли уже запущен Blazor
                if (window.Blazor && window.Blazor._internal) {
                    console.warn("Blazor WebAssembly уже загружен.");
                    setIsLoading(false);
                    return;
                }

                console.log("Загружаем Blazor WebAssembly...");
                await import(`${backendUrl}/api/blazor/resource/blazor.webassembly.js`);

                await window.Blazor.start({
                    loadBootResource: (type, name, defaultUri, integrity) => {
                        return `${backendUrl}/api/blazor/resource/${name}`;
                    }
                });

                console.log("Blazor WebAssembly успешно загружен.");
                setIsLoading(false);
            } catch (error) {
                console.error("Blazor WebAssembly load error:", error);
                setErrorMessage("Failed to load Blazor WebAssembly.");
                setIsLoading(false);
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
            setDividerX(newDividerX);
        }
    };

    const handleMouseUp = () => {
        setIsDragging(false);
        document.body.style.userSelect = 'auto';
    };


    const handleSaveMarkdown = () => {
        setIsFormVisible(true);
        closeDropdown(); // Закрываем меню после клика
// Открываем форму
    };

    const handleUploadMarkdown = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;
        const reader = new FileReader();
        reader.onload = (event) => {
            setMarkdownText(event.target?.result as string);
        };
        reader.readAsText(file);
        closeDropdown();
    };

    const goToDashboard = () => {
        navigate('/dashboard');
    };

    if (isLoading) {
        return (
            <LoadingWrapper>
                <div style={{
                    textAlign: 'center',
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'center',
                }}>
                    <Spinner/>
                    <LoadingText>Loading necessary files...</LoadingText>
                </div>
            </LoadingWrapper>
        );
    }

    return (
        <ProtectedPage>
            {/* Сообщение об ошибке */}
            {errorMessage && (
                <MessageWrapper>
                    {errorMessage}
                    <CloseButton onClick={handleCloseError}>×</CloseButton>
                </MessageWrapper>
            )}
            {isFormVisible && <DocumentForm onSubmit={saveNewDocument} onClose={() => setIsFormVisible(false)} />}
            <EditorContainer>
                <Toolbar>
                    {/* Меню "Файл" */}
                    <Dropdown ref={dropdownRef}>
                        <DropdownButton onClick={toggleDropdown}>File</DropdownButton>
                        <DropdownContent $isOpen={isDropdownOpen} style={{display: isVisible ? "flex" : "none"}}>
                            <DropdownItem onClick={handleSaveMarkdown}>Save</DropdownItem>
                            <DropdownItem>
                                Upload Markdown
                                <input type="file" accept=".md" onChange={handleUploadMarkdown}
                                       style={{display: 'none'}}/>
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
        </ProtectedPage>
    );
};

export default NewDocumentEditor;