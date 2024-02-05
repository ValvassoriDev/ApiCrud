using ApiCrud.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiCrud.Diretorio
{
    public static class EstudantesRotas
    {
        public static void AddEstudantesRotas(this WebApplication app)
        {
            var rotasEstudantes = app.MapGroup("estudantes");


            // Criar = MapPost

            rotasEstudantes.MapPost("", async (AddEstudanteRequest request, AppDbContext context) =>
            {
                var jaEsxite = await context.Estudantes.AnyAsync(estudante => estudante.Nome == request.Nome);

                if (jaEsxite)
                    return Results.Conflict("Ja Existe!");

                var novoEstudante = new Estudante(request.Nome);
                await context.Estudantes.AddAsync(novoEstudante);
                await context.SaveChangesAsync();

                var estudanteRetorno = new EstudanteDto(novoEstudante.Id, novoEstudante.Nome);

                return Results.Ok(estudanteRetorno);

            });

            // Retornar Todos os Estudantes

            rotasEstudantes.MapGet("", async (AppDbContext context) =>
                {
                    var estudantes = await context.Estudantes.Where(estudante => estudante.Ativo)
                    .Select(estudante => new EstudanteDto(estudante.Id, estudante.Nome)).ToListAsync();

                    return estudantes;
                });


            // ATT Nome Estudante
            rotasEstudantes.MapPut("{id:guid}", async (Guid id,AttEstudanteRequest request, AppDbContext context) =>
            {
                var estudante = await context.Estudantes.FirstOrDefaultAsync(estudante => estudante.Id == id);

                if (estudante == null)
                    return Results.NotFound();

                estudante.AttNome(request.Nome);

                await context.SaveChangesAsync();
                return Results.Ok(new EstudanteDto(estudante.Id, estudante.Nome));
            });

            // Delete

            rotasEstudantes.MapDelete("{id}", 
                async (Guid id, AppDbContext context) =>
            {
                    var estudante = await context.Estudantes.SingleOrDefaultAsync(estudante => estudante.Id == id);
                    if (estudante == null)
                        return Results.NotFound();

                estudante.Desativar();
                await context.SaveChangesAsync();
                return Results.Ok();

                    
                    });
        }
    }
}
