import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import RegisterPage from "./pages/RegisterPage.tsx";
import MarkdownEditor from "./pages/MarkdownEditor.tsx";
import NewDocumentEditor from "./pages/NewDocumentEditor.tsx";

const App: React.FC = () => {
  return (
      <Router>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/dashboard" element={<DashboardPage />} ></Route>
            <Route path="/documents/new" element={<NewDocumentEditor />} ></Route>
            <Route path="/documents/:id" element={<MarkdownEditor />} ></Route>
        </Routes>
      </Router>
  );
};

export default App;
