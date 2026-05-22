import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import keycloak from './keycloak';
import { BuscarUsuarioPorEmail } from './services/api';

keycloak.init({ onLoad: 'login-required' }).then(async () => {
    const email = keycloak.tokenParsed?.email;

    if (email) {
        const usuario = await BuscarUsuarioPorEmail(email);

        if (usuario.erro) {
            // Usuário não tem conta bancária — vai pra tela de completar cadastro
            localStorage.setItem("keycloak_email", email);
            localStorage.removeItem("usuario");
        } else {
            // Usuário tem conta — salva e vai pra home
            localStorage.setItem("usuario", JSON.stringify(usuario));
            localStorage.removeItem("keycloak_email");
        }
    }

    ReactDOM.createRoot(document.getElementById('root')!).render(
        <React.StrictMode>
            <App />
        </React.StrictMode>
    );
});
