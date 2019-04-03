using Newtonsoft.Json;
using NLog;
using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Beheer.ViewModels;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Enums;
using QNH.Overheid.KernRegister.Business.KvKSearchApi;
using QNH.Overheid.KernRegister.Business.KvKSearchApi.Entities;
using QNH.Overheid.KernRegister.Business.Service;
using QNH.Overheid.KernRegister.Business.Service.BRMO;
using QNH.Overheid.KernRegister.Business.Service.Users;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_ViewKvKData)]
    public class SearchController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IKvkSearchApi _kvkSearchApi;

        public SearchController(IKvkSearchApi kvkSearchApi)
        {
            _kvkSearchApi = kvkSearchApi;
        }

        //
        // GET: /Search/
        public ActionResult Index(string kvkNummer)
        {
            if (string.IsNullOrEmpty(kvkNummer))
            {
                return View(new KvkSearch() { });
            }
            var service = IocConfig.Container.GetInstance<IKvkSearchService>();

            try
            {
                var kvkInschrijving = service.SearchInschrijvingByKvkNummer(kvkNummer, User.GetUserName());

                // Vul het viewmodel met gegevens uit kvkVermelding
                if (kvkInschrijving != null)
                {
                    var kvkSearch = new KvkSearch
                    {
                        KvkSearchCriteria = { KvkNummer = kvkInschrijving.KvkNummer },
                        KvkSearchResult =
                            new KvkSearchResult
                            {
                                KvkItems =
                                    new List<KvkItem>
                                    {
                                        new KvkItem()
                                        {
                                            KvkNummer = kvkNummer,
                                            Naam = kvkInschrijving.InschrijvingNaam, // kvkInschrijving.Naam,
                                            Vestigingen = kvkInschrijving.Vestigingen.ToList(),
                                            NaamEigenaar = kvkInschrijving.VolledigeNaamEigenaar,
                                            AantalMedewerkers = kvkInschrijving.FulltimeWerkzamePersonen
                                            
                                        }
                                    }
                            }
                    };

                    return View(kvkSearch);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Something went wrong while searching for Inschrijving with kvknummer: " + kvkNummer);

                return View(new KvkSearch
                {
                    KvkSearchResult = new KvkSearchResult
                    {
                        SearchError = ex
                    }
                });
            }

            var emptySearch = new KvkSearch();
            emptySearch.KvkSearchResult.SearchedAndNothingFound = true;

            return View(emptySearch);
        }

        [HttpPost]
        public ActionResult Index(KvkSearch kvkSearch)
        {
            return RedirectToAction("Index", new {kvkNummer = kvkSearch.KvkSearchCriteria.KvkNummer});
        }

        [HttpPost]
        public async Task<ActionResult> SearchByName(string name, int startPage = 0)
        {
            var kvkSearchApiParameters = new KvkSearchApiParameters() { Q = name, StartPage = startPage };
            var result = await _kvkSearchApi.Search(kvkSearchApiParameters);
            return Json(result);
        }

        public ActionResult Import(string kvkNummer, 
            string vestigingNummer = null,
            bool toCrm = false,
            bool toBrmo = false,
            bool toDebiteuren = false,
            bool toCrediteuren = false)
        {
            var anyExport = toCrm || toDebiteuren || toCrediteuren || toBrmo;
            if (!anyExport && !User.IsAllowedAllActions(ApplicationActions.CVnHR_ManageKvKData))
                return RedirectToAction("AccessDenied", "Users", new { actions = ApplicationActions.CVnHR_ManageKvKData });
            else if (toCrm && !User.IsAllowedAllActions(ApplicationActions.CVnHR_ManageCrm))
                return RedirectToAction("AccessDenied", "Users", new { actions = ApplicationActions.CVnHR_ManageCrm });
            else if (toDebiteuren && !User.IsAllowedAllActions(ApplicationActions.CVnHR_Debiteuren))
                return RedirectToAction("AccessDenied", "Users", new { actions = ApplicationActions.CVnHR_Debiteuren });
            else if (toCrediteuren && !User.IsAllowedAllActions(ApplicationActions.CVnHR_Crediteuren))
                return RedirectToAction("AccessDenied", "Users", new { actions = ApplicationActions.CVnHR_Crediteuren });
            else if (toBrmo)
            {
                if (!User.IsAllowedAllActions(ApplicationActions.CVnHR_Brmo))
                    return RedirectToAction("AccessDenied", "Users", new { actions = ApplicationActions.CVnHR_Brmo });
                else if(!SettingsHelper.BrmoApplicationEnabled)
                    return RedirectToAction("Index");
            } 

            using (var nestedContainer = IocConfig.Container.GetNestedContainer())
            {
                if (toBrmo)
                {
                    var service = IocConfig.Container.GetInstance<IKvkSearchService>();

                    // for now always bypass cache to ensure correct version in cache.
                    var kvkInschrijving = service.SearchInschrijvingByKvkNummer(kvkNummer, User.GetUserName(), true);

                    var msg = "";
                    var brmostatus = AddInschrijvingResultStatus.Error;
                    try
                    {
                        // retry with bypassing cache
                        var xDoc = RawXmlCache.Get(kvkNummer,
                            () => { kvkInschrijving = service.SearchInschrijvingByKvkNummer(kvkNummer, User.GetUserName(), true); });

                        var brmoSyncService = nestedContainer.GetInstance<IBrmoSyncService>();
                        brmostatus = brmoSyncService.UploadXDocumentToBrmo(xDoc);
                    }
                    catch (Exception ex)
                    {
                        msg += " " + ex.Message;
                        logger.Warn(ex, msg);
                    }
                    return
                        View(new ImportResultViewModel()
                        {
                            KvkInschrijving = kvkInschrijving,
                            Status = brmostatus,
                            Message = msg
                        });
                }
                else
                {
                    var service = IocConfig.Container.GetInstance<IKvkSearchService>();
                    var kvkInschrijving = service.SearchInschrijvingByKvkNummer(kvkNummer, User.GetUserName());

                    var storageService = nestedContainer.GetInstance<IInschrijvingSyncService>();
                    var status = storageService.AddNewInschrijvingIfDataChanged(kvkInschrijving);

                    if (toCrm)
                    {
                        return string.IsNullOrEmpty(vestigingNummer)
                            ? RedirectToAction("Export", "Vestiging", new { kvkNummer = kvkNummer })
                            : RedirectToAction("ExportVestiging", "Vestiging", new { vestigingNummer = vestigingNummer });
                    }

                    if (toDebiteuren)
                    {
                        return string.IsNullOrEmpty(vestigingNummer)
                            ? RedirectToAction("ExportDebiteuren", "Vestiging", new { kvkNummer = kvkNummer })
                            : RedirectToAction("ExportVestigingDebiteuren", "Vestiging", new { vestigingNummer = vestigingNummer });
                    }

                    if (toCrediteuren)
                    {
                        return string.IsNullOrEmpty(vestigingNummer)
                            ? RedirectToAction("ExportCrediteuren", "Vestiging", new { kvkNummer = kvkNummer })
                            : RedirectToAction("ExportVestigingCrediteuren", "Vestiging", new { vestigingNummer = vestigingNummer });
                    }

                    var result = new ImportResultViewModel() { KvkInschrijving = kvkInschrijving, Status = status };

                    return View(result);
                }
            }
        }

        public ActionResult Details(string kvknummer)
        {
            var service = IocConfig.Container.GetInstance<IKvkSearchService>();
            var kvkInschrijving = service.SearchInschrijvingByKvkNummer(kvknummer, User.GetUserName());
            
            var kvkItem = new KvkItem()
            {
                KvkNummer = kvkInschrijving.KvkNummer,
                Naam = kvkInschrijving.InschrijvingNaam, // kvkInschrijving.Naam,
                PeilMoment = kvkInschrijving.Peilmoment,
                Vestigingen = kvkInschrijving.Vestigingen.ToList(),
                Inschrijving = kvkInschrijving
            };

            return View(kvkItem);
            
        }

        public ActionResult VestigingDetails(string kvknummer, string vestigingId)
        {
            var service = IocConfig.Container.GetInstance<IKvkSearchService>();
            return View(service.SearchVestigingByVestigingsNummer(vestigingId, kvknummer));
        }

        public ActionResult Raw(string kvknummer, string type = "json")
        {
            var service = IocConfig.Container.GetInstance<IKvkSearchService>();
            switch (type)
            {
                case "xml":
                    // retry with bypassing cache
                    var xDoc = RawXmlCache.Get(kvknummer,
                        () => { service.SearchInschrijvingByKvkNummer(kvknummer, User.GetUserName(), true); });

                    return Content(xDoc.ToString(), "application/xml", Encoding.UTF8);
                case "json":
                default:
                    
                    var result = service.GetInschrijvingResponseTypeByKvkNummer(kvknummer);

                    var jsonSerializerSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    };
                    var json = JsonConvert.SerializeObject(result, jsonSerializerSettings);

                    return Content(json, "application/json", Encoding.UTF8);
            }
            
        }
    }
}