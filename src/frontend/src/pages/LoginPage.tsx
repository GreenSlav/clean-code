import React, {useEffect, useState} from 'react';
import styled, {keyframes} from 'styled-components';
import {useNavigate} from 'react-router-dom';

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

const fadeOut = keyframes`
    from {
        opacity: 1;
        transform: translateY(0);
    }
    to {
        opacity: 0;
        transform: translateY(-10px);
    }
`;

const ErrorMessage = styled.div<{ $isHiding: boolean }>`
    position: fixed;
    top: 20px;
    background-color: #e63946; /* Красный фон */
    color: #ffffff; /* Белый текст */
    padding: 1rem 1.5rem;
    border-radius: 8px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    font-size: 1rem;
    text-align: center;
    z-index: 1000;
    display: flex; /* Для размещения текста и кнопки крестика */
    justify-content: space-between; /* Пространство между текстом и крестиком */
    align-items: center;
    width: 300px; /* Фиксированная ширина */
    animation: ${({ $isHiding }) => ($isHiding ? fadeOut : fadeIn)} 0.3s ease-out;
`;

const CloseButton = styled.button`
    background: none;
    border: none;
    color: #ffffff;
    font-size: 1.2rem;
    cursor: pointer;
    margin-left: 10px;
    outline: none;
    transition: color 0.3s;

    &:hover {
        color: #ffcccc; /* Светлый оттенок при наведении */
    }
`;

const LoginPage: React.FC = () => {
    const API_BASE_URL = import.meta.env.VITE_REACT_APP_BACKEND_URL;
    const [identifier, setIdentifier] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [showError, setShowError] = useState(false); // Управляет отображением ошибки
    const [isHiding, setIsHiding] = useState(false); // Управляет анимацией скрытия
    const navigate = useNavigate(); // Хук для перенаправления

    useEffect(() => {
        console.warn(`Вот мое url из докера!!!!! ----- ${API_BASE_URL}`);
    });

    const handleError = (message: string) => {
        setShowError(false); // Скрываем текущую ошибку
        setTimeout(() => {
            setError(message);
            setIsHiding(false);
            setShowError(true); // Показываем ошибку с обновленным текстом
        }, 100); // Делаем паузу перед повторным отображением
    };

    const handleCloseError = (e: React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault(); // Предотвращаем любые действия по умолчанию
        setIsHiding(true); // Запускаем анимацию скрытия
        setTimeout(() => {
            setShowError(false);
            setError(''); // Очищаем текст ошибки
        }, 300); // Тайм-аут совпадает с длительностью анимации
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include', // Для отправки и получения куки
                body: JSON.stringify({identifier, password}),
            });

            const result = await response.json();

            if (!response.ok) {
                handleError(result.message || 'Login failed.');
            } else {
                setError('');
                setShowError(false);
                navigate('/dashboard'); // Перенаправление после успешного входа
            }
        } catch (error) {
            handleError('Something went wrong. Please try again.');
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
                    <Input
                        name="identifier"
                        type="text"
                        placeholder="Username or Email"
                        value={identifier}
                        onChange={(e) => setIdentifier(e.target.value)}
                        required
                    />
                    <Input
                        name="password"
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                    {showError &&
                        <ErrorMessage $isHiding={isHiding}>
                            {error}
                            <CloseButton onClick={handleCloseError}>×</CloseButton>
                        </ErrorMessage>
                    }
                    <Button type="submit">Log In</Button>
                </form>
            </Block>
        </Wrapper>
    );
};

export default LoginPage;