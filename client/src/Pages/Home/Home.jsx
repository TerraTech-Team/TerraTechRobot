import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Home.css';

const Home = () => {
  const navigate = useNavigate();
  const [serverError, setServerError] = useState(null);

  const handleButtonClick = () => {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.multiple = false;

    input.onchange = (e) => {
      const file = e.target.files[0];
      if (file) {
        if (!file.type.match('image.*')) {
          setServerError({
            code: 415,
            message: "Неподдерживаемый тип файла. "
          });
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
      {/* Блок для отображения ошибок сервера */}
      {serverError && (
        <div className="server-error-message">
          <div className="error-header">
            Ошибка {serverError.code}
            <button
              className="error-close-button"
              onClick={() => setServerError(null)}
            >
              ×
            </button>
          </div>
          <div className="error-body">
            {serverError.message}
          </div>
        </div>
      )}
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