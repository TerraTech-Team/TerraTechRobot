import React from 'react';
import { useNavigate } from 'react-router-dom';
import './Home.css';

const Home = () => {
  const navigate = useNavigate();

  const handleButtonClick = () => {
    navigate('/WithImg'); // Переход на страницу с изображением
  };

  return (
    <div className="home-page">
      <div className="robot-container">
        <h1 className="robot-title">Робот для посева</h1>

        <p className="robot-description">
          Загрузите изображение, чтобы отредактировать и скачать программу посева
        </p>

        <div className="upload-section">
          <button
            className="upload-button"
            onClick={handleButtonClick}
          >
            Загрузить изображение
          </button>
        </div>
      </div>
    </div>
  );
};

export default Home;