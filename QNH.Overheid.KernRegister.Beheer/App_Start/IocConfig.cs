using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Driver;
using Oracle.ManagedDataAccess.Client;
using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Business.Business;
using QNH.Overheid.KernRegister.Business.Crm;
using QNH.Overheid.KernRegister.Business.Crm.DocBase;
using QNH.Overheid.KernRegister.Business.Crm.nAdres;
using QNH.Overheid.KernRegister.Business.Crm.Probis;
using QNH.Overheid.KernRegister.Business.Integration;
using QNH.Overheid.KernRegister.Business.Integration.WcfExtensions;
using QNH.Overheid.KernRegister.Business.KvK;
using QNH.Overheid.KernRegister.Business.KvK.Service;
using QNH.Overheid.KernRegister.Business.KvK.WcfExtensions;
using QNH.Overheid.KernRegister.Business.Model;
using QNH.Overheid.KernRegister.Business.Model.Entities;
using QNH.Overheid.KernRegister.Business.Model.nHibernate;
using QNH.Overheid.KernRegister.Business.Service;
using QNH.Overheid.KernRegister.Business.Service.BRMO;
using QNH.Overheid.KernRegister.Business.Service.KvK.v2_5;
using QNH.Overheid.KernRegister.Organization.Resources;
using StructureMap;
using StructureMap.Web;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using NHibernate.Dialect;
using QNH.Overheid.KernRegister.Business.Service.Users;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using System.Data.SqlServerCe;
using QNH.Overheid.KernRegister.Business.KvKSearchApi;

namespace QNH.Overheid.KernRegister.Beheer
{
    public static class IocConfig
    {
        public static bool CreateDatabase => Convert.ToBoolean(ConfigurationManager.AppSettings["CreateDatabase"]);

