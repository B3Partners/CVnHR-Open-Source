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

            debugInfo.DeponeringsStukOrphans?.ForEach(d=> repo.ActualRemove(d));
            debugInfo.FunctieVervullingOrphans?.ForEach(f => repo.ActualRemove(f));
            debugInfo.HandelsnaamOrphans?.ForEach(h=> repo.ActualRemove(h));
            debugInfo.SbiActiviteitOrphans?.ForEach(f=> repo.ActualRemove(f));
            debugInfo.VestigingSbiActiviteitOrphans?.ForEach(f => repo.ActualRemove(f));
            debugInfo.VestigingOrphans?.ForEach(d => repo.ActualRemove(d));
            debugInfo.SbiCodeOrphans?.ForEach(f => repo.ActualRemove(f));
            debugInfo.InschrijvingOrphans?.ForEach(d => repo.ActualRemove(d));

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
