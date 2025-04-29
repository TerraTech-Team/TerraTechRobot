import React from 'react';
import { useNavigate } from 'react-router-dom';
import './Home.css';

const Home = () => {
  const navigate = useNavigate();

  const handleButtonClick = () => {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.multiple = false;

    input.onchange = (e) => {
      const file = e.target.files[0];
      if (file) {
        if (!file.type.match('image.*')) {
          alert('Пожалуйста, выберите файл изображения');
          return;
        }

        const imageUrl = URL.createObjectURL(file);

        navigate('/WithImg', {
          state: {
            imageUrl,
            imageFile: file
          }
        });
      }
    };

    input.click();
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