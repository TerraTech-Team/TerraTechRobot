import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './WithoutImg.css';

const WithoutImg = () => {
    const navigate = useNavigate();

    const [length, setLength] = useState('');
    const [width, setWidth] = useState('');
    const [density, setDensity] = useState(1);
    const [selectedColor, setSelectedColor] = useState('#4CAF50');
    const [serverError, setServerError] = useState(null);
    const colors = ['#4CAF50', '#2196F3', '#FFC107', '#F44336', '#9C27B0'];

    const UploadButtonClick = () => {
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
        <div className="settings-container">
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
                {/* Левая часть - слайдер плотности */}
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

                {/* Центральная часть - зеленое поле */}
                <div className="green-field">
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
                <button className="upload-img" onClick={UploadButtonClick}>Загрузить <br />изображение</button>
            </div>
        </div>
    );
};

export default WithoutImg;