﻿using System.ComponentModel.DataAnnotations;

namespace UKParliament.CodeTest.Services.Models
{
    public class PersonDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
