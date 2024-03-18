using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CategoriasController(AppDbContext context, IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        //[HttpGet("LerArquivoConfiguracao")]
        //public string GetValores()
        //{
        //    var valor1 = _configuration["chave1"];
        //    var valor2 = _configuration ["chave2"];
        //    var secao1 = _configuration ["secao1:chave2"];

        //    return $"Chave1 = {valor1} \nChave2 = {valor2} \nSeção1 => Chave2 = {secao1}";
        //}

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            _logger.LogInformation("=======================GET api/categorias/produtos =============================");

            return _context.Categorias.Include(p => p.Produtos).ToList();
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]   
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            try
            {
                _logger.LogInformation("=======================GET api/categorias =============================");
                return _context.Categorias.AsNoTracking().ToList();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            try
            {                
                var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

                _logger.LogInformation($"=======================GET api/categorias/id = {id} =============================");

                if (categoria is null)
                {
                    _logger.LogInformation($"=======================GET api/categorias/id = {id} NOT FOUND =============================");
                    return NotFound("Categoria não encontrada!");
                }
                return categoria;
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria is null)
                return BadRequest();

            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return new CreatedAtRouteResult("Obtercategoria", new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                return BadRequest();
            }

            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound("categoria não localizado...");
            }
            _context.Categorias.Remove(categoria);
            _context.SaveChanges();

            return Ok(categoria);
        }
    }
}
