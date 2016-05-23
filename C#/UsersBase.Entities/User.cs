using System;
using System.Collections.Generic;

namespace UsersBase.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public string ImageType { get; set; }

        public int Age()
        {
            int age = DateTime.Now.Year - BirthDate.Year;
            if (DateTime.Now.Month < BirthDate.Month || (DateTime.Now.Month == BirthDate.Month && DateTime.Now.Day < BirthDate.Day))
            {
                age--;
            }
            return age;
        }        
    }
}