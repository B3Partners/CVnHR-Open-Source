using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using QNH.Overheid.KernRegister.Business.Integration;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using QNH.Overheid.KernRegister.Organization.Resources;
using QNH.Overheid.KernRegister.Business.Service;

namespace QNH.Overheid.KernRegister.Business.Crm.DocBase
{
    public class DocBaseRepository : IExportService
    {
        #region Constants

        public const string INDEXFIELDS = "INDEXFIELDS";
        public const string RELATIONTYPE = "PERSONEN";
        public const string RELATIONTABLE = "RELATION";

        #endregion

        #region Fields

        private readonly IRelationServiceContract _relationsService;
        private readonly IExportCredentials _credentials;
        private static IPostcodeService _postcodeService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private bool _loggedOn;
        private DateTime _expireLoginTime = DateTime.MinValue;

        #endregion

        #region Constructor

        public DocBaseRepository( IRelationServiceContract relationsService, IExportCredentials credentials, IPostcodeService postcodeService)
        {
            _relationsService = relationsService;
            _credentials = credentials;

            if(_postcodeService == null)
                _postcodeService = postcodeService;
        }

        #endregion

        #region IExportService Implementation

        public IExportResult UpdateExternalRecord(KvkInschrijving kvkInschrijving)
        {
            if (kvkInschrijving == null)
                throw new ArgumentException("kvkInschrijving");

            _logger.Debug("Starting to update Docbase record for inschrijving with kvkNummer: " + kvkInschrijving.KvkNummer);
            _logger.Debug("Number of vestigingen: " + kvkInschrijving.Vestigingen.Count);

            // Login to ensure the cookie.
            Login();

            // Get all records with this KvK number
            var whereClause = FormatWhereClauseDocbase(kvkInschrijving.KvkNummer,
                kvkInschrijving.Vestigingen.Select(v => v.Vestigingsnummer));
            var relationsList = _relationsService.GetRelationsList(RELATIONTYPE, whereClause);

            var indexFields = relationsList.Tables[INDEXFIELDS];

            if (indexFields.Rows.Count == 0)
            {
                _logger.Warn("Could not find any records in DocBase for whereClause: " + whereClause);
                return new ExportResult(false, "Deze inschrijving bestaat nog niet in DocBase.", noItemsFoundInsertInstead: true);
            }

            var errors = new List<string>();

            //  Get the existingVestigingen and the vestigingen that exist in DocBase but don't exist in Kvk
            var existingVestigingen = new Dictionary<Vestiging, int>();
            var nonExistingVestigingen = new List<int>();

            foreach (DataRow row in indexFields.Rows)
            {
                var relId = Convert.ToInt32(row["REL_ID"]);
                var vestigingId = row["VEST_ID"].ToString();
                var existing = kvkInschrijving.Vestigingen.FirstOrDefault(v => v.Vestigingsnummer == vestigingId);
                if (existing != null)
                {
                    if (!existingVestigingen.ContainsKey(existing))
                        existingVestigingen.Add(existing, relId);
                    else
                    {
                        var msg =
                            $"Er bestaan meerdere vestigingen in DocBase met vestigingnummer! VestigingNummer: {vestigingId}, RelIds: {existingVestigingen[existing]} & {relId}";
                        errors.Add(msg);
                        _logger.Warn(msg);
                    }
                }
                else
                    nonExistingVestigingen.Add(relId);
            }

            var changed = false;
            // Update the existingVestigingen
            foreach (var existingVestiging in existingVestigingen)
                changed |= UpdateVestiging(existingVestiging.Value, existingVestiging.Key, ref errors);

            // Now get and create the vestigingen that exist in kvk but don't exist in DocBase yet
            var newVestigingen = kvkInschrijving.Vestigingen.Except(existingVestigingen.Keys);
            foreach (var newVestiging in newVestigingen)
            {
                var newId = CreateVestiging(newVestiging);
                if (newId > 0)
                    _logger.Debug("Succesfully created DocBase vestiging with new Id {0} for vestiging with vestigingnummer {1}", newId, newVestiging.Vestigingsnummer);
                else
                {
                    var msg =
                        $"Could not create a new DocBase vestiging for vestiging with vestigingnummer {newVestiging.Vestigingsnummer}";
                    _logger.Error(msg);
                    var msgNL =
                        $"Kon geen nieuwe DocBase vestiging voor vestiging met vestigingnummer {newVestiging.Vestigingsnummer} aanmaken.";
                    errors.Add(msgNL);
                }
            }

            // Now report the vestigingen that exist in DocBase, but don't exist in kvk in log
            if (nonExistingVestigingen.Any())
            {
                var msg = string.Format("Found some vestigingen in DocBase with KvKnummer {0} which don't exist in " + Default.ApplicationName + ".\nVestigingen DocBase ID(s): {1}.\n\nEither delete the record(s) from DocBase or delete the corresponding KvKnummer from the record(s).", kvkInschrijving.KvkNummer, string.Join(", ", nonExistingVestigingen));
                _logger.Warn(msg);
                var msgNL = string.Format("Vestigingen met KvKnummer {0} in DocBase gevonden die niet bestaan bij de " + Default.ApplicationName + ".\nVestigingen DocBase ID(s): {1}.\n\nVerwijder deze record(s) uit DocBase of verwijder het corresponderende KvKnummer uit deze record(s).", kvkInschrijving.KvkNummer, string.Join(", ", nonExistingVestigingen));
                errors.Add(msgNL);
            }

            if (!errors.Any())
            {
                changed |= newVestigingen.Any();
                return new ExportResult(true, changed ? "De gegevens zijn succesvol geüpdatet in DocBase." : "Geen wijzigingen gevonden met de gegevens in DocBase.");
            }
            else
                return new ExportResult(false, "Er is iets fout gegaan bij de DocBase update", errors: errors);

        }

