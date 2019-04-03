using System;
using System.Linq;
using System.Web.Mvc;
using NHibernate.Util;
using QNH.Overheid.KernRegister.Business.Model;
using QNH.Overheid.KernRegister.Business.Model.Debug;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    public class DebugController : Controller
    {
        public ActionResult Index()
        {
            var debugInfo = GetDatabaseDebugInfo();
            return View(debugInfo);
        }

        private static DatabaseDebugInfo GetDatabaseDebugInfo(IKvkInschrijvingRepository repo = null)
        {
            var debugInfo = new DatabaseDebugInfo();
            try
            {
                repo = repo ?? IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
                debugInfo = repo.GetDebugInfo();
            }
            catch (Exception ex)
            {
                debugInfo.Exception = ex;
            }
            return debugInfo;
        }

        public ActionResult DeleteOrphans(string itemid)
        {
            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();

            var debugInfo = GetDatabaseDebugInfo(repo);
            var orphan = debugInfo.InschrijvingOrphans?.FirstOrDefault(i => i.KvkNummer == itemid);
            if (orphan != null)
            {
                repo.ActualRemove(orphan);
            }

            var vestigingOrphan = debugInfo.VestigingOrphans?.FirstOrDefault(i => i.Vestigingsnummer == itemid);
            if (vestigingOrphan != null)
            {
                repo.ActualRemove(vestigingOrphan);
            }
            return RedirectToAction("Index"); // View(GetDatabaseDebugInfo());
        }

        public ActionResult DeleteAllOrphans(bool vestiging = false)
        {
            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
            var debugInfo = GetDatabaseDebugInfo(repo);

            if (vestiging ? debugInfo.VestigingOrphans != null : debugInfo.InschrijvingOrphans != null)
            {
                try
                {
                    if (vestiging)
                        foreach (var orphan in debugInfo.VestigingOrphans)
                            repo.ActualRemove(orphan);
                    else
                        foreach (var orphan in debugInfo.InschrijvingOrphans)
                            repo.ActualRemove(orphan);
                }
                catch (Exception ex)
                {
                    debugInfo.Exception = ex;
                    return View("Index", debugInfo);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteReallyAllOrphans()
        {
            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
            var debugInfo = GetDatabaseDebugInfo(repo);

            foreach (var d in debugInfo.DeponeringsStukOrphans)
            {
                repo.ActualRemove(d);
            }
            foreach (var f in debugInfo.FunctieVervullingOrphans)
            {
                repo.ActualRemove(f);
            }
            foreach (var h in debugInfo.HandelsnaamOrphans)
            {
                repo.ActualRemove(h);
            }
            foreach (var f in debugInfo.SbiActiviteitOrphans)
            {
                repo.ActualRemove(f);
            }
            foreach (var f in debugInfo.VestigingSbiActiviteitOrphans)
            {
                repo.ActualRemove(f);
            }
            foreach (var d in debugInfo.VestigingOrphans)
            {
                repo.ActualRemove(d);
            }
            foreach (var f in debugInfo.SbiCodeOrphans)
            {
                repo.ActualRemove(f);
            }
            foreach (var i in debugInfo.InschrijvingOrphans)
            {
                repo.ActualRemove(i);
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteDoubles()
        {
            //var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
            //var debugInfo = GetDatabaseDebugInfo(repo);


            //debugInfo.DoubleVestigingen?.ForEach(d => repo.ActualRemove(d));

            return RedirectToAction("Index");
        }
    }
}