        private static ISessionFactory GetSessionFactory(string dbProvider, string schemaName)
        {
            IPersistenceConfigurer persistenceConfigurer = null;
            switch (dbProvider)
            {
                case "NHibernateOracle":
                    persistenceConfigurer = OracleDataClientConfiguration.Oracle10
                                .Driver<OracleManagedDataClientDriver>()
                                .ConnectionString(connstr =>
                                    connstr.FromConnectionStringWithKey("OracleConnection"))
                                .DefaultSchema(schemaName);
                    break;
                case "NHibernateSQL":
                    persistenceConfigurer = MsSqlConfiguration.MsSql2008
                        .Driver<Sql2008ClientDriver>()
                        .ConnectionString(connstr =>
                            connstr.FromConnectionStringWithKey("NHibernateSQLConnection"))
                        .DefaultSchema(schemaName);
                    break;
                case "NHibernatePostGRESQL":
                    persistenceConfigurer = PostgreSQLConfiguration.Standard
                        .Driver<NpgsqlDriver>()
                        .ConnectionString(connst =>
                            connst.FromConnectionStringWithKey("NHibernatePostGRESQLConnection"))
                        .Dialect<PostgreSQLDialect>()
                        .DefaultSchema(schemaName);
                    break;
                default: // SQLCE
                    persistenceConfigurer = MsSqlCeConfiguration.Standard
                        .Driver<SqlServerCeDriver>()
                        .ConnectionString(connstr =>
                            connstr.FromConnectionStringWithKey("NHibernateSQLCEConnection"))
                        .Dialect<FixedMsSqlCe40Dialect>()
                        .DefaultSchema(schemaName);
                    break;
            }


            var nhConfig = Fluently
                .Configure()
                .Database(persistenceConfigurer)
                .ExposeConfiguration(config =>
                {
                    if (dbProvider == "NHibernateSQLCE")
                    {
                        // Ensure the file is created if not yet exists
                        var connString = config.GetProperty(NHibernate.Cfg.Environment.ConnectionString);
                        using (var engine = new System.Data.SqlServerCe.SqlCeEngine(connString))
                        {
                            var file = new System.Data.SqlServerCe.SqlCeConnectionStringBuilder(connString).DataSource
                                            .Replace("|DataDirectory|", AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
                            if (!File.Exists(file))
                                engine.CreateDatabase();
                        }
                    }

                    // Create the script and if specified database
                    var scriptDir = AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "/SQL";
                    var scriptFileName = Path.Combine(scriptDir, $"Generated-SQL-{dbProvider}.sql");
                    if (CreateDatabase || !File.Exists(scriptFileName))
                    {
                        if (!Directory.Exists(scriptDir))
                            Directory.CreateDirectory(scriptDir);
                        var export = new NHibernate.Tool.hbm2ddl.SchemaExport(config);
                        export.SetDelimiter(";");
                        export.SetOutputFile(scriptFileName);
                        export.Execute(true, CreateDatabase, false);
                    }
                })

                // USE MANUAL MAPPINGS
                //.Mappings(mappings => mappings
                //    .FluentMappings.AddFromAssemblyOf<NHUnitOfWork>()
                //    .Conventions.Setup(c => c.Add(DefaultCascade.All())));

                // USE AUTOMAPPINGS
                .Mappings(mappings => mappings
                            .AutoMappings.Add(
                                AutoMap
                                    .AssemblyOf<KvkInschrijving>(new CustomMappingConfiguration())
                                    .UseOverridesFromAssemblyOf<KvkInschrijving>()
                                    //.Where(t => t.Namespace == "QNH.Overheid.KernRegister.Business.Model.Entities")
                                    .Conventions.Setup(c =>
                                    {
                                        c.Add(DefaultCascade.All());
                                        c.Add(new TableNameConvention((instance) =>
                                        {
                                            if (instance.EntityType.Namespace == "QNH.Overheid.KernRegister.Business.Model.RSGB2_2")
                                            {
                                                instance.Schema(ConfigurationManager.AppSettings["RSGB_SchemaName"] ?? "RSGB");
                                            }
                                        }));
                                        c.Add(new ColumnNameConvention());
                                        c.Add(new IndexableConvention());
                                    }))
                            );

            return nhConfig.BuildSessionFactory();
        }

        private static readonly Lazy<Container> _containerBuilder = new Lazy<Container>(ConfigureDependencies, LazyThreadSafetyMode.ExecutionAndPublication);

        public static IContainer Container => _containerBuilder.Value;

        private static Container ConfigureDependencies()
        {
            return new Container(x =>
            {
                // Scan with default conventions
                x.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.AssemblyContainingType<KvkInschrijving>();
                    scan.WithDefaultConventions();
                });

                // Database configuration
                var dbProvider = ConfigurationManager.AppSettings["DatabaseProvider"];
                switch (dbProvider)
                {
                    case "NHibernateOracle":
                    case "NHibernateSQLCE":
                    case "NHibernateSQL":
                    case "NHibernatePostGRESQL":
                        // nHibernate setup.
                        var schemaName = ConfigurationManager.AppSettings["DatabaseSchemaName"];

                        x.ForSingletonOf<ISessionFactory>().Use(GetSessionFactory(dbProvider, schemaName));

                        x.For<ISession>().Use((ctx) => ctx.GetInstance<ISessionFactory>().OpenSession()).AlwaysUnique();
                        x.For(typeof(IRepository<>)).Use(typeof(NHRepository<>)).AlwaysUnique();
                        x.For<IKvkInschrijvingRepository>().Use<KvkInschrijvingNHRepository>().AlwaysUnique();

                        // End nHibernate setup
                        break;
                    default:
                        throw new ConfigurationErrorsException("No valid 'DatabaseProvider' key found in AppSettings. Possible (implemented) values: EF | NHibernateOracle | NHibernateSQLCE | NHibernatePostGRESQL");
                }

                // Services:

                #region SearchService
                var searchServiceCacheTimeInHours = Convert.ToInt32(ConfigurationManager.AppSettings["SearchServiceCacheTimeInHours"] ?? "24");
                RawXmlCache.CacheInHours = searchServiceCacheTimeInHours;

                // Set up KvkSearchService
                var hrDataserviceVersionNumber = ConfigurationManager.AppSettings["HR-DataserviceVersionNumber"];
                if (hrDataserviceVersionNumber == "2.5")
                {
                    // KvK HR-Dataservice
                    var dataService = x.For<Dataservice>()
                        .Use<CustomDataService>()
                        .SelectConstructor(() => new CustomDataService());

                    x.For<IKvkSearchService>().HybridHttpOrThreadLocalScoped().Use<KvkDataSearchService>()
                        .SelectConstructor(() => new KvkDataSearchService(Container.GetInstance<Dataservice>(), "klantReferentie", 0))
                        .Ctor<string>().Is(ConfigurationManager.AppSettings["SearchServiceKlantReferentie"])
                        .Ctor<int>().Is(searchServiceCacheTimeInHours);

                    if (SettingsHelper.BrmoApplicationEnabled)
                    {
                        dataService.SetProperty(ds => ds.Endpoint
                                                        .EndpointBehaviors
                                                        .Add(new RawXmlActionBehavior(RawXmlCache.Add)));

                        // enable to get xml files
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableRawXmlBehavior"] ?? "false"))
                        {
                            dataService.SetProperty(ds => ds.Endpoint
                                .EndpointBehaviors
                                .Add(new RawXMLBehavior(ConfigurationManager.AppSettings["InschrijvingenXmlResponseFile"])));
                        }
                    }
                }
                else if (hrDataserviceVersionNumber == "3.0")
                {
                    // KvK HR-Dataservice
                    var dataService = x.For<Business.KvK.v30.Dataservice>()
                        .Use<Business.KvK.v30.CustomDataService>()
                        .SelectConstructor(() => new Business.KvK.v30.CustomDataService());

                    x.For<IKvkSearchService>().HybridHttpOrThreadLocalScoped().Use<Business.Service.KvK.v30.KvkDataSearchService>()
                        .SelectConstructor(() => new Business.Service.KvK.v30.KvkDataSearchService(Container.GetInstance<Business.KvK.v30.Dataservice>(), "klantReferentie", 0))
                        .Ctor<string>().Is(ConfigurationManager.AppSettings["SearchServiceKlantReferentie"])
                        .Ctor<int>().Is(searchServiceCacheTimeInHours);

                    if (SettingsHelper.BrmoApplicationEnabled)
                    {
                        dataService.SetProperty(ds => ds.Endpoint
                                                        .EndpointBehaviors
                                                        .Add(new RawXmlActionBehavior(RawXmlCache.Add)));

                        // enable to get xml files
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableRawXmlBehavior"] ?? "false"))
                        {
                            dataService.SetProperty(ds => ds.Endpoint
                                .EndpointBehaviors
                                .Add(new RawXMLBehavior(ConfigurationManager.AppSettings["InschrijvingenXmlResponseFile"])));
                        }
                    }
                }
                else
                    throw new ConfigurationErrorsException($"HR-DataserviceVersionNumber {hrDataserviceVersionNumber} not implemented.");

                // Only store kvk numbers in memory cache when Brmo application is setup
                if (SettingsHelper.BrmoApplicationEnabled)
                {
                    x.For<IBrmoSyncService>()
                        .Use<BrmoSyncService>()
                        .SelectConstructor(() => new BrmoSyncService(new BrmoReference()))
                        .Ctor<BrmoReference>().Is(new BrmoReference()
                        {
                            BaseUrl = SettingsHelper.BrmoApplicationBaseUrl,
                            UserName = ConfigurationManager.AppSettings["BrmoApplicationUserName"] ?? "brmo",
                            Password = ConfigurationManager.AppSettings["BrmoApplicationPassword"] ?? "brmo",
                        });
                }

                #endregion

                // Select the CrmApplication to use
                switch (ConfigurationManager.AppSettings["CrmToUse"]) // Default.CrmApplication)
                {
                    case "DocBase":
                        x.For<SecuritySoap>().Use<SecuritySoapClient>()
                                .SelectConstructor(() => new SecuritySoapClient())
                                .SetProperty(ssc => ssc.Endpoint.Behaviors.Add(new CookieManagerBehavior()));
                        x.For<RelationsSoap>().Use<RelationsSoapClient>()
                            .SelectConstructor(() => new RelationsSoapClient())
                            .SetProperty(rsc => rsc.Endpoint.Behaviors.Add(new CookieManagerBehavior()));

                        // We need a postcode service for DocBase Municipality
                        x.ForSingletonOf<IPostcodeService>().Use(new NationaalGeoregisterLocatieService(
                            ConfigurationManager.AppSettings["PostcodeServiceBaseUrl"]
                        ));

                        // Get the docbase credentials from the configuration
                        x.For<IExportCredentials>().Use(new DocBaseCredentials(
                                username: ConfigurationManager.AppSettings["DocBaseUsername"],
                                password: ConfigurationManager.AppSettings["DocBasePassword"],
                                authId: Convert.ToInt32(ConfigurationManager.AppSettings["DocBaseAuthId"] ?? "1"),
                                processId: Convert.ToInt32(ConfigurationManager.AppSettings["DocBaseProcessId"] ?? "1")
                            ));
                        x.For<IExportService>().Use<DocBaseRepository>();
                        break;
                    case "n-Adres":
                        x.For<IExportService>().Use(new NAdresRepository(
                                connectionString: ConfigurationManager.ConnectionStrings["OraclenAdresConnection"].ConnectionString,
                                fullTableName: ConfigurationManager.AppSettings["nAdresFullTableName"],
                                displayName: ConfigurationManager.AppSettings["CrmDisplayName"]
                            ));
                        break;
                    default:
                        throw new ConfigurationErrorsException("CrmApplication from settings is unknown to the application. Value searched for: " + ConfigurationManager.AppSettings["CrmToUse"]);
                }

                // Setup the financial service
                var probisInsertOrUpdateStoredProcedureName = ConfigurationManager.AppSettings["ProbisInsertOrUpdateStoredProcedureName"];
                if (!string.IsNullOrWhiteSpace(probisInsertOrUpdateStoredProcedureName)
                    && !string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["OracleProbisConnection"].ConnectionString))
                {
                    x.For<IFinancialExportService>().Use<ProbisRepository>()
                        .SelectConstructor(() => new ProbisRepository("connectionstring", "insertOrUpdateStoredProcedureName", "displayName"))
                        .Ctor<string>("connectionString").Is(() => ConfigurationManager.ConnectionStrings["OracleProbisConnection"].ConnectionString)
                        .Ctor<string>("insertOrUpdateStoredProcedureName").Is(() => probisInsertOrUpdateStoredProcedureName)
                        .Ctor<string>("displayName").Is(() => Default.FinancialApplication);
                }

                /* 
                 * Setup the usermanager 
                 */
                if (SettingsHelper.UseHardCodedUserManagerForTesting)
                {
                    x.ForSingletonOf<IUserManager>().Use<HardCodedUserManager>() // Use singleton for hardcoded!
                        .SelectConstructor(() => new HardCodedUserManager("userNameToUseWhenEmpty"))
                        .Ctor<string>("userNameToUseWhenEmpty").Is(() => SettingsHelper.UsernameToUseWhenEmpty);
                }
                else
                {
                    var brmoStagingConnectionString = ConfigurationManager.ConnectionStrings["BrmoStagingConnection"]?.ConnectionString;
                    var brmoStagingSchemaName = ConfigurationManager.AppSettings["BrmoStagingDatabaseSchemaName"];
                    var brmoStagingDatabaseParameterCharacter = ConfigurationManager.AppSettings["BrmoStagingDatabaseParameterCharacter"]
                            ?? (dbProvider.ToLowerInvariant().Contains("oracle") ? ":" : "@");
                    var brmoStagingUsernameCaseInsensitiveSearch = Convert.ToBoolean(ConfigurationManager.AppSettings["BrmoStagingUsernameCaseInsensitiveSearch"]);

                    // If left empty, just use the default connectionstring (needs the user tables)
                    if (string.IsNullOrWhiteSpace(brmoStagingConnectionString))
                    {
                        x.For<IDbConnection>().Use((ctx) => ctx.GetInstance<ISession>().Connection);
                        // use the default schemaname from the connection
                        brmoStagingSchemaName = ConfigurationManager.AppSettings["DatabaseSchemaName"];
                    }
                    else
                        x.For<IDbConnection>().Use((ctx) => GetbrmoStagingDbConnection(brmoStagingConnectionString));

                    x.For<IUserManager>().Use<BrmoUserManager>()
                        .SelectConstructor(() => new BrmoUserManager(null, "schemaName", "parameterChar", false))
                        .Ctor<string>("schemaName").Is(brmoStagingSchemaName)
                        .Ctor<string>("parameterChar").Is(brmoStagingDatabaseParameterCharacter)
                        .Ctor<bool>("usernameCaseInsensitiveSearch").Is(brmoStagingUsernameCaseInsensitiveSearch)
                        .AlwaysUnique();
                }

                // Setup the KvK Search Api
                x.For<IKvkSearchApi>().Use<KvkSearchApi>()
                    .SelectConstructor(() => new KvkSearchApi("baseUrl", "searchUrl", "apiKey"))
                    .Ctor<string>("baseUrl").Is(ConfigurationManager.AppSettings["KvkSearchApi.BaseUrl"])
                    .Ctor<string>("searchUrl").Is(ConfigurationManager.AppSettings["KvkSearchApi.SearchUrl"])
                    .Ctor<string>("apiKey").Is(ConfigurationManager.AppSettings["KvkSearchApi.ApiKey"]);
            });
        }

        private static IDbConnection GetbrmoStagingDbConnection(string brmoStagingConnectionString)
        {
            var brmoStagingDbProvider = ConfigurationManager.AppSettings["BrmoStagingDbProvider"];
            switch (brmoStagingDbProvider)
            {
                case "NHibernateOracle":
                    return new OracleConnection(brmoStagingConnectionString);
                case "NHibernateSQL":
                    return new SqlConnection(brmoStagingConnectionString);
                case "NHibernatePostGRESQL":
                    return new NpgsqlConnection(brmoStagingConnectionString);
                case "NHibernateSQLCE":
                    return new SqlCeConnection(brmoStagingConnectionString);
                default:
                    throw new ConfigurationErrorsException("Unknown BrmoStagingDbProvider!");
            }
        }
    }

    public class FixedMsSqlCe40Dialect : NHibernate.Dialect.MsSqlCe40Dialect
    {
        public override bool SupportsVariableLimit => true;
    }
}