        public IExportResult InsertExternalRecord(KvkInschrijving kvkInschrijving)
        {
            if (kvkInschrijving == null)
                throw new ArgumentException("kvkInschrijving");

            _logger.Debug("Starting to insert Docbase record(s) for kvkInschrijving with kvkNummer: " + kvkInschrijving.KvkNummer);
            _logger.Debug("Number of vestigingen: " + kvkInschrijving.Vestigingen.Count);

            // Login to ensure the cookie.
            Login();

            // Ensure no records with this KvK number exist
            var whereClause = FormatWhereClauseDocbase(kvkInschrijving.KvkNummer,
                kvkInschrijving.Vestigingen.Select(v => v.Vestigingsnummer));
            var relationsList = _relationsService.GetRelationsList(RELATIONTYPE, whereClause);
            var indexFields = relationsList.Tables[INDEXFIELDS];

            if (indexFields.Rows.Count > 0)
            {
                var msg = $"Items with kvkNummer {kvkInschrijving.KvkNummer} already exist in DocBase, update instead.";
                _logger.Error(msg);
                var msgNL =
                    $"Er bestaat al een organisatie met kvkNummer {kvkInschrijving.KvkNummer} in DocBase. Gebruik de update functie.";
                return new ExportResult(false, msgNL);
            }

            var newIds = new List<string>();
            var errors = new List<string>();
            foreach (var newVestiging in kvkInschrijving.Vestigingen)
            {
                var newId = CreateVestiging(newVestiging);
                if (newId > 0)
                {
                    var msg  =
                        $"Succesfully created DocBase vestiging with new Id {newId} for vestiging with vestigingnummer {newVestiging.Vestigingsnummer}";
                    _logger.Debug(msg);
                    var msgNL =
                        $"Er is een nieuwe DocBase vestiging aangemaakt met Id {newId} voor de vestiging met vestigingnummer {newVestiging.Vestigingsnummer}.";
                    newIds.Add(msgNL);
                }
                else
                {
                    var msg =
                        $"Could not create a new DocBase vestiging for vestiging with vestigingnummer {newVestiging.Vestigingsnummer}";
                    _logger.Error(msg);
                    var msgNL =
                        $"Er kon geen nieuwe DocBase vestiging aangemaakt worden voor de vestiging met vestigingnummer {newVestiging.Vestigingsnummer}.";
                    errors.Add(msgNL);
                }
            }

            if (!errors.Any())
                return new ExportResult(true, "Succesvol aangemaakt in DocBase. \n\n" + string.Join("\n", newIds));
            else if (newIds.Any())
                return new ExportResult(false, "Er is iets fout gegaan! Niet alle vestigingen zijn aangemaakt in DocBase. Wel aangemaakt: \n\n" + string.Join("\n", newIds), errors: errors);
            else
                return new ExportResult(false, "Er is iets fout gegaan! Er zijn geen gegevens aangemaakt in DocBase.", errors: errors);
        }

