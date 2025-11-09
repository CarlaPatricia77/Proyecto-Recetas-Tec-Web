using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recetas.Core.Interfaces;
using Recetas.Core.Entities;
using Recetas.Infrastructure.DTOs;
using Recetas.Core.QueryFilters;
using Recetas.Infrastructure.Validators;

namespace Recetas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecetaController : ControllerBase
    {
        private readonly IRecetaService _recetaService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public RecetaController(
            IRecetaService recetaService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidationService validationService)
        {
            _recetaService = recetaService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationService = validationService;
        }

        // ==========================
        //        RECETAS CON FILTROS
        // ==========================

        /// <summary>
        /// Obtiene todas las recetas con filtros opcionales y paginación
        /// </summary>
        /// <param name="filters">Filtros de consulta</param>
        /// <returns>Lista de recetas filtradas</returns>
        // GET /api/receta
        [HttpGet]
        public IActionResult GetRecetas([FromQuery] RecetaQueryFilter filters)
        {
            var recetas = _recetaService.GetAllRecetas(filters);

            // Aplicar paginación
            var pagedRecetas = recetas
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize);

            var recetasDto = _mapper.Map<IEnumerable<RecetaDto>>(pagedRecetas);

            return Ok(new
            {
                Data = recetasDto,
                PageNumber = filters.PageNumber,
                PageSize = filters.PageSize,
                TotalRecords = recetas.Count()
            });
        }

        /// <summary>
        /// Obtiene una receta por su ID
        /// </summary>
        /// <param name="id">ID de la receta</param>
        /// <returns>Receta solicitada</returns>
        // GET /api/receta/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRecetaById(int id)
        {
            var receta = await _recetaService.GetRecetaById(id);

            if (receta == null)
                return NotFound($"No se encontró la receta con id {id}.");

            return Ok(_mapper.Map<RecetaDto>(receta));
        }

        /// <summary>
        /// Crea una nueva receta
        /// </summary>
        /// <param name="dto">Datos de la receta</param>
        /// <returns>Receta creada</returns>
        // POST /api/receta
        [HttpPost]
        public async Task<IActionResult> CreateReceta([FromBody] RecetaDto dto)
        {
            var entity = _mapper.Map<Receta>(dto);
            await _recetaService.InsertReceta(entity);

            var result = _mapper.Map<RecetaDto>(entity);
            return CreatedAtAction(nameof(GetRecetaById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Actualiza una receta existente
        /// </summary>
        /// <param name="id">ID de la receta</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Receta actualizada</returns>
        // PUT /api/receta/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateReceta(int id, [FromBody] RecetaDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            var existing = await _unitOfWork.RecetaRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"No existe una receta con id {id}.");

            _mapper.Map(dto, existing);
            _recetaService.UpdateReceta(existing);
            await _unitOfWork.SaveChangesAsync();

            return Ok(_mapper.Map<RecetaDto>(existing));
        }

        /// <summary>
        /// Elimina una receta
        /// </summary>
        /// <param name="id">ID de la receta</param>
        /// <returns>Sin contenido</returns>
        // DELETE /api/receta/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReceta(int id)
        {
            var receta = await _unitOfWork.RecetaRepository.GetByIdAsync(id);
            if (receta == null)
                return NotFound($"No se encontró la receta con id {id}.");

            await _recetaService.DeleteReceta(id);
            return NoContent();
        }

        // ==========================
        //   CASOS DE USO DE NEGOCIO
        // ==========================

        /// <summary>
        /// Obtiene recetas por categoría
        /// </summary>
        // GET /api/receta/por-categoria/{categoriaId}
        [HttpGet("por-categoria/{categoriaId:int}")]
        public async Task<IActionResult> GetRecetasByCategoria(int categoriaId)
        {
            var recetas = await _unitOfWork.RecetaRepository.GetByCategoriaAsync(categoriaId);
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }

        /// <summary>
        /// Obtiene recetas por usuario
        /// </summary>
        // GET /api/receta/por-usuario/{usuarioId}
        [HttpGet("por-usuario/{usuarioId:int}")]
        public async Task<IActionResult> GetRecetasByUsuario(int usuarioId)
        {
            var recetas = await _unitOfWork.RecetaRepository.GetByUsuarioAsync(usuarioId);
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }

        /// <summary>
        /// Busca recetas por ingrediente
        /// </summary>
        // GET /api/receta/buscar-por-ingrediente/{ingrediente}
        [HttpGet("buscar-por-ingrediente/{ingrediente}")]
        public async Task<IActionResult> BuscarPorIngrediente(string ingrediente)
        {
            var recetas = await _unitOfWork.RecetaRepository.BuscarPorIngredienteAsync(ingrediente);
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }

        // ==========================
        //        USUARIOS
        // ==========================

        // GET /api/usuario
        [HttpGet("~/api/usuario")]
        public async Task<IActionResult> GetUsuarios()
        {
            var users = _unitOfWork.UsuarioRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<UsuarioDto>>(users));
        }

        // GET /api/usuario/{id}
        [HttpGet("~/api/usuario/{id:int}")]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            var user = await _unitOfWork.UsuarioRepository.GetById(id);
            return user == null ? NotFound() : Ok(_mapper.Map<UsuarioDto>(user));
        }

        // POST /api/usuario
        [HttpPost("~/api/usuario")]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioDto dto)
        {
            var entity = _mapper.Map<Usuario>(dto);
            await _unitOfWork.UsuarioRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<UsuarioDto>(entity);
            return CreatedAtAction(nameof(GetUsuarioById), new { id = result.Id }, result);
        }

        // PUT /api/usuario/{id}
        [HttpPut("~/api/usuario/{id:int}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            var existing = await _unitOfWork.UsuarioRepository.GetById(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            _unitOfWork.UsuarioRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            return Ok(_mapper.Map<UsuarioDto>(existing));
        }

        // DELETE /api/usuario/{id}
        [HttpDelete("~/api/usuario/{id:int}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var existing = await _unitOfWork.UsuarioRepository.GetById(id);
            if (existing == null) return NotFound();

            await _unitOfWork.UsuarioRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        // ==========================
        //        CATEGORÍAS
        // ==========================

        // GET /api/categoria
        [HttpGet("~/api/categoria")]
        public IActionResult GetCategorias()
        {
            var cats = _unitOfWork.CategoriaRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<CategoriaDto>>(cats));
        }

        // GET /api/categoria/{id}
        [HttpGet("~/api/categoria/{id:int}")]
        public async Task<IActionResult> GetCategoriaById(int id)
        {
            var cat = await _unitOfWork.CategoriaRepository.GetById(id);
            return cat == null ? NotFound() : Ok(_mapper.Map<CategoriaDto>(cat));
        }

        // POST /api/categoria
        [HttpPost("~/api/categoria")]
        public async Task<IActionResult> CreateCategoria([FromBody] CategoriaDto dto)
        {
            var entity = _mapper.Map<Categoria>(dto);
            await _unitOfWork.CategoriaRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<CategoriaDto>(entity);
            return CreatedAtAction(nameof(GetCategoriaById), new { id = result.Id }, result);
        }

        // PUT /api/categoria/{id}
        [HttpPut("~/api/categoria/{id:int}")]
        public async Task<IActionResult> UpdateCategoria(int id, [FromBody] CategoriaDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            var existing = await _unitOfWork.CategoriaRepository.GetById(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            _unitOfWork.CategoriaRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            return Ok(_mapper.Map<CategoriaDto>(existing));
        }

        // DELETE /api/categoria/{id}
        [HttpDelete("~/api/categoria/{id:int}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var existing = await _unitOfWork.CategoriaRepository.GetById(id);
            if (existing == null) return NotFound();

            await _unitOfWork.CategoriaRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}