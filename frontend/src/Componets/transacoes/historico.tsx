import { useEffect, useState } from "react";
import { ListarTransacoes } from "../../services/api";
import { Link } from "react-router-dom";
import "./historico.css";

function HistoricTransacoes (){
    const [transacoes, setTransacoes] = useState<any[]>([]);

    useEffect(() => {
        const dados = localStorage.getItem("usuario");
        if (dados){
            const usuario = JSON.parse(dados);
            ListarTransacoes(usuario.id).then(data => {
                setTransacoes(data);
            });
        }
    }, []);

    return (
        <div className="historico-page">
            <div className="historico-card">
                <Link to="/" className="nav-link">← Voltar</Link>
                <h2>Histórico de Transações</h2>
                {transacoes.length === 0 && <p className="sem-transacoes">Nenhuma transação encontrada.</p>}
                <div className="transacoes-lista">
                    {transacoes.map((t) => (
                        <div className={`transacao-item ${t.Tipo}`} key={t.Id}>
                            <div className="transacao-tipo">{t.Tipo}</div>
                            <div className="transacao-valor">R$ {t.Valor.toFixed(2)}</div>
                            <div className="transacao-data">{new Date(t.Data).toLocaleString()}</div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    )
}

export default HistoricTransacoes;