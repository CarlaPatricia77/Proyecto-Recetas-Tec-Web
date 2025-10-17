using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recetas.Core.Interfaces;
using Recetas.Core.Entities;
using Recetas.Infrastructure.DTOs;

namespace Recetas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecetaController : ControllerBase
    {
        private readonly IRecetasRepository _recetasRepo;
        private readonly IUsuarioRepository _usuariosRepo;
        private readonly ICategoriasRepository _categoriasRepo;
        private readonly IMapper _mapper;

        public RecetaController(
            IRecetasRepository recetasRepo,
            IUsuarioRepository usuariosRepo,
            ICategoriasRepository categoriasRepo,
            IMapper mapper)
        {
            _recetasRepo = recetasRepo;
            _usuariosRepo = usuariosRepo;
            _categoriasRepo = categoriasRepo;
            _mapper = mapper;
        }

        // ==========================
        //        RECETAS
        // ==========================

        // GET /api/receta
        [HttpGet]
        public async Task<IActionResult> GetRecetas()
        {
            var recetas = await _recetasRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }

        // GET /api/receta/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRecetaById(int id)
        {
            var receta = await _recetasRepo.GetByIdAsync(id);
            return receta is null
                ? NotFound($"No se encontró la receta con id {id}.")
                : Ok(_mapper.Map<RecetaDto>(receta));
        }

        // POST /api/receta
        [HttpPost]
        public async Task<IActionResult> CreateReceta([FromBody] RecetaDto dto)
        {
            var entity = _mapper.Map<Receta>(dto);
            await _recetasRepo.InsertAsync(entity);
            var result = _mapper.Map<RecetaDto>(entity);
            return CreatedAtAction(nameof(GetRecetaById), new { id = result.Id }, result);
        }

        // PUT /api/receta/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateReceta(int id, [FromBody] RecetaDto dto)
        {
            if (id != dto.Id) return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            var existing = await _recetasRepo.GetByIdAsync(id);
            if (existing is null) return NotFound($"No existe una receta con id {id}.");

            _mapper.Map(dto, existing);
            await _recetasRepo.UpdateAsync(existing);
            return Ok(_mapper.Map<RecetaDto>(existing));
        }

        // DELETE /api/receta/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReceta(int id)
        {
            var receta = await _recetasRepo.GetByIdAsync(id);
            if (receta is null) return NotFound($"No se encontró la receta con id {id}.");

            await _recetasRepo.DeleteAsync(receta);
            return NoContent();
        }


        // GET /api/receta/por-usuario/{usuarioId}
        [HttpGet("por-usuario/{usuarioId:int}")]
        public async Task<IActionResult> GetRecetasByUsuario(int usuarioId)
        {
            var recetas = await _recetasRepo.GetByUsuarioAsync(usuarioId);
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }
        // ==========================
        //   CASOS DE USO DE NEGOCIO
        // ==========================

        // GET /api/receta/por-categoria/{categoriaId}
        [HttpGet("por-categoria/{categoriaId:int}")]
        public async Task<IActionResult> GetRecetasByCategoria(int categoriaId)
        {
            var recetas = await _recetasRepo.GetByCategoriaAsync(categoriaId);
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }


        // GET /api/receta/buscar-por-ingrediente/{ingrediente}
        [HttpGet("buscar-por-ingrediente/{ingrediente}")]
        public async Task<IActionResult> BuscarPorIngrediente(string ingrediente)
        {
            var recetas = await _recetasRepo.BuscarPorIngredienteAsync(ingrediente);
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }




        // ==========================
        //        USUARIOS
        // ==========================
        // (Se usan rutas absolutas con "~" para no heredar /api/receta)

        // GET /api/usuario
        [HttpGet("~/api/usuario")]
        public async Task<IActionResult> GetUsuarios()
        {
            var users = await _usuariosRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<UsuarioDto>>(users));
        }

        // GET /api/usuario/{id}
        [HttpGet("~/api/usuario/{id:int}")]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            var user = await _usuariosRepo.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(_mapper.Map<UsuarioDto>(user));
        }

        // POST /api/usuario
        [HttpPost("~/api/usuario")]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioDto dto)
        {
            var entity = _mapper.Map<Usuario>(dto);
            await _usuariosRepo.InsertAsync(entity);
            var result = _mapper.Map<UsuarioDto>(entity);
            return CreatedAtAction(nameof(GetUsuarioById), new { id = result.Id }, result);
        }

        // PUT /api/usuario/{id}
        [HttpPut("~/api/usuario/{id:int}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            var existing = await _usuariosRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            _mapper.Map(dto, existing);
            await _usuariosRepo.UpdateAsync(existing);
            return Ok(_mapper.Map<UsuarioDto>(existing));
        }

        // DELETE /api/usuario/{id}
        [HttpDelete("~/api/usuario/{id:int}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var existing = await _usuariosRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            await _usuariosRepo.DeleteAsync(existing);
            return NoContent();
        }

        // ==========================
        //        CATEGORÍAS
        // ==========================

        // GET /api/categoria
        [HttpGet("~/api/categoria")]
        public async Task<IActionResult> GetCategorias()
        {
            var cats = await _categoriasRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CategoriaDto>>(cats));
        }

        // GET /api/categoria/{id}
        [HttpGet("~/api/categoria/{id:int}")]
        public async Task<IActionResult> GetCategoriaById(int id)
        {
            var cat = await _categoriasRepo.GetByIdAsync(id);
            return cat is null ? NotFound() : Ok(_mapper.Map<CategoriaDto>(cat));
        }

        // POST /api/categoria
        [HttpPost("~/api/categoria")]
        public async Task<IActionResult> CreateCategoria([FromBody] CategoriaDto dto)
        {
            var entity = _mapper.Map<Categoria>(dto);
            await _categoriasRepo.InsertAsync(entity);
            var result = _mapper.Map<CategoriaDto>(entity);
            return CreatedAtAction(nameof(GetCategoriaById), new { id = result.Id }, result);
        }

        // PUT /api/categoria/{id}
        [HttpPut("~/api/categoria/{id:int}")]
        public async Task<IActionResult> UpdateCategoria(int id, [FromBody] CategoriaDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            var existing = await _categoriasRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            _mapper.Map(dto, existing);
            await _categoriasRepo.UpdateAsync(existing);
            return Ok(_mapper.Map<CategoriaDto>(existing));
        }

        // DELETE /api/categoria/{id}
        [HttpDelete("~/api/categoria/{id:int}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var existing = await _categoriasRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            await _categoriasRepo.DeleteAsync(existing);
            return NoContent();
        }
    }
}