import React, {useEffect, useState} from "react";
import styled, {css, keyframes} from "styled-components";
import ProtectedPage from "./ProtectedPage.tsx";
import {useNavigate} from "react-router-dom";

const ContainerWrapper = styled.div`
    width: 100%;
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 20px 0;
    box-sizing: border-box;
`;

const TopBar = styled.div`
    width: 80%;
    display: flex;
    justify-content: space-between;
    margin-bottom: 20px;
`;

const Button = styled.button`
    padding: 10px 15px;
    background-color: rgba(20, 183, 166, 0.2);
    color: rgba(20, 183, 166, 1);
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s;
    font-size: 16px;

    &:hover {
        color: rgb(13, 121, 110);
        background-color: #0d2122;
    }
`;

const ButtonWrapper = styled.div`
    display: flex;
    justify-content: center;
    align-content: center;
    height: 100%;
`

const DownloadButton = styled.img`
    display: inline-block;
    object-fit: contain;
    height: 20px;
`

const DeleteButton = styled.button`
    height: 100%;
    padding: 6px 12px;
    margin: 5px;
    background-color: rgba(255, 0, 0, 0.2);
    color: rgb(183, 20, 20);
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s;

    &:hover {
        background-color: rgba(255, 4, 4, 0.1);
        color: rgb(124, 12, 12);
    }
`

const LogoutButton = styled.button`
    padding: 10px 15px;
    background-color: rgba(255, 0, 0, 0.2);
    color: red;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s;
    font-size: 16px;

    &:hover {
        background-color: rgba(255, 0, 0, 0.15);
        color: #b00000;
    }
`;

const DocumentTable = styled.table`
    width: 80%;
    border-collapse: collapse;
`;

const TableHeader = styled.th`
    background-color: #0b0d0e;
    color: white;
    padding: 10px;
    border: 1px solid #333;
`;

const fadeOut = keyframes`
    from {
        opacity: 1;
        transform: scale(1);
    }
    to {
        opacity: 0;
        transform: scale(0.9);
    }
`;

const TableRow = styled.tr<{ $isRemoving?: boolean }>`
    &:nth-child(even) {
        background-color: #1c1c1c;
    }

    ${({$isRemoving}) =>
            $isRemoving &&
            css`
                animation: ${fadeOut} 0.4s ease forwards;
            `}
`;

const TableCell = styled.td`
    padding: 10px;
    border: 1px solid #333;
    text-align: center;
`;

const InsideCellWrapper = styled.div`
    width: 100%;
    height: 100%;
    display: inline-block;
`

const ActionButton = styled.button`
    height: 100%;
    padding: 6px 12px;
    margin: 5px;
    background-color: rgba(20, 183, 166, 0.2);
    color: rgba(20, 183, 166, 1);
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s;

    &:hover {
        background-color: rgba(20, 183, 166, 0.1);
        color: rgb(14, 129, 117);
    }
`;

const LoadingText = styled.p`
    margin-top: 1rem;
    font-size: 1.5rem;
    color: #fafafa;
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

const fadeInModal = keyframes`
    from {
        opacity: 0;
        transform: scale(0.95);
    }
    to {
        opacity: 1;
        transform: scale(1);
    }
`;

const fadeOutModal = keyframes`
    from {
        opacity: 1;
        transform: scale(1);
    }
    to {
        opacity: 0;
        transform: scale(0.95);
    }
`;

const ModalOverlay = styled.div<{ $isVisible: boolean, $isClosing: boolean }>`
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background-color: rgba(0, 0, 0, 0.6);
    display: ${({ $isVisible }) => ($isVisible ? "flex" : "none")};
    align-items: center;
    justify-content: center;
    z-index: 1000;
    animation: ${({ $isClosing }) => ($isClosing ? fadeOutModal : fadeInModal)} 0.3s ease-out;
`;

const ModalContent = styled.div`
    position: relative; /* Добавляем! */
    background: #1c1c1c;
    padding: 20px;
    border-radius: 10px;
    text-align: center;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    width: 320px;
    animation: ${fadeInModal} 0.3s ease-out;
`;

const ModalTitle = styled.h2`
    color: white;
    font-size: 18px;
`;

const ModalButtons = styled.div`
    display: flex;
    justify-content: space-around;
    margin-top: 15px;
`;

const ModalButton = styled.button`
    padding: 8px 15px;
    font-size: 14px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;

    &:first-child {
        background: red;
        color: white;
    }

    &:last-child {
        background: #14b7a6;
        color: black;
    }

    &:hover {
        opacity: 0.8;
    }
`;

const CloseButton = styled.button`
    background: none;
    border: none;
    color: white;
    font-size: 18px;
    cursor: pointer;
    position: absolute;
    top: 10px;
    right: 10px;
