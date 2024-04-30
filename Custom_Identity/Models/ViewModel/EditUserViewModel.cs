using System.ComponentModel.DataAnnotations;


namespace Custom_Identity.Models.ViewModel
{
    public class EditUserViewModel
    {
        public EditUserViewModel() 
        { 
            Claims= new List<string>(); Roles= new List<string>();
        }
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public List<string> Roles { get; set; }

        public IList<string> Claims { get; set;}
    }
}
