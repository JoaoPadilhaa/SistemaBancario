import "./home.css"
import { useEffect, useState } from "react";
import { Depositar, Sacar, Transferir } from "../../services/api";
import {Link , useNavigate} from "react-router-dom";

export const Home = () => {
    const [valor, setValor] = useState<number>(0);
    const [idDestino, setIdDestino] = useState<number>(0);
    const [usuario, setUsuario] = useState<any>(null);
    const navigate = useNavigate();
  

      useEffect(() => {
        const dados = localStorage.getItem("usuario");
        if(dados) {
          setUsuario(JSON.parse(dados));
        }
      }, []);

      async function handleTransferir() {
        const resposta = await Transferir(usuario.id, idDestino, valor);
        if(resposta.erro) {
          alert(resposta.erro);
          return
        }
        const atualizado = {...usuario, saldo: resposta.saldo};
        localStorage.setItem("usuario", JSON.stringify(atualizado));
        setUsuario(atualizado)
      }

      async function handleDepositar(){
       const resposta = await Depositar(usuario.id, valor);
       const atualizado = {...usuario, saldo: resposta.Saldo};
       localStorage.setItem("usuario", JSON.stringify(atualizado));
       setUsuario(atualizado)
      }

      async function handleSacar() {
        const resposta = await Sacar(usuario.id, valor);
        const atualizado = {...usuario, saldo: resposta.Saldo};
        localStorage.setItem("usuario", JSON.stringify(atualizado));
        setUsuario(atualizado);
      }
      async function handleLogout() {
        localStorage.removeItem("usuario");
        navigate("/login");
      }

      
   return (
    <section className="home">
      {!usuario && (
        <div className="guest-actions">
          <h1>Sistema Bancário</h1>
          <p>Faça login ou crie uma conta para continuar</p>
          <div className="guest-links">
            <Link to="/login" className="nav-link">Login</Link>
            <Link to="/registrar" className="nav-link">Criar nova conta</Link>
          </div>
        </div>
      )}

      {usuario && (
        <>
          <div className="header">
            <p className="greeting">Olá, {usuario.titular}!</p>
            <button className="btn-logout" onClick={handleLogout}>Sair</button>
          </div>

          <div className="conta-card">
            <h2>Minha Conta</h2>
            <div className="conta-info">
              <span className="conta-label">Titular</span>
              <span className="conta-value">{usuario.titular}</span>
            </div>
            
            <div className="conta-info">
              <span className="conta-label">Conta Nº</span>
              <span className="conta-value">{usuario.id}</span>
            </div>
            <div className="conta-saldo">
              <span className="conta-label">Saldo</span>
              <span className="saldo-value">R$ {usuario.saldo?.toFixed(2)}</span>
            </div>
          </div>
          <div className="link-transacoes">
              <Link to="/transacoes" className="nav-link">📋 Histórico de Transações</Link>
          </div>
          <div className="operacoes">
            <h3>Operações</h3>
            <div className="operacao-form">
              <input
                type="number"
                placeholder="Valor"
                onChange={(e) => setValor(Number(e.target.value))}
              />
              <div className="operacao-btns">
                <button className="btn-depositar" onClick={handleDepositar}>Depositar</button>
                <button className="btn-sacar" onClick={handleSacar}>Sacar</button>
              </div>
            </div>
          </div>

          <div className="operacoes">
            <h3>Transferência</h3>
            <div className="operacao-form">
              <input
                type="number"
                placeholder="Conta destino (ID)"
                onChange={(e) => setIdDestino(Number(e.target.value))}
              />
              <input
                type="number"
                placeholder="Valor"
                onChange={(e) => setValor(Number(e.target.value))}
              />
              <button className="btn-transferir" onClick={handleTransferir}>Transferir</button>
            </div>
          </div>
        </>
      )}
    </section>
  );
};