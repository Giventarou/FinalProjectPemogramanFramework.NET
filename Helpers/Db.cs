using MySql.Data.MySqlClient;
using System;

namespace DailyFinanceApp
{
    // Satu-satunya tempat yang tahu cara konek ke database.
    // Halaman lain tinggal panggil Db.GetConnection().
    public static class Db
    {
        private const string ConnectionString =
            "Server=localhost;Port=3306;Database=db_keuangan_harian;Uid=root;Pwd=;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        // Dipakai untuk cek koneksi berhasil atau tidak
        public static string TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return "Koneksi berhasil!";
                }
            }
            catch (Exception ex)
            {
                return "Koneksi gagal: " + ex.Message;
            }
        }
    }
}