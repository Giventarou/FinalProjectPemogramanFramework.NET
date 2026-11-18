namespace DailyFinanceApp
{
    // Menyimpan data user yang sedang login.
    // Nanti backend mengisi ini setelah login berhasil.
    public static class Session
    {
        public static int UserId { get; set; }
        public static string Name { get; set; }
        public static string Username { get; set; }
        public static string Role { get; set; }   // "Admin" atau "User"

        public static bool IsAdmin => Role == "Admin";

        public static void Clear()
        {
            UserId = 0;
            Name = null;
            Username = null;
            Role = null;
        }
    }
}