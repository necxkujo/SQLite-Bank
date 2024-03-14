using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Linq;


namespace AtmSqlite
{
   internal class Program
   {   

            //Um Nutzer in verschiedenen Methoden zu haben
            public static string CurrentUser;
            public static string newUserName;
        
            //Main Führt andere Methoden und Zeilen in sich aus
            static void Main(string[] args)
            {
                greetings();
                InitializeDatabase();
                UserLogin();
            
            }

            //Überprüft Datenbank + oder erstellt
            static void InitializeDatabase()
            {
       
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Trying to connect to our Database...");
                Console.ResetColor();
                try
                {

                    // Verbindet mit Datebank.
                    using (var connection = new SqliteConnection("Data Source=atm.db"))
                    {
                        connection.Open();
                        Console.ForegroundColor = ConsoleColor.Green;
                    
                        Console.WriteLine("Connection to Database sucessfull.");
                        Console.ResetColor();
                        var command = connection.CreateCommand();

                    
                        //Erstellt Table falls nicht vorhanden.
                        command.CommandText = @"CREATE TABLE IF NOT EXISTS bankaccounts (
    AccountID INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT NOT NULL,
    pin TEXT NOT NULL,
    Guthaben REAL NOT NULL );";
                        command.ExecuteNonQuery();
                    }
                }

