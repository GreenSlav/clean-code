import React, {useState} from 'react';
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
    width: 40%;
    padding: 30px;
    border-radius: 15px;
    background-color: #0d2122; /* Внутренний блок с акцентным фоном */
    justify-content: center;
    align-items: center;
    box-shadow: 0px 10px 30px rgba(0, 0, 0, 0.5);
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
        box-shadow: 0 0 40px #14b7a6;
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
    animation: ${({$isHiding}) => ($isHiding ? fadeOut : fadeIn)} 0.3s ease-out;
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

const SuccessMessage = styled.div`
    position: fixed; /* Абсолютное позиционирование относительно окна браузера */
    top: 20px; /* Отступ от верхней границы */
    left: 50%; /* Центрируем по горизонтали */
    transform: translateX(-50%); /* Выравниваем центр сообщения */
    background-color: #14b7a6; /* Зеленый фон */
    color: #ffffff; /* Белый текст */
    padding: 1rem 1.5rem;
    border-radius: 8px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    font-size: 1rem;
    text-align: center;
    animation: ${fadeIn} 0.3s ease-out;
    z-index: 1000; /* Поверх остальных элементов */
`;

const RegisterPage: React.FC = () => {
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [isHiding, setIsHiding] = useState(false); // Управляет анимацией скрытия
    const [showError, setShowError] = useState(false); // Управляет отображением ошибки
    const navigate = useNavigate(); // Хук для перенаправления

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

        if (password !== confirmPassword) {
            setError('Passwords do not match.');
            setSuccess('');
            return;
        }

        try {
            const response = await fetch('http://localhost:5001/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include', // Включаем отправку куки
                body: JSON.stringify({username, email, password}),
            });

            const result = await response.json();

            if (!response.ok) {
                handleError(result.message || 'Registration failed.');
                setSuccess('');
            } else {
                setError('');
                setSuccess(result.message || 'Registration successful!');
                setShowError(false);
                setTimeout(() => navigate('/dashboard'), 2000); // Перенаправление через 2 секунды
            }
        } catch (error) {
            handleError('Something went wrong. Please try again.');
            setSuccess('');
        }
    };

    return (
        <Wrapper>
            <Block>
                <Title>Sign Up</Title>
                <form onSubmit={handleSubmit} style={{
                    width: '100%',
                    textAlign: 'center',
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                }}>
                    <Input
                        type="text"
                        placeholder="Username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                    />
                    <Input
                        type="email"
                        placeholder="Email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                    <Input
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                    <Input
                        type="password"
                        placeholder="Confirm Password"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        required
                    />
                    {showError &&
                        <ErrorMessage $isHiding={isHiding}>
                            {error}
                            <CloseButton onClick={handleCloseError}>×</CloseButton>
                        </ErrorMessage>
                    }
                    {success && <SuccessMessage>{success}</SuccessMessage>}
                    <Button type="submit">Register</Button>
                </form>
            </Block>
        </Wrapper>
    );
};

export default RegisterPage;