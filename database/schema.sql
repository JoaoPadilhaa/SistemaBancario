-- Criação do banco de dados
CREATE DATABASE IF NOT EXISTS banco_sistema;
USE banco_sistema;

-- Tabela de contas bancárias
CREATE TABLE contas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    titular VARCHAR(100) NOT NULL,
    saldo DECIMAL(18, 2) NOT NULL DEFAULT 0.00
);

-- Dados de exemplo (opcional, pode remover)
INSERT INTO contas (titular, saldo) VALUES
('Maria Silva', 1500.00),
('João Santos', 300.50);
