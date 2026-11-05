# Pruebas unitarias del módulo Libros

Este proyecto contiene las pruebas unitarias de la Semana 16 para BibliotecaMVC.

## Escenarios evaluados

- Consulta del listado cuando existen libros.
- Consulta del listado cuando no existen registros.
- Consulta de un libro existente.
- Consulta de un libro inexistente.
- Apertura del formulario de creación.
- Registro de un libro válido.
- Rechazo de un registro cuando el modelo es inválido.
- Edición de un libro existente.
- Respuesta cuando se intenta editar un libro inexistente.
- Consulta previa a la eliminación.
- Eliminación de un libro existente.
- Respuesta cuando se intenta eliminar un libro inexistente.
- Validaciones del modelo Libro mediante Fact y Theory.

## Aserciones utilizadas

Las pruebas utilizan Assert.Equal(), Assert.True(), Assert.False(), Assert.Empty(), Assert.NotEmpty() y Assert.NotNull(), además de verificaciones del tipo de resultado MVC.

## Uso de Mocks

LibrosController se prueba de forma aislada mediante Moq. La interfaz ILibroService es simulada con Setup(), Returns() y Verify(), por lo que las pruebas no necesitan una base de datos real.

## Niveles de prueba

- Prueba unitaria: verifica el comportamiento de LibrosController y las validaciones del modelo de manera aislada.
- Prueba de integración: podría comprobar en otro entorno la interacción entre controlador, LibroService, Entity Framework Core y SQL Server.
- Prueba de extremo a extremo: podría comprobar el flujo completo desde el inicio de sesión hasta consultar, registrar, editar o eliminar un libro mediante la interfaz.
