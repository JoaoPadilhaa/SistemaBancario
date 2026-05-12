namespace BankSystem.Models;

public class Usuarios
{   //atributos da classe
    private int id;
    private string email;
    private string senha;
    private int contaId;

    //constructor
    public Usuarios(int id, string email, string senha, int contaId)
    {   
        this.id = id;
        this.email = email;
        this.senha = senha;
        this.contaId = contaId;

    }

    //acesso
    public int Id  {get => id; set => id = value;}
    public string Email  { get => email; set => email = value;}
    public string Senha { get => senha; private set => senha = value;}
    public int ContaId { get => contaId; set => contaId = value;}
}