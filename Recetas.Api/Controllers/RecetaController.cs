using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recetas.Core.Interfaces;
using Recetas.Core.Entities;
using Recetas.Infrastructure.DTOs;
using Recetas.Core.QueryFilters;
using Recetas.Infrastructure.Validators;

namespace Recetas.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de recetas, usuarios y categorías.
    /// Proporciona endpoints para CRUD y consultas especializadas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RecetaController : ControllerBase
    {
        private readonly IRecetaService _recetaService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Constructor del controlador de recetas.
        /// </summary>
        /// <param name="recetaService">Servicio de lógica de negocio para recetas</param>
        /// <param name="unitOfWork">Unit of Work para acceso a datos</param>
        /// <param name="mapper">Automapper para conversión de entidades y DTOs</param>
        /// <param name="validationService">Servicio de validación</param>
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

        /// <summary>
        /// Obtiene todas las recetas con filtros opcionales y paginación.
        /// </summary>
        /// <param name="filters">Filtros de consulta para recetas</param>
        /// <returns>Lista paginada de recetas</returns>
        /// <response code="200">Retorna la lista de recetas</response>
        /// <response code="400">Solicitud incorrecta</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetRecetas([FromQuery] RecetaQueryFilter filters)
        {
            var recetas = _recetaService.GetAllRecetas(filters);
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
        /// Obtiene una receta por su ID.
        /// </summary>
        /// <param name="id">ID de la receta</param>
        /// <returns>Receta solicitada</returns>
        /// <response code="200">Retorna la receta solicitada</response>
        /// <response code="404">Receta no encontrada</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRecetaById(int id)
        {
            var receta = await _recetaService.GetRecetaById(id);
            if (receta == null)
                return NotFound($"No se encontró la receta con id {id}.");
            return Ok(_mapper.Map<RecetaDto>(receta));
        }

        /// <summary>
        /// Crea una nueva receta.
        /// </summary>
        /// <param name="dto">Datos de la receta</param>
        /// <returns>Receta creada</returns>
        /// <response code="201">Receta creada exitosamente</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateReceta([FromBody] RecetaDto dto)
        {
            var entity = _mapper.Map<Receta>(dto);
            await _recetaService.InsertReceta(entity);
            var result = _mapper.Map<RecetaDto>(entity);
            return CreatedAtAction(nameof(GetRecetaById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Actualiza una receta existente.
        /// </summary>
        /// <param name="id">ID de la receta</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Receta actualizada</returns>
        /// <response code="200">Receta actualizada exitosamente</response>
        /// <response code="400">Id no coincide</response>
        /// <response code="404">Receta no encontrada</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// Elimina una receta por su ID.
        /// </summary>
        /// <param name="id">ID de la receta</param>
        /// <returns>Sin contenido</returns>
        /// <response code="204">Receta eliminada exitosamente</response>
        /// <response code="404">Receta no encontrada</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReceta(int id)
        {
            var receta = await _unitOfWork.RecetaRepository.GetByIdAsync(id);
            if (receta == null)
                return NotFound($"No se encontró la receta con id {id}.");
            await _recetaService.DeleteReceta(id);
            return NoContent();
        }

        /// <summary>
        /// Obtiene recetas por categoría.
        /// </summary>
        /// <param name="categoriaId">ID de la categoría</param>
        /// <returns>Lista de recetas de la categoría</returns>
        /// <response code="200">Retorna la lista de recetas</response>
        [HttpGet("por-categoria/{categoriaId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecetasByCategoria(int categoriaId)
        {
            var recetas = await _unitOfWork.RecetaRepository.GetByCategoriaAsync(categoriaId);
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }

        /// <summary>
        /// Obtiene recetas por usuario.
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de recetas del usuario</returns>
        /// <response code="200">Retorna la lista de recetas</response>
        [HttpGet("por-usuario/{usuarioId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecetasByUsuario(int usuarioId)
        {
            var recetas = await _unitOfWork.RecetaRepository.GetByUsuarioAsync(usuarioId);
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }

        /// <summary>
        /// Busca recetas por ingrediente.
        /// </summary>
        /// <param name="ingrediente">Nombre del ingrediente</param>
        /// <returns>Lista de recetas que contienen el ingrediente</returns>
        /// <response code="200">Retorna la lista de recetas</response>
        [HttpGet("buscar-por-ingrediente/{ingrediente}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarPorIngrediente(string ingrediente)
        {
            var recetas = await _unitOfWork.RecetaRepository.BuscarPorIngredienteAsync(ingrediente);
            return Ok(_mapper.Map<IEnumerable<RecetaDto>>(recetas));
        }

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        /// <response code="200">Retorna la lista de usuarios</response>
        [HttpGet("~/api/usuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsuarios()
        {
            var users = _unitOfWork.UsuarioRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<UsuarioDto>>(users));
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario solicitado</returns>
        /// <response code="200">Retorna el usuario solicitado</response>
        /// <response code="404">Usuario no encontrado</response>
        [HttpGet("~/api/usuario/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            var user = await _unitOfWork.UsuarioRepository.GetById(id);
            return user == null ? NotFound() : Ok(_mapper.Map<UsuarioDto>(user));
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="dto">Datos del usuario</param>
        /// <returns>Usuario creado</returns>
        /// <response code="201">Usuario creado exitosamente</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPost("~/api/usuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioDto dto)
        {
            var entity = _mapper.Map<Usuario>(dto);
            await _unitOfWork.UsuarioRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<UsuarioDto>(entity);
            return CreatedAtAction(nameof(GetUsuarioById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Usuario actualizado</returns>
        /// <response code="200">Usuario actualizado exitosamente</response>
        /// <response code="400">Id no coincide</response>
        /// <response code="404">Usuario no encontrado</response>
        [HttpPut("~/api/usuario/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Elimina un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Sin contenido</returns>
        /// <response code="204">Usuario eliminado exitosamente</response>
        /// <response code="404">Usuario no encontrado</response>
        [HttpDelete("~/api/usuario/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var existing = await _unitOfWork.UsuarioRepository.GetById(id);
            if (existing == null) return NotFound();
            await _unitOfWork.UsuarioRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Obtiene todas las categorías.
        /// </summary>
        /// <returns>Lista de categorías</returns>
        /// <response code="200">Retorna la lista de categorías</response>
        [HttpGet("~/api/categoria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias()
        {
            var cats = _unitOfWork.CategoriaRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<CategoriaDto>>(cats));
        }

        /// <summary>
        /// Obtiene una categoría por su ID.
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Categoría solicitada</returns>
        /// <response code="200">Retorna la categoría solicitada</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpGet("~/api/categoria/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoriaById(int id)
        {
            var cat = await _unitOfWork.CategoriaRepository.GetById(id);
            return cat == null ? NotFound() : Ok(_mapper.Map<CategoriaDto>(cat));
        }

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="dto">Datos de la categoría</param>
        /// <returns>Categoría creada</returns>
        /// <response code="201">Categoría creada exitosamente</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPost("~/api/categoria")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategoria([FromBody] CategoriaDto dto)
        {
            var entity = _mapper.Map<Categoria>(dto);
            await _unitOfWork.CategoriaRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<CategoriaDto>(entity);
            return CreatedAtAction(nameof(GetCategoriaById), new { id = result.id }, result);
        }

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Categoría actualizada</returns>
        /// <response code="200">Categoría actualizada exitosamente</response>
        /// <response code="400">Id no coincide</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpPut("~/api/categoria/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategoria(int id, [FromBody] CategoriaDto dto)
        {
            if (id != dto.id) return BadRequest("Id mismatch");
            var existing = await _unitOfWork.CategoriaRepository.GetById(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _unitOfWork.CategoriaRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<CategoriaDto>(existing));
        }

        /// <summary>
        /// Elimina una categoría por su ID.
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Sin contenido</returns>
        /// <response code="204">Categoría eliminada exitosamente</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpDelete("~/api/categoria/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
