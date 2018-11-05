namespace MyWebAppCustomForms.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class UserModel
    {
        public string Id { get; set; }

        [DisplayName("Username")]
        [Required(ErrorMessage = "Please enter your username")]
        public string Name { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Remember me")]
        public bool IsRememberMe { get; set; }

        public string DisplayName { get; set; }
    }
}