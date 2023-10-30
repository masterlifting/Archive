using CommandLine;

namespace rcli.Modules.InitializeSql;

public class InitializeSqlOptions
{
    [Option (
         "database-name"
        , Required = true
        , HelpText = "Database name to run the update against."
    )]
    public string DbName { get; set; }

    [Option (
        "sql-script"
        , Required = true
        , Default = 0
        , HelpText = "The SQL script file-name to run."
    )]
    public string SqlScriptFileName { get; set; }
}