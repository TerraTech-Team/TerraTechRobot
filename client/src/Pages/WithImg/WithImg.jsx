import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './WithImg.css';

const WithImg = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { imageUrl, imageFile } = location.state || {};
    useEffect(() => {
        const handleBackButton = () => {
            navigate('/');
        };

        window.addEventListener('popstate', handleBackButton);

        return () => {
            window.removeEventListener('popstate', handleBackButton);
        };
    }, [navigate]);

    useEffect(() => {
        if (!imageUrl || !imageFile) {
            const back =
                navigate('/', { replace: true });
            return () => clearTimeout(back);
        }
    }, [imageUrl, imageFile, navigate]);

    const [length, setLength] = useState('');
    const [width, setWidth] = useState('');
    const [density, setDensity] = useState(64);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [serverError, setServerError] = useState(null);

    const handleButtonClick = async () => {
        setIsLoading(true);
        setError(null);
        setServerError(null);
        try {
            const formData = new FormData();
            formData.append('image', imageFile);
            formData.append('Quality', density.toString());

            const response = await fetch("https://virtical-robot-5e99.twc1.net/api/image/process", {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                if (response.status === 415) {
                    setServerError({
                        code: 415,
                        message: "Неподдерживаемый тип файла."
                    });
                } else if (response.status === 400) {
                    setServerError({
                        code: 400,
                        message: "Неверный запрос. Пожалуйста, проверьте отправляемые данные."
                    });
                } else if (response.status === 500) {
                    setServerError({
                        code: 500,
                        message: "Внутренняя ошибка сервера. Пожалуйста, попробуйте позже."
                    });
                } else {
                    try {
                        const errorData = await response.json();
                        setServerError({
                            code: response.status,
                            message: errorData.message || `Ошибка ${response.status}`
                        });
                    } catch {
                        setServerError({
                            code: response.status,
                            message: `Произошла ошибка (код ${response.status})`
                        });
                    }
                }
                return;
            }

            const blob = await response.blob();
            const imageDataUrl = URL.createObjectURL(blob);

            navigate('/WithPix', {
                state: {
                    processedImageUrl: imageDataUrl,
                    originalImageUrl: imageUrl,
                    originalImageFile: imageFile,
                    density,
                    responseHeaders: Array.from(response.headers.entries())
                }
            });

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

            {/* Блок для других ошибок */}
            {error && (
                <div className="error-message">
                    <p>{error}</p>
                    <button onClick={() => setError(null)}>Закрыть</button>
                </div>
            )}

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

                <div className="colors-container">
                    <div className="color-palette">
                        <label className="color-label">Доступные <br />цвета</label>
                        <div className="color-circle red" title="Красный (DC143C)"></div>
                        <div className="color-circle orange" title="Оранжевый (FF8C00)"></div>
                        <div className="color-circle gold" title="Золотой (FFD700)"></div>
                        <div className="color-circle green" title="Зеленый (3CB371)"></div>
                        <div className="color-circle blue" title="Голубой (6495ED)"></div>
                        <div className="color-circle purple" title="Фиолетовый (BA55D3)"></div>
                        <div className="color-circle white" title="Белый (F5F5F5)"></div>
                        <div className="color-circle black" title="Черный (1E1E1E)"></div>
                        <div className="color-circle pink" title="Розовый (FFB6C1)"></div>
                        <div className="color-circle brown" title="Коричневый (8B4513)"></div>
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