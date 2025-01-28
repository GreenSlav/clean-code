import React, {useState, useEffect} from "react";
import styled, {keyframes} from "styled-components";

const fadeIn = keyframes`
    from {
        opacity: 0;
        transform: scale(0.9);
    }
    to {
        opacity: 1;
        transform: scale(1);
    }
`;

const Overlay = styled.div`
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.6);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
`;

const ModalContent = styled.div`
    background: #1c1c1c;
    padding: 20px;
    border-radius: 10px;
    width: 400px;
    animation: ${fadeIn} 0.3s ease-in-out;
    position: relative;
`;

const CloseButton = styled.button`
    position: absolute;
    top: 10px;
    right: 10px;
    background: none;
    border: none;
    color: white;
    font-size: 18px;
    cursor: pointer;
`;

const Title = styled.h2`
    text-align: center;
    color: #14b7a6;
    margin-bottom: 15px;
`;

const Label = styled.label`
    display: block;
    color: #ffffff;
`;

const Input = styled.input`
    width: 100%;
    padding: 8px;
    margin-bottom: 10px;
    border: 1px solid #14b7a6;
    border-radius: 5px;
    background: #0d2122;
    color: white;
    box-sizing: border-box;
`;

const CheckboxWrapper = styled.div`
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 15px;
`;

const CollaboratorsList = styled.div`
    max-height: 200px;
    overflow-y: auto;
    background-color: #131313;
    border-radius: 5px;
    padding: 10px;
    display: flex;
    gap: 10px;
    flex-direction: column;
    justify-content: space-between;
`;

const CollaboratorRow = styled.div`
    display: flex;
    justify-content: space-between;
    align-items: center;
    background-color: rgba(29, 204, 190, 0.1);
    padding: 5px 10px;
    border-radius: 5px;
`;

const Select = styled.select`
    background: #0d2122;
    color: white;
    border: 1px solid #14b7a6;
    border-radius: 5px;
    padding: 5px;
`;

const DeleteButton = styled.button`
    background: none;
    border: none;
    color: #ffffff;
    font-size: 34px;
    cursor: pointer;
    margin: 0;
    padding: 0;

    &:hover {
        color: #dedede;
    }
`;

const Divider = styled.div`
    margin: 25px 0;
    height: 3px;
    width: 100%;
    background-color: rgba(20, 183, 166, 0.5);
    border-radius: 5px;
`

const AddButton = styled.button`
    width: 100%;
    padding: 10px;
    background: rgb(8, 70, 63);
    border: none;
    color: white;
    cursor: pointer;
    margin-top: 5px;
    border-radius: 5px;
    transition: background 0.3s;

    &:hover {
        background: #0b7067;
    }
`;

const ApplyButton = styled.button`
    width: 100%;
    padding: 10px;
    background: #14b7a6;
    border: none;
    color: white;
    cursor: pointer;
    border-radius: 5px;
    transition: background 0.3s;

    &:hover {
        background: #0d9488;
    }
`;

const AddCollaboratorButton = styled.button`
    width: 100%;
    padding: 8px;
    background: rgba(20, 183, 166, 0.2);
    color: #14b7a6;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    margin-top: 30px;
    transition: background 0.3s;

    &:hover {
        background: rgba(20, 183, 166, 0.4);
    }
`;

const AddCollaboratorForm = styled.div`
    display: flex;
    flex-direction: column;
    gap: 10px;
    margin-top: 10px;
    animation: ${fadeIn} 0.3s ease-in-out;
`;

const LoadingWrapper = styled.div`
    display: flex;
    justify-content: center;
    align-items: center;
    height: 150px;
`;

interface Collaborator {
    id: string;
    username: string;
    role: "viewer" | "editor" | "owner";
}

interface AccessSettingsProps {
    documentId: string;
    onClose: () => void;
}

const ROLE_PRIORITY = {
    "owner": 3,
    "editor": 2,
    "viewer": 1,
    "none": 0
};

