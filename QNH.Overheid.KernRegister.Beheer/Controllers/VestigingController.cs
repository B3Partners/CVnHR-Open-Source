using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using QNH.Overheid.KernRegister.Beheer.ViewModels;
using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Business.Model;
using QNH.Overheid.KernRegister.Business.Model.Entities;

using NLog;

using StructureMap;
using StructureMap.Pipeline;
using NHibernate.Criterion;
using QNH.Overheid.KernRegister.Beheer.ViewModel;
using QNH.Overheid.KernRegister.Business.Utility;
using NHibernate.Linq;
using System.Diagnostics;
using System.Data.Entity;
using System.Text;
using QNH.Overheid.KernRegister.Business.Crm;
using QNH.Overheid.KernRegister.Business.Model.EntityComparers;
using Rotativa;
using QNH.Overheid.KernRegister.Business.Service.Users;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_ViewKvKData)]
    public class VestigingController : Controller
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        //
        // GET: /Vestiging/
        public ActionResult Index(string searchcriteria, 
            string page,
            bool all = true,
            bool naam = false,
            bool kvkNummerSearch = false,
            bool vestigingsNummer = false,
            bool naamEigenaar = false,
            bool aantalMedewerkers = false,
            bool sbiCode = false,
            bool adres = false,
            bool postcode = false
            )
        {
            if (String.IsNullOrEmpty(searchcriteria))
            {
                return View(new KvkSearch());
            }
            else
                searchcriteria = searchcriteria.Trim();

            if (string.IsNullOrEmpty(page))
                page = "0";

            var findAll = searchcriteria == "%";

            var sw = new Stopwatch();
            sw.Start();

            // TODO: Add search on RSIN
            // TODO: Move this code out of the controller into a service or repository
            IEnumerable<KvkInschrijving> hits;
            KvkSearch kvkSearch;
            using (var nestedContainer = IocConfig.Container.GetNestedContainer())
            {
                var repo = nestedContainer.GetInstance<IRepository<KvkInschrijving>>();
                var vestigingRepo = nestedContainer.GetInstance<IRepository<Vestiging>>();
                var functieVervullingenRepo = nestedContainer.GetInstance<IRepository<FunctieVervulling>>();
                var SBIRepo = nestedContainer.GetInstance<IRepository<SbiActiviteit>>();
                var vestigingSbiRepo = nestedContainer.GetInstance<IRepository<VestigingSbiActiviteit>>();

                try
                {
                    // Doing a join in Oracle seems to cause a lot of performance issues
                    // The following is 'optimized' for nHibernate + Oracle, but probably performs less for EF + SQL ce
                    int postcodeCijfers = -1;
                    var searchcriteriaLower = searchcriteria.ToLower();
                    var searchcriteriaUpperLastLetters = postcode &&
                                                         !int.TryParse(searchcriteriaLower, out postcodeCijfers)
                        ? new string(searchcriteriaLower.Reverse().Take(2).Reverse().ToArray()).ToUpperInvariant()
                        : null;

                    // bluh...
                    var searchCriteriaLowerAddress = searchcriteriaLower.Contains(',')
                        ? searchcriteriaLower.Split(',')
                        : searchcriteriaLower.Split(' ');
                    var searchCriteriaAddressCity = searchCriteriaLowerAddress.Count() > 1
                        ? searchCriteriaLowerAddress.Last().Trim()
                        : null;
                    var searchCriteriaAddress = searchCriteriaAddressCity == null
                        ? searchcriteriaLower
                        : searchcriteriaLower.Replace(searchCriteriaAddressCity, string.Empty).Trim(',', ' ');

#warning //TODO: create special character search options and test!
                    //if (searchcriteriaLower.Length > 4)
                    //    searchcriteriaLower = ReplaceSpecialCharactersForSqlLike(searchcriteriaLower);

                    if (!findAll)
                    {
                        if (all) // setup all
                        {
                            naam = true;
                            kvkNummerSearch = true;
                            vestigingsNummer = true;
                            naamEigenaar = true;
                            aantalMedewerkers = true;
                            sbiCode = true;
                            adres = true;
                            postcode = true;
                        }

                        var sbiHits = SBIRepo.Query()
                            .Where(i => i.KvKInschrijving.GeldigTot > DateTime.Now)
                            //.Include(i=> i.KvKInschrijving)
                            .Where(i =>
                                sbiCode &&
                                (i.SbiCode.Code.Contains(searchcriteria) ||
                                 i.SbiCode.Omschrijving.ToLower().Contains(searchcriteriaLower))
                            )
                            .Select(i => i.KvKInschrijving)
                            .ToList();

                        var vestigingSbiHits = vestigingSbiRepo.Query()
                            .Where(i => i.Vestiging.KvkInschrijving.GeldigTot > DateTime.Now)
                            //.Include(i=> i.Vestiging)
                            .Where(
                                i =>
                                    sbiCode &&
                                    (i.SbiCode.Code.Contains(searchcriteria) ||
                                     i.SbiCode.Omschrijving.ToLower().Contains(searchcriteriaLower)))
                            .Select(i => i.Vestiging.KvkInschrijving)
                            .ToList();

                        var searchcriteriaInt = 0;
                        var parseIntSucceeded = int.TryParse(searchcriteria, out searchcriteriaInt);
                        var inschrijvingenHits = repo.Query()
                            .Where(i => i.GeldigTot > DateTime.Now)
                            .Include(i => i.Vestigingen)
                            .Where(i =>
                                (kvkNummerSearch && i.KvkNummer == searchcriteria)
                                || (naam && i.Naam.ToLower().Contains(searchcriteriaLower))
                                || (naamEigenaar && i.VolledigeNaamEigenaar.ToLower().Contains(searchcriteriaLower))
                                ||
                                (aantalMedewerkers && parseIntSucceeded &&
                                 Convert.ToInt32(i.TotaalWerkzamePersonen) >= searchcriteriaInt)
                            ).ToList();

                        var vestigingenHits = vestigingRepo.Query()

                            .Where(i => i.GeldigTot > DateTime.Now)
                            .Where(i =>
                                (vestigingsNummer && i.Vestigingsnummer == searchcriteria)
                                || (naam && i.Naam.ToLower().Contains(searchcriteriaLower))
                                || (all && i.RSIN == searchcriteria)
                                ||
                                (
                                    adres &&
                                    // bluh...
                                    // normal query on first
                                    (
                                        i.Woonplaats.ToLower().Contains(searchCriteriaAddress) ||
                                        i.PostWoonplaats.ToLower().Contains(searchCriteriaAddress)
                                        || i.Adres.ToLower().Contains(searchCriteriaAddress) ||
                                        i.PostAdres.ToLower().Contains(searchCriteriaAddress)
                                        || i.PostStraat.ToLower().Contains(searchCriteriaAddress) ||
                                        i.Straat.ToLower().Contains(searchCriteriaAddress)
                                    )
                                    // try matching city on last
                                    &&
                                    (
                                        searchCriteriaAddressCity == null ||
                                        i.Woonplaats.ToLower().Contains(searchCriteriaAddressCity) ||
                                        i.PostWoonplaats.ToLower().Contains(searchCriteriaAddressCity)
                                    )
                                )
                                ||
                                (
#warning //TODO: extend searching for postcode and do something with regexes
                                    postcode &&
                                    (!all &&
                                     (
                                         (searchcriteria.StartsWith(i.PostcodeCijfers) && i.PostcodeCijfers != null)
                                         ||
                                         (searchcriteria.StartsWith(i.PostPostcodeCijfers) &&
                                          i.PostPostcodeCijfers != null)
                                     )
                                     &&
                                     (
                                         string.IsNullOrWhiteSpace(searchcriteriaUpperLastLetters)
                                         || searchcriteriaUpperLastLetters == i.PostcodeLetters ||
                                         searchcriteriaUpperLastLetters == i.PostPostcodeLetters
                                     )
                                    )
                                )
                                || (all && i.BagId.Contains(searchcriteria))
                            )
                            .Select(i => i.KvkInschrijving)
                            .Where(i => i.GeldigTot > DateTime.Now)
                            .Include(i => i.Vestigingen)
                            .Distinct()
                            .ToList();

                        var functieVervullingenHits = functieVervullingenRepo.Query()
                            .Where(fv => naamEigenaar && fv.VolledigeNaam.ToLower().Contains(searchcriteriaLower))
                            .Select(fv => fv.KvkInschrijving)
                            .Where(i => i.GeldigTot > DateTime.Now)
                            .Include(i => i.Vestigingen)
                            .Distinct()
                            .ToList();

                        hits = inschrijvingenHits
                            .Concat(vestigingenHits)
                            .Concat(functieVervullingenHits)
                            .Concat(sbiHits)
                            .Concat(vestigingSbiHits)
                            .Distinct(new FunctionalEqualsComparer<KvkInschrijving>());
                    }
                    else // find all
                    {
                        var hitsPerPage = 50;
                        hits = repo.Query()
                            .Where(i => i.GeldigTot > DateTime.Now)
                            .Include(i => i.Vestigingen)
                            .Skip(hitsPerPage * Convert.ToInt32(page))
                            .Take(hitsPerPage);
                    }


                    kvkSearch = new KvkSearch
                    {
                        KvkSearchCriteria = new KvkSearchCriteria
                        {
                            GlobalCriterium = searchcriteria,
                            Page = page,
                            AantalMedewerkers = !all && aantalMedewerkers,
                            Adres = !all && adres,
                            Postcode = !all && postcode,
                            All = all,
                            KvkNummerSearch = !all && kvkNummerSearch,
                            Naam = !all && naam,
                            NaamEigenaar = !all && naamEigenaar,
                            SBICode = !all && sbiCode,
                            VestigingsNummer = !all && vestigingsNummer

                        },
                        KvkSearchResult = new KvkSearchResult()
                        {
                            TotalFound = findAll ? repo.Query().Count(i => i.GeldigTot > DateTime.Now) : hits.Count(),
                            SearchedAndNothingFound = !hits.Any()
                        }
                    };

                    foreach (KvkInschrijving kvkInschrijving in hits)
                    {
                        if (kvkInschrijving == null)
                        {
#warning //TODO: fix this!
                            logger.Error("kvkInschrijving is null!!!");
                            continue;
                        }

                        if (kvkSearch.KvkSearchResult.KvkItems == null)
                        {
                            continue;
                        }
                        var currentVestigingen = kvkInschrijving.Vestigingen == null
                            ? new List<Vestiging>()
                            : kvkInschrijving.Vestigingen.Where(v => v.GeldigTot > DateTime.Now).ToList();
                        kvkSearch.KvkSearchResult.KvkItems.Add(new KvkItem
                        {
                            KvkNummer = kvkInschrijving.KvkNummer,
                            Naam = kvkInschrijving.InschrijvingNaam, // kvkInschrijving.Naam,
                            Vestigingen = currentVestigingen,
                            NaamEigenaar = kvkInschrijving.VolledigeNaamEigenaar,
                            AantalMedewerkers = kvkInschrijving.FulltimeWerkzamePersonen,

                        });
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Something went wrong while searching using the following criteria {0} ",
                        searchcriteria);
                    throw;
                }

                sw.Stop();
                ViewBag.TimeElapsed = sw.Elapsed;

                return View(kvkSearch);
            }
        }

        [HttpPost]
        public ActionResult Index(KvkSearch kvkSearch)
        {
            return RedirectToAction("Index", new
            {
                searchcriteria = kvkSearch.KvkSearchCriteria.GlobalCriterium, 
                page = kvkSearch.KvkSearchCriteria.Page,
                all = kvkSearch.KvkSearchCriteria.All,
                naam = kvkSearch.KvkSearchCriteria.Naam,
                kvkNummerSearch = kvkSearch.KvkSearchCriteria.KvkNummerSearch,
                vestigingsNummer = kvkSearch.KvkSearchCriteria.VestigingsNummer,
                naamEigenaar = kvkSearch.KvkSearchCriteria.NaamEigenaar,
                aantalMedewerkers = kvkSearch.KvkSearchCriteria.AantalMedewerkers,
                sbiCode = kvkSearch.KvkSearchCriteria.SBICode,
                adres = kvkSearch.KvkSearchCriteria.Adres,
                postcode = kvkSearch.KvkSearchCriteria.Postcode
            });
        }

        public ActionResult Details(string kvkNummer)
        {
            var sw = new Stopwatch();
            sw.Start();

            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
            var inschrijving = repo.GetLatestInschrijving(kvkNummer);

            var kvkItem = inschrijving == null
                ? null
                : new KvkItem()
                    {
                        KvkNummer = inschrijving.KvkNummer,
                        Naam = inschrijving.InschrijvingNaam, // inschrijving.Naam,
                        Vestigingen = inschrijving.Vestigingen.ToList(),
                        PeilMoment = inschrijving.Peilmoment,
                        Inschrijving = inschrijving,
                        NaamEigenaar = inschrijving.VolledigeNaamEigenaar,
                        AantalMedewerkers = inschrijving.FulltimeWerkzamePersonen
                    };

            sw.Stop();
            ViewBag.TimeElapsed = sw.Elapsed;
            
            return View(kvkItem);
        }

        public ActionResult VestigingDetails(string kvknummer, string vestigingId)
        {
            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();

            var inschrijving = repo.GetLatestInschrijving(kvknummer);

            return View(inschrijving == null || !inschrijving.Vestigingen.Any()
                ? null
                : inschrijving.Vestigingen.FirstOrDefault(v => v.Vestigingsnummer == vestigingId));
        }

        public ActionResult Delete(string kvkNummer)
        {
            if(!User.IsAllowedAllActions(ApplicationActions.CVnHR_ManageKvKData))
                return RedirectToAction("AccessDenied", "Users", new { actions = ApplicationActions.CVnHR_ManageKvKData });

            KvkInschrijving inschrijving = null;

            try
            {
                using (var nestedContainer = IocConfig.Container.GetNestedContainer())
                {
                    var repo = nestedContainer.GetInstance<IKvkInschrijvingRepository>();
                    inschrijving = repo.GetLatestInschrijving(kvkNummer);

                    if (inschrijving != null)
                    {
                        repo.Remove(inschrijving);
                    }
                    else
                    {
                        return View(new KvKItemDeleteResult
                        {
                            Success = true,
                            AlreadyDeleted = true,
                            KvKNummer = kvkNummer
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return View(new KvKItemDeleteResult { 
                    Success =false,
                    Exception = ex,
                    KvKNummer = kvkNummer,
                    InschrijvingNaam = inschrijving == null ? "Onbekend" : inschrijving.InschrijvingNaam // inschrijving.Naam
                });
            }

            return View(new KvKItemDeleteResult
            {
                Success = true,
                Exception = null,
                KvKNummer = kvkNummer,
                InschrijvingNaam = inschrijving == null ? "Onbekend" : inschrijving.InschrijvingNaam // inschrijving.Naam
            });
        }

        public ActionResult Export(string kvkNummer, bool createNew = false, bool immediatelyCreateNewIfExists = true)
        {
            if (!User.IsAllowedAllActions(ApplicationActions.CVnHR_ManageCrm))
                return RedirectToAction("AccessDenied", "Users", new { actions = ApplicationActions.CVnHR_ManageCrm });

            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
            var inschrijving = repo.GetLatestInschrijving(kvkNummer);

            var kvkItem = new KvkItem()
            {
                KvkNummer = inschrijving.KvkNummer,
                Naam = inschrijving.InschrijvingNaam, // inschrijving.Naam,
                Vestigingen = inschrijving.Vestigingen.ToList(),
                NaamEigenaar = inschrijving.VolledigeNaamEigenaar,
                AantalMedewerkers = inschrijving.FulltimeWerkzamePersonen
            };

            var exportService = IocConfig.Container.GetInstance<IExportService>();

            IExportResult result;
            if (createNew)
                result = exportService.InsertExternalRecord(inschrijving);
            else
            {
                result = exportService.UpdateExternalRecord(inschrijving);

                if (result.NoItemsFoundInsertInstead && immediatelyCreateNewIfExists)
                    result = exportService.InsertExternalRecord(inschrijving);

            }

            return View(new CrmExportResult(result.Succes, result.Message, result.NoItemsFoundInsertInstead, result.Errors) { KvkItem = kvkItem });
        }

        public ActionResult ExportVestiging(string vestigingNummer, bool createNew = false, bool immediatelyCreateNewIfExists = true)
        {
            if (!User.IsAllowedAllActions(ApplicationActions.CVnHR_ManageCrm))
                return RedirectToAction("AccessDenied", "Users", new { actions = ApplicationActions.CVnHR_ManageCrm });

            var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
            var vestiging = repo.GetLatestVestiging(vestigingNummer);

            var exportService = IocConfig.Container.GetInstance<IExportService>();

            IExportResult result;
            if (createNew)
                result = exportService.InsertExternalVestiging(vestiging);
            else
            {
                result = exportService.UpdateExternalVestiging(vestiging);

                if (result.NoItemsFoundInsertInstead && immediatelyCreateNewIfExists)
                    result = exportService.InsertExternalVestiging(vestiging);
            }

            var kvkItem = new KvkItem()
            {
                KvkNummer = vestiging.KvkInschrijving.KvkNummer,
                Naam = vestiging.KvkInschrijving.InschrijvingNaam, // vestiging.KvkInschrijving.Naam,
                Vestigingen = new List<Vestiging> { vestiging },
                NaamEigenaar = vestiging.Naam,
                AantalMedewerkers = vestiging.TotaalWerkzamePersonen
            };

            return View("Export", new CrmExportResult(result.Succes, result.Message, result.NoItemsFoundInsertInstead, result.Errors) { KvkItem = kvkItem });
        }

        public ActionResult ExportDebiteuren(string kvkNummer, bool createNew = false,
            bool immediatelyCreateNewIfExists = true)
        {
            return Export(kvkNummer: kvkNummer, 
                createNew: createNew,
                immediatelyCreateNewIfExists: immediatelyCreateNewIfExists, 
                type: FinancialProcesType.ProbisDebiteuren);
        }

        public ActionResult ExportVestigingDebiteuren(string vestigingNummer, bool createNew = false,
            bool immediatelyCreateNewIfExists = true)
        {
            return ExportVestiging(vestigingNummer: vestigingNummer,
                createNew: createNew,
                immediatelyCreateNewIfExists: immediatelyCreateNewIfExists,
                type: FinancialProcesType.ProbisDebiteuren);
        }

        public ActionResult ExportCrediteuren(string kvkNummer, bool createNew = false,
            bool immediatelyCreateNewIfExists = true)
        {
            return Export(kvkNummer: kvkNummer,
                createNew: createNew,
                immediatelyCreateNewIfExists: immediatelyCreateNewIfExists,
                type: FinancialProcesType.ProbisCrediteuren);
        }

        public ActionResult ExportVestigingCrediteuren(string vestigingNummer, bool createNew = false,
            bool immediatelyCreateNewIfExists = true)
        {
            return ExportVestiging(vestigingNummer: vestigingNummer,
                createNew: createNew,
                immediatelyCreateNewIfExists: immediatelyCreateNewIfExists,
                type: FinancialProcesType.ProbisCrediteuren);
        }

        private ActionResult Export(string kvkNummer, bool createNew, bool immediatelyCreateNewIfExists,
            FinancialProcesType type)
        {
            if ((type == FinancialProcesType.ProbisDebiteuren && !User.IsAllowedAllActions(ApplicationActions.CVnHR_Debiteuren)) 
                || (type == FinancialProcesType.ProbisCrediteuren && !User.IsAllowedAllActions(ApplicationActions.CVnHR_Crediteuren)))
            {
                var msg = $"Action Export for type {type} not allowed for user! {User.Identity.Name}";
                logger.Warn(msg);
                return View("Export", new CrmExportResult(false, msg, false, new[] { msg }));
            }

            try
            {
                var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
                var inschrijving = repo.GetLatestInschrijving(kvkNummer);

                var kvkItem = new KvkItem()
                {
                    KvkNummer = inschrijving.KvkNummer,
                    Naam = inschrijving.InschrijvingNaam, // inschrijving.Naam,
                    Vestigingen = inschrijving.Vestigingen.ToList(),
                    NaamEigenaar = inschrijving.VolledigeNaamEigenaar,
                    AantalMedewerkers = inschrijving.FulltimeWerkzamePersonen
                };

                var exportService = IocConfig.Container.GetInstance<IFinancialExportService>();

                IExportResult result;
                if (createNew)
                    result = exportService.Insert(inschrijving, type);
                else
                {
                    result = exportService.Update(inschrijving, type);

                    if (result.NoItemsFoundInsertInstead && immediatelyCreateNewIfExists)
                        result = exportService.Insert(inschrijving, type);
                }

                return View("Export", new CrmExportResult(result.Succes, result.Message, result.NoItemsFoundInsertInstead, result.Errors)
                {
                    KvkItem = kvkItem,
                    FinancialProcesType = type
                });
            }
            catch (Exception ex)
            {
                var msg = $"Error in Export! Error message: {ex.Message}";
                logger.Error(ex, msg);
                return View("Export", new CrmExportResult(false, msg, false, new[] { ex.Message })
                {
                    KvkItem = new KvkItem(),
                    FinancialProcesType = type
                });
            }
        }

        private ActionResult ExportVestiging(string vestigingNummer, bool createNew, bool immediatelyCreateNewIfExists, FinancialProcesType type)
        {
            if ((type == FinancialProcesType.ProbisDebiteuren && !User.IsAllowedAllActions(ApplicationActions.CVnHR_Debiteuren))
                || (type == FinancialProcesType.ProbisCrediteuren && !User.IsAllowedAllActions(ApplicationActions.CVnHR_Crediteuren)))
            {
                var msg = $"Action ExportVestiging for type {type} not allowed for user! {User.Identity.Name}";
                logger.Warn(msg);
                return View("Export", new CrmExportResult(false, msg, false, new[] { msg }));
            }

            try
            { 
                var repo = IocConfig.Container.GetInstance<IKvkInschrijvingRepository>();
                var vestiging = repo.GetLatestVestiging(vestigingNummer);

                var exportService = IocConfig.Container.GetInstance<IFinancialExportService>();

                IExportResult result;
                if (createNew)
                    result = exportService.InsertVestiging(vestiging, type);
                else
                {
                    result = exportService.UpdateVestiging(vestiging, type);

                    if (result.NoItemsFoundInsertInstead && immediatelyCreateNewIfExists)
                        result = exportService.InsertVestiging(vestiging, type);
                }

                var kvkItem = new KvkItem()
                {
                    KvkNummer = vestiging.KvkInschrijving.KvkNummer,
                    Naam = vestiging.KvkInschrijving.InschrijvingNaam, // vestiging.KvkInschrijving.Naam,
                    Vestigingen = new List<Vestiging> { vestiging },
                    NaamEigenaar = vestiging.Naam,
                    AantalMedewerkers = vestiging.TotaalWerkzamePersonen
                };

                return View("Export", new CrmExportResult(result.Succes, result.Message, result.NoItemsFoundInsertInstead, result.Errors)
                {
                    KvkItem = kvkItem,
                    FinancialProcesType = type
                });
            }
            catch (Exception ex)
            {
                var msg = $"Error in ExportVestiging! Error message: {ex.Message}";
                logger.Error(ex, msg);
                return View("Export", new CrmExportResult(false, msg, false, new[] { ex.Message })
                {
                    KvkItem = new KvkItem(),
                    FinancialProcesType = type
                });
            }
        }
    }
}