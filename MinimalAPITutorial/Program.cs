using MinimalAPITutorial;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<SuperHero>> GetAllHeroes(DataContext context) =>
    await context.SuperHeroes.ToListAsync();

app.MapGet("/", () => "Welcome to the Super Hero DB! ❤️");

app.MapGet("/superhero", async (DataContext context) =>
    await context.SuperHeroes.ToListAsync());

app.MapGet("/superhero/{id}", async (DataContext context, int id) =>
    await context.SuperHeroes.FindAsync(id) is SuperHero hero ?
        Results.Ok(hero) :
        Results.NotFound("Sorry, hero not found. :/"));

app.MapPost("/superhero", async (DataContext context, SuperHero hero) =>
{
    context.SuperHeroes.Add(hero);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllHeroes(context));
});

app.MapPut("/superhero/{id}", async (DataContext context, SuperHero hero, int id) =>
{
    var dbHero = await context.SuperHeroes.FindAsync(id);
    if (dbHero == null) return Results.NotFound("No hero found. :/");

    dbHero.Firstname = hero.Firstname;
    dbHero.Lastname = hero.Lastname;
    dbHero.Heroname = hero.Heroname;
    await context.SaveChangesAsync();

    return Results.Ok(await GetAllHeroes(context));
});

app.MapDelete("/superhero/{id}", async (DataContext context, int id) =>
{
    var dbHero = await context.SuperHeroes.FindAsync(id);
    if (dbHero == null) return Results.NotFound("Who's that?");

    context.SuperHeroes.Remove(dbHero);
    await context.SaveChangesAsync();

    return Results.Ok(await GetAllHeroes(context));
});

app.Run();