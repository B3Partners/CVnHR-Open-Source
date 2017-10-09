using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NHibernate;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using QNH.Overheid.KernRegister.Business.Model.nHibernate;
using QNH.Overheid.KernRegister.Business.Utility;

using NHibernate.Impl;
using NHibernate.Linq;
using QNH.Overheid.KernRegister.Business.Model.Debug;

namespace QNH.Overheid.KernRegister.Business.Model.nHibernate
{
    public class KvkInschrijvingNHRepository : NHRepository<KvkInschrijving>, IKvkInschrijvingRepository
    {
        public KvkInschrijvingNHRepository(ISession session) : base(session)
        {
            
        }

        public override void Remove(KvkInschrijving entity)
        {
            entity.GeldigTot = DateTime.Now;
            foreach (var vestiging in entity.Vestigingen)
                vestiging.GeldigTot = DateTime.Now;
            base.AddOrUpdate(entity);
        }

        public KvkInschrijving GetLatestInschrijving(string kvkNummer)
        {
            var latestInschrijving =
                    Query().SingleOrDefault(k => k.KvkNummer == kvkNummer && k.GeldigTot > DateTime.Now);
            return latestInschrijving;
            //try
            //{
            //    var latestInschrijving =
            //        Query().SingleOrDefault(k => k.KvkNummer == kvkNummer && k.GeldigTot > DateTime.Now);
            //    return latestInschrijving;
            //}
            //catch (InvalidOperationException ex)
            //{
            //    if (ex.Message == "Sequence contains more than one element")
            //    {
            //        var inschrijvingen = Query().Where(k => k.KvkNummer == kvkNummer);
            //        foreach (var inschrijving in inschrijvingen.Except(new[] {inschrijvingen.First()}))
            //        {
            //            inschrijving.GeldigTot = DateTime.Now.AddSeconds(-1);
            //            base.AddOrUpdate(inschrijving);
            //        }
            //        throw new InvalidOperationException("Multiple valid 'inschrijvingen' found. Expired some of the inschrijvingen. Please try again.", ex);
            //    }
            //    throw;
            //}
        }

        private static readonly object _lock = new object();

        /// <summary>
        /// Gets existing sbiCodes
        /// </summary>
        public SbiCode GetSbiCode(string sbiCode, string omschrijving = null)
        {
            var code = QueryOther<SbiCode>().SingleOrDefault(c => c.Code == sbiCode);
            if (code == null)
            {
                lock (_lock)
                {
                    AddOrUpdateOther(new SbiCode()
                    {
                        Code = sbiCode,
                        Omschrijving = omschrijving
                    });
                    code = QueryOther<SbiCode>().SingleOrDefault(c => c.Code == sbiCode);
                }
            }
            return code;
        }

        public Vestiging GetLatestVestiging(string vestigingNummer)
        {
            return GetAllCurrentVestigingen().SingleOrDefault(v => v.Vestigingsnummer == vestigingNummer);
        }

        public IEnumerable<Vestiging> GetAllCurrentVestigingen()
        {
            return QueryOther<Vestiging>()
                .Where(v => v.GeldigTot > DateTime.Now);
        }

        public DatabaseDebugInfo GetDebugInfo()
        {
            var debugInfo = new DatabaseDebugInfo();

            // Get the orphans
            debugInfo.InschrijvingOrphans = Query().Where(i => !i.Vestigingen.Any()).ToList();
            debugInfo.VestigingOrphans = QueryOther<Vestiging>()
                .Where(v => v.KvkInschrijving == null)
                .ToList();

            debugInfo.ExpiredInschrijvingenCount = Query().Count(i => i.GeldigTot <= DateTime.Now);
            debugInfo.ExpiredVestigingenCount = QueryOther<Vestiging>().Count(v => v.GeldigTot <= DateTime.Now);

            debugInfo.DeponeringsStukOrphans = QueryOther<DeponeringsStuk>()
                .Where(d => d.KvkInschrijving == null)
                .ToList();
            debugInfo.FunctieVervullingOrphans = QueryOther<FunctieVervulling>()
                .Where(f => f.KvkInschrijving == null)
                .ToList();
            debugInfo.HandelsnaamOrphans = QueryOther<HandelsNaam>()
                .Where(h=> h.KvkInschrijving == null)
                .ToList();
            debugInfo.SbiActiviteitOrphans = QueryOther<SbiActiviteit>()
                .Where(s=> s.KvKInschrijving == null || s.SbiCode == null)
                .ToList();
            debugInfo.SbiCodeOrphans = QueryOther<SbiCode>()
                .Where(s=> !s.SbiActiviteiten.Any() && !s.VestigingSbiActiviteiten.Any())
                .ToList();
            debugInfo.VestigingSbiActiviteitOrphans = QueryOther<VestigingSbiActiviteit>()
                .Where(v=> v.SbiCode == null || v.Vestiging == null)
                .ToList();

            //TODO: implement correctly..??!
            debugInfo.DoubleVestigingen = Enumerable.Empty<Vestiging>();
            //debugInfo.DoubleVestigingen = GetAllCurrentVestigingen()
            //    .Where(v=> v.KvkInschrijving.GeldigTot > DateTime.Now)
            //    .GroupBy(v => v.Vestigingsnummer)
            //    .Where(g => g.Count() > 1)
            //    .SelectMany(g => g);

            return debugInfo;
        }

        public void ActualRemove(Vestiging vestiging)
        {
            var activiteiten = vestiging.SbiActiviteiten.ToList();
            vestiging.SbiActiviteiten.Clear();

            foreach (var sbiActiviteit in activiteiten)
                sbiActiviteit.Vestiging = null;

            base.RemoveOther(vestiging);
        }

        public void ActualRemove(KvkInschrijving inschrijving)
        {
            var activiteiten = inschrijving.SbiActiviteiten.ToList();
            inschrijving.SbiActiviteiten.Clear();

            foreach (var sbiActiviteit in activiteiten)
                sbiActiviteit.KvKInschrijving = null;

            base.Remove(inschrijving);
        }

        public void ActualRemove<T>(T item)
        {
            base.RemoveOther(item);
        }
    }
}