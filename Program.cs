using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneTimePass
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Generating One Time Password started..."+"\n");

            string username = string.Empty;
            Console.WriteLine("Please insert username :");
            username = Console.ReadLine();

            DateTime date;
            string datestr;
            //DateTime accepts mm/dd/yyyy format so insert this format 
            Console.WriteLine("Please insert date in format MM/DD/YYYY: ");
            datestr = Console.ReadLine();
            date = DateTime.Parse(datestr);

            Console.WriteLine("Input parameters succesfully completed. Generating one-time password");
            string OTP = GenerateOTP();
            Console.WriteLine("OTP Generated is: " + OTP);
            //Opening connection with DB to save the user and OTP
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\OneTimePass\\OneTimePass\\Users.mdf;Integrated Security=True";
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            //Inserting in "Users" table the OTP
            string query = "INSERT INTO Users values (@User, @OTP, @CreatedDateTime, @ExpireDateTime, 1)";
            SqlCommand insert = new SqlCommand(query, con);
            insert.Parameters.AddWithValue("@User", username);
            insert.Parameters.AddWithValue("@OTP", OTP);
            insert.Parameters.AddWithValue("@CreatedDateTime", DateTime.Now);
            insert.Parameters.AddWithValue("@ExpireDateTime", DateTime.Now.AddSeconds(30));

            insert.ExecuteNonQuery();

            //Wait 30 sec
            Thread.Sleep(30000);

            //OTP Expired, update the table  and set isValid=0

            string updateQuery = "UPDATE Users SET isValid = 0 where ExpireDateTime < @ExpireDateTime";
            SqlCommand update = new SqlCommand(updateQuery, con);
            update.Parameters.AddWithValue("@ExpireDateTime", DateTime.Now);
            update.ExecuteNonQuery();

            con.Close();

        }

        static public string GenerateOTP()
        {
            const string src = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = 7;

            var sb = new StringBuilder();
            Random RNG = new Random();

            for (var i = 0; i < length; i++)
            {
                var c = src[RNG.Next(0, src.Length)];
                sb.Append(c);
            }

            return sb.ToString();

        }
    }
}
