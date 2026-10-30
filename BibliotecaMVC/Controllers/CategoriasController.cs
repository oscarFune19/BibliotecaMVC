using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BibliotecaMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(
            IConfiguration configuration,
            ILogger<CategoriasController> logger)
        {
            _logger = logger;

            _connectionString =
                configuration.GetConnectionString("BibliotecaConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión BibliotecaConnection.");
        }

        // =====================================================
        // MOSTRAR CATEGORÍAS
        // =====================================================

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation(
                "Iniciando consulta del listado de categorías.");

            try
            {
                var categorias = new List<Categoria>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql =
                        @"SELECT ID, Nombre, Descripcion
                          FROM Categorias
                          ORDER BY ID;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categorias.Add(new Categoria
                                {
                                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                    Descripcion = reader.GetString(
                                        reader.GetOrdinal("Descripcion"))
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation(
                    "Consulta de categorías completada. Se encontraron {Cantidad} registros.",
                    categorias.Count);

                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al consultar el listado de categorías.");

                throw;
            }
        }

        // =====================================================
        // AGREGAR CATEGORÍA - GET
        // =====================================================

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation(
                "Iniciando apertura del formulario para crear una categoría.");

            return View(new Categoria());
        }

        // =====================================================
        // AGREGAR CATEGORÍA - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            _logger.LogInformation(
                "Iniciando creación de una nueva categoría.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "La creación de la categoría fue detenida porque los datos no son válidos.");

                return View(categoria);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql =
                        @"INSERT INTO Categorias (Nombre, Descripcion)
                          VALUES (@Nombre, @Descripcion);";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Nombre",
                            SqlDbType.NVarChar,
                            100).Value = categoria.Nombre;

                        command.Parameters.Add(
                            "@Descripcion",
                            SqlDbType.NVarChar,
                            250).Value = categoria.Descripcion;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                _logger.LogInformation(
                    "Categoría creada correctamente.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al crear una categoría.");

                throw;
            }
        }

        // =====================================================
        // EDITAR CATEGORÍA - GET
        // =====================================================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation(
                "Iniciando consulta de la categoría con ID {CategoriaId} para editar.",
                id);

            try
            {
                Categoria? categoria = null;

                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql =
                        @"SELECT ID, Nombre, Descripcion
                          FROM Categorias
                          WHERE ID = @ID;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@ID",
                            SqlDbType.Int).Value = id;

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                categoria = new Categoria
                                {
                                    ID = reader.GetInt32(
                                        reader.GetOrdinal("ID")),

                                    Nombre = reader.GetString(
                                        reader.GetOrdinal("Nombre")),

                                    Descripcion = reader.GetString(
                                        reader.GetOrdinal("Descripcion"))
                                };
                            }
                        }
                    }
                }

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría con ID {CategoriaId} para editar.",
                        id);

                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al consultar la categoría con ID {CategoriaId} para editar.",
                    id);

                throw;
            }
        }

        // =====================================================
        // EDITAR CATEGORÍA - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            _logger.LogInformation(
                "Iniciando actualización de la categoría con ID {CategoriaId}.",
                categoria.ID);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "La edición de la categoría con ID {CategoriaId} fue detenida porque los datos no son válidos.",
                    categoria.ID);

                return View(categoria);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql =
                        @"UPDATE Categorias
                          SET Nombre = @Nombre,
                              Descripcion = @Descripcion
                          WHERE ID = @ID;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Nombre",
                            SqlDbType.NVarChar,
                            100).Value = categoria.Nombre;

                        command.Parameters.Add(
                            "@Descripcion",
                            SqlDbType.NVarChar,
                            250).Value = categoria.Descripcion;

                        command.Parameters.Add(
                            "@ID",
                            SqlDbType.Int).Value = categoria.ID;

                        connection.Open();

                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            _logger.LogWarning(
                                "No se encontró la categoría con ID {CategoriaId} para actualizar.",
                                categoria.ID);

                            return NotFound();
                        }
                    }
                }

                _logger.LogInformation(
                    "Categoría con ID {CategoriaId} actualizada correctamente.",
                    categoria.ID);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al actualizar la categoría con ID {CategoriaId}.",
                    categoria.ID);

                throw;
            }
        }

        // =====================================================
        // ELIMINAR CATEGORÍA - GET
        // =====================================================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation(
                "Iniciando consulta de la categoría con ID {CategoriaId} para eliminar.",
                id);

            try
            {
                Categoria? categoria = null;

                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql =
                        @"SELECT ID, Nombre, Descripcion
                          FROM Categorias
                          WHERE ID = @ID;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@ID",
                            SqlDbType.Int).Value = id;

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                categoria = new Categoria
                                {
                                    ID = reader.GetInt32(
                                        reader.GetOrdinal("ID")),

                                    Nombre = reader.GetString(
                                        reader.GetOrdinal("Nombre")),

                                    Descripcion = reader.GetString(
                                        reader.GetOrdinal("Descripcion"))
                                };
                            }
                        }
                    }
                }

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría con ID {CategoriaId} para eliminar.",
                        id);

                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al consultar la categoría con ID {CategoriaId} para eliminar.",
                    id);

                throw;
            }
        }

        // =====================================================
        // ELIMINAR CATEGORÍA - POST
        // =====================================================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation(
                "Iniciando eliminación de la categoría con ID {CategoriaId}.",
                id);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql =
                        @"DELETE FROM Categorias
                          WHERE ID = @ID;";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@ID",
                            SqlDbType.Int).Value = id;

                        connection.Open();

                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            _logger.LogWarning(
                                "No se encontró la categoría con ID {CategoriaId} para eliminar.",
                                id);

                            return NotFound();
                        }
                    }
                }

                _logger.LogInformation(
                    "Categoría con ID {CategoriaId} eliminada correctamente.",
                    id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al eliminar la categoría con ID {CategoriaId}.",
                    id);

                throw;
            }
        }
    }
}
