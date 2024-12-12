import React from 'react';
import styled from 'styled-components';
import { TypeAnimation } from 'react-type-animation';

const TypingWrapper = styled.div`
    height: 50%;
    width: 100%;
    font-size: 3rem;
    color: #ececec; /* Вторичный текст */
    display: flex;
    justify-content: center;
    align-items: center;
    text-align: center; /* Центрирование текста */
    white-space: normal; /* Разрешить перенос строк */
`;


const Typing: React.FC = () => {
    return (
        <TypingWrapper>
            <TypeAnimation
                sequence={[
                    "Let's write some MD?", // Текст для печати
                    3500, // Пауза на 1 секунду
                    'Sign in or sign up!', // Добавляем точку
                ]}
                wrapper="span"
                speed={50} // Скорость печати
                cursor={true} // Включить моргающий курсор
                style={{
                    display: 'inline-block',
                    padding: '0.5rem 1rem', // Отступы внутри текста
                    borderRadius: '10px', // Закругленные углы
                    textAlign: 'center', // Центрирование текста
                    color: '#14b7a6',
                    backgroundColor: '#0d2122', /* Акцентный цвет */
                }}
            />
        </TypingWrapper>
    );
};

export default Typing;
