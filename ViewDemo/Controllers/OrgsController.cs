using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ViewDemo.Filter;
using ViewDemo.Models;

namespace ViewDemo.Controllers
{
    [Authorize]
    [PermissionFilters]
    public class OrgsController : Controller
    {
        private DbModel db = new DbModel();

        // GET: Orgs
        public ActionResult Index()
        {
            IEnumerable<Org> orgs = db.Orgs.ToList();
            Member member = new Member();
            ViewBag.Member = member;
            return View(orgs);
        }

        // GET: Orgs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Org org = db.Orgs.Find(id);
            if (org == null)
            {
                return HttpNotFound();
            }
            return View(org);
        }

        // GET: Orgs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orgs/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Subject,InitDate")] Org org)
        {
            if (ModelState.IsValid)
            {
                db.Orgs.Add(org);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(org);
        }

        // GET: Orgs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Org org = db.Orgs.Find(id);
            if (org == null)
            {
                return HttpNotFound();
            }
            return View(org);
        }

        // POST: Orgs/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Subject,InitDate")] Org org)
        {
            if (ModelState.IsValid)
            {
                db.Entry(org).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(org);
        }

        // GET: Orgs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Org org = db.Orgs.Find(id);
            if (org == null)
            {
                return HttpNotFound();
            }
            return View(org);
        }

        // POST: Orgs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Org org = db.Orgs.Find(id);
            db.Orgs.Remove(org);
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
