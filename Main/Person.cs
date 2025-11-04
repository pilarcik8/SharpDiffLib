using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace TestKniznice
{
    public class Person
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string Gender { get; set; }
        public int Age { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }

        public string CreditCardNumber { get; set; }

        public Address Address { get; set; }

        public Person()
        {
            Title = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Gender = string.Empty;
            Age = 0;
            Company = string.Empty;
            JobTitle = string.Empty;
            CreditCardNumber = string.Empty;
            Address = new Address();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, new[]
            {
                $"Firstname: {FirstName}, Lastname: {LastName}",
                $"Email: {Email}, Phone: {Phone}",
                $"Age: {Age}, Gender: {Gender}",
                $"Company: {Company}, Job: {JobTitle}",
                $"Address: {Address}",
                $"Card: {CreditCardNumber}"
            });
        }
    }

    public class Address
    {
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public Address()
        {
            Street = string.Empty;
            StreetNumber = string.Empty;
            City = string.Empty;
            County = string.Empty;
            State = string.Empty;
            ZipCode = string.Empty;
            Country = string.Empty;
        }

        public override string ToString()
        {
            return $"{Street} {StreetNumber}, {City}, {Country}";
        }
    }
}

