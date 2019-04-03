using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Oracle.ManagedDataAccess.Client;
using QNH.Overheid.KernRegister.Business.Crm.NAdres;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using QNH.Overheid.KernRegister.Organization.Resources;

namespace QNH.Overheid.KernRegister.Business.Crm.nAdres
{
    public class NAdresRepository : IExportService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private string TableName {
            get { return FullTableName.Split('.').Last(); }
        }
        private string FullTableName { get; set; }
        private string ConnectionString { get; set; }

        public string DisplayName { get; private set; }

        #region Constructor

        public NAdresRepository(
            string connectionString,
            string fullTableName = "NADRES.BEDRIJF",
            string displayName = "NADRES")
        {
            FullTableName = fullTableName;
            ConnectionString = connectionString;
            DisplayName = displayName;
        }

        #endregion

        #region IExportService implementation

        public IExportResult UpdateExternalRecord(KvkInschrijving kvkInschrijving)
        {
            if (kvkInschrijving == null)
                throw new ArgumentException("kvkInschrijving");

            _logger.Debug($"Starting to update {DisplayName} for ${Default.ApplicationName} Inschrijving with kvkNummer: {kvkInschrijving.KvkNummer}");
            _logger.Debug("Number of vestigingen: " + kvkInschrijving.Vestigingen.Count);


            // Get all records with this KvK number
            var whereClause = FormatWhereClause(kvkInschrijving.KvkNummer,
                kvkInschrijving.Vestigingen.Select(v => v.Vestigingsnummer));

            var vestigingen = GetnAdresRecords(whereClause);

            if (vestigingen.Rows.Count == 0)
            {
                _logger.Warn($"Could not find any records in {DisplayName} for whereClause: {whereClause}");
                return new ExportResult(false, $"Deze inschrijving bestaat nog niet in {DisplayName}.", noItemsFoundInsertInstead: true);
            }

            var errors = new List<string>();

            //  Get the existingVestigingen and the vestigingen that exist in NADRES but don't exist in Kvk
            var existingVestigingen = new Dictionary<Vestiging, string>();
            var nonExistingVestigingen = new List<int>();

            foreach (DataRow row in vestigingen.Rows)
            {
                var bedrijfsNr = Convert.ToInt32(row["BEDRIJFSNR"]);
                var vestigingId = row["VESTIGINGSNR"].ToString();
                var existing = kvkInschrijving.Vestigingen.FirstOrDefault(v => v.Vestigingsnummer == vestigingId);
                if (existing != null)
                    existingVestigingen.Add(existing, vestigingId);
                else
                    nonExistingVestigingen.Add(bedrijfsNr);
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
                    _logger.Debug($"Succesfully created {DisplayName} vestiging with new Id {newId} for vestiging with vestigingnummer {newVestiging.Vestigingsnummer}");
                else
                {
                    var msg = $"Could not create a new {DisplayName} vestiging for vestiging with vestigingnummer {newVestiging.Vestigingsnummer}";
                    _logger.Error(msg);
                    var msgNL = $"Kon geen nieuwe {DisplayName} vestiging voor vestiging met vestigingnummer {newVestiging.Vestigingsnummer} aanmaken.";
                    errors.Add(msgNL);
                }
            }

            // Now report the vestigingen that exist in DocBase, but don't exist in kvk in log
            if (nonExistingVestigingen.Any())
            {
                var msg = $"Found some vestigingen in {DisplayName} with KvKnummer {kvkInschrijving.KvkNummer} which don't exist in {Default.ApplicationName}\nVestigingen NADRES ID(s): {string.Join(", ", nonExistingVestigingen)}.\n\nEither delete the record(s) from NADRES or delete the corresponding KvKnummer from the record(s).";
                _logger.Warn(msg);
                var msgNL = $"Vestigingen met KvKnummer {kvkInschrijving.KvkNummer} in NADRES gevonden die niet bestaan bij de {Default.ApplicationName}.\nVestigingen NADRES ID(s): {string.Join(", ", nonExistingVestigingen)}.\n\nVerwijder deze record(s) uit NADRES of verwijder het corresponderende KvKnummer uit deze record(s).";
                errors.Add(msgNL);
            }

            if (!errors.Any())
            {
                changed |= newVestigingen.Any();
                return new ExportResult(true, changed ? $"De gegevens zijn succesvol geüpdatet in {DisplayName}." : $"Geen wijzigingen gevonden met de gegevens in {DisplayName}.");
            }
            else
                return new ExportResult(false, $"Er is iets fout gegaan bij de {DisplayName} update", errors: errors);
        }

