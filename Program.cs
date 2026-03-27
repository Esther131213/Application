using Npgsql;

internal class Program
{
    public string user;
    public int user_;
    public string username;

    public bool correct_name;

    static string choice;
    //Speciality
    string specName;
    string specCost;
    int specId;

    //Admin stuff

    /*
    //Doctor stuff
    static int empNumber;
    static string doc_FullName;
    static string specialization_doc;
    static int phoneNum;
    */
    //Patient stuff
    string patName_F;
    string patName_L;
    string adress;
    string gender;
    string password;
    int medicalNumber;
    DateOnly birthday;
    DateOnly date;
    TimeOnly time;

    //Medical_Records
    int recordNumber;
    TimeOnly recordsTime;
    DateOnly recordsDate;
    string diagnosis;
    string description;
    string perscription;


    public static void Main(string[] args)
    {
        Login();
        Console.ReadKey();
    }
    public static NpgsqlConnection GetUserConnection() // For patients and doctors
    {
        string connString = $"Host=postgres.mau.se;Username=ar7661;Password=mj8nrus2;Database=ar7661;Port=55432";

        return new NpgsqlConnection(connString);
    }

    static void Login()
    {
        Console.Clear();

        Console.WriteLine("Answer with all small letters and no spaces.");
        Console.WriteLine("Do you wish to log in as an: Admin, Doctor, Patient ?");
        user = Console.ReadLine();

        if (user == "admin")
        {
            user_ = 1;
        }
        else if (user == "doctor")
        {
            user_ = 2;
        }
        else
        {
            user_ = 3;
        }

        switch (user_)
        {
            case 1:
                Console.Clear();
                Console.WriteLine("You have chosen to log in as an Admin. Please Write your username and password like this; username_password.");
                username = Console.ReadLine();
                AdminCheck();
                if (correct_name = true)
                {
                    AdminMain();
                }
                break;

            case 2:
                Console.Clear();
                Console.WriteLine("You have chosen to log in as a Doctor. Please Write your username and password like this; username_password.");
                username = Console.ReadLine();
                DoctorCheck();
                if (correct_name = true)
                {
                    DoctorMain();
                }
                break;

            case 3:
                Console.Clear();
                Console.WriteLine("You have chosen to log in as a Patient. Please Write your username and password like this; username_password.");
                username = Console.ReadLine();
                PatientCheck();
                if (correct_name = true)
                {
                    PatientMain();
                }
                break;
        }
    }

    static void AdminCheck()
    {
        if (username == "001_password_")
        {
            Console.WriteLine("You are Logged in!");
            correct_name = true;
        }
        else
        {
            Console.WriteLine("incorrect username. Returning to login.");
            Console.ReadKey();
            Login();
        }
    }

    static void DoctorCheck()
    {
        if (username == "002_password_")
        {
            Console.WriteLine("You are Logged in!");
            correct_name = true;
        }
        else
        {
            Console.WriteLine("incorrect username. Returning to login.");
            Console.ReadKey();
            Login();
        }
    }

    static void PatientCheck()
    {
        if (username == "003_password_")
        {
            Console.WriteLine("You are Logged in!");
            correct_name = true;
        }
        else
        {
            Console.WriteLine("incorrect username. Returning to login.");
            Console.ReadKey();
            Login();
        }
    }

    //SQL STUFF______________________________________________________________________________________________________________SQL STUFF________


