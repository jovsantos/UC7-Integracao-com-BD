using apiAutenticacao.Data;
using apiAutenticacao.Models;
using apiAutenticacao.Models.DTO;
using apiAutenticacao.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;

namespace apiAutenticacao.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;


        public AuthService(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<ResponseLogin> Login(LoginDTO dadosUsuario)
        {


            Usuario? usuarioEncontrado = await _context.Usuarios.
               FirstOrDefaultAsync(usuario => usuario.Email == dadosUsuario.Email);

            if (usuarioEncontrado != null)
            {
                bool isValidPassword = Verify(dadosUsuario.Senha, usuarioEncontrado.Senha);

                if (isValidPassword)
                {
                    return new ResponseLogin
                    {
                        Erro = false,
                        Message = "Login realizado com sucesso!",
                        Usuario = usuarioEncontrado
                    };
                }
                return new ResponseLogin
                {
                    Erro = true,
                    Message = "Senha inválida!",
                };
            }
                return new ResponseLogin
                {
                    Erro = true,
                    Message = "Usuário não encontrado!",

                };
        }


        public async Task<ResponseCadastro> CadastrarUsuarioAsync(CadastroUsuarioDTO dadosUsuarioCadastro)
        {
            Usuario? usuarioExistente = await _context.Usuarios.

              FirstOrDefaultAsync(usuario => usuario.Email == dadosUsuarioCadastro.Email);

            if (usuarioExistente != null)

            {
                return new ResponseCadastro
                {
                    Erro = true,
                    Message = "Este email ja esta cadastrado no sistema!",
                };

            }

            Usuario Usuario = new Usuario

            {

                Nome = dadosUsuarioCadastro.Nome,
                Email = dadosUsuarioCadastro.Email,
                Senha = HashPassword(dadosUsuarioCadastro.Senha),
                ConfirmarSenha = HashPassword(dadosUsuarioCadastro.ConfirmarSenha)
            };

            _context.Usuarios.Add(Usuario);
            await _context.SaveChangesAsync();

            return new ResponseCadastro
            {
                Erro = false,
                Message = "Usuário cadastrado com sucesso!",
                Usuario = Usuario
            };
            
        }

        public async Task<ResponseAlterar>AlterarSenhaAsync(AlterarSenhaDTO dadosAlterarSenhaDTO)
        {
            Usuario? usuarioExistente = await _context.Usuarios.

               FirstOrDefaultAsync(usuario => usuario.Email == dadosAlterarSenhaDTO.Email);

            if (usuarioExistente == null)
            {
                return new ResponseAlterar
                {
                  Erro = true,
                   Message = "Usuário não encontrado!",

                };
            }


        }
        
    }
}
