# Sistema Bancário

Sistema bancário completo com autenticação, operações financeiras, Pix, histórico de transações e perfil de usuário.

Desenvolvido para estudo de C# puro (sem frameworks) no backend e React com TypeScript no frontend.

## Tecnologias

**Backend:**
- C# (.NET 8) — servidor HTTP puro com `HttpListener`
- MySQL — banco de dados relacional
- MySql.Data — pacote de conexão com o banco
- Padrão Controller — handlers organizados por domínio

**Frontend:**
- React 18 + TypeScript
- React Router DOM — navegação e proteção de rotas
- Vite — build tool

## Funcionalidades

- Registro de usuário com validação de email duplicado
- Login com email e senha
- Proteção de rotas (redireciona para login se não autenticado)
- Dois tipos de conta: **Corrente** e **Poupança**
- Poupança com rendimento automático de 1.01% a cada 20 segundos
- Saldo atualizado em tempo real no frontend
- Depósito e saque com validações
- Transferência entre contas por ID
- **Pix** — transferência por email com busca de destinatário
- Histórico de transações por conta
- Tela de perfil com avatar e alteração de senha
- Validações de campos obrigatórios em todos os formulários

## Estrutura do Projeto

```
backend/
├── Program.cs                    — Ponto de entrada + timer de rendimento
├── Models/
│   ├── ContaBancaria.cs          — Modelo da conta (tipo, saldo, depositar, sacar)
│   ├── Usuarios.cs               — Modelo do usuário
│   └── Transacao.cs              — Modelo de transação
├── Database/
│   └── BancoDeDados.cs           — Conexão e queries MySQL
└── Server/
    ├── HttpServer.cs             — Roteador HTTP (CORS, rotas, dispatch)
    └── Controllers/
        ├── ContaController.cs    — Depositar, sacar, transferir, listar, buscar
        ├── UsuarioController.cs  — Login, registrar, buscar por email, alterar senha
        ├── TransacaoController.cs — Listar transações
        └── PixController.cs      — Enviar Pix por email

frontend/
├── src/
│   ├── services/api.ts           — Funções de chamada à API
│   ├── Componets/
│   │   ├── home/                 — Tela principal (saldo, operações)
│   │   ├── Login/                — Tela de login
│   │   ├── Create/               — Tela de registro
│   │   ├── Perfill/              — Tela de perfil e alteração de senha
│   │   ├── pixx/                 — Tela de Pix
│   │   ├── transacoes/           — Histórico de transações
│   │   └── ProtectedRoute/       — Componente de proteção de rotas
│   └── Pages/                    — Wrappers de página
└── index.html

database/
└── schema.sql                    — Script de criação do banco
```

## Como Rodar

### Pré-requisitos
- .NET 8 SDK
- Node.js
- MySQL

### 1. Banco de Dados

```sql
CREATE DATABASE IF NOT EXISTS banco_sistema;
USE banco_sistema;

CREATE TABLE contas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    titular VARCHAR(100) NOT NULL,
    saldo DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    tipo VARCHAR(20) NOT NULL DEFAULT 'corrente'
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
| PATCH | /api/perfil | Altera senha do usuário |
| GET | /api/contas | Lista todas as contas |
| POST | /api/contas | Cria conta avulsa |
| GET | /api/contas/{id} | Busca conta por ID |
| POST | /api/contas/{id}/depositar | Deposita valor |
| POST | /api/contas/{id}/sacar | Saca valor |
| POST | /api/contas/transferir | Transfere entre contas por ID |
| GET | /api/contas/{id}/transacoes | Lista transações da conta |
| GET | /api/usuarios/buscar?email= | Busca usuário por email |
| POST | /api/pix | Transfere por email (Pix) |
