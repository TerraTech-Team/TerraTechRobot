import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './WithPix.css';

const WithPix = () => {
    const navigate = useNavigate();
    const location = useLocation();

    const { processedImageUrl, originalImageUrl, originalImageFile } = location.state || {};

    const [length, setLength] = useState('');
    const [width, setWidth] = useState('');
    const [density, setDensity] = useState(64);
    const [selectedColor, setSelectedColor] = useState('#4CAF50');
    const [imageLoaded, setImageLoaded] = useState(false);
    const [isProcessing, setIsProcessing] = useState(false);

    const colors = ['#4CAF50', '#2196F3', '#FFC107', '#F44336', '#9C27B0'];

    const DeleteButtonClick = () => {
        navigate('/WithoutImg');
    };

    useEffect(() => {
        return () => {
            if (processedImageUrl) {
                URL.revokeObjectURL(processedImageUrl);
            }
        };
    }, [processedImageUrl]);


    const handleReprocess = async () => {
        if (!originalImageFile) return;

        setIsProcessing(true);

        try {
            const formData = new FormData();
            formData.append('image', originalImageFile);
            formData.append('Quality', density.toString());

            const response = await fetch("https://virtical-robot-5e99.twc1.net/api/image/process", {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                throw new Error(`Ошибка сервера: ${response.status}`);
            }

            const contentType = response.headers.get('content-type');
            if (contentType?.includes('image/')) {
                const imageBlob = await response.blob();
                const newProcessedImageUrl = URL.createObjectURL(imageBlob);
                navigate('/WithPix', {
                    state: {
                        processedImageUrl: newProcessedImageUrl,
                        originalImageUrl,
                        originalImageFile,
                        density
                    },
                    replace: true
                });
            }
        } catch (error) {
            console.error('Ошибка при повторной обработке:', error);
            alert('Произошла ошибка при повторной обработке изображения');
        } finally {
            setIsProcessing(false);
        }
    };

    return (
        <div className="settings-container">
            <button
                className="download-button"
            >
                Скачать программу
            </button>

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

                {/* Центральная часть - отображение обработанного изображения */}
                <div className="green-field-pix">
                    {processedImageUrl ? (
                        <img
                            src={processedImageUrl}
                            alt="Обработанное изображение"
                            onLoad={() => setImageLoaded(true)}
                        />
                    ) : (
                        <div className="no-image-message">
                            Обработанное изображение не найдено
                        </div>
                    )}
                    {isProcessing && (
                        <div className="processing-overlay">
                            <div className="processing-spinner"></div>
                            <p>Обработка...</p>
                        </div>
                    )}
                </div>

                {/* Правая часть - цвета */}
                <div className="colors-container-pix">
                    <div className="color-palette-pix">
                        <label className="color-label">Выбор цветов</label>

                        {colors.map((color) => (
                            <div key={color} className="color-row">
                                <div
                                    className="color-circle"
                                    style={{ backgroundColor: color }}
                                    onClick={() => setSelectedColor(color)}
                                />
                                <div className="color-rectangle"></div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>

            <div className="buttons-container">
                <button
                    className="reprogress-pixel-button"
                    onClick={handleReprocess}
                >
                    {isProcessing ? 'Обработка...' : 'Переделать'}
                </button>
                <button
                    className="delete-button"
                    onClick={DeleteButtonClick}
                >
                    Удалить
                </button>
            </div>
        </div>
    );
};

export default WithPix;