        public IExportResult InsertExternalRecord(KvkInschrijving kvkInschrijving)
        {
            if (kvkInschrijving == null)
                throw new ArgumentException("kvkInschrijving");

            _logger.Debug("Starting to insert NADRES record(s) for kvkInschrijving with kvkNummer: " + kvkInschrijving.KvkNummer);
            _logger.Debug("Number of vestigingen: " + kvkInschrijving.Vestigingen.Count);


            // Ensure no records with this KvK number exist
            var whereClause = FormatWhereClause(kvkInschrijving.KvkNummer,
                kvkInschrijving.Vestigingen.Select(v => v.Vestigingsnummer));

            var vestigingen = GetnAdresRecords(whereClause);

            if (vestigingen.Rows.Count > 0)
            {
                var msg = $"Items with kvkNummer {kvkInschrijving.KvkNummer} already exist in {DisplayName}, update instead.";
                _logger.Error(msg);
                var msgNL =
                    $"Er bestaat al een organisatie met kvkNummer {kvkInschrijving.KvkNummer} in {DisplayName}. Gebruik de update functie, of zet het KVK_LEIDEND vinkje aan.";
                return new ExportResult(false, msgNL);
            }

            var newIds = new List<string>();
            var errors = new List<string>();
            foreach (var newVestiging in kvkInschrijving.Vestigingen)
            {
                var newId = CreateVestiging(newVestiging);
                if (newId > 0)
                {
                    var msg =
                        $"Succesfully created {DisplayName} vestiging with new Id {newId} for vestiging with vestigingnummer {newVestiging.Vestigingsnummer}";
                    _logger.Debug(msg);
                    var msgNL =
                        $"Er is een nieuwe {DisplayName} vestiging aangemaakt met Id {newId} voor de vestiging met vestigingnummer {newVestiging.Vestigingsnummer}.";
                    newIds.Add(msgNL);
                }
                else
                {
                    var msg =
                        $"Could not create a new {DisplayName} vestiging for vestiging with vestigingnummer {newVestiging.Vestigingsnummer}";
                    _logger.Error(msg);
                    var msgNL =
                        $"Er kon geen nieuwe {DisplayName} vestiging aangemaakt worden voor de vestiging met vestigingnummer {newVestiging.Vestigingsnummer}.";
                    errors.Add(msgNL);
                }
            }

            if (!errors.Any())
                return new ExportResult(true, $"Succesvol aangemaakt in {DisplayName}. \n\n" + string.Join("\n", newIds));
            else if (newIds.Any())
                return new ExportResult(false, $"Er is iets fout gegaan! Niet alle vestigingen zijn aangemaakt in {DisplayName}. Wel aangemaakt: \n\n" + string.Join("\n", newIds), errors: errors);
            else
                return new ExportResult(false, $"Er is iets fout gegaan! Er zijn geen gegevens aangemaakt in {DisplayName}.", errors: errors);
        }