        public IExportResult UpdateExternalVestiging(Vestiging vestiging)
        {
            if (vestiging == null)
                throw new ArgumentException("vestiging");

            _logger.Debug("Starting to update Docbase record for vestiging with vestigingNummer: " + vestiging.Vestigingsnummer);

            // Login to ensure the cookie.
            Login();

            // Get all records with this KvK number
            var whereClause = $"VEST_ID = '{vestiging.Vestigingsnummer}'";
            var relationsList = _relationsService.GetRelationsList(RELATIONTYPE, whereClause);

            var indexFields = relationsList.Tables[INDEXFIELDS];

            if (indexFields.Rows.Count == 0)
            {
                _logger.Warn("Could not find any records in DocBase for whereClause: " + whereClause);
                return new ExportResult(false, "Deze vestiging bestaat nog niet in DocBase.", noItemsFoundInsertInstead: true);
            }

            var errors = new List<string>();

            if (indexFields.Rows.Count > 1)
            { 
                foreach(DataRow row in indexFields.Rows)
                    errors.Add("Vestiging met REL_ID: " + row["REL_ID"]);
                _logger.Error("Found multiple DocBase records with Vestiging_ID: " + vestiging.Vestigingsnummer + "\n" + string.Join("\n", errors));
                return new ExportResult(false, "Er bestaan meerdere vestigingen in DocBase met dit vestigingnummer!", errors: errors);
            }

            var relId = Convert.ToInt32(indexFields.Rows[0]["REL_ID"]);
            var changed = UpdateVestiging(relId, vestiging, ref errors);

            if (!errors.Any())
                return new ExportResult(true, changed ? "De gegevens zijn succesvol geüpdatet in DocBase." : "De gegevens kwamen overeen met de gegevens in DocBase.");
            else
                return new ExportResult(false, "Er is iets fout gegaan bij de DocBase update", errors: errors);
        }

        public IExportResult InsertExternalVestiging(Vestiging vestiging)
        { 
            if (vestiging == null)
                throw new ArgumentException("vestiging");

            _logger.Debug("Starting to insert Docbase record for kvkVestiging with vestigingsNummer: " + vestiging.Vestigingsnummer);

            // Login to ensure the cookie.
            Login();

            // Ensure no records with this KvK number exist
            var whereClause = $"VEST_ID = '{vestiging.Vestigingsnummer}'";
            var relationsList = _relationsService.GetRelationsList(RELATIONTYPE, whereClause);
            var indexFields = relationsList.Tables[INDEXFIELDS];

            string msgNL;
            if (indexFields.Rows.Count > 0)
            {
                var msg =
                    $"Items with vestigingsNummer {vestiging.Vestigingsnummer} already exists in DocBase, update instead.";
                _logger.Error(msg);
                msgNL =
                    $"Er bestaat al een vestiging met vestigingNummer {vestiging.Vestigingsnummer} in DocBase. Gebruik de update functie.";
                return new ExportResult(false, msgNL);
            }
                        
            var errors = new List<string>();
            var newId = CreateVestiging(vestiging);
            if (newId > 0)
            {
                var msg =
                    $"Succesfully created DocBase vestiging with new Id {newId} for vestiging with vestigingnummer {vestiging.Vestigingsnummer}";
                _logger.Debug(msg);
                msgNL =
                    $"Er is een nieuwe DocBase vestiging aangemaakt met Id {newId} voor de vestiging met vestigingnummer {vestiging.Vestigingsnummer}.";
            }
            else
            {
                var msg =
                    $"Could not create a new DocBase vestiging for vestiging with vestigingnummer {vestiging.Vestigingsnummer}";
                _logger.Error(msg);
                msgNL =
                    $"Er kon geen nieuwe DocBase vestiging aangemaakt worden voor de vestiging met vestigingnummer {vestiging.Vestigingsnummer}.";
                errors.Add(msgNL);
            }

            if (!errors.Any())
                return new ExportResult(true, "Succesvol aangemaakt in DocBase. \n\n" + newId);
            else
                return new ExportResult(false, "Er is iets fout gegaan! Er zijn geen gegevens aangemaakt in DocBase.", errors: errors);
        }

