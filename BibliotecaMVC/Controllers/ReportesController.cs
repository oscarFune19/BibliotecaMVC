using BibliotecaMVC.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = IdentitySeeder.RolesConsulta)]
    public class ReportesController : Controller
    {
        private readonly BibliotecaDbContext _context;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(
            BibliotecaDbContext context,
            ILogger<ReportesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GenerarPdf()
        {
            _logger.LogInformation(
                "Iniciando generación del reporte PDF de libros.");

            try
            {
                var libros = await _context.Libros
                    .AsNoTracking()
                    .OrderBy(l => l.ID)
                    .ToListAsync();

                var fechaGeneracion = DateTime.Now;

                var documento = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(35);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(text => text.FontSize(10));

                        page.Header()
                            .Column(column =>
                            {
                                column.Item()
                                    .Text("Reporte de Libros")
                                    .FontSize(22)
                                    .Bold()
                                    .FontColor(Colors.Blue.Medium);

                                column.Item()
                                    .Text("BibliotecaMVC - Sistema de gestión bibliotecaria")
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken2);

                                column.Item()
                                    .PaddingTop(4)
                                    .Text($"Fecha de generación: {fechaGeneracion:dd/MM/yyyy HH:mm}")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            });

                        page.Content()
                            .PaddingVertical(15)
                            .Column(column =>
                            {
                                column.Spacing(10);

                                column.Item()
                                    .Text($"Total de libros: {libros.Count}")
                                    .SemiBold()
                                    .FontColor(Colors.Grey.Darken3);

                                column.Item()
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.ConstantColumn(45);
                                            columns.RelativeColumn(2.4f);
                                            columns.RelativeColumn(2.0f);
                                            columns.RelativeColumn(1.6f);
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell()
                                                .Element(EstiloEncabezado)
                                                .Text("ID")
                                                .SemiBold()
                                                .FontColor(Colors.White);

                                            header.Cell()
                                                .Element(EstiloEncabezado)
                                                .Text("Nombre del libro")
                                                .SemiBold()
                                                .FontColor(Colors.White);

                                            header.Cell()
                                                .Element(EstiloEncabezado)
                                                .Text("Autor")
                                                .SemiBold()
                                                .FontColor(Colors.White);

                                            header.Cell()
                                                .Element(EstiloEncabezado)
                                                .Text("Categoría")
                                                .SemiBold()
                                                .FontColor(Colors.White);
                                        });

                                        foreach (var libro in libros)
                                        {
                                            table.Cell()
                                                .Element(EstiloCelda)
                                                .Text(libro.ID.ToString());

                                            table.Cell()
                                                .Element(EstiloCelda)
                                                .Text(libro.Titulo);

                                            table.Cell()
                                                .Element(EstiloCelda)
                                                .Text(libro.Autor);

                                            table.Cell()
                                                .Element(EstiloCelda)
                                                .Text(libro.Categoria);
                                        }

                                        if (libros.Count == 0)
                                        {
                                            table.Cell()
                                                .ColumnSpan(4)
                                                .Element(EstiloCelda)
                                                .AlignCenter()
                                                .Text("No hay libros registrados para mostrar.");
                                        }
                                    });
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.Span("Página ");
                                text.CurrentPageNumber();
                                text.Span(" de ");
                                text.TotalPages();
                            });
                    });
                });

                var pdf = documento.GeneratePdf();

                _logger.LogInformation(
                    "Reporte PDF generado correctamente con {Cantidad} libros.",
                    libros.Count);

                return File(
                    pdf,
                    "application/pdf",
                    "ReporteLibros.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al generar el reporte PDF de libros.");

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcel()
        {
            _logger.LogInformation(
                "Iniciando generación del reporte Excel de libros.");

            try
            {
                var libros = await _context.Libros
                    .AsNoTracking()
                    .OrderBy(l => l.ID)
                    .ToListAsync();

                var fechaGeneracion = DateTime.Now;

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Reporte de Libros");

                worksheet.Cell("A1").Value =
                    "BibliotecaMVC - Reporte de Libros";
                worksheet.Range("A1:D1").Merge();

                worksheet.Cell("A2").Value =
                    $"Fecha de generación: {fechaGeneracion:dd/MM/yyyy HH:mm}";
                worksheet.Range("A2:D2").Merge();

                worksheet.Cell("A3").Value =
                    $"Total de libros: {libros.Count}";
                worksheet.Range("A3:D3").Merge();

                var titulo = worksheet.Range("A1:D1");
                titulo.Style.Font.Bold = true;
                titulo.Style.Font.FontSize = 16;
                titulo.Style.Font.FontColor = XLColor.White;
                titulo.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
                titulo.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                var subtitulo = worksheet.Range("A2:D3");
                subtitulo.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                const int filaEncabezados = 5;

                worksheet.Cell(filaEncabezados, 1).Value = "ID";
                worksheet.Cell(filaEncabezados, 2).Value = "Nombre del libro";
                worksheet.Cell(filaEncabezados, 3).Value = "Autor";
                worksheet.Cell(filaEncabezados, 4).Value = "Categoría";

                var encabezados =
                    worksheet.Range(filaEncabezados, 1, filaEncabezados, 4);

                encabezados.Style.Font.Bold = true;
                encabezados.Style.Font.FontColor = XLColor.White;
                encabezados.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
                encabezados.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;
                encabezados.Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;
                encabezados.Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;

                var fila = filaEncabezados + 1;

                foreach (var libro in libros)
                {
                    worksheet.Cell(fila, 1).Value = libro.ID;
                    worksheet.Cell(fila, 2).Value = libro.Titulo;
                    worksheet.Cell(fila, 3).Value = libro.Autor;
                    worksheet.Cell(fila, 4).Value = libro.Categoria;

                    fila++;
                }

                if (libros.Count > 0)
                {
                    var datos = worksheet.Range(
                        filaEncabezados + 1,
                        1,
                        fila - 1,
                        4);

                    datos.Style.Border.OutsideBorder =
                        XLBorderStyleValues.Thin;
                    datos.Style.Border.InsideBorder =
                        XLBorderStyleValues.Thin;
                }
                else
                {
                    worksheet.Cell(fila, 1).Value =
                        "No hay libros registrados para mostrar.";
                    worksheet.Range(fila, 1, fila, 4).Merge();
                    worksheet.Range(fila, 1, fila, 4)
                        .Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;
                }

                worksheet.Columns(1, 4).AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                _logger.LogInformation(
                    "Reporte Excel generado correctamente con {Cantidad} libros.",
                    libros.Count);

                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "ReporteLibros.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al generar el reporte Excel de libros.");

                throw;
            }
        }

        private static IContainer EstiloEncabezado(IContainer container)
        {
            return container
                .Background(Colors.Blue.Medium)
                .Border(0.5f)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6);
        }

        private static IContainer EstiloCelda(IContainer container)
        {
            return container
                .BorderBottom(0.5f)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6);
        }
    }
}
