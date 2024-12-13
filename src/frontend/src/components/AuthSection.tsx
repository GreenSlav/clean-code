import React from 'react';
import styled from 'styled-components';
import { useNavigate } from 'react-router-dom';

const AuthWrapper = styled.div`
    height: 50%;
    width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 2rem; /* Расстояние между кнопками */
  color: #fafafa; /* Светлый текст */
`;

const Button = styled.button`
    padding: 0.75rem 1.5rem;
    font-size: 2rem;
    border: none;
    border-radius: 8px;
    width: 50%;
    color: #0d2122;
    background-color: #14b7a6; /* Акцентный цвет */
    cursor: pointer;
    transition: background-color 0.4s ease, box-shadow 0.4s ease, color 0.4s ease;
    box-shadow: 10px 10px 40px black;

    &:hover {
        color: white;
        background-color: #0d2122; /* Более насыщенный акцентный цвет */
        box-shadow: 0 0 20px #14b7a6;
    }

    &:active {
        color: white;
        background-color: #031010; /* Более насыщенный акцентный цвет */
        box-shadow: 0 0 40px #14b7a6;
    }
`;

const AuthSection: React.FC = () => {
  const navigate = useNavigate();

  const goToLogin = () => navigate('/login'); // Переход на страницу логина
  const goToRegister = () => navigate('/register'); // Переход на страницу регистрации

  return (
    <AuthWrapper>
      <Button onClick={goToLogin}>Sign in</Button>
      <Button onClick={goToRegister}>Sign up</Button>
    </AuthWrapper>
  );
};

export default AuthSection;
