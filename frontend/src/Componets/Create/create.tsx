import { useState } from 'react';
import './create.css';
import { BuscarUsuarioPorEmail, Registrar } from '../../services/api';
import { Link, useNavigate } from 'react-router-dom';

export const Create = () => {
    const [email, setEmail] = useState<string>("");
    const [senha, setSenha] = useState<string>("");
    const [titular, setTitular] = useState<string>("");
    const [saldoInicial, setSaldoInicial] = useState<number>(0);
    const [mensagem, setMensagem] = useState<string>("");
    const [sucesso, setSucesso] = useState<boolean>(false);
    const [tipo, setTipo] = useState<string>("");
    const navigate = useNavigate();

    //função que vai registrar o usuario
    async function handleRegistrar() {
        if (!titular.trim()){
            setMensagem("Nome é obrigatório");
            setSucesso(false);
            return;
        }
        if (!email.trim()){
            setMensagem("Email é obrigatório");
            setSucesso(false);
            return;
        }
        if(!email.includes("@")){
            setMensagem("Email inválido");
            setSucesso(false);
            return;
        }
        if(!senha.trim()){
            setMensagem("Senha é obrigatório");
            setSucesso(false);
            return;
        }
        if(!tipo){
            setMensagem("Selecione o tipo de conta");
            setSucesso(false);
            return;
        }
        if(isNaN(saldoInicial) || saldoInicial < 0 ){
            setMensagem("Saldo inicial inválido");
            setSucesso(false);
            return;
        }
        const emailExistente = await BuscarUsuarioPorEmail(email);
        if(!emailExistente.erro) {
            setMensagem("Email já cadastrado");
            setSucesso(false);
            return;
        }

        const resposta = await Registrar(email, senha, titular, saldoInicial, tipo);
        if (resposta.erro) {
            setMensagem(resposta.erro);
            setSucesso(false);
        } else {
            localStorage.setItem("usuario", JSON.stringify(resposta));
            setMensagem(resposta.mensagem);
            setSucesso(true);
            setTimeout(() => {
                navigate("/");
            }, 2000);
        }
    }

    return (
        <div className="register-page">
            <div className="register-card">
                <Link to="/" className="nav-link">← Home</Link>
                <h2>Criar Conta</h2>
                <div className="register-form">
                    <input
                        type="text"
                        placeholder="Digite seu nome"
                        onChange={(e) => setTitular(e.target.value)}
                    />
                    <input
                        type="email"
                        placeholder="Digite seu Email"
                        onChange={(e) => setEmail(e.target.value)}
                    />
                    <select className="select-tipo" onChange={(e) => setTipo(e.target.value)}>
                        <option value="">Selecione o tipo de conta</option>
                        <option value="corrente">Conta Corrente</option>
                        <option value="poupanca">Conta Poupança (rende 1% a cada 20s)</option>
                    </select>
                    <input
                        type="password"
                        placeholder="Digite sua Senha"
                        onChange={(e) => setSenha(e.target.value)}
                    />
                    <input
                        type="number"
                        placeholder="Saldo inicial"
                        onChange={(e) => setSaldoInicial(Number(e.target.value))}
                    />
                    <button onClick={handleRegistrar}>Registrar</button>
                </div>
                {mensagem && <p className={sucesso ? "msg-sucesso" : "msg-erro"}>{mensagem}</p>}
            </div>
        </div>
    );
};