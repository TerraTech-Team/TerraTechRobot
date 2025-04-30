import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './WithImg.css';

const WithImg = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { imageUrl, imageFile } = location.state || {};

    if (!imageUrl || !imageFile) {
        return (
            <div className="error-message">
                <p>Изображение не загружено или произошла ошибка</p>
                <button onClick={() => navigate('/')}>Вернуться на главную</button>
            </div>
        );
    }
    const [length, setLength] = useState('');
    const [width, setWidth] = useState('');
    const [density, setDensity] = useState(64);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleButtonClick = async () => {
        setIsLoading(true);
        setError(null);

        try {
            const formData = new FormData();

            formData.append('image', imageFile);
            formData.append('Quality', density.toString());

            const response = await fetch("https://virtical-robot-5e99.twc1.net/api/image/process", {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || `Ошибка сервера: ${response.status}`);
            }


            const blob = await response.blob();
            const imageUrl = URL.createObjectURL(blob);
            navigate('/WithPix', { state: { processedImageUrl: imageUrl } });


        } catch (error) {
            console.error('Ошибка:', error);
            setError(error.message || 'Произошла ошибка при обработке изображения');
        } finally {
            setIsLoading(false);
        }
    };
    const DeleteButtonClick = () => {
        navigate('/WithoutImg');
    };

    return (
        <div className="settings-container">
            {/* Ввод размера */}
            <div className="dimensions-container">
                <div className="dimension-input">
                    <label className="dimension-label">Длина</label>
                    <input
                        type="number"
                        value={length}
                        onChange={(e) => setLength(e.target.value)}
                        className="size-input"
                        min="1"
                    />
                </div>

                <div className="dimension-input">
                    <label className="dimension-label">Ширина</label>
                    <input
                        type="number"
                        value={width}
                        onChange={(e) => setWidth(e.target.value)}
                        className="size-input"
                        min="1"
                    />
                </div>
            </div>

            <div className="main-content">
                <div className="density-slider-container">
                    <label className="input-label">Плотность <br />посева</label>
                    <div className="slider-wrapper">
                        <input
                            type="range"
                            value={density}
                            onChange={(e) => setDensity(e.target.value)}
                            className="density-slider-vertical"
                            min="16"
                            max="128"
                            step="1"
                            orient="vertical"
                        />
                        <span className="density-value">{density}</span>
                    </div>
                </div>

                <div className="green-field">
                    <img
                        src={imageUrl}
                        alt="Загруженное изображение"
                        className="uploaded-image"
                    />
                </div>

                {/* Правая часть - цвета */}
                <div className="colors-container">
                    <div className="color-palette">
                        <label className="color-label">Доступные <br />цвета</label>
                        <div className="color-circle red"></div>
                        <div className="color-circle orange"></div>
                        <div className="color-circle gold"></div>
                        <div className="color-circle green"></div>
                        <div className="color-circle blue"></div>
                        <div className="color-circle purple"></div>
                        <div className="color-circle white"></div>
                        <div className="color-circle black"></div>
                        <div className="color-circle pink"></div>
                        <div className="color-circle brown"></div>
                    </div>
                </div>
            </div>
            <div className="buttons-container">
                <button
                    className="create-pixel-button"
                    onClick={handleButtonClick}
                    disabled={isLoading}
                >
                    {isLoading ? 'Обработка...' : 'Создать пиксель-арт'}
                </button>
                <button className="delete-button" onClick={DeleteButtonClick}>
                    Удалить
                </button>
            </div>
        </div>
    );
};

export default WithImg;