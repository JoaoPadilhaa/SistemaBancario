# Sistema Bancário

Sistema bancário completo com autenticação via Keycloak, operações financeiras, Pix, histórico de transações e perfil de usuário.

Desenvolvido para estudo de C# puro (sem frameworks) no backend, React com TypeScript no frontend, Keycloak para autenticação e Docker para orquestração dos serviços.

## Tecnologias

**Backend:**
- C# (.NET 8) — servidor HTTP puro com `HttpListener`
- MySQL — banco de dados relacional
- MySql.Data — pacote de conexão com o banco
- Microsoft.IdentityModel.Tokens — validação de JWT
- Padrão Controller — handlers organizados por domínio

**Frontend:**
- React 18 + TypeScript
- React Router DOM — navegação e proteção de rotas
- keycloak-js — integração com Keycloak
- Vite — build tool

**Infraestrutura:**
- Docker + Docker Compose
- Keycloak 24 — servidor de autenticação (OAuth2/OpenID Connect)
- MySQL 8 — banco do Keycloak (separado do banco da aplicação)

## Funcionalidades

- Autenticação via Keycloak (OAuth2/OpenID Connect)
- Validação de token JWT no backend usando chaves públicas do Keycloak
- Provisionamento automático de conta bancária no primeiro login
- Dois tipos de conta: **Corrente** e **Poupança**
- Poupança com rendimento automático de 1% a cada 20 segundos
- Saldo atualizado em tempo real
- Depósito e saque com validações
- Transferência entre contas por ID
- **Pix** — transferência por email com busca de destinatário
- Histórico de transações por conta
- Tela de perfil com avatar e alteração de senha
- Hash de senha com SHA256
- Proteção de todas as rotas da API com token JWT

## Arquitetura

```
Frontend (React) → Keycloak (autenticação)
Frontend (React) → Backend C# (operações bancárias)
Backend C# → valida token JWT com chaves públicas do Keycloak
Backend C# → MySQL (dados bancários)
Keycloak → MySQL separado (dados de autenticação)
```

## Estrutura do Projeto

```
backend/
├── Program.cs                    — Ponto de entrada + timer de rendimento
├── Models/
│   ├── ContaBancaria.cs          — Modelo da conta
│   ├── Usuarios.cs               — Modelo do usuário
│   └── Transacao.cs              — Modelo de transação
├── Database/
│   └── BancoDeDados.cs           — Conexão e queries MySQL
└── Server/
    ├── HttpServer.cs             — Roteador HTTP + validação JWT
    └── Controllers/
        ├── ContaController.cs    — Depositar, sacar, transferir, listar, buscar
        ├── UsuarioController.cs  — Provisionar, buscar por email, alterar senha
        ├── TransacaoController.cs — Listar transações
        └── PixController.cs      — Enviar Pix por email

frontend/
├── src/
│   ├── keycloak.ts               — Configuração do Keycloak
│   ├── UserContext.tsx           — Contexto global do usuário
│   ├── services/api.ts           — Funções de chamada à API (com token JWT)
│   ├── Componets/
│   │   ├── home/                 — Tela principal
│   │   ├── Cadastro/             — Completar cadastro (primeiro login)
│   │   ├── Perfill/              — Perfil e alteração de senha
│   │   ├── pixx/                 — Pix por email
│   │   ├── transacoes/           — Histórico de transações
│   │   └── ProtectedRoute/       — Proteção de rotas
│   └── Pages/                    — Wrappers de página
└── index.html

database/
└── schema.sql                    — Script de criação do banco

docker-compose.yml                — MySQL + Keycloak em containers
```

## Como Rodar

### Pré-requisitos
- .NET 8 SDK
- Node.js
- Docker Desktop

### 1. Subir os containers (MySQL + Keycloak)

```bash
docker compose up -d
```

Aguarda ~60 segundos e acessa `http://localhost:8080`.

### 2. Configurar o Keycloak

1. Loga com `admin` / `admin1234`
2. Cria realm: `banco-sistema`
3. Cria client: `banco-frontend` (OpenID Connect, Direct access grants ON, redirect `http://localhost:3000/*`)
4. Em **Realm settings → Login**: habilita **User registration**

### 3. Banco de dados da aplicação

Conecta no MySQL local (porta 3306) e executa o `database/schema.sql`.

### 4. Backend

```bash
cd backend
dotnet add package MySql.Data
dotnet add package Microsoft.IdentityModel.Tokens
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet run
```

O servidor inicia em `http://localhost:5000`.

> Ajuste a connection string no `Program.cs` com seu usuário e senha do MySQL.

### 5. Frontend

```bash
cd frontend
npm install
npm run dev
```

O frontend inicia em `http://localhost:3000` e redireciona automaticamente para o login do Keycloak.

## Fluxo de Autenticação

```
1. Usuário acessa http://localhost:3000
2. Keycloak redireciona para tela de login
3. Após login, Keycloak retorna token JWT
4. Frontend busca conta bancária pelo email do token
5. Se não tem conta → tela de completar cadastro
6. Se tem conta → home com dados e operações
7. Todas as requisições ao backend incluem o token JWT
8. Backend valida o token com chaves públicas do Keycloak
```

## Rotas da API

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | /api/provisionar | Cria conta bancária no primeiro login |
| PATCH | /api/perfil | Altera senha do usuário |
| GET | /api/contas | Lista todas as contas |
| GET | /api/contas/{id} | Busca conta por ID |
| POST | /api/contas/{id}/depositar | Deposita valor |
| POST | /api/contas/{id}/sacar | Saca valor |
| POST | /api/contas/transferir | Transfere entre contas por ID |
| GET | /api/contas/{id}/transacoes | Lista transações da conta |
| GET | /api/usuarios/buscar?email= | Busca usuário por email |
| POST | /api/pix | Transfere por email (Pix) |

> Todas as rotas exigem token JWT válido no header `Authorization: Bearer <token>`
