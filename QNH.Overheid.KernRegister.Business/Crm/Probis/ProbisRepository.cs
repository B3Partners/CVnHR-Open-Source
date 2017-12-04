using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Oracle.ManagedDataAccess.Client;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using QNH.Overheid.KernRegister.Organization.Resources;

namespace QNH.Overheid.KernRegister.Business.Crm.Probis
{
    public class ProbisRepository : IFinancialExportService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public string DisplayName { get; }

        private string ConnectionString { get; }

        private string InsertOrUpdateStoredProcedureName { get; }

        public ProbisRepository(
            string connectionString,
            string insertOrUpdateStoredProcedureName,
            string displayName = "Probis")
        {
            ConnectionString = connectionString;
            InsertOrUpdateStoredProcedureName = insertOrUpdateStoredProcedureName;
            DisplayName = displayName;
        }

        #region IFinancialExportService implementation

        public IExportResult Update(KvkInschrijving kvkInschrijving, FinancialProcesType type)
        {
            return InsertOrUpdateExternalRecord(kvkInschrijving, type);
        }

        public IExportResult Insert(KvkInschrijving kvkInschrijving, FinancialProcesType type)
        {
            return InsertOrUpdateExternalRecord(kvkInschrijving, type);
        }

        public IExportResult UpdateVestiging(Vestiging vestiging, FinancialProcesType type)
        {
            return InsertOrUpdateExternalVestiging(vestiging, type);
        }

        public IExportResult InsertVestiging(Vestiging vestiging, FinancialProcesType type)
        {
            return InsertOrUpdateExternalVestiging(vestiging, type);
        }

        public void UpdateAllExistingVestigingen(IEnumerable<Vestiging> vestigingen, Logger functionalLogger, int maxDegreeOfParallelism,
            FinancialProcesType type)
        {
            throw new NotImplementedException();
        }

        #endregion

        private IExportResult InsertOrUpdateExternalRecord(KvkInschrijving kvkInschrijving, FinancialProcesType type)
        {
            if (kvkInschrijving == null)
                throw new ArgumentException("kvkInschrijving");

            _logger.Debug($"Starting to update {DisplayName} for ${Default.ApplicationName} Inschrijving with kvkNummer: {kvkInschrijving.KvkNummer}");
            _logger.Debug("Number of vestigingen: " + kvkInschrijving.Vestigingen.Count);

            // TODO: get all vestigingnummers for kvkid and check if any removed (for logging).

            var errors = new List<string>();
            var successMsgs = new List<string>();
            // Insert all vestigingen
            foreach (var vestiging in kvkInschrijving.Vestigingen.OrderByDescending(v=> v.IsHoofdvestiging).ThenBy(v=> v.Naam))
            {
                var vestigingResult = InsertOrUpdateExternalVestiging(vestiging, type);
                if (!vestigingResult.Succes)
                {
                    errors.AddRange(vestigingResult.Errors);
                }
                else
                {
                    successMsgs.Add(vestigingResult.Message);
                }
            }

            if (!errors.Any())
            {
                var msg = $"De gegevens zijn succesvol aangemaak of geüpdatet in {DisplayName}.";
                msg += "\n" + string.Join("\n", successMsgs);
                _logger.Debug(msg);
                return new ExportResult(true, msg);
            }
            else
            {
                var msg =
                    $@"Er is iets fout gegaan bij de {DisplayName} update van {kvkInschrijving.Vestigingen.Count} vestigingen. Aantal fouten: {errors.Count}.
Succesvol verwerkte vestigingen:
{string.Join("\n", successMsgs)}
";
                _logger.Error(msg + $" Errors: {string.Join(" - ", errors)}");
                return new ExportResult(false, msg, errors: errors);
            }
        }

        private IExportResult InsertOrUpdateExternalVestiging(Vestiging vestiging, FinancialProcesType type)
        {
            var result = "error";
            var msg = "";
            using (var myConn = new OracleConnection(ConnectionString))
            {
                var cmd = new OracleCommand(InsertOrUpdateStoredProcedureName, myConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                myConn.Open();

                var isPostbus = !string.IsNullOrWhiteSpace(vestiging.Postbusnummer);

                cmd.Parameters.Add("p_relatie_type", OracleDbType.Varchar2).Value = 
                    type == FinancialProcesType.ProbisDebiteuren ? 'D' : 'C';
                cmd.Parameters.Add("p_kvknummer", OracleDbType.Varchar2).Value = vestiging.KvkInschrijving.KvkNummer;
                cmd.Parameters.Add("p_naam", OracleDbType.Varchar2).Value = vestiging.KvkInschrijving.InschrijvingNaam;
                cmd.Parameters.Add("p_vestigingsnummer", OracleDbType.Varchar2).Value = vestiging.Vestigingsnummer;
                cmd.Parameters.Add("p_naam_vestiging", OracleDbType.Varchar2).Value = vestiging.Naam;
                cmd.Parameters.Add("p_straat", OracleDbType.Varchar2).Value =
                    isPostbus ? "Postbus" : vestiging.PostStraat;
                cmd.Parameters.Add("p_huisnummer", OracleDbType.Varchar2).Value = 
                    isPostbus ? vestiging.Postbusnummer : vestiging.PostHuisnummer;
                cmd.Parameters.Add("p_huisnummer_toevoeging", OracleDbType.Varchar2).Value = vestiging.PostHuisnummerToevoeging;
                cmd.Parameters.Add("p_postcode_cijfers", OracleDbType.Varchar2).Value = vestiging.PostPostcodeCijfers;
                cmd.Parameters.Add("p_postcode_letters", OracleDbType.Varchar2).Value = vestiging.PostPostcodeLetters;
                cmd.Parameters.Add("p_woonplaats", OracleDbType.Varchar2).Value = vestiging.PostWoonplaats;
                cmd.Parameters.Add("x_return_status", OracleDbType.Varchar2, 1).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("x_return_message", OracleDbType.Varchar2, 32767).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                result = cmd.Parameters["x_return_status"].Value.ToString();
                msg = cmd.Parameters["x_return_message"].Value.ToString();
            }
            var success = result.ToUpper() == "S";
            return new ExportResult(success, $"Success result: {result} - message: {msg}", errors: success ? null : new[] { msg });
        }

    }
}
