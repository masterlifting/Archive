using CommandLine;

namespace rcli.Modules.HelloWorld;

public class HelloWorldOptions
{
    [Option (
        'h'
        , "hello-world"
        , Required = true
        , Default = "Refractory"
        , HelpText = "Give the hello-word addressee."
    )]
    public string Name { get; set; }

    [Option (
        'r'
        , "return-value"
        , Required = false
        , Default = 0
        , HelpText = "Return value."
    )]
    public int ReturnValue { get; set; }
}