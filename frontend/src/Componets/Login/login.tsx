import { useState } from "react";
import { Login as LoginAPI } from "../../services/api";
import { Link, useNavigate } from "react-router-dom";
import "./login.css";


function Logarr(){
    const [email, setEmail] = useState<string>("");
    const [senha, setSenha] = useState<string>("");
    const [erro, setErro] = useState<string>("");
    const [sucesso, setSucesso] = useState<string>("");
    const navigate = useNavigate();
    
    async function handleLogin(){
        const resposta = await LoginAPI(email,senha);

        if (resposta.erro) {
            setErro(resposta.erro);
            setSucesso("");
        } else {
            localStorage.setItem("usuario",JSON.stringify(resposta));
            setErro("");
            setSucesso("Login realizado com sucesso");
            setTimeout(() => {
                navigate("/");
            }, 2000);
        }
    }

    return(
        <div className="login-page">
            <div className="login-card">
                <Link to={"/"} className="nav-link">← Home</Link>
                <h2>Login</h2>
                <div className="login-form">
                    <input 
                        type="email"
                        placeholder="Digite seu email"
                        onChange={(e)=> setEmail(e.target.value)} 
                    />
                    <input
                        type="password"
                        placeholder="Digite sua senha"
                        onChange={(e) => setSenha(e.target.value)}
                    />
                    <button onClick={handleLogin}>Entrar</button>
                </div>
                {erro && <p className="login-erro">{erro}</p>}
                {sucesso && <p className="login-sucesso">{sucesso}</p>}
            </div>
        </div>
    )
}

export default Logarr;