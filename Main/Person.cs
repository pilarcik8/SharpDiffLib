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
        public string? Iban;
        public string? Title { get; set; } //0
        /*---------------------------------------*/
        public string? FavouriteColor;
        public string? FirstName { get; set; } //1
        /*---------------------------------------*/
        public string? BitcoinAddress;
        public string? LastName { get; set; } //2
        /*---------------------------------------*/
        public string? EmailUserName;
        public string? Email { get; set; } //3
        /*---------------------------------------*/
        public string? PhoneExtension;
        public string? Phone { get; set; } //4
        /*---------------------------------------*/
        public string? FavouriteWord;
        public string? Gender { get; set; }  //5
        /*---------------------------------------*/
        public string? FavouriteMusicGenre;
        public string? StreetNumber { get; set; } //6

        /*---------------------------------------*/
        public string? CompanyCatchPhrase;
        public string? Company { get; set; } //7
        /*---------------------------------------*/
        public string? JobDescriptor;
        public string? JobTitle { get; set; } //8
        /*---------------------------------------*/
        public string? CreditAccount;
        public string? CreditCardNumber { get; set; } //9
        /*---------------------------------------*/
        public string? StreetSuffix;
        public string? Street { get; set; } //10
        /*---------------------------------------*/
        public string? CityPrefix;
        public string? City { get; set; } //11
        /*---------------------------------------*/
        public string? CountyCode;
        public string? County { get; set; } //12
        /*---------------------------------------*/

        public string? State { get; set; } //13
        public string? StateAbbr;
        /*---------------------------------------*/
        public string? ZipPlus4;
        public string? ZipCode { get; set; } //14
        /*---------------------------------------*/
        public string? CountryCode;
        public string? Country { get; set; } //15
    }
}

