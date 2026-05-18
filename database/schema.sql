-- Criação do banco de dados
CREATE DATABASE IF NOT EXISTS banco_sistema;
USE banco_sistema;

-- Tabela de contas bancárias
CREATE TABLE contas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    titular VARCHAR(100) NOT NULL,
    saldo DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    tipo VARCHAR(20) NOT NULL DEFAULT 'corrente'  -- 'corrente' ou 'poupanca'
);

-- Tabela de usuários
CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(100) NOT NULL UNIQUE,
    senha VARCHAR(255) NOT NULL,
    conta_id INT,
    FOREIGN KEY (conta_id) REFERENCES contas(id)
);

-- Tabela de transações
CREATE TABLE transacoes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    conta_id INT NOT NULL,
    tipo VARCHAR(20) NOT NULL,  -- 'deposito', 'saque', 'transferencia', 'pix', 'pix recebido'
    valor DECIMAL(18, 2) NOT NULL,
    data_hora DATETIME NOT NULL DEFAULT NOW(),
    FOREIGN KEY (conta_id) REFERENCES contas(id)
);

-- Dados de exemplo (opcional, pode remover)
-- INSERT INTO contas (titular, saldo, tipo) VALUES ('Maria Silva', 1500.00, 'corrente');
-- INSERT INTO usuarios (email, senha, conta_id) VALUES ('maria@email.com', '1234', 1);
