namespace apiAutenticacao.Models.Response
{
    public class ResponseAlterar
    {

        public string Erro { get; set; }
        public string Message { get; set; } = string.Empty;
        public Usuario? Usuario { get; set; } = new Usuario();  

    }
}
