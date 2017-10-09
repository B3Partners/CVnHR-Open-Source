using System;
using System.Globalization;
using System.Linq;
using QNH.Overheid.KernRegister.Business.Integration;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Service.KvK.v2_3
{
    public static class KvkSearchServiceExtensions
    {
        public static void SetVestigingAdres(this Vestiging vestiging, LocatieType locatieType)
        {
            var vestigingAdres = locatieType.Items.FirstOrDefault();
            object temAdres = null;
            if (locatieType.Items.Length > 1)
            {
                temAdres = locatieType.Items.LastOrDefault();
            }
            if (locatieType.Items.Length > 2)
            {
                throw new Exception($"More adress types then expected! Excpected: 0, actual: {locatieType.Items.Length}");
            }

            if (vestigingAdres is BinnenlandsAdresType)
            {
                var adres = vestigingAdres as BinnenlandsAdresType;
                vestiging.Adres = locatieType.volledigAdres;
                vestiging.Straat = adres.straatnaam;
                vestiging.Huisnummer = adres.huisnummer;
                vestiging.Huisnummertoevoeging = adres.huisnummerToevoeging;
                if (temAdres != null)
                {
                    if (temAdres is BinnenlandsAdresType)
                    {
                        var ttmAdres = temAdres as BinnenlandsAdresType;
                        vestiging.Huisnummertoevoeging += $" t/m {ttmAdres.huisnummer}{ttmAdres.huisnummerToevoeging}";
                    }
                    else
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
            var vestigingAdres = locatieType.Items.FirstOrDefault();
            object temAdres = null;
            if (locatieType.Items.Length > 1)
            {
                temAdres = locatieType.Items.LastOrDefault();
            }
            if (locatieType.Items.Length > 2)
            {
                throw new Exception($"More adress types then expected! Excpected: 0, actual: {locatieType.Items.Length}");
            }

            if (vestigingAdres is BinnenlandsAdresType)
            {
                var postAdres = vestigingAdres as BinnenlandsAdresType;
                vestiging.PostStraat = postAdres.straatnaam;
                vestiging.PostHuisnummer = postAdres.huisnummer;
                vestiging.PostHuisnummerToevoeging = postAdres.huisnummerToevoeging;
                if (temAdres != null)
                {
                    if (temAdres is BinnenlandsAdresType)
                    {
                        var ttmAdres = temAdres as BinnenlandsAdresType;
                        vestiging.PostHuisnummerToevoeging += $" t/m {ttmAdres.huisnummer}{ttmAdres.huisnummerToevoeging}";
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
            if(communicatieGegevens == null)
                communicatieGegevens = new CommunicatiegegevensType();

            if (communicatieGegevens.communicatienummer == null)
                communicatieGegevens.communicatienummer = new CommunicatienummerType[] { };

            vestiging.Email = communicatieGegevens.emailAdres == null ? null : communicatieGegevens.emailAdres.FirstOrDefault();

            CommunicatienummerType telefoon =
                communicatieGegevens.communicatienummer.ToList()
                    .FirstOrDefault(cn => cn.soort == SoortCommunicatieNummerType.Telefoon);
            if (telefoon != null)
            {
                vestiging.Telefoon = telefoon.nummer;
            }
            CommunicatienummerType fax =
                communicatieGegevens.communicatienummer.ToList()
                    .FirstOrDefault(cn => cn.soort == SoortCommunicatieNummerType.Fax);
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
                kvkInschrijving.RegistratieDatumAanvang = KvkSearchService.VALUENOTPROVIDED;

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


                if (nnpt.heeftGedeponeerd == null)
                    kvkInschrijving.EigenaarHeeftGedeponeerd = IndicatieType.Nee.ToString();
                else
                {
                    kvkInschrijving.EigenaarHeeftGedeponeerd = IndicatieType.Ja.ToString();
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
                            DatumDeponering = DateTime.ParseExact(deponering.Item.datumDeponering,"yyyyMMdd", CultureInfo.InvariantCulture),
                            Type = deponering.Item.ToString().Replace("QNH.Overheid.KernRegister.Business.Integration.Deponering", "").Replace("Type", ""),
                            Status = deponering.Item.status.ToString(),
                        };

                        if(deponering.Item.gaatOver != null)
                        {
                            deponeringsStuk.GaatOver = deponering.Item.gaatOver.Item.ToString().Replace("QNH.Overheid.KernRegister.Business.Integration.Deponering", "").Replace("Type", "");
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

                #region All other NietNatuurlijkPersoonTypes
                //else if(eigenaar is EenmanszaakMetMeerdereEigenarenType)
                //{
                //    var emmet = (EenmanszaakMetMeerdereEigenarenType)eigenaar;
                //}
                //else if(eigenaar is RechtspersoonInOprichtingType)
                //{
                //    var riot = (RechtspersoonInOprichtingType)eigenaar;
                //}
                //else if(eigenaar is SamenwerkingsverbandType)
                //{
                //    var svt = (SamenwerkingsverbandType)eigenaar;
                //}
                #endregion
            }
            else if (eigenaar is NatuurlijkPersoonType) // According to the documentation the eigenaar is always a NietNatuurlijkPersoonType
            {
                var npt = (NatuurlijkPersoonType)eigenaar;

                kvkInschrijving.DatumOprichting = npt.registratie.datumAanvang;
                kvkInschrijving.DatumUitschrijving = npt.registratie.datumEinde;
            }
            else
                throw new NotImplementedException("Eigenaar is both not NietNatuurlijkPersoonType and not NatuurlijkPersoonType ??");


            var bijzToestand = eigenaar.bijzondereRechtstoestand;
            if(bijzToestand != null)
            {
                if (bijzToestand.Item1 is BijzondereRechtstoestandTypeFaillissement)
                {
                    kvkInschrijving.BijzondereRechtsToestand = "Faillissement";
                }
                else if (bijzToestand.Item1 is BijzondereRechtstoestandTypeSchuldsanering)
                {
                    kvkInschrijving.BijzondereRechtsToestand = "Schuldsanering";
                }
                else if (bijzToestand.Item1 is BijzondereRechtstoestandTypeSurseanceVanBetaling)
                {
                    var surceanse = bijzToestand.Item1 as BijzondereRechtstoestandTypeSurseanceVanBetaling;
                    kvkInschrijving.BijzondereRechtsToestand = string.Format("Surseance van betaling (duur: {0}, status: {1})",
                        surceanse.duur,
                        surceanse.status);
                }
                kvkInschrijving.RedenInsolventie = bijzToestand.redenInsolventie.ToString();

                //kvkInschrijving.BijzondereRechtsToestand = 
                //    bijzToestand.ItemElementName.ToString() + ": " + bijzToestand.redenInsolventie.ToString();
            }

            var beperking = eigenaar.beperkingInRechtshandeling;
            if (beperking != null)
            {
                kvkInschrijving.BeperkingInRechtshandeling = string.Format("{0} (Rechtelijke uitspraak - datum: {1}, naam: {2}, plaats: {3})",
                    beperking.soort,
                    beperking.Item.datumUitspraak,
                    beperking.Item.naam,
                    beperking.Item.plaats);
            }
            else
                kvkInschrijving.BeperkingInRechtshandeling = IndicatieType.Nee.ToString();


            // Add all functieVervullingen
            if(eigenaar != null && eigenaar.heeft != null)
                foreach (var functieVervulling in eigenaar.heeft)
                { 
                    var titel = functieVervulling.Item.functieTitel;
                    var persoon = functieVervulling.Item.Item.Item;

                    var functie = new FunctieVervulling
                    {
                        Functie = functieVervulling.Item.ToString().Replace("QNH.Overheid.KernRegister.Business.Integration.", "").Replace("Type", ""),
                        VolledigeNaam = persoon.volledigeNaam ?? (persoon is NaamPersoonType ? ((NaamPersoonType)persoon).naam : ""),
                        Schorsing = functieVervulling.Item.schorsing != null ? IndicatieType.Ja.ToString() : IndicatieType.Nee.ToString(),
                        LangstZittende = functieVervulling.Item.langstzittende.ToString()
                    };

                    functie.FunctieTitel = titel == null
                        ? functie.Functie
                        : string.Format("{0}, indicatie Statutair: {1}", titel.functieTitel, titel.indicatieStatutair);

                    if (functieVervulling.Item is AansprakelijkeType)
                    {
                        var aansprakelijke = functieVervulling.Item as AansprakelijkeType;
                        if(aansprakelijke.bevoegdheid != null)
                            functie.Bevoegdheid =
                                string.Format("{0} (beperking: {1}, overige beperking: {2})",
                                    aansprakelijke.bevoegdheid.soort,
                                    aansprakelijke.bevoegdheid.beperkingInEuros == null 
                                        ? IndicatieType.Nee.ToString() 
                                        : aansprakelijke.bevoegdheid.beperkingInEuros.waarde + " " + aansprakelijke.bevoegdheid.beperkingInEuros.valuta,
                                    aansprakelijke.bevoegdheid.overigeBeperking);

                        functie.HandelingsBekwaam = aansprakelijke.handelingsbekwaam.ToString();
                    }
                    else if(functieVervulling.Item is OverigeFunctionarisType)
                    {
                        var overigeFunctionaris = functieVervulling.Item as OverigeFunctionarisType;
                        functie.Functie += ": " + overigeFunctionaris.functie;
                        if(overigeFunctionaris.bevoegdheid != null)
                            functie.Bevoegdheid = overigeFunctionaris.bevoegdheid.soort.ToString();
                        functie.HandelingsBekwaam = "Afwijkend Aansprakelijkheidsbeding: " + overigeFunctionaris.afwijkendAansprakelijkheidsbeding.ToString();
                    }
                    else if (functieVervulling.Item is PubliekrechtelijkeFunctionarisType)
                    {
                        var publiekrechtelijkeFunctionaris = functieVervulling.Item as PubliekrechtelijkeFunctionarisType;
                        functie.Functie += ": " + publiekrechtelijkeFunctionaris.functie;
                        if(publiekrechtelijkeFunctionaris.bevoegdheid != null)
                            functie.Bevoegdheid = publiekrechtelijkeFunctionaris.bevoegdheid.soort.ToString();
                        functie.HandelingsBekwaam = IndicatieType.Ja.ToString();
                        if (publiekrechtelijkeFunctionaris.Item.Item != null
                            && publiekrechtelijkeFunctionaris.Item.Item.beperkingInRechtshandeling != null)
                            functie.HandelingsBekwaam = publiekrechtelijkeFunctionaris.Item.Item.beperkingInRechtshandeling.soort.ToString();

                    }
                    else if (functieVervulling.Item is BestuursfunctieType)
                    {
                        var bestuursFunctie = functieVervulling.Item as BestuursfunctieType;
                        functie.Functie += ": " + bestuursFunctie.functie;
                        if(bestuursFunctie.bevoegdheid != null)
                            functie.Bevoegdheid = string.Format("{0} (met andere personen: {1})",
                                bestuursFunctie.bevoegdheid.soort,
                                bestuursFunctie.bevoegdheid.bevoegdMetAnderePersonen);
                        functie.HandelingsBekwaam = IndicatieType.Ja.ToString();
                        if (bestuursFunctie.Item.Item != null
                            && bestuursFunctie.Item.Item.beperkingInRechtshandeling != null)
                            functie.HandelingsBekwaam = bestuursFunctie.Item.Item.beperkingInRechtshandeling.soort.ToString();


                    }
                    else if (functieVervulling.Item is GemachtigdeType)
                    {
                        var gemachtigde = functieVervulling.Item as GemachtigdeType;
                        functie.Functie += ": " + gemachtigde.functie;
                        if(gemachtigde.volmacht != null)
                            functie.Bevoegdheid = "Volmacht: " + gemachtigde.volmacht.Item.ToString().Replace("Type", "");
                        functie.HandelingsBekwaam = IndicatieType.Ja.ToString();
                        if (gemachtigde.Item.Item != null
                            && gemachtigde.Item.Item.beperkingInRechtshandeling != null)
                            functie.HandelingsBekwaam = gemachtigde.Item.Item.beperkingInRechtshandeling.soort.ToString();

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
                        functie.HandelingsBekwaam = IndicatieType.Ja.ToString();
                        if (functionaris.Item.Item != null
                            && functionaris.Item.Item.beperkingInRechtshandeling != null)
                            functie.HandelingsBekwaam = functionaris.Item.Item.beperkingInRechtshandeling.soort.ToString();

                    }

                    if(!string.IsNullOrEmpty(functie.Bevoegdheid))
                        functie.Bevoegdheid = functie.Bevoegdheid.Replace("QNH.Overheid.KernRegister.Business.Integration.", "");

                    kvkInschrijving.FunctieVervullingen.Add(functie);
                }

            // Add the activeiten as a many to many relationship

            if(maatschappelijkeActiviteit.manifesteertZichAls != null)
            {
                if (maatschappelijkeActiviteit.manifesteertZichAls.onderneming != null)
                {
                    var onderneming = maatschappelijkeActiviteit.manifesteertZichAls.onderneming;
                    if (onderneming.hoofdSbiActiviteit != null)
                        foreach (var sbiActiviteit in onderneming.hoofdSbiActiviteit)
                        {
                            kvkInschrijving.SbiActiviteiten.Add(new SbiActiviteit()
                            {
                                SbiCode = new SbiCode() { Code = sbiActiviteit.sbiCode, Omschrijving = sbiActiviteit.omschrijving },
                                KvKInschrijving = kvkInschrijving,
                                IsHoofdSbiActiviteit = true
                            });
                        }

                    if (onderneming.nevenSbiActiviteit != null)
                        foreach (var sbiActiviteit in onderneming.nevenSbiActiviteit)
                        {
                            // Apparently it can happen an activiteit is added as both hoofd and neven activiteit
                            if (kvkInschrijving.SbiActiviteiten.Any(a => a.SbiCode.Code == sbiActiviteit.sbiCode))
                                continue;

                            kvkInschrijving.SbiActiviteiten.Add(new SbiActiviteit()
                            {
                                SbiCode = new SbiCode() { Code = sbiActiviteit.sbiCode, Omschrijving = sbiActiviteit.omschrijving },
                                KvKInschrijving = kvkInschrijving,
                                IsHoofdSbiActiviteit = false
                            });
                        }

                    kvkInschrijving.FulltimeWerkzamePersonen = onderneming.fulltimeWerkzamePersonen;
                    kvkInschrijving.ParttimeWerkzamePersonen = onderneming.parttimeWerkzamePersonen;
                    kvkInschrijving.TotaalWerkzamePersonen = onderneming.totaalWerkzamePersonen;
                }
                else if (maatschappelijkeActiviteit.manifesteertZichAls.registratie != null)
                {
                    //void
                }
            }


        }

        public static string SchrijfKapitaalType(this KapitaalType kapitaal)
        {
            if (kapitaal == null)
                return null;

            var result = new System.Text.StringBuilder();
            if(kapitaal.aandelen != null)
                result.Append("Aandelen: " + string.Join(" | ", kapitaal.aandelen.Select(k=> string.Format("{0} {1} ({2})", k.aantal, k.soort, k.waarde.valuta))));

            if (kapitaal.bedrag != null)
                result.AppendFormat("{0} {1} {2}", kapitaal.aandelen == null ? "" : " | ", kapitaal.bedrag.waarde, kapitaal.bedrag.valuta);

            var notTooLong = result.ToString();

            if (notTooLong.Length > 255)
                notTooLong = notTooLong.Substring(0, 252) + "...";

            return notTooLong;
        }
    }
}