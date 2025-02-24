import React from 'react';
import styled from 'styled-components';
import AuthSection from "../components/AuthSection.tsx";
import Typing from "../components/Typing.tsx";

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
    height: 50%;
    width: 60%;
    padding: 10px;
    border-radius: 15px;
    justify-content: space-around;
    align-items: center;
`

// const LogoWrapper = styled.div`
//     display: flex;
//     align-items: center;
//     justify-content: center;
//     height: 100%;
//     width: 50%;
// `;

// const Logo = styled.img`
//     max-width: 100%;
//     max-height: 100%;
//     object-fit: contain;
//     filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.5));
// `;

const TypingAndAuth = styled.div`
    width: 100%;
    height: 100%;
`

const HomePage: React.FC = () => {
    return (
        <Wrapper>
            <Block>
                <TypingAndAuth>
                    <Typing></Typing>
                    <AuthSection></AuthSection>
                </TypingAndAuth>
            </Block>
        </Wrapper>
    );
};

export default HomePage;