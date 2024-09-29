using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Core_Common2.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0}欄位為必須")]
        [Display(Name = "帳號")]
        [BindProperty]
        [BindRequired]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "{0}欄位為必須")]
        [Display(Name = "密碼")]
        [BindProperty]
        [BindRequired]
        public string? Password { get; set; }
    }
}
