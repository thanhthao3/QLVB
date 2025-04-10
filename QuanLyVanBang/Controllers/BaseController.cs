using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyVanBang.Controllers
{
	public class BaseController : Controller
	{
		public string CurrentUser
		{
			get
			{
				return HttpContext.Session.GetString("USERNAME");
			}
			set
			{
				HttpContext.Session.SetString("USERNAME", value);
			}
		}
		public string CurrentUserID
		{
			get
			{
				return HttpContext.Session.GetString("NGUOIDUNGID");
			}
			set
			{
				HttpContext.Session.SetString("NGUOIDUNGID", value);
			}
		}

		public string RoleUser
		{
			get
			{
				return HttpContext.Session.GetString("ROLE");
			}
			set
			{
				HttpContext.Session.SetString("ROLE", value);
			}
		}
		public bool IsLogin
		{
			get
			{
				return !string.IsNullOrEmpty(CurrentUser);
			}
		}
        public override void OnActionExecuting(ActionExecutingContext context)
        {
			ViewBag.Role = RoleUser;
            ViewBag.IsLogin = IsLogin;
            base.OnActionExecuting(context);
        }
    }
}
