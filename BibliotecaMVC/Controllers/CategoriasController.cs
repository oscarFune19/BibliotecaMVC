using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BibliotecaMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;

        public CategoriasController(IConfiguration configuration)
        {
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

            return View(categorias);
        }


        // =====================================================
        // AGREGAR CATEGORÍA - GET
        // =====================================================

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Categoria());
        }


        // =====================================================
        // AGREGAR CATEGORÍA - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

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

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // EDITAR CATEGORÍA - GET
        // =====================================================

        [HttpGet]
        public IActionResult Edit(int id)
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
                return NotFound();
            }

            return View(categoria);
        }


        // =====================================================
        // EDITAR CATEGORÍA - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

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
                        return NotFound();
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // ELIMINAR CATEGORÍA - GET
        // =====================================================

        [HttpGet]
        public IActionResult Delete(int id)
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
                return NotFound();
            }

            return View(categoria);
        }


        // =====================================================
        // ELIMINAR CATEGORÍA - POST
        // =====================================================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
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
                        return NotFound();
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}