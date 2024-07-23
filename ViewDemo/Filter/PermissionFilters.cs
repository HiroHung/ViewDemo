using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ViewDemo.Models;

namespace ViewDemo.Filter
{
    public class PermissionFilters : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DbModel db = new DbModel();

            //判斷是否有登入(表單驗證有通過)
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                filterContext.Controller.ViewBag.Side = "";

                return;
            }

            // 取得UserId
            int userId = Convert.ToInt32(((FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.Name);

            Member member = db.Members.Find(userId);
            //判斷是否有此使用者，若無則登出
            if (member == null)
            {
                FormsAuthentication.SignOut();
                return;
            }

            //取得使用者的權限
            string userPermissions = member.Permission;
            //取得符合使用者的資料庫權限資料
            var permissions = db.Permissions.Where(x => userPermissions.Contains(x.Code)).ToList();

            //判斷使用者是否有當下的controller權限
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            //若無則登出
            if (!permissions.Any(x => x.ControllerName == controllerName))
            {
                FormsAuthentication.SignOut();
                filterContext.Result = new RedirectResult("~/Home/Login");
                return;
            }

            //取得使用者的側邊欄
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class=\"nav\">");
            //取得第一層
            var firstPermissions = permissions.Where(x => x.ParentId == null).ToList();
            foreach (var permission in firstPermissions)
            {
                string current = "";
                string open = "";
                if (permission.ControllerName.Contains(controllerName))
                {
                    current = "current";
                    open = "open";
                }
                if (!permission.ChildPermissions.Any())
                {
                    sb.Append($"<li class=\"{current}\">");
                    sb.Append($"<a href=\"{permission.Url}\"> ");
                    sb.Append($"<i class=\"glyphicon {permission.Icon}\"></i>");
                    sb.Append($"{permission.Subject}");
                    sb.Append("</a>");
                    sb.Append("</li>");
                }
                else
                {
                    sb.Append($"<li class=\"submenu {open}\">");
                    sb.Append($"<a href=\"#\"> ");
                    sb.Append($"<i class=\"glyphicon {permission.Icon}\"></i>");
                    sb.Append($"{permission.Subject}");
                    sb.Append("<span class=\"caret pull-right\"></span>");
                    sb.Append("</a>");
                    if (permission.ChildPermissions.Any())
                    {
                        //使用遞迴
                        sb.Append(GetSub(permission.ChildPermissions, userPermissions));
                    }
                }
            }
            sb.Append("</ul>");


            filterContext.Controller.ViewBag.Side = sb.ToString();
        }
        private string GetSub(ICollection<Permission> permissions, string userPermissions)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var permission in permissions)
            {
                if (userPermissions.Contains(permission.Code))
                {
                    sb.Append("<li>");
                    sb.Append($"<a href=\"{permission.Url}\"> ");
                    sb.Append($"{permission.Subject}");
                    sb.Append("</a>");
                    if (permission.ChildPermissions.Any())
                    {
                        sb.Append(GetSub(permission.ChildPermissions, userPermissions));
                    }
                    sb.Append("</li>");
                }
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

    }
}