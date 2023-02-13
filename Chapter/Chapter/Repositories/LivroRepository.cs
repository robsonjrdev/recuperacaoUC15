using Chapter.Contexts;
using Chapter.Models;
using Microsoft.EntityFrameworkCore;

namespace Chapter.Repositories
{
    public class LivroRepository
    {
        private readonly ChapterContext _context;

        public LivroRepository(ChapterContext chapterContext)
        {
            _context = chapterContext;
        }

        public List<Livro> Listar()
        {
            return _context.Livros.ToList();
        }

        public Livro BuscarPorId(int id)
        {
            return _context.Livros.Find(id);
        }

        public void Cadastrar(Livro l)
        {
            _context.Livros.Add(l);

            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            Livro l = _context.Livros.Find(id);

            _context.Livros.Remove(l);

            _context.SaveChanges();
        }

        public void Alterar(int id, Livro l)
        {
            Livro livroBuscado = _context.Livros.Find(id);

            if (livroBuscado != null)
            {
                livroBuscado.Titulo = l.Titulo;
                livroBuscado.QuantidadePaginas = l.QuantidadePaginas;
                livroBuscado.Disponivel = l.Disponivel;

                _context.Livros.Update(livroBuscado);

                _context.SaveChanges();
            }
        }
    }
}
