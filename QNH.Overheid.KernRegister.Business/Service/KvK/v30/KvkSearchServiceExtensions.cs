using System;
using System.Globalization;
using System.Linq;
using QNH.Overheid.KernRegister.Business.KvK.v30;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Service.KvK.v30
{
    public static class KvkSearchServiceExtensions
    {
        public static void SetVestigingAdres(this Vestiging vestiging, LocatieType locatieType)
        {
            var vestigingAdres = locatieType?.adres?.Item;
            if (vestigingAdres == null)
            {
                return;
            }

            // TODO!
            object temAdres = null;
            if (locatieType?.adres?.Item?.extraElementen != null)
            {
                if (locatieType?.adres?.Item?.extraElementen.Length > 1)
                {
                    temAdres = locatieType?.adres?.Item?.extraElementen.LastOrDefault();
                }
                if (locatieType?.adres?.Item?.extraElementen.Length > 2)
                {
                    throw new Exception($"More adress types then expected! Excpected: 0, actual: {locatieType?.adres?.Item?.extraElementen.Length}");
                }
            }

            if (vestigingAdres is BinnenlandsAdresType)
            {
                var adres = vestigingAdres as BinnenlandsAdresType;
                vestiging.Adres = locatieType.volledigAdres;
                vestiging.Straat = adres.straatnaam;
                vestiging.Huisnummer = adres.huisnummer;
                vestiging.Huisnummertoevoeging = $"{adres.huisletter}{adres.huisnummerToevoeging}";
                if (temAdres != null)
                {
                    if (temAdres is BinnenlandsAdresType)
                    {
                        var ttmAdres = temAdres as BinnenlandsAdresType;
                        vestiging.Huisnummertoevoeging += $" t/m {ttmAdres.huisnummer}{ttmAdres.huisletter}{ttmAdres.huisnummerToevoeging}";
                    }
                    else // TODO!!!
                    {
                        throw new Exception($"Could not determine type of temAdres: {temAdres.GetType()}");
                    }
                }
                vestiging.Gemeente = adres.plaats;
                vestiging.Woonplaats = adres.plaats;
                vestiging.PostcodeCijfers = adres.postcode.cijfercombinatie;
                vestiging.PostcodeLetters = adres.postcode.lettercombinatie;
                vestiging.BagId = adres.bagId == null ? null : adres.bagId.identificatieAdresseerbaarObject + ";" + adres.bagId.identificatieNummeraanduiding;
            }
            else if (vestigingAdres is BuitenlandsAdresType)
            {
                var adres = vestigingAdres as BuitenlandsAdresType;
                vestiging.Adres = locatieType.volledigAdres;
                vestiging.Straat = adres.straatHuisnummer;
                vestiging.Huisnummer = string.Empty;
                vestiging.Huisnummertoevoeging = string.Empty;
                vestiging.Gemeente = $"Regio: {adres.regio}, Land: {adres.land}";
                vestiging.Woonplaats = adres.postcodeWoonplaats;
                vestiging.PostcodeCijfers = string.Empty;
                vestiging.PostcodeLetters = string.Empty;
            }
            else
            {
                throw new NotImplementedException($"Unknown adresType: {vestigingAdres.GetType()}");
            }
        }

        public static void SetVestigingPostAdres(this Vestiging vestiging, LocatieType locatieType)
        {
            var vestigingAdres = locatieType?.adres?.Item;
            if (vestigingAdres == null)
            {
                return;
            }

            // TODO!
            object temAdres = null;
            if (locatieType?.adres?.Item?.extraElementen != null)
            {
                if (locatieType?.adres?.Item?.extraElementen.Length > 1)
                {
                    temAdres = locatieType?.adres?.Item?.extraElementen.LastOrDefault();
                }
                if (locatieType?.adres?.Item?.extraElementen.Length > 2)
                {
                    throw new Exception($"More adress types then expected! Excpected: 0, actual: {locatieType?.adres?.Item?.extraElementen.Length}");
                }
            }

            if (vestigingAdres is BinnenlandsAdresType)
            {
                var postAdres = vestigingAdres as BinnenlandsAdresType;
                vestiging.PostStraat = postAdres.straatnaam;
                vestiging.PostHuisnummer = postAdres.huisnummer;
                vestiging.PostHuisnummerToevoeging = $"{postAdres.huisletter}{postAdres.huisnummerToevoeging}";
                if (temAdres != null)
                {
                    // TODO!
                    if (temAdres is BinnenlandsAdresType)
                    {
                        var ttmAdres = temAdres as BinnenlandsAdresType;
                        vestiging.PostHuisnummerToevoeging += $" t/m {ttmAdres.huisnummer}{ttmAdres.huisletter}{ttmAdres.huisnummerToevoeging}";
                    }
                    else
                    {
                        throw new Exception($"Could not determine type of temAdres: {temAdres.GetType()}");
                    }
                }
                vestiging.PostPostcodeCijfers = postAdres.postcode.cijfercombinatie;
                vestiging.PostPostcodeLetters = postAdres.postcode.lettercombinatie;
                vestiging.PostWoonplaats = postAdres.plaats;
                vestiging.Postbusnummer = postAdres.postbusnummer;
            }
            else if (vestigingAdres is BuitenlandsAdresType)
            {
                var postAdres = vestigingAdres as BuitenlandsAdresType;
                vestiging.PostAdres = locatieType.volledigAdres;
                vestiging.PostStraat = postAdres.straatHuisnummer;
                vestiging.PostHuisnummer = string.Empty;
                vestiging.PostHuisnummerToevoeging = string.Empty;
                vestiging.PostGemeente = $"Regio: {postAdres.regio}, Land: {postAdres.land}";
                vestiging.PostWoonplaats = postAdres.postcodeWoonplaats;
                vestiging.PostPostcodeCijfers = string.Empty;
                vestiging.PostPostcodeLetters = string.Empty;
                vestiging.Postbusnummer = $"Regio: {postAdres.regio}, Land: {postAdres.land}"; ;
            }
            else
            {
                throw new NotImplementedException($"Unknown adresType: {vestigingAdres.GetType()}");
            }
        }

        public static void SetVestigingCommunicatieGegevens(this Vestiging vestiging, CommunicatiegegevensType communicatieGegevens)
        {
            if (communicatieGegevens == null)
                communicatieGegevens = new CommunicatiegegevensType();

            if (communicatieGegevens.communicatienummer == null)
                communicatieGegevens.communicatienummer = new CommunicatienummerType[] { };

            vestiging.Email = communicatieGegevens.emailAdres?.FirstOrDefault();

            var telefoon =
                communicatieGegevens.communicatienummer.ToList()
                    .FirstOrDefault(cn => cn.soort.omschrijving.ToLowerInvariant() == "telefoon");
            if (telefoon != null)
            {
                vestiging.Telefoon = telefoon.nummer;
            }
            var fax =
                communicatieGegevens.communicatienummer.ToList()
                    .FirstOrDefault(cn => cn.soort.omschrijving.ToLowerInvariant() == "fax");
            if (fax != null)
            {
                vestiging.Fax = fax.nummer;
            }
        }

        public static void SetRaadpleegInformatie(this KvkInschrijving kvkInschrijving, MaatschappelijkeActiviteitType maatschappelijkeActiviteit)
        {
            // Voeg registratie moment toe
            if (maatschappelijkeActiviteit.registratie != null)
            {
                kvkInschrijving.RegistratieDatumAanvang = maatschappelijkeActiviteit.registratie.datumAanvang;
                kvkInschrijving.RegistratieDatumEinde = maatschappelijkeActiviteit.registratie.datumEinde; // can be null or empty!
            }
            else
                kvkInschrijving.RegistratieDatumAanvang = KvkDataSearchService.VALUENOTPROVIDED;

            // Set the rechtsvorm
            var eigenaar = maatschappelijkeActiviteit.heeftAlsEigenaar.Item;

            kvkInschrijving.PersoonRechtsvorm = eigenaar.persoonRechtsvorm;
            kvkInschrijving.UitgebreideRechtsvorm = eigenaar.uitgebreideRechtsvorm;
            kvkInschrijving.VolledigeNaamEigenaar = eigenaar.volledigeNaam;

            if (eigenaar is NietNatuurlijkPersoonType)
            {
                var nnpt = (NietNatuurlijkPersoonType)eigenaar;

                kvkInschrijving.DatumOprichting = nnpt.registratie.datumAanvang;
                kvkInschrijving.DatumUitschrijving = nnpt.datumUitschrijving;

                if (eigenaar is RechtspersoonType)
                {
                    var rechtspersoon = (RechtspersoonType)eigenaar;
                    kvkInschrijving.GeplaatstKapitaal = rechtspersoon.geplaatstKapitaal.SchrijfKapitaalType();
                    kvkInschrijving.GestortKapitaal = rechtspersoon.gestortKapitaal.SchrijfKapitaalType();

                    // override the default
                    kvkInschrijving.DatumOprichting = rechtspersoon.datumOprichting;
                    kvkInschrijving.DatumUitschrijving = rechtspersoon.datumUitschrijving;
                }
                else if (eigenaar is BuitenlandseVennootschapType)
                {
                    var bvt = (BuitenlandseVennootschapType)eigenaar;
                    kvkInschrijving.GeplaatstKapitaal = bvt.geplaatstKapitaal.SchrijfKapitaalType();
                }
                else if (eigenaar is EenmanszaakMetMeerdereEigenarenType)
                {
                    //var emmet = (EenmanszaakMetMeerdereEigenarenType)eigenaar;
                }
                else if (eigenaar is RechtspersoonInOprichtingType)
                {
                    //var riot = (RechtspersoonInOprichtingType)eigenaar;
                }
                else if (eigenaar is SamenwerkingsverbandType)
                {
                    //var svt = (SamenwerkingsverbandType)eigenaar;
                }
                else
                {
                    throw new NotImplementedException("Onbekend eigenaar type! " + eigenaar.GetType());
                }


                if (nnpt.heeftGedeponeerd == null)
                    kvkInschrijving.EigenaarHeeftGedeponeerd = "Nee"; // TODO: IndicatieType.Nee.ToString();
                else
                {
                    kvkInschrijving.EigenaarHeeftGedeponeerd = "Ja"; // TODO: IndicatieType.Ja.ToString();
                    //string.Join("|", nnpt.heeftGedeponeerd.Select(d =>
                    //  string.Format("DepotId: {0}, datum {1}, gaatOver {2}, status: {3}",
                    //                d.Item.depotId,
                    //                d.Item.datumDeponering,
                    //                d.Item.gaatOver,
                    //                d.Item.statusSpecified ? d.Item.status.ToString() : KvkSearchService.VALUENOTPROVIDED)));


                    foreach (var deponering in nnpt.heeftGedeponeerd)
                    {
                        var deponeringsStuk = new DeponeringsStuk
                        {
                            DepotId = deponering.Item.depotId,
                            DatumDeponering =
                                DateTime.ParseExact(deponering.Item.datumDeponering, "yyyyMMdd",
                                    CultureInfo.InvariantCulture),
                            Type =
                                deponering.Item.ToString()
                                    .Replace("QNH.Overheid.KernRegister.Business.Integration.Deponering", "")
                                    .Replace("Type", ""),
                            Status = deponering.Item.status.ToString(),
                        };

                        if (deponering.Item.gaatOver != null)
                        {
                            deponeringsStuk.GaatOver =
                                deponering.Item.gaatOver.Item.ToString()
                                    .Replace("QNH.Overheid.KernRegister.Business.Integration.Deponering", "")
                                    .Replace("Type", "");
                        }

                        #region Possible types of Deponering

                        //if(deponering.Item is DeponeringAansprakelijkheidsverklaringType)
                        //{
                        //    var type = deponering.Item as DeponeringAansprakelijkheidsverklaringType;
                        //    type.
                        //}
                        //if(deponering.Item is DeponeringAanvullendeMededelingType)
                        //{}
                        //if(deponering.Item is DeponeringBijzondereDeponeringType)
                        //{}
                        //if(deponering.Item is DeponeringJaarstukHalfjaarKwartaalcijfersType)
                        //{}
                        //if(deponering.Item is DeponeringJaarstukJaarrekeningType)
                        //{}
                        //if(deponering.Item is DeponeringJaarstukJaarrekeningOngewijzigdVastgesteldType)
                        //{}
                        //if(deponering.Item is DeponeringOverblijvendeAansprakelijkheidType)
                        //{}

                        #endregion

                        kvkInschrijving.Deponeringen.Add(deponeringsStuk);
                    }
                }
            }
            else if (eigenaar is NatuurlijkPersoonType) // According to the documentation the eigenaar is always a NietNatuurlijkPersoonType
            {
                var npt = (NatuurlijkPersoonType)eigenaar;

                //kvkInschrijving.DatumOprichting = npt.registratie.datumAanvang; // => Is birthdate of the owner... 
                kvkInschrijving.DatumOprichting = maatschappelijkeActiviteit.registratie.datumAanvang;
                kvkInschrijving.DatumUitschrijving = npt.registratie.datumEinde;
            }
            else
                throw new NotImplementedException("Eigenaar is both not NietNatuurlijkPersoonType and not NatuurlijkPersoonType ??");


            var bijzToestand = eigenaar.bijzondereRechtstoestand;
            if (bijzToestand != null)
            {
                kvkInschrijving.BijzondereRechtsToestand = bijzToestand.soort.ToString(); // TODO!

                //if (bijzToestand.Item1 is BijzondereRechtstoestandTypeFaillissement)
                //{
                //    kvkInschrijving.BijzondereRechtsToestand = "Faillissement";
                //}
                //else if (bijzToestand.Item1 is BijzondereRechtstoestandTypeSchuldsanering)
                //{
                //    kvkInschrijving.BijzondereRechtsToestand = "Schuldsanering";
                //}
                //else if (bijzToestand.Item1 is BijzondereRechtstoestandTypeSurseanceVanBetaling)
                //{
                //    var surceanse = bijzToestand.Item1 as BijzondereRechtstoestandTypeSurseanceVanBetaling;
                //    kvkInschrijving.BijzondereRechtsToestand =
                //        $"Surseance van betaling (duur: {surceanse.duur}, status: {surceanse.status})";
                //}
                //else
                //{
                //    throw new NotImplementedException("Unknown bijzToestand.Item: " + bijzToestand.Item1.GetType());
                //}
                kvkInschrijving.RedenInsolventie = bijzToestand.redenEindeInsolventie.ToString();

                var uitspraak = bijzToestand.Item;
                if (uitspraak != null)
                {
                    kvkInschrijving.RechterlijkeUitspraak = $"{uitspraak.datum}: {uitspraak.naam}, {uitspraak.plaats}";
                }

                //kvkInschrijving.BijzondereRechtsToestand = 
                //    bijzToestand.ItemElementName.ToString() + ": " + bijzToestand.redenInsolventie.ToString();
            }

            var beperking = eigenaar.beperkingInRechtshandeling;
            if (beperking != null)
            {
                kvkInschrijving.BeperkingInRechtshandeling =
                    $"{beperking.soort} (Rechtelijke uitspraak - datum: {beperking.Item.datum}, naam: {beperking.Item.naam}, plaats: {beperking.Item.plaats})";
            }
            else
                kvkInschrijving.BeperkingInRechtshandeling = "Nee";// TODO! IndicatieType.Nee.ToString();


            // Add all functieVervullingen
            if (eigenaar != null && eigenaar.heeft != null)
                foreach (var functieVervulling in eigenaar.heeft)
                {
                    var titel = functieVervulling.Item.functietitel;
                    var persoon = functieVervulling.Item.Item.Item;

                    var functie = new FunctieVervulling
                    {
                        Functie = functieVervulling.Item.ToString().Replace("QNH.Overheid.KernRegister.Business.Integration.", "").Replace("Type", "")
                                                                    .Replace("QNH.Overheid.KernRegister.Business.KvK.", "")
                                                                    .Replace("v30.", ""),
                        VolledigeNaam = persoon.volledigeNaam ?? (persoon is NaamPersoonType ? ((NaamPersoonType)persoon).naam : ""),
                        Schorsing = functieVervulling.Item.schorsing != null ? "Ja" : "Nee",//TODO IndicatieType.Ja.ToString() : IndicatieType.Nee.ToString(),
                        LangstZittende = "" // functieVervulling.Item.functietite.ItemElementName.ToString() //TODO .langstzittende.ToString()
                    };

                    functie.FunctieTitel = titel == null
                        ? functie.Functie
                        : $"{titel.titel}, indicatie Statutair: {titel.isStatutaireTitel}";

                    if (functieVervulling.Item is AansprakelijkeType)
                    {
                        var aansprakelijke = functieVervulling.Item as AansprakelijkeType;
                        if (aansprakelijke.bevoegdheid != null)
                            functie.Bevoegdheid =
                                $"{aansprakelijke.bevoegdheid.soort.omschrijving} (beperking: {(aansprakelijke.bevoegdheid.beperkingInEuros == null ? "Nee" : aansprakelijke.bevoegdheid.beperkingInEuros.waarde + " " + aansprakelijke.bevoegdheid.beperkingInEuros.valuta)}, overige beperking: {aansprakelijke.bevoegdheid.overigeBeperking.omschrijving})";// TODO IndicatieType.Nee.ToString() 

                        functie.HandelingsBekwaam =
                            $"{aansprakelijke.handlichting?.isVerleend?.code} - {aansprakelijke.handlichting.isVerleend.omschrijving} - {aansprakelijke.handlichting.isVerleend.referentieType}";
                    }
                    else if (functieVervulling.Item is OverigeFunctionarisType)
                    {
                        var overigeFunctionaris = functieVervulling.Item as OverigeFunctionarisType;
                        functie.Functie += ": " + overigeFunctionaris.functie.omschrijving;
                        if (overigeFunctionaris.bevoegdheid != null)
                            functie.Bevoegdheid = overigeFunctionaris.bevoegdheid.soort.omschrijving;
                        functie.HandelingsBekwaam = "Afwijkend Aansprakelijkheidsbeding: " + overigeFunctionaris.heeftAfwijkendAansprakelijkheidsbeding?.omschrijving;
                    }
                    else if (functieVervulling.Item is PubliekrechtelijkeFunctionarisType)
                    {
                        var publiekrechtelijkeFunctionaris = functieVervulling.Item as PubliekrechtelijkeFunctionarisType;
                        functie.Functie += ": " + publiekrechtelijkeFunctionaris.functie.omschrijving;
                        if (publiekrechtelijkeFunctionaris.bevoegdheid != null)
                            functie.Bevoegdheid = publiekrechtelijkeFunctionaris.bevoegdheid.soort.omschrijving;
                        functie.HandelingsBekwaam = "Ja"; //TODO IndicatieType.Ja.ToString();
                        if (publiekrechtelijkeFunctionaris.Item.Item != null
                            && publiekrechtelijkeFunctionaris.Item.Item.beperkingInRechtshandeling != null)
                            functie.HandelingsBekwaam = publiekrechtelijkeFunctionaris.Item.Item.beperkingInRechtshandeling.soort.omschrijving;

                    }
                    else if (functieVervulling.Item is BestuursfunctieType)
                    {
                        var bestuursFunctie = functieVervulling.Item as BestuursfunctieType;
                        functie.Functie += ": " + bestuursFunctie.functie.omschrijving + ", " + bestuursFunctie.functie.referentieType;
                        if (bestuursFunctie.bevoegdheid != null)
                            functie.Bevoegdheid =
                                $"{bestuursFunctie.bevoegdheid.soort.omschrijving} (met andere personen: {bestuursFunctie.bevoegdheid.isBevoegdMetAnderePersonen.omschrijving}, {bestuursFunctie.bevoegdheid.isBevoegdMetAnderePersonen.referentieType})";
                        if (bestuursFunctie.monistischeBestuurder != null)
                            functie.Bevoegdheid +=
                                $" (monistischeBestuurder rol: {bestuursFunctie.monistischeBestuurder.rol?.omschrijving})";
                        functie.FunctieTitel = $"{bestuursFunctie.functie.omschrijving}: {bestuursFunctie.functie.referentieType}";
                        functie.HandelingsBekwaam = "Ja";// IndicatieType.Ja.ToString();
                        if (bestuursFunctie.Item.Item != null
                            && bestuursFunctie.Item.Item.beperkingInRechtshandeling != null)
                            functie.HandelingsBekwaam = bestuursFunctie.Item.Item.beperkingInRechtshandeling.soort.omschrijving;


                    }
                    else if (functieVervulling.Item is GemachtigdeType)
                    {
                        var gemachtigde = functieVervulling.Item as GemachtigdeType;
                        functie.Functie += ": " + gemachtigde.functie.omschrijving;
                        if (gemachtigde.volmacht != null)
                            functie.Bevoegdheid = $"Volmacht: {gemachtigde.volmacht.typeVolmacht.code}, {gemachtigde.volmacht.typeVolmacht.omschrijving}";
                        //.Item.ToString().Replace("Type", "");
                        functie.HandelingsBekwaam = "Ja"; // TODO IndicatieType.Ja.ToString();
                        if (gemachtigde.Item.Item != null
                            && gemachtigde.Item.Item.beperkingInRechtshandeling != null)
                            functie.HandelingsBekwaam = gemachtigde.Item.Item.beperkingInRechtshandeling.soort.omschrijving;

                    }
                    else if (functieVervulling.Item is FunctionarisBijzondereRechtstoestandType)
                    {
                        var functionaris = functieVervulling.Item as FunctionarisBijzondereRechtstoestandType;

                        #region Types of Functionarissen
                        //if (functionaris is BewindvoerderType)
                        //{ 
                        //    var bewindvoerder = functionaris as BewindvoerderType;
                        //}
                        //else if (functionaris is RechterCommissarisType)
                        //{ 
                        //    var rechterCommissaris = functionaris as RechterCommissarisType;

                        //}
                        //else if (functionaris is CuratorType)
                        //{
                        //    var curator = functionaris as CuratorType;
                        //}
                        #endregion

                        functie.Bevoegdheid = functie.Functie;
                        functie.HandelingsBekwaam = "Ja"; // IndicatieType.Ja.ToString();
                        if (functionaris.Item.Item != null
                            && functionaris.Item.Item.beperkingInRechtshandeling != null)
                            functie.HandelingsBekwaam = functionaris.Item.Item.beperkingInRechtshandeling.soort.omschrijving;

                    }
                    else
                        throw new NotImplementedException("Unknown functieVervulling.Item type: " + functieVervulling.Item.GetType());

                    if (!string.IsNullOrEmpty(functie.Bevoegdheid))
                        functie.Bevoegdheid = functie.Bevoegdheid.Replace("QNH.Overheid.KernRegister.Business.Integration.", "")
                            .Replace("QNH.Overheid.KernRegister.Business.KvK.", "");

                    kvkInschrijving.FunctieVervullingen.Add(functie);
                }

            // Add the activeiten as a many to many relationship

            if (maatschappelijkeActiviteit.manifesteertZichAls != null)
            {
                if (maatschappelijkeActiviteit.manifesteertZichAls.onderneming != null)
                {
                    var onderneming = maatschappelijkeActiviteit.manifesteertZichAls.onderneming;
                    if (onderneming.sbiActiviteit != null)
                        foreach (var sbiActiviteit in onderneming.sbiActiviteit)
                        {
                            kvkInschrijving.SbiActiviteiten.Add(new SbiActiviteit()
                            {
                                SbiCode = new SbiCode() { Code = sbiActiviteit.sbiCode.code, Omschrijving = sbiActiviteit.sbiCode.omschrijving },
                                KvKInschrijving = kvkInschrijving,
                                IsHoofdSbiActiviteit = sbiActiviteit.isHoofdactiviteit.omschrijving == "ja" // TODO!
                            });
                        }

                    kvkInschrijving.FulltimeWerkzamePersonen = onderneming.voltijdWerkzamePersonen;
                    kvkInschrijving.ParttimeWerkzamePersonen = onderneming.deeltijdWerkzamePersonen;
                    kvkInschrijving.TotaalWerkzamePersonen = onderneming.totaalWerkzamePersonen;
                }
                else if (maatschappelijkeActiviteit.manifesteertZichAls.extraElementen != null)
                {
                    throw new NotImplementedException("Unknown manifesteerZichAls.registratie "
                        + string.Join(" - ", maatschappelijkeActiviteit.manifesteertZichAls.extraElementen.Select(ee=> ee.GetType())));
                    //void
                }
            }

            kvkInschrijving.BerichtenBoxNaam = maatschappelijkeActiviteit.berichtenbox?.berichtenboxnaam;
        }

        public static string SchrijfKapitaalType(this KapitaalType kapitaal)
        {
            if (kapitaal == null)
                return null;

            var result = new System.Text.StringBuilder();
            if (kapitaal.aandeelSamenstelling != null)
                result.Append("AandeelSamenstelling: " + string.Join(" | ", kapitaal.aandeelSamenstelling.Select(k =>
                                  $"{k.aantal} {k.aandeel}"))); // TODO!

            if (kapitaal.bedrag != null)
                result.AppendFormat("{0} {1} {2}", kapitaal.aandeelSamenstelling == null ? "" : " | ", kapitaal.bedrag.waarde, kapitaal.bedrag.valuta?.omschrijving);

            var notTooLong = result.ToString();

            if (notTooLong.Length > 255)
                notTooLong = notTooLong.Substring(0, 252) + "...";

            return notTooLong;
        }
    }
}