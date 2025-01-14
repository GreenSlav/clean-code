import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import styled, { keyframes } from 'styled-components';

const fadeIn = keyframes`
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
`;

const MessageWrapper = styled.div`
    position: fixed;
    top: 20px;
    left: 50%;
    transform: translateX(-50%);
    background-color: #ff4c4c;
    color: white;
    padding: 1rem 2rem;
    border-radius: 8px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    animation: ${fadeIn} 0.5s ease-out;
    font-size: 1rem;
    text-align: center;
`;

const ProtectedPage: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
    const [showMessage, setShowMessage] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const verifyUser = async () => {
            try {
                const response = await fetch('http://localhost:5001/api/auth/verify', {
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
        // Показываем индикатор загрузки, пока идет проверка
        return <div>Loading...</div>;
    }

    return (
        <>
            {showMessage && <MessageWrapper>You are not authorized. Redirecting...</MessageWrapper>}
            {isAuthenticated && children}
        </>
    );
};

export default ProtectedPage;