                //Gibt Fehlermeldung aus
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"An error eccured: {ex.Message}");
                    Console.ResetColor();
                }
            }

            //Begrüßt Nutzer
            static void greetings()
            {
                Console.WriteLine("Hello, World!");
                Console.WriteLine("Welcome to SQLiteBank");
            }

            //Erstellt BankAccount in der Database
            static void UserProfile()
            {
                string wTC;

                //Stellt eine Verbindung zur Datenbank her
                using (var connection = new SqliteConnection("Data Source=atm.db"))
                {

                    //Öffnet die Verbindung zur Datenbank
                    connection.Open();
                    var insertCommand = connection.CreateCommand();

                    //Gibt Nutzer die Wahl einen Account zu erstellen
                    Console.WriteLine("Do you want to create an account? (y/ n)");
                    UserExist = "no";
                    Console.Write("Choice: ");
                    wTC = Console.ReadLine();
                    if (wTC == "y" || wTC == "yes")
                    {
                        string newUserPin;
                        Console.Clear();
                        Console.WriteLine("To register, please enter your Name.");
                        Console.Write("Name: ");
                        newUserName = Console.ReadLine();

                        static string ToUpperFirstLetter(string source)
                        {
                            if (string.IsNullOrEmpty(source))
                                return string.Empty;

                            char[] letters = source.ToCharArray();
                            letters[0] = char.ToUpper(letters[0]);

                            for (int i = 1; i < letters.Length; i++)
                            {
                                letters[i] = char.ToLower(letters[i]);
                            }

                        return new string(letters);
                    }
                        IsUserIsAlreadyExisting();
                        if (UserExist == "yes")
                        {
                            Console.WriteLine("User already exists");
                            UserLogin();
                        }
                        else
                        {

                            Console.WriteLine("Please enter the Password of your choice!");
                            Console.Write("Password: ");
                            newUserPin = Console.ReadLine();

                            //Gibt an in welcher Tabelle welche Werte wo eingegeben werden sollen
                            string formattedName = ToUpperFirstLetter(newUserName);
                            insertCommand.CommandText = "INSERT INTO bankaccounts (username, pin, Guthaben) VALUES (@username, @pin, @balance)";
                            insertCommand.Parameters.AddWithValue("@username", formattedName);
                            insertCommand.Parameters.AddWithValue("@pin", newUserPin);
                            insertCommand.Parameters.AddWithValue("@balance", 1000.0);
                            insertCommand.ExecuteNonQuery();

                            Console.WriteLine(@"New User created. You will get to the LogIn now.");
                            UserLogin();
                            connection.Close();
                        }
                    }
                    else if (wTC == "no" || wTC == "n")
                    {
                        UserLogin();
                    }
                }
            }

            //Nutzer/Admin Anmeldefenster
            static void UserLogin()
            {

                //Für die Anmeldung: Name und Pin
                string UserInput1;
                string UserInput2;
                string AdminPassword;
                int tries = 3;
                ConsoleColor showColor = ConsoleColor.DarkCyan;
                ConsoleColor editColor = ConsoleColor.DarkGreen;
                ConsoleColor deleteColor = ConsoleColor.DarkMagenta;
                ConsoleColor exitColor = ConsoleColor.DarkRed;

                Console.WriteLine("Please enter your name.");
                Console.Write("Name: ");
                UserInput1 = Console.ReadLine();
                CurrentUser = UserInput1;

                //Schaut ob Admin Name eingegeben wurde, sonst normaler Login
                if (UserInput1 == "Admin" || UserInput1 == "admin")
                {
                    Console.Clear();
                    Console.WriteLine(@"Nice try...
    Self destruction...");
                    Environment.Exit(0);

                }
                    if (UserInput1 == "SecretAdmin")
                    {
                    string AdminInput;
                    Console.Write("Password: ");
                    AdminPassword = Console.ReadLine();

                    //Gleicht das eingegebene Passwort mit dem Adminpasswort ab, sonst reset
                    if (AdminPassword == "Admin123")
                    {
                        LoggedIn = "yes";
                        Console.WriteLine("Welcome Admin");
                        while (LoggedIn == "yes")
                        {

                            Console.WriteLine("What do you want to do?");
                            Console.ForegroundColor = showColor;
                            Console.WriteLine("1. Show Users");
                            Console.ResetColor();

                            Console.ForegroundColor = editColor;
                            Console.WriteLine("2. Edit Users");
                            Console.ResetColor();

                            Console.ForegroundColor = deleteColor;
                            Console.WriteLine("3. Delete Users");
                            Console.ResetColor();

                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine("4. LogOut");
                            Console.ResetColor();

                            Console.ForegroundColor = exitColor;
                            Console.WriteLine("5. Exit");
                            Console.ResetColor();
                            Console.Write("Choice: ");
                            AdminInput = Console.ReadLine();
                        
                            if (AdminInput == "1" || AdminInput == "Show Users" || AdminInput == "show Users" || AdminInput == "Show users" || AdminInput == "ShowUsers" || AdminInput == "showusers")
                            { 
                                Console.Clear();
                                ShowUsers();
                            }
                                else if (AdminInput == "2" || AdminInput == "Edit Users" || AdminInput == "edit Users" || AdminInput == "Edit users" || AdminInput == "EditUser" || AdminInput == "edituser")
                                {
                                    EditUser();  
                                }
                                else if (AdminInput == "3" || AdminInput == "Delete Users" || AdminInput == "delete Users" || AdminInput == "Delete users" || AdminInput == "DeleteUsers" || AdminInput == "deleteusers")
                                {
                                    DeleteUser();
                                }
                                else if (AdminInput == "4" || AdminInput == "LogOut" || AdminInput == "logOut" || AdminInput == "Logout" || AdminInput == "Log Out" || AdminInput == "Log out" || AdminInput == "log Out")
                                {
                                    Console.Clear();
                                    LoggedIn = "no";
                                    UserLogin();
                                }
                                else if (AdminInput == "5" || AdminInput == "exit" || AdminInput == "Exit")
                                {
                                    Console.Clear();
                                    Console.WriteLine("See you soon!");                        
                                    break;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Sorry, your Input doesn't make sense :)");
                                }





                            // git sample
                    }
                    }
                    else
                    {
                        Console.Clear();
                        greetings();
                        UserLogin();
                    }
                }
                if (UserInput1 == "" || UserInput1 == " ")
                {
                UserLogin();
                }
                else
                {
                    using (var connection = new SqliteConnection("Data Source=atm.db"))
                    {

                        //Wählt den BankAccount aus der Datenbank der vorher gewählt wurde
                        connection.Open();
                        var selectCommand = connection.CreateCommand();
                        selectCommand.CommandText = "SELECT username, Guthaben, Pin FROM bankaccounts WHERE username LIKE @username";
                        selectCommand.Parameters.AddWithValue($"@username", UserInput1);

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var username = reader.GetString(0);
                                var guthaben = reader.GetDouble(1);
                                var pin = reader.GetString(2);
                                LoggedIn = "yes";
                                Console.Write("Password: ");
                                var userInput2 = Console.ReadLine();        

                                //Gleicht eingegebenen Pin mit der Datenbank ab und reduziert Versuche
                                while (tries > 0)
                                {
                                    if (userInput2 == pin)
                                    {
                                        UserInput1 = null;
                                        Console.Clear();
                                        Console.WriteLine($"Welcome {username}, you currently have: ${guthaben} in your Bank-Account");
                                        UserInput2 = null;
                                        tries = 0;
                                        CurrentUser = username;
                                        connection.Close();
                                        UserActions();
                                        break;
                                    }
                                    else 
                                    {
                                        Console.WriteLine($"Wrong Password, tries left {tries}");
                                        userInput2 = null;
                                        userInput2 = Console.ReadLine();
                                        tries -= 1;
                                    }
                                }

                                if (tries == 0)
                                {
                                    Console.WriteLine("Try again later!");
                                    Environment.Exit(0);
                                }
                            }
                                UserInput1 = null;
                                Console.WriteLine("User not found");
                                UserProfile();     
                        } 
                        connection.Close();
                    }
                }
            }

            public static string LoggedIn = "no";

            //Nutzeroberfläche
            static void UserActions()
            {

                ConsoleColor withdrawColor = ConsoleColor.Yellow;
                ConsoleColor depositColor = ConsoleColor.Green;
                ConsoleColor transferColor = ConsoleColor.Cyan;
                ConsoleColor exitColor = ConsoleColor.Red;

                while (LoggedIn == "yes") 
                { 
                    string choice;
                    Console.WriteLine("Please choose one of the following actions.");
                    Console.ForegroundColor = withdrawColor;
                    Console.WriteLine("1. Withdraw");
                    Console.ResetColor();

                    Console.ForegroundColor = depositColor;
                    Console.WriteLine("2. Deposit");
                    Console.ResetColor();

                    Console.ForegroundColor = transferColor;
                    Console.WriteLine("3. Transfer");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("4. LogOut");
                    Console.ResetColor();

                    Console.ForegroundColor = exitColor;
                    Console.WriteLine("5. Exit");
                    Console.ResetColor();

                //Entscheiden welche Methode ausgeführt wird
                    Console.Write("Choice: ");
                    choice = Console.ReadLine();

                    if (choice == "1" || choice == "withdraw" || choice == "Withdraw")
                    {
                       Withdraw();
                    }
                    else if (choice == "2" || choice == "deposit" || choice == "Deposit")
                    {
                        Deposit();
                    }
                    else if (choice == "3" || choice == "transfer" || choice == "Transfer")
                    {
                        TransferToRealPerson();
                    }
                    else if (choice == "4" || choice == "logout" || choice == "Logout" || choice == "LogOut" || choice == "logOut")
                    {
                        Console.Clear();
                        LoggedIn = "no";
                        UserLogin();
                    }
                    else if (choice == "5" || choice == "exit" || choice == "Exit")
                    {
                        Console.WriteLine("Hope to see you soon!");
                        Environment.Exit(0);
                        break;
                    }
                    else 
                    {
                        Console.Clear();
                        Console.WriteLine("Sorry, your Input doesn't make sense :)");
                    }
                }
            }

            public static double guthaben;
            public static double rGuthaben;
            public static double neueBalance;
            public static double UpdatedBalance;

            //Lässt Nutzer Geld auszahlen
            static void Withdraw()
            {
                double Withdrawal;
                using (var connection = new SqliteConnection("Data source=atm.db"))
                {
                    connection.Open();
                    Console.Clear();
                    Console.WriteLine("How much would you like to withdraw?");
                    Console.Write("Withdrawal: ");
                    Withdrawal = int.Parse(Console.ReadLine());
                    var selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = "SELECT username, Guthaben FROM bankaccounts WHERE username LIKE @username";
                    selectCommand.Parameters.AddWithValue("@username", CurrentUser);
                    selectCommand.ExecuteNonQuery();

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var username = reader.GetString(0);
                            guthaben = reader.GetDouble(1);
                            neueBalance = guthaben - Withdrawal;
                        }
                    }
                    if (guthaben > Withdrawal && Withdrawal > 0)
                    {
                        UpdatedBalance = Convert.ToDouble(neueBalance);
                        connection.Close();
                        BalanceUpdate();
                    }
                    else
                    {
                        Console.WriteLine("Your Balance isn't high enough!");
                        connection.Close();
                        Withdraw();
                    }
                }
                Console.WriteLine("Withdraw Method finished");
            }

            //Aktuallisiert das Guthaben, was in der Withdraw/Deposit Methode verwendet wird.
            static void BalanceUpdate()
            {
                using (var connection = new SqliteConnection("Data source=atm.db"))
                {
                    connection.Open();
                    var selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = "SELECT username, Guthaben FROM bankaccounts WHERE username LIKE @username";
                    selectCommand.Parameters.AddWithValue("@username", CurrentUser);
                    selectCommand.CommandText = "UPDATE bankaccounts SET Guthaben = @guthaben WHERE username LIKE @username";
                    selectCommand.Parameters.AddWithValue("@guthaben", UpdatedBalance);
                    Console.Clear();
                    Console.WriteLine($"Your Balance is now: {UpdatedBalance}");
                    selectCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }

            //Lässt Nutzer Geld einzahlen
            static void Deposit()
            {

                double deposit1;

                using (var connection = new SqliteConnection("Data source=atm.db"))
                {
                    connection.Open();
                    Console.WriteLine("How much would you like to deposit?");
                    Console.Write("Deposit: ");
                    deposit1 = int.Parse(Console.ReadLine());
                    var selectCommand = connection.CreateCommand();
                    if (deposit1 > 0)
                    {
                        selectCommand.CommandText = "SELECT username, Guthaben FROM bankaccounts WHERE username LIKE @username";
                        selectCommand.Parameters.AddWithValue("@username", CurrentUser);
                        selectCommand.ExecuteNonQuery();
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var username = reader.GetString(0);
                                guthaben = reader.GetDouble(1);
                                neueBalance = guthaben + deposit1;
                            }
                        }
                        UpdatedBalance = Convert.ToDouble(neueBalance);
                        connection.Close();
                        BalanceUpdate();
                    }
                    else
                    {
                        Console.WriteLine("You can't go under $0!");
                        Deposit();
                    }
                
                }
            }

            public static double Amount;
            public static double sUserBalance;
            public static double rUserBalance;
            public static string rUserName;
            public static string UserExist = "no";

            static void IsUserIsAlreadyExisting()
            {
                using (var connection = new SqliteConnection("Data source=atm.db"))
                {

                    connection.Open();

                    var selectCommand = connection.CreateCommand();
                
                    selectCommand.CommandText = "SELECT username, Guthaben FROM bankaccounts WHERE username LIKE @username";
                    selectCommand.Parameters.AddWithValue($"@username", newUserName);
                    selectCommand.ExecuteNonQuery();

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var username = reader.GetString(0);
                            rGuthaben = reader.GetDouble(1);
                            connection.Close();
                            UserExist = "yes";
                            break;
                        }    
                    }
                }
            }
            static void UserExist2()
            {
                using (var connection = new SqliteConnection("Data source=atm.db"))
                {

                    connection.Open();

                    var selectCommand = connection.CreateCommand();

                    selectCommand.CommandText = "SELECT username, Guthaben FROM bankaccounts WHERE username LIKE @username";
                    selectCommand.Parameters.AddWithValue($"@username", username);
                    selectCommand.ExecuteNonQuery();

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var username = reader.GetString(0);
                            rGuthaben = reader.GetDouble(1);
                            connection.Close();
                            UserExist = "yes";
                            break;
                        }
                    }
                }
            }


        //Überprüft, ob die Person, an die das Geld gesendet wird existiert.
        static void TransferToRealPerson()
            {

                bool SameAccount = false;
                using (var connection = new SqliteConnection("Data source=atm.db"))
                {

                while (SameAccount == false)
                {
                    connection.Open();

                    var selectCommand = connection.CreateCommand();
                    Console.WriteLine("Please enter the name of the reciever:");
                    Console.Write("Name: ");
                    rUserName = Console.ReadLine();
                    
                    //Überprüft, ob man sich selber Geld überweisen möchte.
                    if (rUserName == CurrentUser)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You can't transfer Money to your own account!");
                        Console.ResetColor();
                        SameAccount = true;
                        break;
                    }
                    else
                    {
                        SameAccount = false;
                    }
                    selectCommand.CommandText = "SELECT username, Guthaben FROM bankaccounts WHERE username LIKE @username";
                    selectCommand.Parameters.AddWithValue($"@username", rUserName);
                    selectCommand.ExecuteNonQuery();



                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var username = reader.GetString(0);
                            rGuthaben = reader.GetDouble(1);
                            connection.Close();
                            Transfer();
                            break;
                        }
                        if (UserExist == "no")
                        {
                            Console.Clear();
                            Console.WriteLine("Couldn't find this User.");
                        }
                    }
                    break;
                }
                }
            }
            //Lässt Nutzer Geld aneinander überweisen
            static void Transfer()
            {
   
                using (var connection = new SqliteConnection("Data source=atm.db"))
                {

                    UserExist = "no";

                    connection.Open();

                    var selectCommand = connection.CreateCommand();

                    getUserBalanceS();
                    connection.Close();

                    while (true)
                    {
                        Console.WriteLine($"How much do you want to send to {rUserName}?");
                        Console.Write("Amount: ");
                        Amount = int.Parse(Console.ReadLine());
                        Convert.ToDouble(Amount);

                        sUserBalance = guthaben - Amount;
                        rUserBalance = rGuthaben + Amount;

                        if (guthaben > Amount && Amount >= 0)
                        {
                            connection.Close();
                            TransferAction();
                            break;
                        }
                        else if (sUserBalance < 0)
                        {
                            Console.WriteLine("Transactions don't work like that!");
                        }
                        {
                            Console.WriteLine("This transaction is to high for your current balance!");
                        }
                    }
                    connection.Close();
                }
            }

            //Aktuallisiert das Guthaben vom Nutzer der empfängt bei einer Transaktion
            static void TransferAction()
            {
                using (var connection = new SqliteConnection("Data source=atm.db"))
                {

                    connection.Open(); 
                    var selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = "SELECT * FROM bankaccounts WHERE username = @username";
                    selectCommand.Parameters.AddWithValue("@username", rUserName);
                    selectCommand.CommandText = "UPDATE bankaccounts SET Guthaben = @guthaben WHERE username = @username";
                    selectCommand.Parameters.AddWithValue("@guthaben", rUserBalance);
                    selectCommand.ExecuteNonQuery();
                    connection.Close();
                    TransferAction2();
                }
            }

            //Aktuallisiert das Guthaben vom Nutzer der sendet bei einer Transaktion
            static void TransferAction2()
            {
                using (var connection = new SqliteConnection("Data source=atm.db"))
                {
                    connection.Open();
                    var selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = "SELECT * FROM bankaccounts WHERE username = @username";
                    selectCommand.Parameters.AddWithValue("@username", CurrentUser);
                    selectCommand.CommandText = "UPDATE bankaccounts SET Guthaben = @guthaben WHERE username = @username";
                    selectCommand.Parameters.AddWithValue("@guthaben", sUserBalance);
                    selectCommand.ExecuteNonQuery();
                    connection.Close();

                    UserExist = "yes";
                    Console.Clear();
                    Console.WriteLine($"You've send ${Amount} to {rUserName}");
                }
            }
        
            //Holt sich den aktuellen Kontostand des Senders.
            static void getUserBalanceS()
            {
                using (var connection = new SqliteConnection("Data source = atm.db"))
                {
                    connection.Open();
                    var selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = "SELECT username, Guthaben FROM bankaccounts WHERE username LIKE @username";
                    selectCommand.Parameters.AddWithValue($"@username", CurrentUser);
                    selectCommand.ExecuteNonQuery();
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var username = reader.GetString(0);
                            guthaben = reader.GetDouble(1);
                        }
                    }
                }
            }

            //Listet alle in der Datenbank eingetragenen BankAccounts auf
            static void ShowUsers()
            {
                using (var connection = new SqliteConnection("Data Source=atm.db"))
                {
                    connection.Open();     
                    var selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = "SELECT username, Guthaben FROM bankaccounts;";           
                    selectCommand.ExecuteNonQuery();
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var username = reader.GetString(0);
                            var guthaben = reader.GetDouble(1);
                            Console.WriteLine($"Username: {username}, Balance: {guthaben}");                     
                        }                                 
                    }    
                }
            }

            //Löscht BankAccount nach Wahl
            static void DeleteUser()
            {
                using (var connection = new SqliteConnection("Data Source=atm.db"))
                {
                while (true)
                {
                    connection.Open();
                    var selectCommand = connection.CreateCommand();
                    Console.WriteLine("Which User do you want to delete?");
                    selectCommand.CommandText = "SELECT username, Guthaben, Pin FROM bankaccounts WHERE username = @username";
                    username = Console.ReadLine();

                    UserExist2();
                    if (UserExist == "no")
                    {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("This User doesn't exist!");
                    Console.ResetColor();
                    UserExist = "no";
                    break;
                    }

                    selectCommand.Parameters.AddWithValue($"@username", username);
                    selectCommand.CommandText = "DELETE FROM  bankaccounts WHERE username = @username";
                    selectCommand.ExecuteNonQuery(); 
                    Console.WriteLine($"{username} was deleted.");
                    break;
                }
            }
            }

            public static string username;

            //Bearbeitung des ausgewählten BankAccounts
            static void EditUser()
            {
                string newPin;   
                double newerBalance;
                double newBalance;
                string newName;
                using (var connection = new SqliteConnection("Data source=atm.db"))
                {
                while (true)
                {

                    
                    connection.Open();
                    var selectCommand = connection.CreateCommand();
                    Console.WriteLine("Which User do you want to edit?");
                    username = Console.ReadLine();
                    UserExist2();
                    if (UserExist == "no")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("This User doesn't exist!");
                        Console.ResetColor();
                        UserExist = "no";
                        break;
                    }
                    selectCommand.CommandText = "SELECT * FROM bankaccounts WHERE username = @username";
                    selectCommand.Parameters.AddWithValue("@username", username);

                    //Aktualisiert den Namen
                    Console.Write("Enter the new Name: ");
                    newName = Console.ReadLine();
                    selectCommand.CommandText = "UPDATE bankaccounts SET username = @newUsername WHERE username = @username";
                    selectCommand.Parameters.AddWithValue("@newUsername", newName);
                    selectCommand.ExecuteNonQuery();

                    //Aktualisiert den PIN
                    Console.Write("Enter the new Passwort: ");
                    newPin = Console.ReadLine();
                    selectCommand.CommandText = "UPDATE bankaccounts SET Pin = @pin WHERE username = @username";
                    selectCommand.Parameters.AddWithValue("@pin", newPin);
                    selectCommand.ExecuteNonQuery();

                    //Aktualisiert das Guthaben
                    Console.Write("Enter the new Balance: ");
                    newBalance = int.Parse(Console.ReadLine());
                    newerBalance = Convert.ToDouble(newBalance);
                    selectCommand.CommandText = "UPDATE bankaccounts SET Guthaben = @guthaben WHERE username = @username";
                    selectCommand.Parameters.AddWithValue("@guthaben", newerBalance);
                    selectCommand.ExecuteNonQuery();

                    //Gibt Meldung aus, wenn erfolgreich ausgeführt
                    Console.WriteLine("User information updated successfully!");
                    break;
                }
            }
            }
   }
}