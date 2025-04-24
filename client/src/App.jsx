import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import React from 'react';
import Home from './Pages/Home/Home.jsx';
import WithImg from './Pages/WithImg/WithImg.jsx';
import WithoutImg from './Pages/WithoutImg/WithoutImg.jsx';
import WithPix from './Pages/WithPix/WithPix.jsx';
import './App.css';

function App() {
  return (
    <Router>
      <Routes>
        {/*Начальная страница */}
        <Route path="/" element={<Home />} />
        {/* Страница с изображением*/}
        <Route path="/WithImg" element={<WithImg />} />
        {/* Страница с пиксель-артом*/}
        <Route path="/WithPix" element={<WithPix />} />
        {/* Страница без изображения*/}
        <Route path="/WithoutImg" element={<WithoutImg />} />
      </Routes>
    </Router>
  );
}

export default App;