        public IExportResult UpdateExternalVestiging(Vestiging vestiging)
        {
            if (vestiging == null)
                throw new ArgumentException("vestiging");

            _logger.Debug($"Starting to update {DisplayName} record for vestiging with vestigingNummer: " + vestiging.Vestigingsnummer);

            // Get all records with this KvK number
            var whereClause = FormatWhereClauseVestiging(vestiging.KvkInschrijving.KvkNummer, vestiging.Vestigingsnummer);
            var vestigingen = GetnAdresRecords(whereClause);

            if (vestigingen.Rows.Count == 0)
            {
                _logger.Warn($"Could not find any records in {DisplayName} for whereClause: " + whereClause);
                return new ExportResult(false, $"Deze vestiging bestaat nog niet in {DisplayName}.", noItemsFoundInsertInstead: true);
            }

            var errors = new List<string>();

            if (vestigingen.Rows.Count > 1)
            {
                foreach (DataRow row in vestigingen.Rows)
                    errors.Add("Vestiging met BEDRIJFSNR: " + row["BEDRIJFSNR"]);
                _logger.Error($"Found multiple {DisplayName} records with Vestiging_ID: " + vestiging.Vestigingsnummer + "\n" + string.Join("\n", errors));
                return new ExportResult(false, $"Er bestaan meerdere vestigingen in {DisplayName} met dit vestigingnummer!", errors: errors);
            }

            var changed = UpdateVestiging(vestiging.Vestigingsnummer, vestiging, ref errors);
            if (!errors.Any())
                return new ExportResult(true, 
                    changed 
                        ? $"De gegevens zijn succesvol geüpdatet in {DisplayName}."
                        : $"De gegevens kwamen overeen met de gegevens in {DisplayName} of KVK_LEIDEND staat uit.");
            else
                return new ExportResult(false, $"Er is iets fout gegaan bij de {DisplayName} update", errors: errors);
        }

        public IExportResult InsertExternalVestiging(Vestiging vestiging)
        {
            if (vestiging == null)
                throw new ArgumentException("vestiging");

            _logger.Debug($"Starting to insert {DisplayName} record for kvkVestiging with vestigingsNummer: " + vestiging.Vestigingsnummer);

            // Ensure no records with this KvK number exist
            var whereClause = FormatWhereClauseVestiging(vestiging.KvkInschrijving.KvkNummer, vestiging.Vestigingsnummer);
            var vestigingen = GetnAdresRecords(whereClause);

            string msgNL;
            if (vestigingen.Rows.Count > 0)
            {
                var msg =
                    $"Items with vestigingsNummer {vestiging.Vestigingsnummer} already exists in {DisplayName}, update instead.";
                _logger.Error(msg);
                msgNL =
                    $"Er bestaat al een vestiging met vestigingNummer {vestiging.Vestigingsnummer} in {DisplayName}. Gebruik de update functie, of zet het KVK_LEIDEND vinkje aan";
                return new ExportResult(false, msgNL);
            }

            var errors = new List<string>();
            var newId = CreateVestiging(vestiging);
            if (newId > 0)
            {
                var msg =
                    $"Succesfully created {DisplayName} vestiging with new Id {newId} for vestiging with vestigingnummer {vestiging.Vestigingsnummer}";
                _logger.Debug(msg);
                msgNL =
                    $"Er is een nieuwe {DisplayName} vestiging aangemaakt met Id {newId} voor de vestiging met vestigingnummer {vestiging.Vestigingsnummer}.";
            }
            else
            {
                var msg =
                    $"Could not create a new {DisplayName} vestiging for vestiging with vestigingnummer {vestiging.Vestigingsnummer}";
                _logger.Error(msg);
                msgNL =
                    $"Er kon geen nieuwe {DisplayName} vestiging aangemaakt worden voor de vestiging met vestigingnummer {vestiging.Vestigingsnummer}.";
                errors.Add(msgNL);
            }

            if (!errors.Any())
                return new ExportResult(true, $"Succesvol aangemaakt in {DisplayName}. \n\n" + newId);
            else
                return new ExportResult(false, $"Er is iets fout gegaan! Er zijn geen gegevens aangemaakt in {DisplayName}.", errors: errors);
        }

