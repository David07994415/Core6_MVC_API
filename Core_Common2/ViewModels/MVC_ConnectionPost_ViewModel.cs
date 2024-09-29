using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Core_Common2.ViewModels
{
    public class MVC_ConnectionPost_ViewModel
    {
        [Required(ErrorMessage ="名字欄位為必須")]
        [Display(Name ="名字")]
        [BindProperty]
        [BindRequired]
        public string? Name { get; set; }

        [Required(ErrorMessage = "歲數欄位為必須")]
        [Display(Name = "歲數")]
        [BindProperty]
        [BindRequired]
        public int? Age { get; set; }

        [Required(ErrorMessage = "日期欄位為必須")]
        [Display(Name = "日期")]
        [BindProperty]
        [BindRequired]
        public DateTime? DateTime { get; set; }

        [Required(ErrorMessage = "是否勾選欄位為必須")]
        [Display(Name = "是否勾選")]
        [BindProperty]
        [BindRequired]
        public bool IsBool { get; set; } // bool 不可為 nullable

        //// ModelState 會不通過
        //[BindNever]
        //public string NoNeedBind { get; set; }
    }
}