        public void UpdateAllExistingExternalVestigingen(IEnumerable<Vestiging> vestigingen, Logger functionalLogger, int maxDegreeOfParallelism)
        {
            // Login to ensure the cookie.
            Login();

            // Get all records with this KvK number
            var whereClause = string.Format("KVK_ID IS NOT NULL AND VEST_ID IS NOT NULL");
            var relationsList = _relationsService.GetRelationsList(RELATIONTYPE, whereClause);

            var indexFields = relationsList.Tables[INDEXFIELDS];

            if (indexFields.Rows.Count == 0)
            {
                functionalLogger.Warn("Geen vestigingen gevonden in DocBase!");
                _logger.Warn("Could not find any records in DocBase for whereClause: " + whereClause);
                return;
            }

            var errors = new List<string>();
            var nonExistingVestigingen = new List<string>();
            int changed = 0;
            var changedIds = new List<string>();

            functionalLogger.Debug("{0} vestigingen gevonden in DocBase.", indexFields.Rows.Count);

            Parallel.ForEach(indexFields.Rows.OfType<DataRow>(), new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (row) =>
            {
                var relId = Convert.ToInt32(row["REL_ID"]);
                var vestigingId = row["VEST_ID"].ToString();
                try
                {
                    var existing = vestigingen.SingleOrDefault(v => v.Vestigingsnummer == vestigingId);
                    if (existing != null)
                    {
                        if (UpdateVestiging(relId, existing, ref errors, functionalLogger.Trace))
                        {
                            changed++;
                            changedIds.Add(
                                $"KvKnummer: {existing.KvkInschrijving.KvkNummer}, vestigingsnummer: {existing.Vestigingsnummer}");
                        }
                    }
                    else
                    {
                        nonExistingVestigingen.Add(
                            $"Could not find vestiging with ID {relId} and VestigingsNummer {vestigingId} in Kernregister Database.");
                        errors.Add(string.Format("Kon geen vestiging vinden voor RelId {0} met KvK ID {2} en VestigingsNummer {1}", relId, vestigingId, row["KVK_ID"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error at vestigingId: " + vestigingId + " and relId: " + relId + "!");
                    var msg = $"Fout bij vestiging voor RelId {relId} en vestigingsNummer {vestigingId}: {ex.Message}";
                    functionalLogger.Error(msg);
                    errors.Add(msg);
                }
            });

            functionalLogger.Debug("{0} vestigingen gewijzigd in DocBase.", changed);
            functionalLogger.Debug("Gewijzigde vestigingen: {0}", string.Join(" | ", changedIds));

            if(errors.Any())
            {
                _logger.Error("Found errors while updating all vestigingen!");
                _logger.Error(string.Join(Environment.NewLine, nonExistingVestigingen));

                functionalLogger.Error("{0} fouten gevonden tijdens het updaten.", errors.Count);
                functionalLogger.Error(string.Join(Environment.NewLine, errors));

                throw new DataException("Found errors while updating all vestigingen! Check the logs.");
            }
        }

        #endregion

        /// <summary>
        /// Logs on to Docbase using the username and password from configuration
        /// </summary>
        private void Login()
        {
            if (!_loggedOn || _expireLoginTime > DateTime.Now)
            {
                _loggedOn = Login(_credentials.Username, _credentials.Password);
                if (!_loggedOn)
                {
                    _logger.Warn("Could not logon user: {0}", _credentials.Username);
                    throw new DocBaseNotAuthenticatedException();
                }
                else
                {
                    _logger.Debug("Succesfully logged on to DocBase.");
                    _expireLoginTime = DateTime.Now.AddMinutes(1);
                }
            }
        }

        private bool Login(string userName, string password)
        {
            var loginResponse = _relationsService.Login(new LoginRequest(userName, password));
            return loginResponse != null && loginResponse.LoginResult;
        }

        /// <summary>
        /// Creates a new vestiging in DocBase
        /// </summary>
        private int CreateVestiging(Vestiging newVestiging)
        {
            Login();

            var ifFieldDataSet = _relationsService.GetNewRelationIndexFields(RELATIONTYPE);
            var ifFields = ifFieldDataSet.Tables[INDEXFIELDS];

            UpdateDataTableFieldsIfChanged(ref ifFields, newVestiging);

            return _relationsService.CreateRelation(new CreateRelationRequest(RELATIONTYPE, _credentials.AuthId, _credentials.ProcessId, ifFieldDataSet)).CreateRelationResult;
            
        }

        /// <summary>
        /// Checks for changes on the vestiging and updates the changes if any
        /// </summary>
        private bool UpdateVestiging(int relId, Vestiging vestiging, ref List<string> errors, Action<string> logChange = null)
        {
            Login();

            var ifFieldDataSet = _relationsService.GetExistingRelationIndexFields(relId, RELATIONTYPE);
            var ifFields = ifFieldDataSet.Tables[INDEXFIELDS];

            // Checkerdecheckski
            if (ifFields.GetValue("VEST_ID") != vestiging.Vestigingsnummer)
            {
                var msg =
                    $"DocBase record with id {relId} has a VEST_ID of {ifFields.GetValue("VEST_ID")} while trying to update vestiging with vestigingsnummer {vestiging.Vestigingsnummer}...";
                _logger.Error(msg);
                errors.Add(msg);
                return false;
                //throw new InvalidOperationException(msg);
            }

            // Search for changed values and update those
            var changed = UpdateDataTableFieldsIfChanged(ref ifFields, vestiging, logChange);
            if (changed)
            {
                // Now update the DocBase Relation using the service
                // Functionally decided not to use the existing, but always overwrite
                //var authId = Convert.ToInt32(ifFieldDataSet.Tables[RELATIONTABLE].Rows[0]["AUTOPROFIEL_ID"]);
                var authId = _credentials.AuthId;
                var updateSuccess = _relationsService.UpdateRelation(new UpdateRelationRequest(relId, RELATIONTYPE, authId, _credentials.ProcessId, ifFieldDataSet));
                if (updateSuccess.UpdateRelationResult)
                    _logger.Debug("Succesfully updated vestiging with vestigingnummer {0} and DocBase REL_ID {1}", vestiging.Vestigingsnummer, relId);
                else
                {
                    var msg =
                        $"Could not update vestiging with vestigingnummer {vestiging.Vestigingsnummer} and DocBase REL_ID {relId}";
                    _logger.Error(msg);
                    var msgNL =
                        $"Kon de vestiging met vestigingnummer {vestiging.Vestigingsnummer} en DocBase REL_ID {relId} niet updaten.";
                    errors.Add(msgNL);
                    return false;
                }
            }

            return changed;
        }

        /// <summary>
        /// Updates the dataTableFields if any changes are found and returns whether or not any changes where found.
        /// </summary>
        private bool UpdateDataTableFieldsIfChanged(ref DataTable ifFields, Vestiging vestiging, Action<string> logChange = null)
        { 
            // Ensure the mapping is Set.
            SetMapping();

            var changed = false;
            
            // Update the DocBase ifFields only if data is changed.
            foreach (var map in Mapping)
            {
                var vestigingValue = map.Value(vestiging) ?? "";
                var docBaseValue = ifFields.GetValue(map.Key) ?? "";
                if (docBaseValue != vestigingValue)
                {
                    // Ensure not to override "important" DocBase keys
                    if (DoNotOverrideDocBaseKeys.Contains(map.Key)
                        && !string.IsNullOrWhiteSpace(docBaseValue))
                        continue;

                    // Ensure not to override DocBase if KvK is empty, but DocBase is not
                    if (DoNotOverrideDocBaseKeysIfEmpty.Contains(map.Key)
                        && string.IsNullOrWhiteSpace(vestigingValue)
                        && !string.IsNullOrWhiteSpace(docBaseValue))
                        continue;

                    // Ensure not to override DocBase if DocBase has a "postbus" and KvK not
                    if (DoNotOverrideDocBaseIfPostbus.Contains(map.Key))
                    {
                        var docBaseIsPostbus = (ifFields.GetValue("PASTRAATNAAM") ?? "").ToLower().Contains("postbus");
                        var vestigingIsPostbus = !string.IsNullOrWhiteSpace(vestiging.Postbusnummer);

                        if(docBaseIsPostbus && !vestigingIsPostbus)
                            continue;
                    }

                    changed = true;
                    ifFields.SetValue(map.Key, vestigingValue);

                    var changeMessage =
                        $"Value changed for Vestiging {vestiging.Vestigingsnummer}, map.Key: |{map.Key}|, map.Value: |{vestigingValue}|, docBaseValue: |{docBaseValue}|";
                    _logger.Debug(changeMessage);
                    if (logChange != null)
                        logChange(changeMessage);
                }
            }

            // If any changes found, set the "Last mutation date"
            if (changed)
                ifFields.SetValue("LAATSTE_MUTATIEDT", DateTime.Now.ToString());

            return changed;
        }

        private string FormatWhereClauseDocbase(string kvkNummer, IEnumerable<string> vestigingNummers)
        {
            return
                $"KVK_ID = '{kvkNummer}' OR VEST_ID in ({string.Join(",", vestigingNummers.Select(n => $"'{n}'"))})";
        }

        #region DocBase Mapping

        private static readonly Dictionary<string, Func<Vestiging, string>> Mapping = new Dictionary<string, Func<Vestiging, string>>();
        private static readonly List<string> DoNotOverrideDocBaseKeys = new List<string> { 
            "TELEFOON",
            "FAX",
            "E_MAIL",
            "KVK_ID",
            "VEST_ID",
        };
        private static readonly List<string> DoNotOverrideDocBaseKeysIfEmpty = new List<string> {
            "PAPOSTCODE_ZOEK",
            "PAPOSTCODE1",
            "PAPOSTCODE2",
            "PAPOSTCODE", 
            "PAHUISNR",
            "PAHUISNRTOEV",
            "PASTRAATNAAM",
            "PAADRES_1",
            "PAPLAATS",
            "PAGEMEENTE",
            "PAPROVINCIE",
            "B_BOX_NAAM"
        };
        private static readonly List<string> DoNotOverrideDocBaseIfPostbus = new List<string> {
            "PAPOSTCODE_ZOEK",
            "PAPOSTCODE1" ,
            "PAPOSTCODE2", 
            "PAPOSTCODE",
            "PAHUISNR",
            "PAHUISNRTOEV",
            "PASTRAATNAAM",
            "PAADRES_1",
            "PAPLAATS",
            "PAGEMEENTE"
        };

        private static void SetMapping()
        {
            if (Mapping.Any())
                return;

            // REL_ID, SAMENGESTELDE_NAAM, POSTCODE_ZOEK, ADRES_1, PC_PLAATS, NAT_RECHTSPERSOON, BEDRIJFSNAAM, HANDELSNAAM, RECHTSVORM, 
            // ACHTERNAAM, TITEL_VOOR, VOORLETTERS, TUSSENVOEGSEL, TITEL_NA, GESLACHT, DATUM_OVERLEDEN, E_MAIL, POSTCODE1, POSTCODE2, POSTCODE, 
            // HUISNRTOEV, STRAATNAAM, PLAATS, LANDCODE, LAND, GEMEENTE, PROVINCIE, PAPOSTCODE_ZOEK, PAPOSTCODE1, PAPOSTCODE2, PAPOSTCODE, PAHUISNR, 
            // PAHUISNRTOEV, PASTRAATNAAM, PAADRES_1, PAPLAATS, PALANDCODE, PALAND, PAPC_PLAATS, PAGEMEENTE, PAPROVINCIE, TELEFOON, FAX, LAATSTE_MUTATIEDT, 
            // AANHEFOMSCHRIJVING, ADRESAANHEF, BRIEFAANHEF, KIXCODE, PAKIXCODE, BSN_FINUMMER, HUISNR, KVK_ID, VEST_ID, RSIN, EORI

            //_mapping.Add("SAMENGESTELDE_NAAM", (vestiging) => { return vestiging.Naam; });
            Mapping.Add("POSTCODE_ZOEK", (vestiging) => string.Concat(vestiging.PostcodeCijfers, vestiging.PostcodeLetters, vestiging.Huisnummer, vestiging.Huisnummertoevoeging));
            Mapping.Add("ADRES_1", (vestiging) =>
                $"{vestiging.Straat} {vestiging.Huisnummer + vestiging.Huisnummertoevoeging}");
            //_mapping.Add("PC_PLAATS", (vestiging) => { return vestiging.Woonplaats; });

            // TODO: check Rechtsvorm
            // Set based on PREFIX value
            Mapping.Add("NAT_RECHTSPERSOON", (vestiging) => "Rechtspersoon"); // "Natuurlijk Persoon" => in dit geval altijd rechtspersoon

            Mapping.Add("BEDRIJFSNAAM", (vestiging) => vestiging.Naam.Replace(Vestiging.RECHTSPERSOONPREFIX, string.Empty));
            Mapping.Add("HANDELSNAAM", (vestiging) => vestiging.KvkInschrijving.InschrijvingNaam);

            //_mapping.Add("RECHTSVORM", (vestiging) => { return vestiging.; });
            //_mapping.Add("ACHTERNAAM", (vestiging) => { return vestiging.KvkInschrijving.; });
            //_mapping.Add("TITEL_VOOR", (vestiging) => { return vestiging.; });
            //_mapping.Add("VOORLETTERS", (vestiging) => { return vestiging.; });
            //_mapping.Add("TUSSENVOEGSEL", (vestiging) => { return vestiging.; });
            //_mapping.Add("TITEL_NA", (vestiging) => { return vestiging.; });
            //_mapping.Add("GESLACHT", (vestiging) => { return vestiging.; });
            //_mapping.Add("DATUM_OVERLEDEN", (vestiging) => { return vestiging.; });
            Mapping.Add("E_MAIL", (vestiging) => vestiging.Email);
            Mapping.Add("POSTCODE1", (vestiging) => vestiging.PostcodeCijfers);
            Mapping.Add("POSTCODE2", (vestiging) => vestiging.PostcodeLetters);
            Mapping.Add("POSTCODE", (vestiging) => vestiging.PostcodeCijfers + " " + vestiging.PostcodeLetters);
            Mapping.Add("HUISNRTOEV", (vestiging) => vestiging.Huisnummertoevoeging);
            Mapping.Add("STRAATNAAM", (vestiging) => vestiging.Straat);
            Mapping.Add("PLAATS", (vestiging) => vestiging.Woonplaats);
            //_mapping.Add("LANDCODE", (vestiging) => { return vestiging.; });
            //_mapping.Add("LAND", (vestiging) => { return vestiging.; });
            Mapping.Add("GEMEENTE", (vestiging) =>
                _postcodeService.GetMunicipalityForPostcode(string.Concat(vestiging.PostcodeCijfers, vestiging.PostcodeLetters)) ?? vestiging.Gemeente);
            Mapping.Add("PROVINCIE", (vestiging) => 
                _postcodeService.GetCountrySubdivisionForPostcode(string.Concat(vestiging.PostcodeCijfers, vestiging.PostcodeLetters)));
            Mapping.Add("PAPOSTCODE_ZOEK", (vestiging) => string.Concat(vestiging.PostPostcodeCijfers, vestiging.PostPostcodeLetters
                    , string.IsNullOrEmpty(vestiging.Postbusnummer) 
                            ? string.Concat(vestiging.PostHuisnummer, vestiging.PostHuisnummerToevoeging) 
                            : vestiging.Postbusnummer));

            Mapping.Add("PAPOSTCODE1", (vestiging) => vestiging.PostPostcodeCijfers);
            Mapping.Add("PAPOSTCODE2", (vestiging) => vestiging.PostPostcodeLetters);
            Mapping.Add("PAPOSTCODE", (vestiging) => $"{vestiging.PostPostcodeCijfers} {vestiging.PostPostcodeLetters}");
            Mapping.Add("PAHUISNR", (vestiging) => string.IsNullOrEmpty(vestiging.Postbusnummer) ? vestiging.PostHuisnummer : vestiging.Postbusnummer);
            Mapping.Add("PAHUISNRTOEV", (vestiging) => vestiging.PostHuisnummerToevoeging);
            Mapping.Add("PASTRAATNAAM", (vestiging) => string.IsNullOrEmpty(vestiging.Postbusnummer) ? vestiging.PostStraat : "Postbus");
            Mapping.Add("PAADRES_1", (vestiging) => 
                string.IsNullOrWhiteSpace(vestiging.Postbusnummer)
                ? $"{vestiging.PostStraat} {vestiging.PostHuisnummer + vestiging.PostHuisnummerToevoeging}"
                    : string.Concat("Postbus ", vestiging.Postbusnummer));
            Mapping.Add("PAPLAATS", (vestiging) => vestiging.PostWoonplaats);
            //_mapping.Add("PALANDCODE", (vestiging) => { return vestiging.; });
            //_mapping.Add("PALAND", (vestiging) => { return vestiging.; });
            Mapping.Add("PAGEMEENTE", (vestiging) => 
                _postcodeService.GetMunicipalityForPostcode(string.Concat(vestiging.PostPostcodeCijfers, vestiging.PostPostcodeLetters)) ?? vestiging.PostGemeente);
            Mapping.Add("PAPROVINCIE", (vestiging) =>
                _postcodeService.GetCountrySubdivisionForPostcode(string.Concat(vestiging.PostPostcodeCijfers, vestiging.PostPostcodeLetters)));
            Mapping.Add("TELEFOON", (vestiging) => vestiging.Telefoon);
            Mapping.Add("FAX", (vestiging) => vestiging.Fax);

            // Set only if changes are made!
            //_mapping.Add("LAATSTE_MUTATIEDT", (vestiging) => { return DateTime.Now.ToString(); });

            //_mapping.Add("AANHEFOMSCHRIJVING", (vestiging) => { return vestiging.; });
            //_mapping.Add("ADRESAANHEF", (vestiging) => { return vestiging.; });
            //_mapping.Add("BRIEFAANHEF", (vestiging) => { return vestiging.; });
            //_mapping.Add("KIXCODE", (vestiging) => { return vestiging.; });
            //_mapping.Add("PAKIXCODE", (vestiging) => { return vestiging.; });
            //_mapping.Add("BSN_FINUMMER", (vestiging) => { return vestiging.; });
            Mapping.Add("HUISNR", (vestiging) => vestiging.Huisnummer);
            Mapping.Add("KVK_ID", (vestiging) => vestiging.KvkInschrijving.KvkNummer);
            Mapping.Add("VEST_ID", (vestiging) => vestiging.Vestigingsnummer);
            Mapping.Add("RSIN", (vestiging) => vestiging.RSIN);
            Mapping.Add("EORI", (vestiging) => vestiging.EORI);

            Mapping.Add("VK_INDICATIE_BB", (vestiging) => string.IsNullOrWhiteSpace(vestiging.KvkInschrijving?.BerichtenBoxNaam) ? "N" : "J");
            Mapping.Add("B_BOX_NAAM", (vestiging) => vestiging.KvkInschrijving?.BerichtenBoxNaam);
        }

        #endregion

    }
}