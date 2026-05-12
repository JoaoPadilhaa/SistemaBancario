import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';

// DICA: Este é o ponto de entrada do React.
// Ele monta o componente <App /> dentro da div#root do index.html.

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
