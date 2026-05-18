const API_URL = "http://localhost:5000";

export async function GetContasBancarias() {
    const response = await fetch(`${API_URL}/api/contas`);

    return response.json();
}

//requisição post com o valor de deposito
export async function Depositar(id:number, valor:number) {
    const response = await fetch (`${API_URL}/api/contas/${id}/depositar`, {
        method : "POST",
        headers: {'Content-type':'application/json'},
        body: JSON.stringify({valor})
    });
    return response.json();
}   

//Requisição post com o valor que vai ser sacado, envia para api e espera acontecer
export async function Sacar (id:number, valor:number){
    const response = await fetch (`${API_URL}/api/contas/${id}/sacar`, {
        method: "POST",
        headers: { 'Content-Type': 'application/json'},
        body: JSON.stringify({valor})
    })
    return response.json();
}

//requisição post para criar conta
export async function CriarConta(titular:string, valor:number){
    const response = await fetch (`${API_URL}/api/contas`, {
        method: "POST",
        headers: {'Content-type':'application/json'},
        body: JSON.stringify({titular, saldoInicial: valor})
    })

    return response.json();       
}

//requisição post para transferir saldo
export async function Transferir(idOrigem:number, idDestino:number, valor:number){
    const response = await fetch(`${API_URL}/api/contas/transferir`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body:JSON.stringify({idOrigem, idDestino, valor})
    });
    return response.json();
}

//requisição post para registrar
export async function Registrar(email:string, senha:string, titular:string, saldoInicial:number, tipo:string)
{
    const response = await fetch(`${API_URL}/api/registrar`,{
        method: "POST",
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({email, senha,titular,saldoInicial, tipo})
        
    });
    return response.json();
}

//requisição post para logar
export async function Login(email:string, senha: string){
    const response = await fetch(`${API_URL}/api/login`,{
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({email, senha})
    });
    return response.json();
}

//requisição get para listar todas transações
export async function ListarTransacoes(contaId:number)
{
    const response = await fetch(`${API_URL}/api/contas/${contaId}/transacoes`);
    return response.json();
}

//requisição get para buscar saldo atualizado(para contas poupança, que ficam rendendo)
export async function BuscarSaldo(contaId:number) {
    const response = await fetch (`${API_URL}/api/contas/${contaId}`);
    return response.json();
}

//requisição get para buscar usuario por email
export async function BuscarUsuarioPorEmail(email:string)
{   
    //EncodeURICompoente garante que o email enviado, seja enviado exatamente como foi escrito
    const response = await fetch(`${API_URL}/api/usuarios/buscar?email=${encodeURIComponent(email)}`);
    return response.json();
}

//requisição post para enviar o pix
export async function EnviarPix(idOrigem: number, emailDestino: string, valor:number)
{
    const response = await fetch(`${API_URL}/api/pix`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({idOrigem, emailDestino, valor})
    });
    return response.json();
}

export async function AlterarSenha(email:string, senhaAtual:string, novaSenha:string)
{
    const response = await fetch(`${API_URL}/api/perfil`, {
        method: "PATCH",
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({email, senhaAtual, novaSenha})
    });
    return response.json();
}