        public void UpdateAllExistingExternalVestigingen(IEnumerable<Vestiging> vestigingen, NLog.Logger functionalLogger, int maxDegreeOfParallelism)
        {
            var whereClause = string.Format("KVKNR IS NOT NULL AND VESTIGINGSNR IS NOT NULL AND KVK_LEIDEND = 'J'");
            var vestigingenRecords = GetnAdresRecords(whereClause);

            if (vestigingenRecords.Rows.Count == 0)
            {
                functionalLogger.Warn($"Geen vestigingen gevonden in {DisplayName}!");
                _logger.Warn($"Could not find any records in {DisplayName} for whereClause: " + whereClause);
                return;
            }

            var errors = new List<string>();
            var nonExistingVestigingen = new List<string>();
            int changed = 0;
            var changedIds = new List<string>();

            functionalLogger.Debug($"{0} vestigingen gevonden in {DisplayName} (waar het KVK_LEIDEND vinkje aan staat).", vestigingenRecords.Rows.Count);

            Parallel.ForEach(vestigingenRecords.Rows.OfType<DataRow>(), new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (row) =>
            {
                var bedrijfsNr = Convert.ToInt32(row["BEDRIJFSNR"]);
                var vestigingId = row["VESTIGINGSNR"].ToString();
                try
                {
                    var existing = vestigingen.SingleOrDefault(v => v.Vestigingsnummer == vestigingId);
                    if (existing != null)
                    {
                        if (UpdateVestiging(vestigingId, existing, ref errors, functionalLogger.Trace))
                        {
                            changed++;
                            changedIds.Add(
                                $"KvKnummer: {existing.KvkInschrijving.KvkNummer}, vestigingsnummer: {existing.Vestigingsnummer}");
                        }
                    }
                    else
                    {
                        nonExistingVestigingen.Add(
                            $"Could not find vestiging with ID {bedrijfsNr} and VestigingsNummer {vestigingId} in Kernregister Database.");
                        errors.Add(string.Format("Kon geen vestiging vinden voor BEDRIJFSNR {0} met KvK ID {2} en VestigingsNummer {1}", bedrijfsNr, vestigingId, row["KVKNR"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error at vestigingId: " + vestigingId + " and relId: " + bedrijfsNr + "!");
                    var msg =
                        $"Fout bij vestiging voor BEDRIJFSNR {bedrijfsNr} en vestigingsNummer {vestigingId}: {ex.Message}";
                    functionalLogger.Error(msg);
                    errors.Add(msg);
                }
            });

            functionalLogger.Debug($"{changed} vestigingen gewijzigd in {DisplayName}.");
            functionalLogger.Debug("Gewijzigde vestigingen: {0}", string.Join(" | ", changedIds));

            if (errors.Any())
            {
                _logger.Error("Found errors while updating all vestigingen!");
                _logger.Error(string.Join(Environment.NewLine, nonExistingVestigingen));

                functionalLogger.Error("{0} fouten gevonden tijdens het updaten.", errors.Count);
                functionalLogger.Error(string.Join(Environment.NewLine, errors));

                throw new DataException("Found errors while updating all vestigingen! Check the logs.");
            }
        }

        #endregion

        #region Private methods

        private DataTable GetnAdresRecords(string sqlWhereClause)
        {
            var sql = $"SELECT * FROM {FullTableName} WHERE {sqlWhereClause}";

            var table = new DataSet();
            using (var da = new OracleDataAdapter(sql, ConnectionString))
            {
                da.Fill(table, TableName);
            }
            return table.Tables[TableName];
        }

        private string FormatWhereClause(string kvkNummer, IEnumerable<string> vestigingNummers)
        {
            return
                $"KVKNR = '{kvkNummer}' OR VESTIGINGSNR in ({string.Join(",", vestigingNummers.Select(n => $"'{n}'"))})";
        }

        private string FormatWhereClauseVestiging(string kvkNummer, string vestigingsNr)
        {
            return $"KVKNR = '{kvkNummer}' AND VESTIGINGSNR = '{vestigingsNr}'";
        }

        private int CreateVestiging(Vestiging vestiging)
        {
            // Ensure the mapping is Set.
            SetMapping();

            var insertIntoColumns = new List<string>();
            var insertIntoValues = new List<string>();

            foreach (var map in _mapping)
            {
                insertIntoColumns.Add($"{map.Key}");
                insertIntoValues.Add($"'{(map.Value(vestiging) ?? "").Replace("'", "''")}'");
            }

            var insertIntoString =
                $"INSERT INTO {FullTableName} (BEDRIJFSNR, {"DATUMINVOER"}, {"TIJDINVOER"}, {string.Join(",", insertIntoColumns)}) ";
            var valuesString =
                $"VALUES (NADRES.SEQBEDRIJFSID.NEXTVAL, {"SYSDATE"}, {"SYSTIMESTAMP"}, {string.Join(",", insertIntoValues)}) ";

            var sql = string.Concat(insertIntoString, valuesString);

            using (var myConn = new OracleConnection(ConnectionString))
            {
                var myCommand = new OracleCommand(sql, myConn);
                myConn.Open();
                myCommand.ExecuteNonQuery();
            }

            // Get all records with this KvK number
            var whereClause = FormatWhereClauseVestiging(vestiging.KvkInschrijving.KvkNummer,vestiging.Vestigingsnummer);
            var vestigingen = GetnAdresRecords(whereClause);
            if (vestigingen.Rows.Count != 1)
            {
                _logger.Warn($"Could not find any (or too many??) records in {DisplayName} for whereClause: " + whereClause);
                return -1;
            }

            return Convert.ToInt32(vestigingen.Rows[0]["BEDRIJFSNR"]);
            
        }

        /// <summary>
        /// Checks for changes on the vestiging and updates the changes if any
        /// </summary>
        private bool UpdateVestiging(string vestigingsNr, Vestiging vestiging, ref List<string> errors, Action<string> logChange = null)
        {
            // Get all records with this KvK number
            var whereClause = FormatWhereClauseVestiging(kvkNummer: vestiging.KvkInschrijving.KvkNummer, vestigingsNr: vestiging.Vestigingsnummer);
            var nAdresRecord = GetnAdresRecords(whereClause);

            // Checkerdecheckski
            if (nAdresRecord.GetValue("VESTIGINGSNR") != vestiging.Vestigingsnummer)
            {
                var msg =
                    $"{DisplayName} record with id {vestigingsNr} has a VEST_ID of {nAdresRecord.GetValue("VESTIGINGSNR")} while trying to update vestiging with vestigingsnummer {vestiging.Vestigingsnummer}...";
                _logger.Error(msg);
                errors.Add(msg);
                return false;
            }

            // Check if KVK_LEIDEND
            if (nAdresRecord.GetValue("KVK_LEIDEND").ToUpper() != "J")
            {
                _logger.Trace($"{DisplayName} record with id {vestigingsNr} has KVK_LEIDEND = '{nAdresRecord.GetValue("KVK_LEIDEND")}' so the vestiging is not being updated.");
                return false;
            }

            var changes = GetChanges(nAdresRecord, vestiging, logChange);
            if (changes.Any())
            {
                // Now update the NADRES Relation using the service
                // Functionally decided not to use the existing, but always overwrite

                var updateSuccess = false;
                try
                {
                    var sql =
                        $"UPDATE {FullTableName} SET {string.Join(",", changes.Select(v => $"{v.Key}='{v.Value.Replace("'", "''")}'"))}, DATUMWIJZIGKVK=SYSDATE WHERE {whereClause}";

                    using (var myConn = new OracleConnection(ConnectionString))
                    {
                        var myCommand = new OracleCommand(sql, myConn);
                        myConn.Open();
                        myCommand.ExecuteNonQuery();
                    }

                    updateSuccess = true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error on executing update query");
                }

                if (updateSuccess)
                    _logger.Debug("Succesfully updated vestiging with vestigingnummer {0} and NADRES VESTIGINGSNR {1}", vestiging.Vestigingsnummer, vestigingsNr);
                else
                {
                    var msg =
                        $"Could not update vestiging with vestigingnummer {vestiging.Vestigingsnummer} and {DisplayName} VESTIGINGSNR {vestigingsNr}";
                    _logger.Error(msg);
                    var msgNL =
                        $"Kon de vestiging met vestigingnummer {vestiging.Vestigingsnummer} en {DisplayName} VESTIGINGSNR {vestigingsNr} niet updaten.";
                    errors.Add(msgNL);
                    return false;
                }
            }

            return changes.Any();
        }

        public Dictionary<string, string> GetChanges(DataTable nAdresRow, Vestiging vestiging, Action<string> logChange = null)
        {
            SetMapping();

            var changes = new Dictionary<string, string>();

            foreach (var map in _mapping)
            {
                var originalValue = nAdresRow.GetValue(map.Key) ?? "";
                var newValue = map.Value(vestiging) ?? "";

                if (originalValue != newValue)
                {
                    // Ensure not to override "important" NADRES keys
                    if (DoNotOverrideNadresKeys.Contains(map.Key)
                        && !string.IsNullOrWhiteSpace(originalValue))
                        continue;

                    // Ensure not to override NADRES if KvK is empty, but NADRES is not
                    if (DoNotOverrideNadresKeysIfEmpty.Contains(map.Key)
                        && string.IsNullOrWhiteSpace(newValue)
                        && !string.IsNullOrWhiteSpace(originalValue))
                        continue;

                    // Ensure not to override n-Adres if n-Adres has a "postbus" and KvK not
                    if (DoNotOverrideNadresIfPostbus.Contains(map.Key))
                    {
                        var nAdresIsPostbus = (nAdresRow.GetValue("POSTADRES") ?? "").ToLower().Contains("postbus");
                        var vestigingIsPostbus = !string.IsNullOrWhiteSpace(vestiging.Postbusnummer);

                        if (nAdresIsPostbus && !vestigingIsPostbus)
                            continue;
                    }

                    // Now add the actual change
                    changes.Add(map.Key, newValue);

                    // Log the change
                    var changeMessage =
                        $"Value changed for NAdres BEDRIJFSNR |{nAdresRow.GetValue("BEDRIJFSNR")}|, KvKNummer |{vestiging.KvkInschrijving.KvkNummer}|, Vestiging |{vestiging.Vestigingsnummer}|, map.Key: |{map.Key}|, map.Value: |{newValue}|, nAdresValue: |{originalValue}|";
                    _logger.Debug(changeMessage);
                    if (logChange != null)
                        logChange(changeMessage);
                }
            }

            return changes;
        }

        #endregion

        #region NADRES mapping

        private static readonly Dictionary<string, Func<Vestiging, string>> _mapping = new Dictionary<string, Func<Vestiging, string>>();
        private static readonly List<string> DoNotOverrideNadresKeys = new List<string> { 
            "LANDCODE",
            "EMAILADRES"
        };
        private static readonly List<string> DoNotOverrideNadresKeysIfEmpty = new List<string> {
            "TELNR1",
            "FAXNR",
        };
        private static readonly List<string> DoNotOverrideNadresIfPostbus = new List<string> {
            "POSTADRES",
            "HUISNRPA",  
            "HUISLTRPA",
            "TOEVOEGINGPA",
            "PCPOST",
            "PLAATSPA"
        };

        private static void SetMapping()
        {
            if (_mapping.Any())
                return;

            //_mapping.Add("BEDRIJFSNR"               , (vestiging) =>{ return vestiging.; );
            _mapping.Add("BEDRIJFSNAAM"             , (vestiging) => vestiging.Naam.Replace(Vestiging.RECHTSPERSOONPREFIX, string.Empty).ToMaxChars(60));
            //_mapping.Add("BEDRIJFSNAAM2"	        , (vestiging) => vestiging.KvkInschrijving.InschrijvingNaam.ToMaxChars(60));
            //_mapping.Add("ZOEKCODE"                 , (vestiging) => vestiging.Naam.Replace(Vestiging.RECHTSPERSOONPREFIX, string.Empty).ToMaxChars(10));
            _mapping.Add("BEZOEKADRES"              , (vestiging) => vestiging.Straat.ToMaxChars(43));
            _mapping.Add("HUISNRBA"		            , (vestiging) => vestiging.Huisnummer.ToMaxChars(5));
            //_mapping.Add("HUISLTRBA"		        , (vestiging) =>{ return vestiging.Huisnummertoevoeging; });
            _mapping.Add("TOEVOEGINGBA"	            , (vestiging) => vestiging.Huisnummertoevoeging.ToMaxChars(4));
            _mapping.Add("PLAATSBA"		            , (vestiging) => vestiging.Woonplaats.ToMaxChars(24));
            _mapping.Add("PCBEZOEK"	                , (vestiging) => (vestiging.PostcodeCijfers + vestiging.PostcodeLetters).ToMaxChars(15));
            _mapping.Add("POSTADRES", (vestiging) => string.IsNullOrWhiteSpace(vestiging.Postbusnummer) ? vestiging.PostStraat.ToMaxChars(43) : "Postbus");
            _mapping.Add("HUISNRPA", (vestiging) => (string.IsNullOrEmpty(vestiging.Postbusnummer) ? vestiging.PostHuisnummer : vestiging.Postbusnummer).ToMaxChars(5));
            //_mapping.Add("HUISLTRPA"		        , (vestiging) =>{ return vestiging.PostHuisnummerToevoeging; });
            _mapping.Add("TOEVOEGINGPA"	            , (vestiging) => vestiging.PostHuisnummerToevoeging.ToMaxChars(4));
            _mapping.Add("PCPOST"                   , (vestiging) => (vestiging.PostPostcodeCijfers + vestiging.PostPostcodeLetters).ToMaxChars(15));
            _mapping.Add("PLAATSPA"		            , (vestiging) => vestiging.PostWoonplaats.ToMaxChars(24));
            //_mapping.Add("FACTUURADRES"	            , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("HUISNRFA"                 , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("HUISLTRFA"		        , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("TOEVOEGINGFA"	            , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("PCFACTUUR"                , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("PLAATSFA"	                , (vestiging) =>{ return vestiging.; });
            _mapping.Add("LANDCODE"	                , (vestiging) => "NL");
            _mapping.Add("TELNR1"	                , (vestiging) => vestiging.Telefoon.ToMaxChars(15));
            //_mapping.Add("TELNR2"		            , (vestiging) =>{ return vestiging.Telefoon; });
            _mapping.Add("FAXNR"		            , (vestiging) => vestiging.Fax.ToMaxChars(15));
            _mapping.Add("KVKNR"		            , (vestiging) => vestiging.KvkInschrijving.KvkNummer);
            //_mapping.Add("BTWNR"		            , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("DATUMINVOER"              , (vestiging) =>{ return DateTime.Now.ToString(); });
            //_mapping.Add("TIJDINVOER"               , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("DATUMWIJZIGEN"	        , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("TIJDWIJZIGEN"             , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("URL"                      , (vestiging) => { return vestiging.; });
            _mapping.Add("EMAILADRES"               , (vestiging) => vestiging.Email.ToMaxChars(80));
            //_mapping.Add("GEWIJZIGDDOOR"	        , (vestiging) =>{ return vestiging.; });
            _mapping.Add("B_STATUS"                 , (vestiging) => "Actief");
            _mapping.Add("NAAMEIGENAAR"	            , (vestiging) => "Infopunt");
            _mapping.Add("AFDELINGEIGENAAR"	        , (vestiging) => "SENM");
            //_mapping.Add("TAV"                      , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("AANHEF"			        , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("OVERIGEGEGEVENS"          , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("MANAGEMENTINFO"           , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("CATEGORIEN"	            , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("ROCORDLOCK"		        , (vestiging) =>{ return vestiging.; });
            _mapping.Add("BED_ZICHTBAARHEID"        , (vestiging) => "0");
            //_mapping.Add("BED_ALTBEDRIJFSNAAM"      , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("BED_ALTBEDRIJFSNAAM"      , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("KMAFSTAND"                , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("GEP_BEDRIJFSPAGINA"       , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("GEP_WACHTWOORD"	        , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("GEP_TOONPROJECTEN"	    , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("GEP_TOONURENREG"	        , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("GEP_TOONLICENTIES"	    , (vestiging) =>{ return vestiging.; });
            _mapping.Add("VESTIGINGSNR"             , (vestiging) => vestiging.Vestigingsnummer);
            //_mapping.Add("DATUMWIJZIGKVK"		    , (vestiging) =>{ return vestiging.; });
            _mapping.Add("HOOFD_NEVENVESTIGING"     , (vestiging) => vestiging.IsHoofdvestiging ? "H" : "N");
            //_mapping.Add("B_SOORT"                  , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("LATITUDE"		            , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("LONGITUDE"		        , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("GEB_CONT_PERSNR"          , (vestiging) =>{ return vestiging.; });
            //_mapping.Add("GEB_CONT_NAAM"            , (vestiging) =>{ return vestiging.; });
            _mapping.Add("KVK_LEIDEND"              , (vestiging) => "J");

        }

        #endregion
    }
}