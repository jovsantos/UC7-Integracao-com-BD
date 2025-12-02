using apiAutenticacao.Data;
using apiAutenticacao.Models;
using apiAutenticacao.Models.DTO;
using apiAutenticacao.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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

        public async Task<ResponseAlterar> AlterarSenhaAsync(AlterarSenhaDTO dadosAlterarSenhaDTO)
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

            bool isValidPassword = Verify(dadosAlterarSenhaDTO.SenhaAtual, usuarioExistente.Senha);

            if (!isValidPassword)
            {
                return new ResponseAlterar
                {
                    Erro = true,
                    Message = "Senha atual inválida!",
                };
            }


            if (dadosAlterarSenhaDTO.NovaSenha != dadosAlterarSenhaDTO.ConfirmarSenha)
            {
                return new ResponseAlterar
                {
                    Erro = true,
                    Message = "A nova senha e a confirmação de senha não conferem!",
                };
            }

            usuarioExistente.Senha = HashPassword(dadosAlterarSenhaDTO.NovaSenha);
            

            Usuario usuario = new Usuario()
            {
                Email = dadosAlterarSenhaDTO.Email,
                Senha = HashPassword(dadosAlterarSenhaDTO.SenhaAtual),
            };

            _context.Usuarios.Update(usuarioExistente);
            await _context.SaveChangesAsync();

            return new ResponseAlterar
            {
                Erro = false,
                Message = "Senha alterada com sucesso!",
                Usuario = usuarioExistente

            };

        }
        
        
    }
}
