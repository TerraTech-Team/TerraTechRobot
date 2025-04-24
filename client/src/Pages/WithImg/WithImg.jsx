import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './WithImg.css';

const WithImg = () => {
    const navigate = useNavigate();

    const [size, setSize] = useState();
    const [density, setDensity] = useState(1);
    const [selectedColor, setSelectedColor] = useState('#4CAF50');

    const colors = ['#4CAF50', '#2196F3', '#FFC107', '#F44336', '#9C27B0'];
    const DeleteButtonClick = () => {
        navigate('/WithoutImg');
    }
    const handleButtonClick = () => {
        navigate('/WithPix');

    }
    return (
        <div className="settings-container">


            {/* Ввод размера */}
            <div className="size-input-container">
                <label className="input-label-size">Размер</label>
                <input
                    type="number"
                    value={size}
                    onChange={(e) => setSize(e.target.value)}
                    className="size-input"
                    min="1"
                />
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
                            min="1"
                            max="64"
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


            <div class="buttons-container">
                <button class="create-pixel-button" onClick={handleButtonClick}>Создать <br />пиксель-арт</button>
                <button class="delete-button" onClick={DeleteButtonClick}>Удалить</button>
            </div>
        </div>
    );
};

export default WithImg;