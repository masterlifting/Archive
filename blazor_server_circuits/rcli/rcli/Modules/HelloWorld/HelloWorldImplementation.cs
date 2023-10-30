namespace rcli.Modules.HelloWorld;

public class HelloWorldImplementation
{
    public static async Task<int>Run( HelloWorldOptions options)
    {
        Console.WriteLine($"Hello {options.Name}!");
        return await Task.FromResult(options.ReturnValue);
    }
}