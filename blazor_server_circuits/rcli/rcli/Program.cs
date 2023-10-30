#region setup bootstrap logger

// Setup logging before anything else.
using CommandLine;
using CommandLine.Text;
using Lungmuss.Refractory.Library.Exceptions;
using rcli.Modules.HelloWorld;
using rcli.Modules.InitializeSql;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/bootstrap.log")
    .Enrich.WithAssemblyName()
    .Enrich.WithAssemblyVersion()
    .Enrich.WithAssemblyInformationalVersion()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .CreateBootstrapLogger();

var assembly = System.Reflection.Assembly.GetExecutingAssembly();
var informationalVersion = assembly
    .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
    .OfType<System.Reflection.AssemblyInformationalVersionAttribute>()
    .FirstOrDefault();

Log.ForContext<Program>()
    .Information("Assembly full name: {Fullname}", assembly.FullName);
Log.ForContext<Program>()
    .Information("Location: {Location}", assembly.Location);
Log.ForContext<Program>()
    .Information("Informational version: {InformationalVersion}", informationalVersion?.InformationalVersion ?? "N/A");


#endregion

void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errors)
{
    var helpText = HelpText.AutoBuild(result, h =>
    {
        var version = informationalVersion?.InformationalVersion ?? "N/A";
        // Add customizations to the help text here.
        h.AdditionalNewLineAfterOption = false; // Remove the default newline between options
        h.Heading = $"rcli {version} - Copyright (c) 2023 Chemikalien-Gesellschaft Hans Lungmuß mbH & Co. KG"; // Customize this line as you wish
        return HelpText.DefaultParsingErrorsHandler(result, h);
    }, e => e);

    Console.WriteLine(helpText);
}

async Task<int> HandleError(IEnumerable<Error> arg)
{
    return await Task.FromResult(0);
}

int result = -1;

if (args[0] == "database")
{
    var parsed = Parser.Default.ParseArguments<InitializeSqlOptions>(args)
        .WithParsed( o => result = InitializeSqlImplementation.Run(o).Result)
        .WithNotParsed( e => result =  HandleError(e).Result);

    return result;
}

throw new BusinessException("Nothing to do.");