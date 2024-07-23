using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ViewDemo.Filter;
using ViewDemo.Models;

namespace ViewDemo.Controllers
{
    [Authorize]
    [PermissionFilters]
    public class MembersController : Controller
    {
        private DbModel db = new DbModel();

        // GET: Members
        public ActionResult Index()
        {
            var members = db.Members.Include(m => m.MyOrg);
            return View(members.ToList());
        }

        // GET: Members/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // GET: Members/Create
        public ActionResult Create()
        {
            ViewBag.Tree = GetTree();

            ViewBag.OrgId = new SelectList(db.Orgs, "Id", "Subject");
            return View();
        }

        private string GetTree()
        {
            //組字串
            StringBuilder sb = new StringBuilder();
            //取得權限
            var permissions = db.Permissions.ToList();

            //開始組字串
            sb.Append("[");
            //第一層權限
            var firstPermissions = permissions.Where(x => x.ParentId == null).ToList();
            //使用遞迴組字串
            var treeString = GetTreeString(firstPermissions);
            sb.Append(treeString);
            sb.Append("]");

            return sb.ToString();
        }

        private string GetTreeString(ICollection<Permission> permissions)
        {
            //組字串
            StringBuilder sb = new StringBuilder();
            foreach (var permission in permissions)
            {
                sb.Append("{");
                sb.Append($"'id': '{permission.Code}',");
                sb.Append($"'text': '{permission.Subject}'");
                if (permission.ChildPermissions.Any())
                {
                    sb.Append(",'children':");
                    sb.Append("[");
                    sb.Append(GetTreeString(permission.ChildPermissions));
                    sb.Append("]");
                }
                sb.Append("},");
            }
            string result = sb.ToString().TrimEnd(',');
            return result;
        }

        // POST: Members/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Member member)
        {
            if (ModelState.IsValid)
            {
                member.PasswordSalt = Utility.CreateSalt();
                var hashPassword = Utility.GenerateHashWithSalt(member.Password, member.PasswordSalt);
                member.Password = hashPassword;

                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrgId = new SelectList(db.Orgs, "Id", "Subject", member.OrgId);
            return View(member);
        }

        // GET: Members/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrgId = new SelectList(db.Orgs, "Id", "Subject", member.OrgId);
            ViewBag.Tree = GetTree();
            return View(member);
        }

        // POST: Members/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Account,OrgId,PasswordSalt,Password,Name,Email,Telphone,Mobile,Address,Permission,Gender")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrgId = new SelectList(db.Orgs, "Id", "Subject", member.OrgId);
            return View(member);
        }

        // GET: Members/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = db.Members.Find(id);
            db.Members.Remove(member);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
