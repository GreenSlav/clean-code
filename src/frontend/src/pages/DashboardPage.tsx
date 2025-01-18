import React from 'react';
import styled from 'styled-components';
import ProtectedPage from "./ProtectedPage.tsx";
import {useNavigate} from "react-router-dom";

const Wrapper = styled.div`
    display: flex;
    width: 100vw;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100vh;
    background-color: #0d2122; /* Темный фон */
    color: #fafafa; /* Светлый текст */
`;

const Block = styled.div`
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    width: 60%;
    padding: 20px;
    background-color: #0b0d0e; /* Внутренний блок */
    border-radius: 15px;
    box-shadow: 0px 10px 30px rgba(0, 0, 0, 0.5);
    text-align: center;
`;

const Title = styled.h1`
    font-size: 2.5rem;
    font-weight: bold;
    color: #14b7a6; /* Акцентный цвет */
    margin-bottom: 1rem;
`;

const Subtitle = styled.p`
    font-size: 1.25rem;
    color: #a1a1aa; /* Вторичный текст */
    margin-bottom: 1.5rem;
`;

const InfoSection = styled.div`
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    margin-top: 1rem;
`;

const InfoBox = styled.div`
    background-color: #0d9488; /* Акцентный цвет */
    color: #ffffff; /* Текст на акцентном фоне */
    padding: 1rem 1.5rem;
    margin: 0.5rem 0;
    width: 80%;
    border-radius: 10px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    text-align: left;
    font-size: 1rem;
`;

const Button = styled.button`
    padding: 0.75rem 1.5rem;
    font-size: 1.2rem;
    border: none;
    border-radius: 8px;
    background-color: #14b7a6; /* Акцентный цвет */
    color: #0d2122; /* Текст кнопки */
    cursor: pointer;
    margin-top: 1rem;
    transition: background-color 0.4s ease, box-shadow 0.4s ease, color 0.4s ease;

    &:hover {
        color: white;
        background-color: #0d2122; /* Более насыщенный акцентный цвет */
        box-shadow: 0 0 20px #14b7a6;
    }

    &:active {
        color: white;
        background-color: #031010; /* Темный акцент */
        box-shadow: 0 0 5px #14b7a6;
    }
`;

const DashboardPage: React.FC = () => {
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            const response = await fetch('http://localhost:5001/api/auth/logout', {
                method: 'POST',
                credentials: 'include', // Для отправки куки
            });

            if (response.ok) {
                // alert('Logout successful!');
                navigate('/'); // Перенаправление на главную страницу
            } else {
                // alert('Failed to log out. Please try again.');
            }
        } catch (error) {
            console.error('Logout error:', error);
            // alert('Something went wrong. Please try again.');
        }
    };


    return (
        <ProtectedPage>
            <Wrapper>
                <Block>
                    <Title>Dashboard</Title>
                    <Subtitle>Welcome, user!</Subtitle>
                    <InfoSection>
                        <InfoBox>
                            <strong>Account:</strong> john.doe@example.com
                        </InfoBox>
                        <InfoBox>
                            <strong>Role:</strong> Admin
                        </InfoBox>
                        <InfoBox>
                            <strong>Last Login:</strong> 2023-12-01 14:35:00
                        </InfoBox>
                    </InfoSection>
                    <Button onClick={() => navigate("/editor")}>Go to editor</Button>
                    <Button onClick={handleLogout}>Log Out</Button>
                </Block>
            </Wrapper>
        </ProtectedPage>
    );
};

export default DashboardPage;
