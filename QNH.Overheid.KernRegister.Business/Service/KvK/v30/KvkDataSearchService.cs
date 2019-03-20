using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;
using NLog;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.KvK.Exceptions;
using QNH.Overheid.KernRegister.Business.KvK.v30;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Service.KvK.v30
{
    public class KvkDataSearchService : IKvkSearchService
    {
        public const string VALUENOTPROVIDED = "[Not provided]";

        private readonly Dataservice _service;

        private readonly string _klantReferentie;
        private readonly int _cacheInHours;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Logger KvkCountingLogger = LogManager.GetLogger("kvkCountingLogger");
        private static readonly Logger KvkErrorLogger = LogManager.GetLogger("kvkerror");

        private static readonly ConcurrentDictionary<string, KvkInschrijving> _cache = new ConcurrentDictionary<string, KvkInschrijving>();

        private bool CacheEnabled => _cacheInHours > 0;

        public KvkDataSearchService(Dataservice service)
        {
            _service = service;
        }

        public KvkDataSearchService(Dataservice service, string klantReferentie, int cacheInHours)
            : this(service)
        {
            _klantReferentie = klantReferentie;
            _cacheInHours = cacheInHours;
        }

        public KvkInschrijving SearchInschrijvingByKvkNummer(string kvkNummer, string requesterName, bool bypassCache = false)
        {
            if (string.IsNullOrWhiteSpace(kvkNummer))
                return null;
            else
                kvkNummer = kvkNummer.Trim();

            // First check the cache
            if (!bypassCache && CacheEnabled && _cache.ContainsKey(kvkNummer))
            {
                var inschrijving = _cache[kvkNummer];
                if (inschrijving.OpgevraagdOp.AddHours(_cacheInHours) > DateTime.Now)
                    return inschrijving;
            }

            // Roep "Dataservice Inschrijving" aan met kvkNummer als sleutel
            // Via deze service verkrijgen we ook de vestigingsnummers (of zelfs complete vestiging informatie!?) van de hoofdvestiging en de nevenvestigingen
            var maatschappelijkeActiviteit = GeefMaatschappelijkeActiviteit(kvkNummer, requesterName, out string peilmoment, out IEnumerable<ValidationMessage> errors);

            KvkInschrijving kvkInschrijving = null;
            if (maatschappelijkeActiviteit != null)
            {
                try
                {
                    kvkInschrijving = FillKvkVermelding(maatschappelijkeActiviteit);
                    kvkInschrijving.Peilmoment = peilmoment;
                    kvkInschrijving.OpgevraagdOp = DateTime.Now;
                }
                catch (MissingHoofdVestigingException ex)
                {
                    var msg = $"Ontbrekende hoofdvestiging voor KVKnummer {kvkNummer}.";
                    Logger.Warn(msg);
                    throw new MissingHoofdVestigingException(msg, ex);
                }
                //catch (TooManyHoofdVestigingenException)
                //{
                //    var msg = "Te veel hoofdvestigingen voor KVKnummer {kvkNummer}";
                //    logger.Warn(msg);
                //}
            }
            else if (errors != null)
            {
                var msg = "Foutmelding bij KVKnummer " + kvkNummer + ": " + string.Join("; ", errors.Select(e => e.MessageType + ": " + e.Message));
                Logger.Error(msg);

                // Log the specific kvk Error
                KvkErrorLogger.Error(kvkNummer + " | " + string.Join("; ", errors.Select(e => e.MessageType + ": " + e.Message)));

                throw new KvkServerException(msg);
            }

            // Add the current requested inschrijving to cache
            if (CacheEnabled)
            {
                if (!_cache.ContainsKey(kvkNummer))
                    _cache.TryAdd(kvkNummer, kvkInschrijving);
                else
                    _cache[kvkNummer] = kvkInschrijving;
            }

            return kvkInschrijving;
        }

        public Vestiging SearchVestigingByVestigingsNummer(string vestigingsNummer, string kvkNummer = null)
        {
            // If a vestiging already exists in the cache by its kvkNummer, 
            // get that first, otherwise request the vestiging directly
            if (CacheEnabled && kvkNummer != null)
            {
                // First check the cache
                if (_cache.ContainsKey(kvkNummer))
                {
                    var inschrijving = _cache[kvkNummer];
                    if (inschrijving.OpgevraagdOp.AddHours(_cacheInHours) > DateTime.Now)
                    {
                        var vestiging = inschrijving.Vestigingen.FirstOrDefault(v => v.Vestigingsnummer == vestigingsNummer);
                        if (vestiging != null)
                            return vestiging;
                    }
                }
            }

            // No vestiging found in Cache so request from the service
            var request = new KernRegister.Business.KvK.v30.ophalenVestigingRequest()
            {
                ophalenVestigingRequest1 = new KernRegister.Business.KvK.v30.VestigingRequestType()
                {
                    klantreferentie = _klantReferentie,
                    Item = vestigingsNummer,
                    ItemElementName = ItemChoiceType1.vestigingsnummer
                }
            };
            var result = _service.ophalenVestiging(request)?.ophalenVestigingResponse1;

            return CreateVestiging(result?.product?.Item);
        }

        /// <summary>
        /// </summary>
        /// <param name="maatschappelijkeActiviteit"></param>
        /// <returns>KvkVermelding and peilmoment as string</returns>
        private KvkInschrijving FillKvkVermelding(MaatschappelijkeActiviteitType maatschappelijkeActiviteit)
        {
            var kvkInschrijving = new KvkInschrijving
            {
                KvkNummer = maatschappelijkeActiviteit.kvkNummer,
                Naam = maatschappelijkeActiviteit.naam,
            };

            // Add handelsnamen
            if (maatschappelijkeActiviteit.manifesteertZichAls != null
                && maatschappelijkeActiviteit.manifesteertZichAls.onderneming != null
                && maatschappelijkeActiviteit.manifesteertZichAls.onderneming.handeltOnder != null)
            {
                var handelsnamen = maatschappelijkeActiviteit.manifesteertZichAls.onderneming.handeltOnder.Select(o => new HandelsNaam
                {
                    Handelsnaam = o.handelsnaam.naam
                });
                if (handelsnamen.Any())
                {
                    //kvkInschrijving.Naam += KvkInschrijving.Separator + string.Join(KvkInschrijving.Separator, handelsnamen);
                    kvkInschrijving.HandelsNamen = handelsnamen.ToList();
                }
            }

            // Voeg alle raadpleeg informatie toe
            kvkInschrijving.SetRaadpleegInformatie(maatschappelijkeActiviteit);

            // Er zijn drie hoofdentiteiten die aangeven hoe een kvk inschrijving er uit ziet:
            // - manifesteertZichAls
            // - WordtGeleidVanuit
            // - HeeftAlsEigenaar

            // Voeg hoofdvestiging toe
            VestigingRelatieType geleidVanuit = maatschappelijkeActiviteit.wordtGeleidVanuit;

            // Check of er uberhaupt een hoofdvestiging is,
            // Als dat niet het geval is er altijd een eigenaar, als zijnde rechtspersoon
            // Dit heeft te maken met de rechtsvorm
            // De MissingHoofdVestigingException mag dus ook nooit voorkomen. 
            if (geleidVanuit == null)
            {
                Vestiging vestiging = CreateVestiging(maatschappelijkeActiviteit.heeftAlsEigenaar.Item);
                if (vestiging != null)
                {
                    kvkInschrijving.Vestigingen.Add(vestiging);
                    return kvkInschrijving;
                }
                else
                    throw new MissingHoofdVestigingException();
            }

            Vestiging hoofdvestiging = CreateVestiging(geleidVanuit.Item);
#warning //TODO: check if all info is there  //SearchVestigingByVestigingsNummer(geleidVanuitItem.Item.vestigingsnummer);
            if (hoofdvestiging != null)
            {
                hoofdvestiging.IsHoofdvestiging = true;
                kvkInschrijving.Vestigingen.Add(hoofdvestiging);
            }

            // Vind alle niet commerciele nevenvestigingen
            if (maatschappelijkeActiviteit.wordtUitgeoefendIn != null && maatschappelijkeActiviteit.wordtUitgeoefendIn.Any())
                foreach (var vestiging in maatschappelijkeActiviteit.wordtUitgeoefendIn)
                {
                    Vestiging nevenvestiging = CreateVestiging(vestiging.nietCommercieleVestiging);
                    if (nevenvestiging == null)
                        continue;

                    // Het komt blijkbaar voor dat een Hoofdvestiging ook als nevenvestiging vermeld wordt.
                    // Om te voorkomen dat een vestiging dubbel toegevoegd wordt
                    if (kvkInschrijving.Vestigingen.Any(v => v.Vestigingsnummer == nevenvestiging.Vestigingsnummer))
                        continue;

                    nevenvestiging.IsHoofdvestiging = false;
                    kvkInschrijving.Vestigingen.Add(nevenvestiging);
                }

#warning //TODO: check if indeed this may or can happen? And is this the correct solution?
            if (maatschappelijkeActiviteit.manifesteertZichAls == null)
            {
                Vestiging vestiging = CreateVestiging(maatschappelijkeActiviteit.heeftAlsEigenaar.Item);
                if (vestiging != null)
                    kvkInschrijving.Vestigingen.Add(vestiging);
                return kvkInschrijving;
            }


            // Voeg nevenvestigingen toe
            var uitGeoefendIn = maatschappelijkeActiviteit.manifesteertZichAls.onderneming.wordtUitgeoefendIn;
            foreach (var vestiging in uitGeoefendIn)
            {
                Vestiging nevenvestiging = CreateVestiging(vestiging.commercieleVestiging);
#warning // TODO: check if all info is there  // SearchVestigingByVestigingsNummer(vestiging.commercieleVestiging.vestigingsnummer);
                if (nevenvestiging == null)
                    continue;

                // Het komt blijkbaar voor dat een Hoofdvestiging ook als nevenvestiging vermeld wordt.
                // Om te voorkomen dat een vestiging dubbel toegevoegd wordt
                if (kvkInschrijving.Vestigingen.Any(v => v.Vestigingsnummer == nevenvestiging.Vestigingsnummer))
                    continue;

                nevenvestiging.IsHoofdvestiging = false;
                kvkInschrijving.Vestigingen.Add(nevenvestiging);
            }

            return kvkInschrijving;
        }

        private MaatschappelijkeActiviteitType GeefMaatschappelijkeActiviteit(string kvkNummer, string requesterName, out string peilmoment, out IEnumerable<ValidationMessage> errors)
        {
            errors = null;
            MaatschappelijkeActiviteitType maatschappelijkeActiviteit = null;
            peilmoment = null;
            try
            {
                var result = DoeOpvragingBijKvk(kvkNummer, requesterName);
                if (!KvkMaatschappelijkeActiviteitProductValidator.HasErrors(result))
                {
                    maatschappelijkeActiviteit = result.product.maatschappelijkeActiviteit;
                    Debug.Assert(maatschappelijkeActiviteit != null,
                        "Because of validation maatschappelijkeActiviteit != null");
                    peilmoment = result.peilmoment;
                }
                else
                {
                    errors = KvkMaatschappelijkeActiviteitProductValidator.GetAllRuleViolations(result);
                }
            }
            catch (FaultException ex)
            {
                var fault = ex.CreateMessageFault();
                if (fault.HasDetail)
                {
                    var f = fault.GetReaderAtDetailContents();
                    var x = f.ReadOuterXml();
                    var invalidXml = RawXmlCache.Get(kvkNummer);
                    errors = new List<ValidationMessage>()
                    {
                        new ValidationMessage($"Er is een fout opgetreden bij de KvK. FoutBericht(en): {ex.Message} {ex.Reason} - {x}; xml: {invalidXml}",
                                                ValidationMessageType.Error)
                    };
                }
                else
                {
                    throw;
                }
            }

            return maatschappelijkeActiviteit;
        }

        public InschrijvingResponseType DoeOpvragingBijKvk(string kvkNummer, string requesterName)
        {
            KvkCountingLogger.Info($"Calling HR-Dataservice v3.0 {kvkNummer} {requesterName}");

            var request = new ophalenInschrijvingRequest()
            {
                ophalenInschrijvingRequest1 = new InschrijvingRequestType()
                {
                    klantreferentie = _klantReferentie,
                    Item = kvkNummer,
                    ItemElementName = ItemChoiceType.kvkNummer
                }
            };
            return _service.ophalenInschrijving(request)?.ophalenInschrijvingResponse1;
        }

        #region Create Vestiging

        /// <summary>
        /// Creates a Vestiging instance from the basisType type.
        /// </summary>
        /// <param name="basisType"></param>
        /// <returns></returns>
        private Vestiging CreateVestiging(BasisType basisType)
        {
            if (basisType == null)
                return null;

            Vestiging vestiging = null;
            if (basisType is VestigingType)
                vestiging = CreateVestigingFromVestigingType((VestigingType)basisType);
            else if (basisType is PersoonType)
                vestiging = CreateVestigingFromPersoonType((PersoonType)basisType);
            else
                throw new NotImplementedException("Cannot create Vestiging from derived type " + basisType.GetType() + ": not yet implemented.");

            // Voeg registratie moment toe
            if (basisType.registratie != null)
            {
                vestiging.RegistratieDatumAanvang = basisType.registratie.datumAanvang;
                vestiging.RegistratieDatumEinde = basisType.registratie.datumEinde; // can be null or empty!
            }
            else
                vestiging.RegistratieDatumAanvang = DateTime.MinValue.ToString("yyyyMMdd"); // VALUENOTPROVIDED;

            // Voeg geldigheid toe
            vestiging.GeldigTot = DateTime.MaxValue;
            vestiging.IngevoegdOp = DateTime.Now.Date;

            return vestiging;
        }

        private Vestiging CreateVestigingFromVestigingType(VestigingType vestigingType)
        {
            if (vestigingType is CommercieleVestigingType)
            {
                var commercieleVestiging = vestigingType as CommercieleVestigingType;

                Vestiging vestiging = null;
                if (commercieleVestiging != null)
                {
                    vestiging = new Vestiging
                    {
                        Naam = commercieleVestiging.handeltOnder[0].handelsnaam.naam,
                        Vestigingsnummer = commercieleVestiging.vestigingsnummer
                    };

                    vestiging.SetVestigingAdres(commercieleVestiging.bezoekLocatie);
                    if (commercieleVestiging.postLocatie == null)
                        commercieleVestiging.postLocatie = commercieleVestiging.bezoekLocatie;
                    vestiging.SetVestigingPostAdres(commercieleVestiging.postLocatie);
                    vestiging.SetVestigingCommunicatieGegevens(commercieleVestiging.communicatiegegevens);

                    var onderneming = commercieleVestiging.isEenUitoefeningVan?.onderneming;
                    var eigenaar = onderneming?.isEenManifestatieVan?.maatschappelijkeActiviteit?.heeftAlsEigenaar.Item;
                    if (eigenaar is NietNatuurlijkPersoonType)
                    {
                        vestiging.RSIN = (eigenaar as NietNatuurlijkPersoonType).rsin;
                    }
                }

                // Add the activeiten as a many to many relationship
                if (commercieleVestiging.activiteiten != null)
                {
                    foreach (var sbiActiviteit in commercieleVestiging.activiteiten.sbiActiviteit)
                    {
                        // Apparently it can happen an activiteit is added as both hoofd and neven activiteit
                        if (vestiging.SbiActiviteiten.Any(a => a.SbiCode.Code == sbiActiviteit.sbiCode.code))
                            continue;

                        vestiging.SbiActiviteiten.Add(new VestigingSbiActiviteit()
                        {
                            SbiCode = new SbiCode() { Code = sbiActiviteit.sbiCode.code, Omschrijving = sbiActiviteit.sbiCode.omschrijving },
                            Vestiging = vestiging,
                            IsHoofdSbiActiviteit = sbiActiviteit.isHoofdactiviteit.omschrijving == "ja" // TODO!
                        });
                    }
                }

                vestiging.FulltimeWerkzamePersonen = commercieleVestiging.voltijdWerkzamePersonen ?? "0";
                vestiging.ParttimeWerkzamePersonen = commercieleVestiging.deeltijdWerkzamePersonen ?? "0";
                vestiging.TotaalWerkzamePersonen = commercieleVestiging.totaalWerkzamePersonen ?? "0";

                return vestiging;
            }
            else if (vestigingType is NietCommercieleVestigingType)
            {
                var nietCommercieleVestiging = vestigingType as NietCommercieleVestigingType;

                Vestiging vestiging = null;
                if (nietCommercieleVestiging != null)
                {
                    vestiging = new Vestiging
                    {
                        Naam = nietCommercieleVestiging.eersteHandelsnaam,
                        Vestigingsnummer = nietCommercieleVestiging.vestigingsnummer
                    };

                    vestiging.SetVestigingAdres(nietCommercieleVestiging.bezoekLocatie);
                    if (nietCommercieleVestiging.postLocatie == null)
                        nietCommercieleVestiging.postLocatie = nietCommercieleVestiging.bezoekLocatie;
                    vestiging.SetVestigingPostAdres(nietCommercieleVestiging.postLocatie);
                    vestiging.SetVestigingCommunicatieGegevens(nietCommercieleVestiging.communicatiegegevens);

                    var eigenaar = nietCommercieleVestiging.isEenUitoefeningVan?.maatschappelijkeActiviteit?.heeftAlsEigenaar.Item;
                    if (eigenaar is NietNatuurlijkPersoonType)
                    {
                        vestiging.RSIN = (eigenaar as NietNatuurlijkPersoonType).rsin;
                    }
                }

                // Add the activeiten as a many to many relationship
                foreach (var sbiActiviteit in nietCommercieleVestiging.activiteiten.sbiActiviteit)
                {
                    // Apparently it can happen an activiteit is added as both hoofd and neven activiteit
                    if (vestiging.SbiActiviteiten.Any(a => a.SbiCode.Code == sbiActiviteit.sbiCode.code))
                        continue;

                    vestiging.SbiActiviteiten.Add(new VestigingSbiActiviteit()
                    {
                        SbiCode = new SbiCode() { Code = sbiActiviteit.sbiCode.code, Omschrijving = sbiActiviteit.sbiCode.omschrijving },
                        Vestiging = vestiging,
                        IsHoofdSbiActiviteit = sbiActiviteit.isHoofdactiviteit.omschrijving == "ja" // TODO!
                    });
                }

                vestiging.FulltimeWerkzamePersonen = "";
                vestiging.ParttimeWerkzamePersonen = "";
                vestiging.TotaalWerkzamePersonen = "";

                return vestiging;
            }
            else
                throw new NotImplementedException("No other type of vestiging then commercielevestiging or nietCommercieleVestiging is yet implemented...");
        }

        private Vestiging CreateVestigingFromPersoonType(PersoonType persoonType)
        {
            Vestiging vestiging = null;
            if (persoonType != null)
            {
                if (persoonType is RechtspersoonType) // == && NietNatuurlijkPersoonType
                {
                    var rpt = (RechtspersoonType)persoonType;
                    vestiging = new Vestiging
                    {
                        Naam = Vestiging.RECHTSPERSOONPREFIX + rpt.volledigeNaam,
                        Vestigingsnummer = rpt.rsin, // FOR Vestigingsnummer we use the RSIN since the Rechtspersoontype does not have a vestigingsnummer (effectively not a vestiging);
                        RSIN = rpt.rsin
                    };

                    vestiging.SetVestigingAdres(rpt.bezoekLocatie);
                    if (rpt.postLocatie == null)
                        rpt.postLocatie = rpt.bezoekLocatie;
                    vestiging.SetVestigingPostAdres(rpt.postLocatie);
                    vestiging.SetVestigingCommunicatieGegevens(rpt.communicatiegegevens);

                    vestiging.FulltimeWerkzamePersonen = "";
                    vestiging.ParttimeWerkzamePersonen = "";
                    vestiging.TotaalWerkzamePersonen = "";
                }
                else if (persoonType is NietNatuurlijkPersoonType)
                {
                    var svt = (NietNatuurlijkPersoonType)persoonType;
                    vestiging = new Vestiging
                    {
                        Naam = $"[{persoonType.GetType().Name}: ]" + svt.volledigeNaam,
                        Vestigingsnummer = svt.rsin, // FOR Vestigingsnummer we use the RSIN since the Rechtspersoontype does not have a vestigingsnummer (effectively not a vestiging);
                        RSIN = svt.rsin
                    };

                    vestiging.SetVestigingAdres(svt.bezoekLocatiePersoon);
                    if (svt.postLocatiePersoon == null)
                        svt.postLocatiePersoon = svt.bezoekLocatiePersoon;
                    vestiging.SetVestigingPostAdres(svt.postLocatiePersoon);

                    vestiging.FulltimeWerkzamePersonen = "";
                    vestiging.ParttimeWerkzamePersonen = "";
                }
                else //if (persoonType is NatuurlijkPersoonType && persoonType is NaamPersoonType)
                    Logger.Error("No valid persoonType found to create a Vestiging. PersoonType: " +
                                 persoonType.GetType().ToString());
            }
            else
                Logger.Warn("No persoonType found: maatschappelijkeActiviteit.heeftAlsEigenaar.Item is null...");

            #region // DEBUG
            //var sb = new System.Text.StringBuilder();

            //var settings =
            //    new System.Xml.XmlWriterSettings { Encoding = System.Text.Encoding.UTF8, Indent = true };

            //using (var xmlWriter = System.Xml.XmlWriter.Create(sb, settings))
            //{
            //    if (xmlWriter != null)
            //    {
            //        new System.Xml.Serialization.XmlSerializer(persoonType.GetType()).Serialize(xmlWriter, persoonType);
            //    }
            //}

            //logger.Debug(sb.ToString());
            #endregion

            return vestiging;
        }

        #endregion

        public Pong Ping()
        {
            Pong pong;

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var request = new ophalenInschrijvingRequest()
                {
                    ophalenInschrijvingRequest1 = new KernRegister.Business.KvK.v30.InschrijvingRequestType()
                    {
                        klantreferentie = _klantReferentie,
                        Item = "30152124",
                        ItemElementName = ItemChoiceType.kvkNummer
                    }
                };
                var result = _service.ophalenInschrijving(request)?.ophalenInschrijvingResponse1;

                sw.Stop();


                pong = new Pong() { IsSuccesful = true, ResponseTime = sw.Elapsed };
            }
            catch (Exception ex)
            {
                pong = new Pong() { IsSuccesful = false, ExceptionMessage = ex.Message };
                Logger.DebugException("Something went wrong while calling genereerProduct service for Ping test", ex);
            }
            return pong;

        }

        public dynamic GetInschrijvingResponseTypeByKvkNummer(string kvkNummer, bool bypassCache = false)
        {
            try
            {
                var request = new ophalenInschrijvingRequest()
                {
                    ophalenInschrijvingRequest1 = new InschrijvingRequestType()
                    {
                        klantreferentie = _klantReferentie,
                        Item = kvkNummer,
                        ItemElementName = ItemChoiceType.kvkNummer
                    }
                };
                return _service.ophalenInschrijving(request);
            }
            catch (FaultException ex)
            {
                var fault = ex.CreateMessageFault();
                if (fault.HasDetail)
                {
                    var f = fault.GetReaderAtDetailContents();
                    var x = f.ReadOuterXml();
                    var invalidXml = RawXmlCache.Get(kvkNummer);
                    throw new Exception($"Er is een fout opgetreden bij de KvK. FoutBericht(en): {ex.Message} {ex.Reason} - {x}; xml: {invalidXml}");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}