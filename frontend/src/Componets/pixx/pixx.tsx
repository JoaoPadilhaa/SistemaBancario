import { useEffect, useState } from "react";
import { BuscarUsuarioPorEmail, EnviarPix } from "../../services/api";
import { Link, useNavigate } from "react-router-dom";
import "./pixx.css";

function Pixx(){
    const [emailPix, setEmailPix] = useState("");
    const [destinatario, setDestinatario] = useState<any>(null);
    const [valorPix, setValorPix] = useState(0);
    const [usuario, setUsuario] = useState<any>(null);
    const navigate = useNavigate();
    const [flag, setFlag] = useState<boolean>(false);

    useEffect(() => {
        const dados = localStorage.getItem("usuario");
        if(dados){
            const usuario = JSON.parse(dados);
            setUsuario(usuario);
        }
    }, [])

    async function handleBuscarDestinatario(){
        const resposta = await BuscarUsuarioPorEmail(emailPix);
        if (resposta.erro){
            setDestinatario(null);
            alert(resposta.erro);
        } else {
            setDestinatario(resposta);
        }
    }

    async function handleEnviarPix() {
        const resposta = await EnviarPix(usuario.id, emailPix, valorPix);
        if(resposta.erro){
            alert(resposta.erro)
            return;
        }
        const atualizado = {...usuario, saldo: resposta.saldo};
        localStorage.setItem("usuario", JSON.stringify(atualizado));
        setUsuario(atualizado)
        setFlag(true)
        setTimeout(() => {
            navigate('/');
        }, 2000);
    }

    return (
        <div className="pix-page">
            <div className="pix-card">
                <Link to="/" className="nav-link">← Home</Link>
                <h2>Enviar Pix</h2>

                <div className="pix-busca">
                    <input
                        type="email"
                        placeholder="Digite o email do destinatário"
                        onChange={(e) => setEmailPix(e.target.value)}
                    />
                    <button className="btn-buscar" onClick={handleBuscarDestinatario}>Buscar</button>
                </div>

                {destinatario && (
                    <div className="destinatario-card">
                        <p className="destinatario-label">✅ Destinatário encontrado</p>
                        <div className="destinatario-info">
                            <span className="info-label">Titular</span>
                            <span className="info-value">{destinatario.titular}</span>
                        </div>
                        <div className="destinatario-info">
                            <span className="info-label">Chave Pix</span>
                            <span className="info-value">{destinatario.email}</span>
                        </div>
                        <div className="destinatario-info">
                            <span className="info-label">Conta Nº</span>
                            <span className="info-value">{destinatario.contaId}</span>
                        </div>
                        <input
                            type="number"
                            className="input-valor"
                            placeholder="Valor a enviar"
                            onChange={(e) => setValorPix(Number(e.target.value))}
                        />
                        <button className="btn-pix" onClick={handleEnviarPix}>💸 Enviar Pix</button>
                        {flag && <p className="msg-sucesso">Pix enviado com sucesso!</p>}
                    </div>
                )}
            </div>
        </div>
    )
}

export default Pixx;
