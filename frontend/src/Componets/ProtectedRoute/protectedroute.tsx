import { Navigate } from "react-router-dom";
import keycloak from "../../keycloak";

interface Props {
    children: React.ReactNode;
}

function ProtectRoute({ children }: Props) {
    // Se o Keycloak não tá autenticado, manda pro login
    if (!keycloak.authenticated) {
        keycloak.login();
        return null;
    }

    // Se tá autenticado mas não tem conta bancária, manda completar cadastro
    const temConta = localStorage.getItem("usuario");
    const temEmailPendente = localStorage.getItem("keycloak_email");

    if (!temConta && temEmailPendente) {
        return <Navigate to="/completar-cadastro" />;
    }

    // Tudo certo — renderiza a página
    return <>{children}</>;
}

export default ProtectRoute;
