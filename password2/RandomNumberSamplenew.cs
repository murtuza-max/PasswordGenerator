using Microsoft.Data.SqlClient;
using System.Data;
class RandomNumberSamplenew
{
    const string CONNECTION_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=PasswordData;Integrated Security=True";
    const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
    const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string NUMERIC_CHARACTERS = "0123456789";
    const string SPECIAL_CHARACTERS = @"!#$%&*@\";
    public static int datasize = 300000;
    public static int tasksize = 10; 


    public static async Task Main(string[] args)
    {
        var watch = new System.Diagnostics.Stopwatch();
        Console.WriteLine("password insertion in progress...");
        watch.Start();
        await RunAllMeth();
        watch.Stop();
        Console.WriteLine($"Execution Time: {(watch.ElapsedMilliseconds) / 1000} SEC");
        
    }

    public static async Task RunAllMeth()
    {
        int taskdatasize = datasize / tasksize;
        List<Task> tasks = new List<Task>();

        for (int i = 0; i < tasksize; i++)
        {
            SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            tasks.Add(Insertdata1(taskdatasize, conn));
        }
        if ((datasize % tasksize) > 0)
        {
            SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            tasks.Add(Insertdata1((datasize % 10), conn));
        }

        Task t = Task.WhenAll(tasks);
        try
        {
            await t;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Insertion Exception : {ex.Message}");
        }
        if (t.Status == TaskStatus.RanToCompletion)
            Console.WriteLine("password insertion Completed...");
        else if (t.Status == TaskStatus.Faulted)
            Console.WriteLine("password insertion Failed...");
    }

    public static async Task Insertdata1(int tasksize, SqlConnection conn)
    {
        SqlCommand cmd = new SqlCommand("sp_InsertPass", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@Pass", SqlDbType.VarChar, 20);
        conn.Open();
        for (int i = 0; i < tasksize; i++)
        {
            var pass = GeneratePassword(20);
            cmd.Parameters["@Pass"].Value = pass;
            await cmd.ExecuteNonQueryAsync();
        }
        conn.Close();
    }

    public static string GeneratePassword(int lengthOfPassword)
    {
        string characterSet = "";
        characterSet += LOWERCASE_CHARACTERS;
        characterSet += UPPERCASE_CHARACTERS;
        characterSet += NUMERIC_CHARACTERS;
        characterSet += SPECIAL_CHARACTERS;

        char[] password = new char[lengthOfPassword];
        int characterSetLength = characterSet.Length;
        Random random = new Random();

        for (int characterPosition = 0; characterPosition < lengthOfPassword; characterPosition++)
        {
            password[characterPosition] = characterSet[random.Next(characterSetLength - 1)];
        }
        return string.Join(null, password);
    }
}