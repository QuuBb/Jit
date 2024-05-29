using API;
using API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VisitDb>(opt => opt.UseInMemoryDatabase("VisitList"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// GET methods

app.MapGet("/visits", (VisitDb db) =>
{
    return db.Visits.ToList();
})
.WithOpenApi();

app.MapGet("/visits/{id}", (int id, VisitDb db) =>
{
    var result = db.Visits.Find(id);
    return result == null ? Results.NotFound($"No visit with given id: {id}") : Results.Ok(result);
    
})
.WithOpenApi();

// POST methods

app.MapPost("/addVisit", (Visit visit, VisitDb db) =>
{
    // If date is already booked
    var alreadyBooked = db.Visits.Where(v => v.date == visit.date).FirstOrDefault();
    if (alreadyBooked != null)
    {
        return Results.BadRequest("Can't set visit on this date");
    }
    // If date is outside working hours
    if (visit.date.Hour < 8 && visit.date.Hour >= 16)
    {
        return Results.BadRequest("Time set outside working hours");
    }
    db.Visits.Add(visit);
    db.SaveChanges();

    return Results.Created($"Visit with id {visit.id} was created", visit);

}).WithOpenApi();

// PUT methods

app.MapPut("/updateVisit/{id}", (int id, Visit visitInput, VisitDb db) => { 
    var visitToChange = db.Visits.Find(id);
    if (visitToChange == null) return Results.NotFound();

    // If date is already booked
    var alreadyBooked = db.Visits.Where(v => v.date == visitInput.date).FirstOrDefault();
    if (alreadyBooked != null)
    {
        return Results.BadRequest("Can't set visit on this date");
    }
    // If date is outside working hours
    else if (visitInput.date.Hour < 8 && visitInput.date.Hour >= 16)
        {
        return Results.BadRequest("Time set outside working hours");
    }
    else
    {
        visitToChange.person = visitInput.person;
        visitToChange.date = visitInput.date;
        visitToChange.cat = visitInput.cat;
        db.SaveChanges();
        return Results.Ok("Your visit has been changed");
    }
    
}).WithOpenApi();

// DELETE methods

app.MapDelete("/delete/{id}", (int id, VisitDb db) => {
    var visit = db.Visits.Find(id);
    if(visit == null) return Results.NotFound();
    db.Visits.Remove(visit);
    db.SaveChanges();
    return Results.Ok($"Visit of id {id} has been deleted");
}).WithOpenApi(); 
app.Run();