`;

interface DocumentType {
    id: string;
    title: string;
    lastEdited: string;
    role: string;
}

const DashboardPage: React.FC = () => {
    const navigate = useNavigate();
    const [documents, setDocuments] = useState<DocumentType[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState("");
    const [removingDocument, setRemovingDocument] = useState<string | null>(null);
    const [isConfirmModalOpen, setIsConfirmModalOpen] = useState(false);
    const [isClosing, setIsClosing] = useState(false);
    const [selectedDocId, setSelectedDocId] = useState<string | null>(null);

    useEffect(() => {
        fetch("http://localhost:5001/api/documents", {
            method: "GET",
            credentials: "include", // Отправка куки для аутентификации
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Failed to fetch documents");
                }
                return response.json();
            })
            .then(data => {
                // Убираем дубликаты перед сохранением
                const uniqueDocs = Array.from(new Set(data.map(doc => doc.id)))
                    .map(id => data.find(doc => doc.id === id));
                setDocuments(uniqueDocs);
                setIsLoading(false);
            })
            .catch(error => {
                console.error("Error fetching documents:", error);
                setErrorMessage("Failed to load documents.");
                setIsLoading(false);
            });
    }, []);

    const handleLogout = async () => {
        try {
            const response = await fetch("http://localhost:5001/api/auth/logout", {
                method: "POST",
                credentials: "include", // Для отправки куки
            });

            if (response.ok) {
                navigate("/"); // Перенаправление на главную страницу
            }
        } catch (error) {
            console.error("Logout error:", error);
        }
    };

    const downloadDocument = async (documentId) => {
        try {
            const response = await fetch(`http://localhost:5001/api/documents/${documentId}/download`, {
                method: "GET",
                credentials: "include",
            });

            if (!response.ok) {
                throw new Error("Failed to download document");
            }

            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = "document.md";
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        } catch (error) {
            setErrorMessage("Error downloading document: " + error.message)
            setTimeout(() => setErrorMessage(""), 2000);
        }
    };

    const deleteDocument = async (documentId: string) => {
        if (!window.confirm("Are you sure you want to delete this document?")) return;

        setRemovingDocument(documentId); // 🔴 Запускаем анимацию

        setTimeout(async () => {
            try {
                const response = await fetch(`http://localhost:5001/api/documents/${documentId}`, {
                    method: "DELETE",
                    credentials: "include",
                });

                if (!response.ok) {
                    throw new Error("Failed to delete document");
                }

                setDocuments(prevDocs => {
                    const updatedDocs = prevDocs.filter(doc => doc.id !== documentId);
                    console.log("Updated Documents List:", updatedDocs);
                    return updatedDocs;
                });
                setRemovingDocument(null); // Сбрасываем состояние
            } catch (error) {
                //alert("Error deleting document: " + error.message);
                setRemovingDocument(null); // В случае ошибки возвращаем всё обратно
            }
        }, 400); // ⏳ Ждём 400 мс перед удалением
    };

    const openConfirmModal = (documentId: string) => {
        setSelectedDocId(documentId);
        setIsConfirmModalOpen(true);
        setIsClosing(false);
    };

    const closeConfirmModal = () => {
        setIsClosing(true);
        setTimeout(() => {
            setSelectedDocId(null);
            setIsConfirmModalOpen(false);
        }, 300); // ⏳ Ждем завершения анимации перед скрытием
    };

    const confirmDelete = async () => {
        if (!selectedDocId) return;
        setRemovingDocument(selectedDocId);

        setTimeout(async () => {
            try {
                const response = await fetch(`http://localhost:5001/api/documents/${selectedDocId}`, {
                    method: "DELETE",
                    credentials: "include",
                });

                if (!response.ok) {
                    throw new Error("Failed to delete document");
                }

                setDocuments(prevDocs => prevDocs.filter(doc => doc.id !== selectedDocId));
                setRemovingDocument(null);
                closeConfirmModal();
            } catch (error) {
                //("Error deleting document: " + error.message);
                setRemovingDocument(null);
                closeConfirmModal();
            }
        }, 400);
    };


    return (
        <ProtectedPage>
            {isLoading ? (
                <LoadingText>Loading documents...</LoadingText>
            ) : errorMessage ? (
                <MessageWrapper>{errorMessage}</MessageWrapper>
            ) : (
                <ContainerWrapper>
                    {/* Добавил верхний бар с кнопками */}
                    <TopBar>
                        <Button onClick={() => navigate("/documents/new")}>New Document</Button>
                        <LogoutButton onClick={handleLogout}>Logout</LogoutButton>
                    </TopBar>

                    <DocumentTable>
                        <thead>
                        <tr>
                            <TableHeader>Title</TableHeader>
                            <TableHeader>Created At</TableHeader>
                            <TableHeader>Role</TableHeader>
                            <TableHeader>Actions</TableHeader>
                        </tr>
                        </thead>
                        <tbody>
                        {documents.map((doc) => (
                            <TableRow key={doc.id} $isRemoving={removingDocument === doc.id}>
                                <TableCell>{doc.title}</TableCell>
                                <TableCell>{doc.lastEdited}</TableCell>
                                <TableCell>{doc.role}</TableCell>
                                <TableCell>
                                    <ActionButton onClick={() => {
                                        navigate(`/documents/${doc.id}`)
                                        window.location.reload();
                                    }}>
                                        Open
                                    </ActionButton>
                                    <ActionButton onClick={() => downloadDocument(doc.id)}>Download</ActionButton>
                                    {doc.role === "owner" && (
                                        <DeleteButton onClick={() => openConfirmModal(doc.id)}>Delete</DeleteButton>
                                    )}
                                </TableCell>
                            </TableRow>
                        ))}
                        </tbody>
                    </DocumentTable>
                </ContainerWrapper>
            )}
            <ModalOverlay $isVisible={isConfirmModalOpen} $isClosing={isClosing} onClick={closeConfirmModal}>
                <ModalContent onClick={(e) => e.stopPropagation()}>
                    <CloseButton onClick={closeConfirmModal}>×</CloseButton>
                    <ModalTitle>Are you sure you want to delete this document?</ModalTitle>
                    <ModalButtons>
                        <ModalButton onClick={confirmDelete}>Delete</ModalButton>
                        <ModalButton onClick={closeConfirmModal}>Cancel</ModalButton>
                    </ModalButtons>
                </ModalContent>
            </ModalOverlay>
        </ProtectedPage>
    );
};

export default DashboardPage;