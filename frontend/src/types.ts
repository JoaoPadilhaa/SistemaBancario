// DICA: Defina aqui os tipos TypeScript que representam os dados.
// Isso garante tipagem forte no frontend.

// TODO: Interface que representa uma conta bancária
// export interface ContaBancaria {
//   id: number;
//   titular: string;
//   saldo: number;
// }

// TODO: Interface para o body de criação de conta
// export interface CriarContaRequest {
//   titular: string;
//   saldoInicial: number;
// }

// TODO: Interface para o body de depósito/saque
// export interface OperacaoRequest {
//   valor: number;
// }

export interface ContaBancaria
{
    Id: number;
    Titular: string;
    Saldo: number;
}

export interface CriarContaRequest
{
    Titular: string;
    SaldoInicial: number;
}
