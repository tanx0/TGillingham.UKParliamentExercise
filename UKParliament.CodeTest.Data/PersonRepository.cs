﻿using Microsoft.EntityFrameworkCore;
using UKParliament.CodeTest.Data;

public class PersonRepository(PersonManagerContext ctx) : IPersonRepository
{
    public async Task<IEnumerable<PersonEntity>> GetPeopleAsync()
    {
        return await ctx.People.ToListAsync();
    }

    public async Task<PersonEntity> AddPersonAsync(PersonEntity person)
    {
        await ctx.AddAsync(person);
        await ctx.SaveChangesAsync();
        return person;
    }

    public async Task<PersonEntity?> GetPersonAsync(int id)
    {
        return await ctx.People.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdatePersonAsync(PersonEntity person)
    {
        ctx.Update(person);
        await ctx.SaveChangesAsync();
    }

    public async Task<PersonEntity?> SearchForPersonAsync(string firstName, string lastName, DateTime dateOfBirth)
    {
        return await ctx.People
            .FirstOrDefaultAsync(p => p.FirstName == firstName && p.LastName == lastName && p.DateOfBirth == dateOfBirth);
    }
}
