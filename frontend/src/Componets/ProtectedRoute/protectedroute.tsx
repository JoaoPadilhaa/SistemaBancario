import { Navigate } from "react-router-dom";

interface Props {
    children: React.ReactNode;
}

//Dentro das rotas, vamos passar <ProtectRoute><aqui dentro o elemento></ProtectRoute>
//O elemento automaticamente se torna child(filho) do ProtectRoute, então podemos acessar o elemento filho atraves da prop Children
function ProtectRoute({children}: Props) {
    //Pega os dados do usuario salvo no localStorage(se tiver algum usuario logado)
    const dados = localStorage.getItem("usuario");

    //Se não tiver usuario logado, retorna para tela de login
    if (!dados) {
        return <Navigate to="/login" />;
    }

    //se tiver, retorna o elemento filho
    return <>{children}</>;
}

export default ProtectRoute;