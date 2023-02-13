using Chapter.Controllers;
using Chapter.Interfaces;
using Chapter.Models;
using Chapter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteChapter.Controller
{
    public class LoginControllerTeste
    {
        [Fact]
        public void LoginController_Retornar_Usuario_Invalido()
        {
            //Arrange - Preparação
            //Cria o repositório espelhado/fake passando a interface que queremos espelhar
            var repositoryEspelhado = new Mock<IUsuarioRepository>();

            //Configurando o repositório espelhando e falando o que que ele tem que retornar
            //O retorno do usuario falso vai ser nulo
            repositoryEspelhado.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).Returns((Usuario)null);

            //Acessando os métodos do controller sem ser pelo navegador, fazemos uma instância do objeto Login passando o repository dele
            var controller = new LoginController(repositoryEspelhado.Object);

            //fazendo um objeto para passar para o método login que pede uma estrutura de dados específica
            LoginViewModel dadosUsuario = new LoginViewModel();
            dadosUsuario.email = "batata@email.com";
            dadosUsuario.senha = "batata";

            //Act - Ação
            //passando os dados para o método login
            var resultado = controller.Login(dadosUsuario);

            //Assert - Verificação
            //verificando se o resultado é do tipo não autorizado, passando o resultado para ser verificado
            Assert.IsType<UnauthorizedObjectResult>(resultado);
        }

        [Fact]
        public void LoginController_Retorna_Token()
        {
            //Arrange - Preparação
            //Criando um objeto do tipo usuário
            Usuario usuarioRetornado = new Usuario();
            usuarioRetornado.Email = "email@email.com";
            usuarioRetornado.Senha = "1234";
            usuarioRetornado.Tipo = "0";
            usuarioRetornado.Id = 1;

            var repositoryEspelhado = new Mock<IUsuarioRepository>();

            repositoryEspelhado.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).Returns(usuarioRetornado);

            //O método de login não vai comparar as informações do usuário com o usuário retornado
            //fazendo um objeto para passar para o método login que pede uma estrutura de dados específica
            LoginViewModel dadosUsuario = new LoginViewModel();
            dadosUsuario.email = "batata@email.com";
            dadosUsuario.senha = "batata";

            //Acessando os métodos do controller sem ser pelo navegador, fazemos uma instância do objeto Login passando o repository dele
            var controller = new LoginController(repositoryEspelhado.Object);

            //Criando a variavel para fazer a comparação do nosso token
            string issuerValido = "chapter.webapi";

            //Act - Ação
            //O valor do issue vem criptografado então vamos fazer a descriptografia para poder comparar o valor do Issuer(token)

            //Armazenando o resultado em um OkObjectResult para pegar o valor do token
            OkObjectResult resultado = (OkObjectResult)controller.Login(dadosUsuario);

            //Pegando a sequência de caracteres o rest do token
            //token = 'aiusdhaiudfa.iaushdiuahyd.'
            string tokenString = resultado.Value.ToString().Split(' ')[3];

            //Fazendo o processo de decriptação o token
            //Instanciamos um objeto da biblioteca JWT (como é usado para fazer o token)
            var jwtHandler = new JwtSecurityTokenHandler();

            var tokenJwt = jwtHandler.ReadJwtToken(tokenString);

            //Assert - Verificação
            Assert.Equal(issuerValido, tokenJwt.Issuer);
        }
    }
}
