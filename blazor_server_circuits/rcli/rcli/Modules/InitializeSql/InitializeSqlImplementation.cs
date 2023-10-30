using Dapper;
using Npgsql;
using Lungmuss.Refractory.Library;

namespace rcli.Modules.InitializeSql;

public class InitializeSqlImplementation
{
    public static async Task<int>Run(InitializeSqlOptions options)
    {
        var cb = new NpgsqlConnectionStringBuilder
        {
            Host = EnvironmentVars.pgDbHost, Database = options.DbName, Username = EnvironmentVars.pgDbUser,
            Port = Convert.ToInt32(EnvironmentVars.pgDbPort), Password = EnvironmentVars.pgDbPassword
        };
        
        var sqlScript = await File.ReadAllTextAsync(options.SqlScriptFileName);
        await using var conn = new NpgsqlConnection(cb.ToString());
        return await conn.ExecuteAsync(sqlScript);
    }
}