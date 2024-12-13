import React, { useState } from 'react';
import styled from 'styled-components';

const Wrapper = styled.div`
    width: 100vw;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100vh;
    background-color: #0b0d0e; /* Темный фон */
    color: #fafafa; /* Светлый текст */
`;

const Block = styled.div`
    display: flex;
    flex-direction: column;
    height: 70%;
    width: 40%;
    padding: 20px;
    border-radius: 15px;
    background-color: #0d2122; /* Внутренний блок с акцентным фоном */
    justify-content: center;
    align-items: center;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5);
`;

const Title = styled.h1`
    font-size: 2.5rem;
    font-weight: bold;
    color: #14b7a6; /* Акцентный цвет */
    margin-bottom: 1rem;
`;

const Input = styled.input`
    margin: 1rem 0;
    padding: 0.75rem;
    width: 80%;
    font-size: 1rem;
    border: 2px solid #0d9488; /* Темный акцент */
    border-radius: 8px;
    background-color: #0b0d0e; /* Поле ввода: темный фон */
    color: #fafafa; /* Светлый текст */
    outline: none;

    &:focus {
        border-color: #14b7a6; /* Акцентный цвет при фокусе */
    }
`;

const Button = styled.button`
    padding: 0.75rem 1.5rem;
    font-size: 1.5rem;
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
        background-color: #031010; /* Более насыщенный акцентный цвет */
        box-shadow: 0 0 5px #14b7a6;
    }
`;

const ErrorMessage = styled.p`
    color: #e63946; /* Красный цвет для ошибки */
    font-size: 1rem;
    margin: 0.5rem 0;
`;

const LoginPage: React.FC = () => {
    const [error, setError] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        // Пример валидации (добавь свою логику)
        const username = (e.target as HTMLFormElement).username.value.trim();
        const password = (e.target as HTMLFormElement).password.value.trim();

        if (!username || !password) {
            setError('Both fields are required.');
        } else {
            setError('');
            alert('Login successful!'); // Здесь можно добавить логику отправки формы
        }
    };

    return (
        <Wrapper>
            <Block>
                <Title>Sign In</Title>
                <form onSubmit={handleSubmit} style={{
                    width: '100%',
                    textAlign: 'center',
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                }}>
                    <Input name="username" type="text" placeholder="Username" required />
                    <Input name="password" type="password" placeholder="Password" required />
                    {error && <ErrorMessage>{error}</ErrorMessage>}
                    <Button type="submit">Log In</Button>
                </form>
            </Block>
        </Wrapper>
    );
};

export default LoginPage;
