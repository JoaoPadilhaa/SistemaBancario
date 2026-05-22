import { useEffect, useState } from "react";
import "./perfill.css";
import { AlterarSenha } from "../../services/api";
import { Link } from "react-router-dom";
import { useUsuario } from "../../UserContext";

function Perfill(){
    const { usuario, setUsuario} = useUsuario();
    const [mostrarForm, setMostrarForm] = useState<boolean>(false);
    const [senhaAtual, setSenhaAtual] = useState<string>("");
    const [novaSenha, setNovaSenha] = useState<string>("");
    const [confirmarSenha, setConfirmarSenha] = useState<string>("");
    const [mensagem, setMensagem] = useState<string>("");


    async function handleAtualizarSenha(){
        if(!senhaAtual || !novaSenha || !confirmarSenha){
            setMensagem("Preencha todos os campos");
            setTimeout(() => {
                setMensagem("");
            }, 2000);
            return;
        }   
        if(novaSenha !== confirmarSenha){
            setMensagem("Senhas não coincidem");
            setTimeout(() => {
                setMensagem("");
            }, 2000);
            return;
        }

        const resposta = await AlterarSenha(usuario.email, senhaAtual, novaSenha);

        if (resposta.erro) {
            setMensagem(resposta.erro);
            setTimeout(() => {
                setMensagem("");
            }, 2000);
        }
        else {
            setMensagem("Senha alterada com sucesso");
            setTimeout(() => {
                setMensagem("");
                setMostrarForm(false);
            }, 2000);
        }
        
    }

    return (
        <div className="perfil-page">
            <div className="perfil-card">
                <Link to="/" className="nav-link">← Home</Link>
                <h2>Meu Perfil</h2>

                {usuario && (
                    <div className="perfil-info">
                        <div className="avatar">
                            {usuario.titular?.charAt(0).toUpperCase()}
                        </div>
                        <div className="perfil-campo">
                            <span className="campo-label">Nome</span>
                            <span className="campo-value">{usuario.titular}</span>
                        </div>
                        <div className="perfil-campo">
                            <span className="campo-label">Email</span>
                            <span className="campo-value">{usuario.email}</span>
                        </div>
                        <div className="perfil-campo">
                            <span className="campo-label">Tipo de Conta</span>
                            <span className="campo-value">{usuario.tipo === "poupanca" ? "Poupança" : "Corrente"}</span>
                        </div>
                        <div className="perfil-campo">
                            <span className="campo-label">Senha</span>
                            <span className="campo-value">••••••••••</span>
                        </div>

                        {mostrarForm && (
                            <div className="senha-form">
                                <input type="password" placeholder="Senha Atual" onChange={(e) => setSenhaAtual(e.target.value)} />
                                <input type="password" placeholder="Nova Senha" onChange={(e) => setNovaSenha(e.target.value)} />
                                <input type="password" placeholder="Confirmar Nova Senha" onChange={(e) => setConfirmarSenha(e.target.value)} />
                                <div className="senha-btns">
                                    <button className="btn-confirmar" onClick={handleAtualizarSenha}>Confirmar</button>
                                    <button className="btn-cancelar" onClick={() => setMostrarForm(false)}>Cancelar</button>
                                </div>
                                {mensagem && <p className={mensagem.includes("sucesso") ? "msg-sucesso" : "msg-erro"}>{mensagem}</p>}
                            </div>
                        )}

                        <button className="btn-alterar-senha" onClick={() => setMostrarForm(!mostrarForm)}>
                            {mostrarForm ? "Fechar" : "Alterar Senha"}
                        </button>
                    </div>
                )}
            </div>
        </div>
    )
}

export default Perfill;