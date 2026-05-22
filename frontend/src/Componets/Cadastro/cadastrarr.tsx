import { useState } from "react";
import { Provisionar } from "../../services/api";
import "./cadastrarr.css";

function Cadastrarr() {
    const [titular, setTitular] = useState<string>("");
    const [tipo, setTipo] = useState<string>("");
    const [saldoInicial, setSaldoInicial] = useState<number>(0);

    const email = localStorage.getItem("keycloak_email") ?? "";

    async function handleCadastrar() {
        const resposta = await Provisionar(email, titular, tipo, saldoInicial)
        if(!resposta.erro) {
            localStorage.setItem("usuario", JSON.stringify(resposta));
            localStorage.removeItem("keycloak_email");
            window.location.href = "/";
        }
    }

    return (
        <div className="cadastro-page">
            <div className="cadastro-card">
                <div className="cadastro-header">
                    <div className="cadastro-icon">🏦</div>
                    <h2>Criar Conta Bancária</h2>
                    <p className="cadastro-email">{email}</p>
                </div>

                <div className="cadastro-form">
                    <input
                        type="text"
                        placeholder="Nome do titular"
                        onChange={(e) => setTitular(e.target.value)}
                    />
                    <select className="select-tipo" onChange={(e) => setTipo(e.target.value)}>
                        <option value="">Selecione o tipo de conta</option>
                        <option value="poupanca">Poupança (rende 1% a cada 20s)</option>
                        <option value="corrente">Conta Corrente</option>
                    </select>
                    <input
                        type="number"
                        placeholder="Saldo inicial"
                        onChange={(e) => setSaldoInicial(Number(e.target.value))}
                    />
                    <button type="button" onClick={handleCadastrar}>Criar Conta</button>
                </div>
            </div>
        </div>
    )
}

export default Cadastrarr;