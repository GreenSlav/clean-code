import React, {useEffect, useRef, useState} from "react";
import styled from "styled-components";

const Overlay = styled.div`
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
`;

const FormContainer = styled.form`
    display: flex;
    flex-direction: column;
    gap: 10px;
    padding: 15px;
    background: #0d2122;
    border-radius: 10px;
    color: white;
    width: 300px;
    position: relative;
    
`;

const Label = styled.label`
    display: flex;
    flex-direction: column;
    gap: 5px;
    font-size: 1rem;
`;

const Input = styled.input`
    width: 100%;
    padding: 8px;
    border-radius: 5px;
    border: 1px solid #14b7a6;
    background: black;
    color: white;
    box-sizing: border-box;
`;

const CheckboxLabel = styled.label`
    display: flex;
    align-items: center;
    gap: 5px;
    font-size: 1rem;
    accent-color: #14b7a6;
`;

const Button = styled.button`
    padding: 10px;
    border-radius: 5px;
    border: none;
    background: #14b7a6;
    color: white;
    cursor: pointer;
    transition: background 0.3s ease;

    &:hover {
        background: #0d9488;
    }
`;

const CloseButton = styled.button`
    position: absolute;
    top: 10px;
    right: 10px;
    background: none;
    border: none;
    color: white;
    font-size: 1.2rem;
    cursor: pointer;
    transition: color 0.3s;

    &:hover {
        color: #ffcccc;
    }
`;

interface DocumentFormProps {
    onSubmit: (title: string, isPrivate: boolean) => void;
    onClose: () => void; // Закрытие формы
}

const DocumentForm: React.FC<DocumentFormProps> = ({ onSubmit, onClose }) => {
    const [title, setTitle] = useState("");
    const [isPrivate, setIsPrivate] = useState(false);
    const formRef = useRef<HTMLFormElement>(null); // Используем ref для отслеживания кликов вне формы

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit(title, isPrivate);
        onClose(); // Закрываем форму после отправки
    };

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (formRef.current && !formRef.current.contains(event.target as Node)) {
                onClose();
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [onClose]);

    return (
        <Overlay>
            <FormContainer ref={formRef} onSubmit={handleSubmit}>
                <CloseButton onClick={onClose}>×</CloseButton>
                <Label>
                    Title:
                    <Input
                        type="text"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                        required
                    />
                </Label>

                <CheckboxLabel>
                    <input
                        type="checkbox"
                        checked={isPrivate}
                        onChange={(e) => setIsPrivate(e.target.checked)}
                    />
                    Private document
                </CheckboxLabel>

                <Button type="submit">Create Document</Button>
            </FormContainer>
        </Overlay>
    );
};

export default DocumentForm;