    //ADMIN STUFF____________________________________________________________________________________________________________ADMIN STUFF______
    public static void AdminMain()
    {
        Console.Clear();
        Console.WriteLine("What do you want to do? Write the number of your option.");
        Console.WriteLine(" ");
        Console.WriteLine("1. Add a specialization."); //Done
        Console.WriteLine("2. Add a Doctor."); //Done
        Console.WriteLine("3. Delete a Doctor.");
        Console.WriteLine("4. Patient Information. (inc. upcoming appointments)");
        Console.WriteLine("5. Back to Login.");

        choice = Console.ReadLine();
        if (choice == "5")
        {
            Login();
        }
        else if (choice == "1")
        {
            Console.Clear();
            Console.WriteLine("Add a specialization!");
            Console.WriteLine("Please state the name of the new specialization you wish to add.");
            string specName = Console.ReadLine();
            Console.WriteLine(" ");
            Console.WriteLine("Please state the visit cost.");
            string specCost = Console.ReadLine();
            if (!int.TryParse(specCost, out int Cost))
            {
                Console.WriteLine("Specialization cost must be a number. Try again.");
                return;
            }
            Console.WriteLine(" ");
            Console.WriteLine("Please state the specialization ID.");
            string specId = Console.ReadLine();
            if (!int.TryParse(specId, out int spec_Id))
            {
                Console.WriteLine("Specialization ID must be a number. Try again.");
                return;
            }
            Console.WriteLine(" ");
            Console.WriteLine("You have added specialization: " + specName + ", with a visit cost of: " + specCost + ".");

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Specialization
                Spec_Id, Cost_, Spec_Name)
                VALUES (@Spec_Id, @Cost_, @Spec_Name)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Spec_Id", spec_Id);
                    cmd.Parameters.AddWithValue("Spec_Name", specName);
                    cmd.Parameters.AddWithValue("Cost_", Cost);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine("Press any key to return to Admin menu.");
            Console.ReadKey();
            AdminMain();
        }
        else if (choice == "2")
        {
            Console.Clear();
            Console.WriteLine("Add a Doctor");
            Console.WriteLine("Please state the employee number of the new doctor.");
            string empNumber = Console.ReadLine();
            if (!int.TryParse(empNumber, out int empNum))
            {
                Console.WriteLine("Employee number must be a number. Try again.");
                return;
            }
            Console.WriteLine(" ");
            Console.WriteLine("Please state the doctors full name.");
            string doc_FullName = Console.ReadLine();
            Console.WriteLine(" ");
            Console.WriteLine("Please state the doctors specialization.");
            string specialization_doc = Console.ReadLine();
            if (!int.TryParse(specialization_doc, out int spec_Id))
            {
                Console.WriteLine("Specialization ID must be a number. Try again.");
                return;
            }
            Console.WriteLine(" ");
            Console.WriteLine("Please state the password for the doctor.");
            string password = Console.ReadLine();
            Console.WriteLine("You have added Doctor " + doc_FullName + ", Employee number: " + empNumber + ", Specialization: " + specialization_doc + ".");
            Console.WriteLine(" ");

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Doctor
                (Password_, Doctor_Id, Name_, Spec_Id)
                VALUES (@Password_, @Doctor_Id, @Name_, @Spec_Id)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Spec_Id", spec_Id);
                    cmd.Parameters.AddWithValue("Password_", password);
                    cmd.Parameters.AddWithValue("Name_", doc_FullName);
                    cmd.Parameters.AddWithValue("Doctor_Id",empNum);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Press any key to return to Admin menu.");
            Console.ReadKey();
            AdminMain();
        }
        else if (choice == "3")
        {
            Console.Clear();
            Console.WriteLine("Delete a Doctor");
            Console.WriteLine("Please state the employee number of the doctor you wish to delete.");
            empNumber = int.Parse(Console.ReadLine());
            Console.WriteLine(" ");
            Console.WriteLine("Deleted Doctor " + empNumber + ".");
            Console.ReadKey();
            AdminMain();
        }
        else if (choice == "4")
        {
            Console.Clear();

            Console.ReadKey();
            AdminMain();
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Unvalid option. Press any button to try again.");
            Console.ReadKey();
            AdminMain();
        }
    }
    //DOCTOR STUFF__________________________________________________________________________________________________________DOCTOR STUFF______
    public static void DoctorMain()
    {
        Console.Clear();
        Console.WriteLine("What do you want to do? Write the number of your option.");
        Console.WriteLine(" ");
        Console.WriteLine("1. Availability.");
        Console.WriteLine("2. Appointments.");
        Console.WriteLine("3. Patient information.");
        Console.WriteLine("4. Add Patient."); //Done
        Console.WriteLine("5. Add Medical record."); //Done
        Console.WriteLine("6. Back to Login.");

        choice = Console.ReadLine();
        if (choice == "6")
        {
            Login();
        }
        else if (choice == "1")
        {
            Console.Clear();

            Console.ReadKey();
            DoctorMain();
        }
        else if (choice == "2")
        {
            Console.Clear();

            Console.ReadKey();
            DoctorMain();
        }
        else if (choice == "3")
        {
            Console.Clear();
            Console.WriteLine("Here are the patients.");;

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"SELECT * Patient";

                using (var cmd = new NpgsqlCommand(query, conn))
                {

                } 
            }

            Console.ReadKey();
            DoctorMain();
        }
        else if (choice == "4")
        {
            Console.Clear();
            Console.WriteLine("Write the first name of the patient you wish to add.");
            patName_F = Console.ReadLine();
            Console.WriteLine(" ");
            Console.WriteLine("Write the last name of the patient you wish to add.");
            patName_L = Console.ReadLine();
            Console.WriteLine(" ");
            Console.WriteLine("Write the Gender of the patient.");
            gender = Console.ReadLine();
            Console.WriteLine(" ");
            Console.WriteLine("Write the Adress of the patient.");
            adress = Console.ReadLine();
            Console.WriteLine(" ");
            Console.WriteLine("Write the Medical Number for the patient.");
            medicalNumber = int.Parse(Console.ReadLine());
            Console.WriteLine(" ");
            Console.WriteLine("Write the birthday for the patient.");
            birthday = DateOnly.Parse(Console.ReadLine());
            Console.WriteLine(" ");
            Console.WriteLine("Write the password for the patient.");
            password = Console.ReadLine();
            Console.WriteLine(" ");
            Console.WriteLine("Write the phonenumber for the patient.");
            phoneNum = int.Parse(Console.ReadLine());
            Console.WriteLine(" ");
            date = DateOnly.FromDateTime(DateTime.Now);
            time = TimeOnly.FromDateTime(DateTime.Now);
            Console.WriteLine("Patient has been created.");
            Console.WriteLine("New patient: " + patName_F + " " + patName_L + ", Gender: " + gender + ", Adress: " + adress + ", Medical Number: " + medicalNumber + ", Birthday: " + birthday + ", Phone number: " + phoneNum + ", Added on date: " + date + ", At time: " + time);
            Console.WriteLine();

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Patient
                (F_Name, L_Name, Gender, Adress, Password_, Registration_Date, Medical_Number, Birth_Date, Phone_Number)
                VALUES(F_Name, L_Name, Gender, Adress, Password_, Registration_Date, Medical_Number, Birth_Date, Phone_Number)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("F_Name", patName_F);
                    cmd.Parameters.AddWithValue("L_Name", patName_L);
                    cmd.Parameters.AddWithValue("Gender", gender);
                    cmd.Parameters.AddWithValue("Adress", adress);
                    cmd.Parameters.AddWithValue("Password_", password);
                    cmd.Parameters.AddWithValue("Registration_Date", date);
                    cmd.Parameters.AddWithValue("Medical_Number", medicalNumber);
                    cmd.Parameters.AddWithValue("Birth_Date", birthday);
                    cmd.Parameters.AddWithValue("Phone_Number", phoneNum);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine("Press any button to continue.");
            Console.ReadLine();
            DoctorMain();
        }
        else if (choice == "5") 
        {
            Console.Clear();
            Console.WriteLine("Add a medical record for your patient.");
            Console.WriteLine("Write the patients medical number.");
            recordNumber = int.Parse(Console.ReadLine());
            Console.WriteLine("Write the patients booking date.");
            recordsDate = DateOnly.Parse(Console.ReadLine());
            Console.WriteLine("Write the patients booking time.");
            recordsTime = TimeOnly.Parse(Console.ReadLine());
            Console.WriteLine("Write the patients Diagnosis.");
            diagnosis = Console.ReadLine();
            Console.WriteLine("Write the diagnosis description.");
            description = Console.ReadLine();
            Console.WriteLine("Write the administered perscription.");
            perscription = Console.ReadLine();
            Console.WriteLine("Medical Record Created!");

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Medical_Records
                (Record_Number, Booking_Time, Booking_Date, Diagnosis, Description, Perscription)
                VALUES(Record_Number, Booking_Time, Booking_Date, Diagnosis, Description, Perscription)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Record_Number", recordNumber);
                    cmd.Parameters.AddWithValue("Booking_Time", recordsTime);
                    cmd.Parameters.AddWithValue("Booking_Date", recordsDate);
                    cmd.Parameters.AddWithValue("Diagnosis", diagnosis);
                    cmd.Parameters.AddWithValue("Description", description);
                    cmd.Parameters.AddWithValue("Perscription", perscription);

                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine(" ");
            Console.WriteLine("Press any button to continue.");
            Console.ReadKey();
            DoctorMain();

        }
        else
        {
            Console.Clear();
            Console.WriteLine("Unvalid option. Press any button to try again.");
            Console.ReadKey();
            DoctorMain();
        }
    }

    //PATIENT STUFF__________________________________________________________________________________________________________PATIENT STUFF______
    public static void PatientMain()
    {
        Console.Clear();
        Console.WriteLine("What do you want to do? Write the number of your option.");
        Console.WriteLine(" ");
        Console.WriteLine("1. Register.");
        Console.WriteLine("2. User Information.");
        Console.WriteLine("3. Book an appointment/show appointments.");
        Console.WriteLine("4. Diagnosis and descriptions");
        Console.WriteLine("5. Back to Login.");

        choice = Console.ReadLine();
        if (choice == "5")
        {
            Login();
        }
        else if (choice == "1")
        {
            Console.Clear();
            Console.WriteLine("Register.");

            Console.ReadKey();
            PatientMain();
        }
        else if (choice == "2")
        {
            Console.Clear();

            Console.ReadKey();
            PatientMain();

        }
        else if (choice == "3")
        {
            Console.Clear();

            Console.ReadKey();
            PatientMain();

        }
        else if (choice == "4")
        {
            Console.Clear();

            Console.ReadKey();
            PatientMain();

        }
        else
        {
            Console.Clear();
            Console.WriteLine("Unvalid option. Press any button to try again.");
            Console.ReadKey();
            PatientMain();
        }
    }
}