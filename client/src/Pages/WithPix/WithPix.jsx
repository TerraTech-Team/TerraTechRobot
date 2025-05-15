import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './WithPix.css';

const WithPix = () => {
    const navigate = useNavigate();
    const location = useLocation();

    const {
        processedImageUrl,
        originalImageUrl,
        originalImageFile,
        density: initialDensity,
        responseHeaders,
        colors: initialColors
    } = location.state || {};

    const [length, setLength] = useState(10000);
    const [width, setWidth] = useState(10000);
    const [density, setDensity] = useState(initialDensity || 64);
    const [selectedColor, setSelectedColor] = useState(null);
    const [isProcessing, setIsProcessing] = useState(false);
    const [isDownloading, setIsDownloading] = useState(false);
    const [processedImageBlob, setProcessedImageBlob] = useState(null);
    const [serverColors, setServerColors] = useState(initialColors || []);
    const [error, setError] = useState(null);
    const [imageLoaded, setImageLoaded] = useState(false);
    const [serverError, setServerError] = useState(null);


    const parseColorsFromHeaders = (headers) => {
        const colors = [];
        const headersMap = new Map(headers);

        for (let i = 1; i <= 10; i++) {
            const headerName = `color-${i}`;
            const foundHeader = Array.from(headersMap.entries())
                .find(([key]) => key.toLowerCase() === headerName.toLowerCase());

            if (foundHeader) {
                const rgbValue = foundHeader[1];
                colors.push({
                    id: i,
                    name: headerName,
                    rgb: rgbValue.trim(),
                    cssValue: `rgb(${rgbValue.trim()})`
                });
            }
        }

        return colors.sort((a, b) => a.id - b.id);
    };
    useEffect(() => {
        console.log('Initial colors:', initialColors);
        console.log('Response headers:', responseHeaders);

        if (responseHeaders && responseHeaders.length > 0) {
            const colors = parseColorsFromHeaders(responseHeaders);
            console.log('Parsed colors:', colors);
            setServerColors(colors);
        } else if (initialColors) {
            setServerColors(initialColors);
        }
    }, [responseHeaders, initialColors]);

    const DeleteButtonClick = () => {
        navigate('/WithoutImg');
    };


    const handleReprocess = async () => {
        if (!originalImageFile) return;

        setIsProcessing(true);
        setError(null);

        try {
            const formData = new FormData();
            formData.append('image', originalImageFile);
            formData.append('Quality', density.toString());

            const response = await fetch("https://virtical-robot-5e99.twc1.net/api/image/process ", {
                method: 'POST',
                body: formData,
                mode: 'cors'
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
            const colors = parseColorsFromHeaders(response.headers);
            console.log('Полученные цвета:', colors);

            const contentType = response.headers.get('content-type');
            if (contentType?.includes('image/')) {
                const imageBlob = await response.blob();
                const newProcessedImageUrl = URL.createObjectURL(imageBlob);
                setProcessedImageBlob(imageBlob);
                setServerColors(colors);
                setImageLoaded(false);

                navigate('/WithPix', {
                    state: {
                        processedImageUrl: newProcessedImageUrl,
                        originalImageUrl,
                        originalImageFile,
                        density,
                        colors
                    },
                    replace: true
                });
            }
        } catch (error) {
            console.error('Ошибка при обработке:', error);
            setError(`Ошибка обработки изображения: ${error.message}`);
        } finally {
            setIsProcessing(false);
        }
    };

    const handleDownloadCode = async () => {
        const blobToSend = processedImageBlob ||
            (processedImageUrl ? await fetch(processedImageUrl).then(r => r.blob()) : null);

        if (!blobToSend) {
            setError('Пожалуйста, сначала обработайте изображение');
            return;
        }

        setIsDownloading(true);
        setError(null);

        try {
            const formData = new FormData();
            formData.append('Image', blobToSend, 'image.png');
            formData.append('Length', length.toString());
            formData.append('Width', width.toString());

            const response = await fetch("https://virtical-robot-5e99.twc1.net/api/code/generate ", {
                method: 'POST',
                body: formData
            });

            if (!response.ok) throw new Error(`Ошибка сервера: ${response.status}`);

            const blob = await response.blob();
            const url = URL.createObjectURL(blob);

            const a = document.createElement('a');
            a.href = url;
            a.download = 'robot_code.bin';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            setTimeout(() => URL.revokeObjectURL(url), 100);
        } catch (error) {
            console.error('Ошибка:', error);
            setError(`Ошибка скачивания: ${error.message}`);
        } finally {
            setIsDownloading(false);
        }
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

            {/* Кнопка скачивания */}
            <button
                className="download-button"
                onClick={handleDownloadCode}
                disabled={isDownloading || !processedImageUrl}
            >
                {isDownloading ? 'Скачивание...' : 'Скачать программу'}
            </button>

            {/* Сообщение об ошибке */}
            {error && (
                <div className="error-message">
                    {error}
                    <button onClick={() => setError(null)}>Закрыть</button>
                </div>
            )}

            {/* Поля длины и ширины */}
            <div className="dimensions-container">
                <div className="dimension-input">
                    <label className="dimension-label">Длина</label>
                    <input
                        type="number"
                        value={length}
                        onChange={(e) => setLength(Number(e.target.value))}
                        className="size-input"
                        min="1"
                    />
                </div>
                <div className="dimension-input">
                    <label className="dimension-label">Ширина</label>
                    <input
                        type="number"
                        value={width}
                        onChange={(e) => setWidth(Number(e.target.value))}
                        className="size-input"
                        min="1"
                    />
                </div>
            </div>

            <div className="main-content">
                {/* Ползунок плотности */}
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

                {/* Блок с изображением */}
                <div className="green-field-pix">
                    {processedImageUrl ? (
                        <img
                            src={processedImageUrl}
                            alt="Обработанное изображение"
                            onLoad={() => setImageLoaded(true)}
                            style={{ display: imageLoaded ? 'block' : 'none' }}
                        />
                    ) : (
                        <div className="no-message">
                            Обработанное изображение не найдено
                        </div>
                    )}
                    {!imageLoaded && processedImageUrl && (
                        <div className="image-loading">Загрузка изображения...</div>
                    )}

                </div>

                {/* Палитра цветов */}
                <div className="colors-container-pix">
                    <div className="color-palette-pix">
                        <label className="color-label">Цвета <br />изображения</label>
                        {serverColors.length > 0 ? (
                            <div className="color-grid">
                                {serverColors.map((color) => {
                                    const rgbToHex = (rgb) => {
                                        if (!rgb) return '';
                                        const parts = rgb.split(',').map(part => parseInt(part.trim()));
                                        if (parts.length !== 3) return '';

                                        const toHex = (num) => {
                                            const hex = num.toString(16);
                                            return hex.length === 1 ? '0' + hex : hex;
                                        };

                                        return `#${toHex(parts[0])}${toHex(parts[1])}${toHex(parts[2])}`.toUpperCase();
                                    };

                                    const hexColor = rgbToHex(color.rgb);

                                    return (
                                        <div
                                            key={color.id}
                                            className={`color-item ${selectedColor === color.rgb ? 'selected' : ''}`}
                                            style={{ backgroundColor: `rgb(${color.rgb})` }}
                                        >
                                            <span className="color-tooltip">
                                                {color.name.replace('color-', 'Цвет ')}: {hexColor}
                                            </span>
                                        </div>
                                    );
                                })}
                            </div>
                        ) : (
                            <div className="no-message">
                                Цвета не загружены
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* Кнопки управления */}
            <div className="buttons-container">
                <button
                    className="reprogress-pixel-button"
                    onClick={handleReprocess}
                    disabled={isProcessing}
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