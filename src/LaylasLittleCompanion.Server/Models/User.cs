using System.Collections.Generic;


namespace LaylasLittleCompanion.Server.Models
{
    public class User
    {
        //public bool Mature { get; set; }
        //public string Status { get; set; }
        public string Display_Name { get; set; }
        public string _Id { get; set; }

    }

    public class TeamResponse
    {
        //public int _Id { get; set; }
        //public string Name { get; set; }
        //public string Info { get; set; }
        //public string Display_Name  { get; set; }
        //public DateTime Created_At { get; set; }
        //public DateTime Updated_At { get; set; }
        //public string Logo { get; set; }
        //public string Banner { get; set; }
        //public string Background { get; set; }
        public List<User> Users { get; set; }
    }
}