const AccessSettingsModal: React.FC<AccessSettingsProps> = ({documentId, onClose}) => {
    const API_BASE_URL = import.meta.env.VITE_REACT_APP_BACKEND_URL;
    const [title, setTitle] = useState("");
    const [isPrivate, setIsPrivate] = useState(false);
    const [collaborators, setCollaborators] = useState<Collaborator[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [newUsername, setNewUsername] = useState("");
    const [newRole, setNewRole] = useState<Collaborator["role"]>("viewer");
    const [showAddForm, setShowAddForm] = useState(false);
    const [requesterRole, setRequesterRole] = useState<"viewer" | "editor" | "owner">("viewer");

    useEffect(() => {
        fetch(`http://${API_BASE_URL}/api/documents/${documentId}/settings`, {
            method: "GET",
            credentials: "include",
        })
            .then((res) => res.json())
            .then((data) => {
                setTitle(data.title);
                setIsPrivate(data.isPrivate);
                setCollaborators(data.collaborators.filter((c: Collaborator) => c.username !== "currentUser")); // 🔥 Фильтруем себя
                setRequesterRole(data.requesterRole);
                setIsLoading(false);
            })
            .catch((err) => {
                console.error("Error fetching settings:", err);
                setIsLoading(false);
            });
    }, [documentId]);

    const handleRoleChange = (userId: string, newRole: string) => {
        setCollaborators((prev) =>
            prev.map((collab) =>
                collab.id === userId ? {...collab, role: newRole as Collaborator["role"]} : collab
            )
        );
    };

    const addCollaborator = () => {
        if (!newUsername.trim()) return;

        fetch(`http://${API_BASE_URL}/api/documents/${documentId}/collaborators`, {
            method: "POST",
            credentials: "include",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({username: newUsername, role: newRole}),
        })
            .then((res) => {
                if (!res.ok) throw new Error("Failed to add collaborator");
                return res.json();
            })
            .then((newCollab) => {
                setCollaborators([...collaborators, newCollab]);
                setNewUsername("");
                setShowAddForm(false);
            })
            .catch((err) => console.error("Error adding collaborator:", err));
    };

    const removeCollaborator = (userId: string, userRole: string) => {
        if (ROLE_PRIORITY[userRole as keyof typeof ROLE_PRIORITY] >= ROLE_PRIORITY[requesterRole]) return; // 🔥 Проверка прав

        fetch(`http://${API_BASE_URL}/api/documents/${documentId}/collaborators/${userId}`, {
            method: "DELETE",
            credentials: "include",
        })
            .then((res) => {
                if (!res.ok) throw new Error("Failed to remove collaborator");
                setCollaborators(collaborators.filter((collab) => collab.id !== userId));
            })
            .catch((err) => console.error("Error removing collaborator:", err));
    };

    const applySettings = () => {
        fetch(`http://${API_BASE_URL}/api/documents/${documentId}/settings`, {
            method: "PUT",
            credentials: "include",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({title, isPrivate, collaborators}),
        })
            .then((res) => {
                if (!res.ok) throw new Error("Failed to update settings");
                onClose();
            })
            .catch((err) => console.error("Error updating settings:", err));
    };

    return (
        <Overlay onClick={onClose}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
                <CloseButton onClick={onClose}>×</CloseButton>
                <Title>Access Settings</Title>

                {isLoading ? (
                    <LoadingWrapper>Loading...</LoadingWrapper>
                ) : (
                    <>
                        <Label>Title:</Label>
                        <Input value={title} onChange={(e) => setTitle(e.target.value)}/>

                        <CheckboxWrapper>
                            <Label>
                                <input type="checkbox"
                                       style={
                                           {
                                               marginRight: "7px",
                                               accentColor: "#14b7a6"
                                           }
                                       }
                                       checked={isPrivate}
                                       onChange={(e) => setIsPrivate(e.target.checked)}/>
                                Private</Label>
                        </CheckboxWrapper>

                        {collaborators.length > 0 &&
                            <CollaboratorsList>
                                {collaborators.map((collab) => (
                                    <CollaboratorRow key={collab.id}>
                                        <span>{collab.username}</span>
                                        <Select
                                            value={collab.role}
                                            disabled={ROLE_PRIORITY[collab.role] >= ROLE_PRIORITY[requesterRole]}
                                            onChange={(e) => handleRoleChange(collab.id, e.target.value)}
                                        >
                                            <option value="viewer">Viewer</option>
                                            {ROLE_PRIORITY[requesterRole] >= ROLE_PRIORITY["editor"] &&
                                                <option value="editor">Editor</option>}
                                            {ROLE_PRIORITY[collab.role] === ROLE_PRIORITY["owner"] &&
                                                <option value="owner">Owner</option>}
                                        </Select>
                                        {
                                            ROLE_PRIORITY[requesterRole] === ROLE_PRIORITY["owner"] &&
                                            <DeleteButton
                                                onClick={() => removeCollaborator(collab.id, collab.role)}
                                            >
                                                ×
                                            </DeleteButton>
                                        }
                                    </CollaboratorRow>
                                ))}
                            </CollaboratorsList>
                        }

                        <AddCollaboratorButton onClick={() => setShowAddForm(!showAddForm)}>+ Add
                            Collaborator</AddCollaboratorButton>
                        {showAddForm && (
                            <AddCollaboratorForm>
                                <Input placeholder="Username" value={newUsername}
                                       onChange={(e) => setNewUsername(e.target.value)}/>
                                <Select value={newRole}
                                        onChange={(e) => setNewRole(e.target.value as Collaborator["role"])}>
                                    <option value="viewer">Viewer</option>
                                    <option value="editor">Editor</option>
                                </Select>
                                <AddButton onClick={addCollaborator}>Add</AddButton>
                            </AddCollaboratorForm>
                        )}
                        <Divider/>
                        <ApplyButton onClick={applySettings}>Apply</ApplyButton>
                    </>
                )}
            </ModalContent>
        </Overlay>
    );
};

export default AccessSettingsModal;