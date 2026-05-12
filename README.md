# Sistema Bancário

Sistema bancário simples com autenticação de usuários, operações financeiras e histórico de transações.

Projeto desenvolvido para estudo de C# puro (sem frameworks) no backend e React com TypeScript no frontend.

## Tecnologias

**Backend:**
- C# (.NET 8) — servidor HTTP puro com `HttpListener`
- MySQL — banco de dados relacional
- MySql.Data — pacote de conexão com o banco

**Frontend:**
- React 18 + TypeScript
- React Router DOM — navegação entre páginas
- Vite — build tool

## Funcionalidades

- Registro de usuário (cria conta bancária automaticamente)
- Login com email e senha
- Visualização de saldo e dados da conta
- Depósito
- Saque (com validação de saldo)
- Transferência entre contas
- Histórico de transações

## Estrutura do Projeto

```
backend/
├── Program.cs                 — Ponto de entrada
├── Models/
│   ├── ContaBancaria.cs       — Modelo da conta
│   ├── Usuarios.cs            — Modelo do usuário
│   └── Transacao.cs           — Modelo de transação
├── Database/
│   └── BancoDeDados.cs        — Conexão e queries MySQL
└── Server/
    └── HttpServer.cs          — Servidor HTTP e rotas

frontend/
├── src/
│   ├── services/api.ts        — Chamadas à API
│   ├── Componets/
│   │   ├── home/              — Tela principal (logado)
│   │   ├── Login/             — Componente de login
│   │   ├── Create/            — Componente de registro
│   │   └── transacoes/        — Histórico de transações
│   └── Pages/                 — Páginas (wrappers)
└── index.html

database/
└── schema.sql                 — Script de criação do banco
```

## Como Rodar

### Pré-requisitos
- .NET 8 SDK
- Node.js
- MySQL

### 1. Banco de Dados

Crie o banco e as tabelas no MySQL:

```sql
CREATE DATABASE IF NOT EXISTS banco_sistema;
USE banco_sistema;

CREATE TABLE contas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    titular VARCHAR(100) NOT NULL,
    saldo DECIMAL(18, 2) NOT NULL DEFAULT 0.00
);

CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(100) NOT NULL UNIQUE,
    senha VARCHAR(255) NOT NULL,
    conta_id INT,
    FOREIGN KEY (conta_id) REFERENCES contas(id)
);

CREATE TABLE transacoes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    conta_id INT NOT NULL,
    tipo VARCHAR(20) NOT NULL,
    valor DECIMAL(18, 2) NOT NULL,
    data_hora DATETIME NOT NULL DEFAULT NOW(),
    FOREIGN KEY (conta_id) REFERENCES contas(id)
);
```

### 2. Backend

```bash
cd backend
dotnet add package MySql.Data
dotnet run
```

O servidor inicia em `http://localhost:5000`.

> Ajuste a connection string no `Program.cs` com seu usuário e senha do MySQL.

### 3. Frontend

```bash
cd frontend
npm install
npm run dev
```

O frontend inicia em `http://localhost:3000`.

## Rotas da API

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | /api/registrar | Cria usuário + conta |
| POST | /api/login | Autentica usuário |
| GET | /api/contas | Lista todas as contas |
| POST | /api/contas | Cria conta avulsa |
| POST | /api/contas/{id}/depositar | Deposita valor |
| POST | /api/contas/{id}/sacar | Saca valor |
| POST | /api/contas/transferir | Transfere entre contas |
| GET | /api/contas/{id}/transacoes | Lista transações da conta |
