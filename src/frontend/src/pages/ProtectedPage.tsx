import React, {useEffect, useState} from 'react';
import {useNavigate} from 'react-router-dom';
import styled, {keyframes} from 'styled-components';

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

const FullScreenWrapper = styled.div`
    width: 100vw;
    height: 100vh;
    background-color: #0b0d0e;
`

const MessageWrapper = styled.div`
    position: fixed;
    top: 45%; /* Центрирование по вертикали */
    left: 50%; /* Центрирование по горизонтали */
    background-color: #e63946; /* Красный фон */
    transform: translate(-50%, 0px);
    color: #ffffff; /* Белый текст */
    padding: 1rem 1.5rem;
    border-radius: 8px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    font-size: 1rem;
    text-align: center;
    z-index: 1000;
    display: flex; /* Для размещения текста и кнопки крестика */
    justify-content: center; /* Центрирование текста внутри */
    align-items: center;
    width: 300px; /* Фиксированная ширина */
    animation: ${fadeIn} 0.5s ease-out;
`;

const spin = keyframes`
    from {
        transform: rotate(0deg);
    }
    to {
        transform: rotate(360deg);
    }
`;

const LoadingWrapper = styled.div`
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    width: 100vw;
    background-color: #0b0d0e; /* Темный фон */
    color: #fafafa;
`;

const Spinner = styled.div`
    width: 50px;
    height: 50px;
    border: 5px solid #0d9488; /* Цвет обводки */
    border-top: 5px solid #14b7a6; /* Акцентный цвет */
    border-radius: 50%;
    animation: ${spin} 1s linear infinite; /* Анимация вращения */
`;

const LoadingText = styled.p`
    margin-top: 1rem;
    font-size: 1.5rem;
    color: #fafafa; /* Белый текст */
        //animation: ${fadeIn} 0.5s ease-out;
`;

const ProtectedPage: React.FC<{ children: React.ReactNode }> = ({children}) => {
    const API_BASE_URL = import.meta.env.VITE_REACT_APP_BACKEND_URL;
    const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
    const [showMessage, setShowMessage] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const verifyUser = async () => {
            try {
                const response = await fetch(`${API_BASE_URL}/api/auth/verify`, {
                    method: 'GET',
                    credentials: 'include', // Для отправки куки
                });

                if (response.ok) {
                    setIsAuthenticated(true);
                } else {
                    setIsAuthenticated(false);
                    setShowMessage(true);
                    setTimeout(() => navigate('/'), 2000); // Перенаправление через 2 секунды
                }
            } catch (error) {
                setIsAuthenticated(false);
                setShowMessage(true);
                setTimeout(() => navigate('/'), 2000); // Перенаправление через 2 секунды
            }
        };

        verifyUser();
    }, [navigate]);

    if (isAuthenticated === null) {
        return (
            <LoadingWrapper>
                <div style={
                    {
                        textAlign: 'center',
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        justifyContent: 'center',
                    }}>
                    <Spinner/>
                    <LoadingText>Checking authentication...</LoadingText>
                </div>
            </LoadingWrapper>
        );
    }

    return (
        <>
            {showMessage &&
                <FullScreenWrapper>
                    <MessageWrapper>You are not authorized. Redirecting...</MessageWrapper>
                </FullScreenWrapper>}
            {isAuthenticated && children}
        </>
    );
};

export default ProtectedPage;