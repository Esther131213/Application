using Npgsql;
using System;

internal class Program
{
    // Instance state
    public string user;
    public int user_;
    public string username;
    public bool correct_name;
    string choice;

    // Speciality
    string specName;
    string specCost;
    int specId;

    // Patient stuff
    string patName_F;
    string patName_L;
    string adress;
    string gender;
    string password;
    int medicalNumber;
    DateOnly birthday;
    DateOnly date;
    TimeOnly time;
    public int loggedInMedicalNumber = -1;
    public int loggedInDoctorId = -1;

    // Medical_Records
    int recordNumber;
    TimeOnly recordsTime;
    DateOnly recordsDate;
    string diagnosis;
    string description;
    string perscription;

    public static void Main(string[] args)
    {
        var app = new Program();
        app.Login();
        Console.ReadKey();
    }

    public NpgsqlConnection GetUserConnection() // For patients and doctors
    {
        string connString = $"Host=postgres.mau.se;Username=ar7661;Password=mj8nrus2;Database=ar7661;Port=55432";
        return new NpgsqlConnection(connString);
    }

    void Login()
    {
        Console.Clear();

        Console.WriteLine("Answer with all small letters and no spaces.");
        Console.WriteLine("Do you wish to log in as an: Admin, Doctor, Patient?");
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
                if (correct_name)
                {
                    AdminMain();
                }
                break;

            case 2:
                Console.Clear();
                Console.WriteLine("You have chosen to log in as a Doctor. Please Write your ID number.");
                username = Console.ReadLine();
                DoctorCheck();
                if (correct_name)
                {
                    DoctorMain();
                }
                break;

            case 3:
                Console.Clear();
                Console.WriteLine("You have chosen to log in as a Patient. Please Write your Medical Number.");
                username = Console.ReadLine();
                PatientCheck();
                if (correct_name)
                {
                    PatientMain();
                }
                break;
        }
    }

    void AdminCheck()
    {
        if (username == "001_password_")
        {
            Console.WriteLine("You are Logged in!");
            correct_name = true;
        }
        else
        {
            Console.WriteLine("incorrect. Returning to login.");
            Console.ReadKey();
            Login();
        }
    }

    void DoctorCheck()
    {
        // Allow "doctorId_password" or prompt for password if only id provided.
        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Enter doctor id (or doctorId_password):");
            username = Console.ReadLine();
        }

        int docId;
        string pwd;

        if (username.Contains('_'))
        {
            var parts = username.Split(new[] { '_' }, 2);
            if (!int.TryParse(parts[0], out docId))
            {
                Console.WriteLine("Invalid doctor id format. Returning to login.");
                Console.ReadKey();
                Login();
                return;
            }
            pwd = parts[1];
        }
        else
        {
            if (!int.TryParse(username, out docId))
            {
                Console.WriteLine("Invalid doctor id format. Returning to login.");
                Console.ReadKey();
                Login();
                return;
            }
            Console.Write("Password: ");
            pwd = Console.ReadLine();
        }

        try
        {
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = "SELECT Password_ FROM Doctor WHERE Doctor_Id = @Doctor_Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Doctor_Id", docId);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result.ToString() == pwd)
                    {
                        Console.WriteLine("You are Logged in!");
                        loggedInDoctorId = docId;   // store the logged-in doctor's numeric id
                        correct_name = true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect doctor id or password. Returning to login.");
                        Console.ReadKey();
                        Login();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            Console.ReadKey();
            Login();
        }
    }

    void PatientCheck()
    {
        // Allow "medicalNumber_password" or prompt for password if only number provided.
        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Enter medical number (or medicalNumber_password):");
            username = Console.ReadLine();
        }

        int medNum;
        string pwd;

        if (username.Contains('_'))
        {
            var parts = username.Split(new[] { '_' }, 2);
            if (!int.TryParse(parts[0], out medNum))
            {
                Console.WriteLine("Invalid medical number format. Returning to login.");
                Console.ReadKey();
                Login();
                return;
            }
            pwd = parts[1];
        }
        else
        {
            if (!int.TryParse(username, out medNum))
            {
                Console.WriteLine("Invalid medical number format. Returning to login.");
                Console.ReadKey();
                Login();
                return;
            }
            Console.Write("Password: ");
            pwd = Console.ReadLine();
        }

        try
        {
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = "SELECT Password_ FROM Patient WHERE Medical_Number = @Medical_Number";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Medical_Number", medNum);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result.ToString() == pwd)
                    {
                        Console.WriteLine("You are Logged in!");
                        loggedInMedicalNumber = medNum;   // store the logged-in patient's numeric id
                        correct_name = true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect medical number or password. Returning to login.");
                        Console.ReadKey();
                        Login();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            Console.ReadKey();
            Login();
        }
    }

    // ADMIN STUFF
    public void AdminMain()
    {
        Console.Clear();
        Console.WriteLine("What do you want to do? Write the number of your option.");
        Console.WriteLine(" ");
        Console.WriteLine("1. Add a specialization."); //Done
        Console.WriteLine("2. Add a Doctor.");//Done
        Console.WriteLine("3. Delete a Doctor.");//Done
        Console.WriteLine("4. Patient Information. (inc. upcoming appointments)");//Done mostly. Shows patient info and medical records, but not appointments.
        Console.WriteLine("5. View existing Doctors (+ login info).");//Done
        Console.WriteLine("6. Back to Login."); //Done

        choice = Console.ReadLine();
        if (choice == "6")
        {
            Login();
        }
        else if (choice == "1")
        {
            Console.Clear();
            Console.WriteLine("Add a specialization!");

            Console.WriteLine("List of all existing Specializations:");
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Specialization ORDER BY Spec_Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"Spec_Id: {reader["Spec_Id"]}, Cost: {reader["Cost_"]}, Spec_Name: {reader["Spec_Name"]}");
                    }
                }
            }
            Console.WriteLine("________________________________________________");
            Console.WriteLine(" ");
            Console.WriteLine("Please state the name of the new specialization you wish to add.");
            string specName = Console.ReadLine();
            Console.WriteLine(" ");
            Console.WriteLine("Please state the visit cost.");
            int Cost = ReadInt("Visit cost: ");
            Console.WriteLine(" ");
            Console.WriteLine("Please state the specialization ID.");
            int spec_Id = ReadInt("Specialization ID: ");
            Console.WriteLine(" ");
            Console.WriteLine("You have added specialization: " + specName + ", with a visit cost of: " + Cost + ".");

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Specialization (Spec_Id, Cost_, Spec_Name)
                                 VALUES (@Spec_Id, @Cost_, @Spec_Name)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Spec_Id", spec_Id);
                    cmd.Parameters.AddWithValue("Cost_", Cost);
                    cmd.Parameters.AddWithValue("Spec_Name", specName);
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
            Console.WriteLine(" ");
            Console.WriteLine("All doctors already in system:");
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Doctor ORDER BY Doctor_Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"Doctor Id: {reader["Doctor_Id"]}, Doctor Name: {reader["Name_"]}, Specialization: {reader["Spec_Id"]}");
                    }
                }
            }
            Console.WriteLine("________________________________________________");
            Console.WriteLine(" ");
            int empNum = ReadInt("Employee number of the new doctor: ");
            Console.WriteLine(" ");
            Console.WriteLine("Please state the doctors full name.");
            string doc_FullName = Console.ReadLine();
            Console.WriteLine(" ");

            Console.WriteLine("Please state the doctors specialization ID. List of all specializations here:");
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Specialization ORDER BY Spec_Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"Spec_Id: {reader["Spec_Id"]}, Cost: {reader["Cost_"]}, Spec_Name: {reader["Spec_Name"]}");
                    }
                }
            }
            Console.WriteLine(" ");
            int spec_Id = ReadInt("Specialization ID: ");
            Console.WriteLine(" ");
            Console.WriteLine("Please state the password for the doctor.");
            string password = Console.ReadLine();
            Console.WriteLine("You have added Doctor " + doc_FullName + ", Employee number: " + empNum + ", Specialization: " + spec_Id + ".");
            Console.WriteLine(" ");

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Doctor (Password_, Doctor_Id, Name_, Spec_Id)
                                 VALUES (@Password_, @Doctor_Id, @Name_, @Spec_Id)";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Spec_Id", spec_Id);
                    cmd.Parameters.AddWithValue("Password_", password);
                    cmd.Parameters.AddWithValue("Name_", doc_FullName);
                    cmd.Parameters.AddWithValue("Doctor_Id", empNum);
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
            Console.WriteLine("List of all existing Doctors:");
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Doctor ORDER BY Doctor_Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"Doctor Id: {reader["Doctor_Id"]}, Doctor Name: {reader["Name_"]}, Specialization: {reader["Spec_Id"]}");
                    }
                }
            }
            Console.WriteLine(" ");
            int empNumber = ReadInt("Employee number of the doctor to delete: ");
            Console.WriteLine(" ");
            Console.WriteLine("Deleting Doctor " + empNumber + ".");
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"DELETE FROM Doctor WHERE Doctor_Id = @Doctor_Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Doctor_Id", empNumber);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Doctor " + empNumber + " has been deleted.");
            Console.WriteLine(" ");
            Console.WriteLine("All doctors currently in system:");
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Doctor ORDER BY Doctor_Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"Doctor Id: {reader["Doctor_Id"]}, Doctor Name: {reader["Name_"]}, Specialization: {reader["Spec_Id"]}");
                    }
                }
            }
            Console.ReadKey();
            AdminMain();
        }
        else if (choice == "4")
        {
            Console.Clear();
            Console.WriteLine("Here are all the existing patients.");

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"SELECT * FROM Patient";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"MedicalNumber: {reader["Medical_Number"]}, Name: {reader["F_Name"]} {reader["L_Name"]}");
                    }
                }
            }

            Console.WriteLine("Which patient would you like to look closer at? (ID)");

            int selectedNumber = ReadInt("Patient ID: ");

            // Show basic patient info
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Patient WHERE Medical_Number = @Medical_Number";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Medical_Number", selectedNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine($"MedicalNumber: {reader["Medical_Number"]}, Name: {reader["F_Name"]} {reader["L_Name"]}, Phone Number: {reader["Phone_Number"]}");
                            Console.WriteLine($"Gender: {reader["Gender"]}, Address: {reader["Adress"]}");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Patient not found.");
                            Console.ReadKey();
                            AdminMain();
                            return;
                        }
                    }
                }
            }

            // Show medical records (diagnosis, description, perscription) for the selected patient
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string recQuery = @"
                SELECT Booking_Date, Booking_Time, Diagnosis, Description, Perscription
                FROM Medical_Records
                WHERE Record_Number = @Record_Number
                ORDER BY Booking_Date DESC, Booking_Time DESC";
                using (var cmd = new NpgsqlCommand(recQuery, conn))
                {
                    cmd.Parameters.AddWithValue("Record_Number", selectedNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No medical records found for this patient.");
                        }
                        else
                        {
                            Console.WriteLine("Medical records:");
                            while (reader.Read())
                            {
                                var bookingDate = reader["Booking_Date"];
                                var bookingTime = reader["Booking_Time"];
                                Console.WriteLine($"Date: {bookingDate}, Time: {bookingTime}");
                                Console.WriteLine($"  Diagnosis   : {reader["Diagnosis"]}");
                                Console.WriteLine($"  Description : {reader["Description"]}");
                                Console.WriteLine($"  Prescription: {reader["Perscription"]}");
                                Console.WriteLine("--------------------------------------------------");
                            }
                        }
                    }
                }
            }
            Console.WriteLine(" ");
            Console.WriteLine("Press any button to return to Admin menu.");
            Console.ReadKey();
            AdminMain();

            Console.ReadKey();
            AdminMain();
        }
        else if (choice == "5")
        {
            Console.Clear();
            Console.WriteLine("All existing Doctors in the system:");
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Doctor ORDER BY Doctor_Id";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"Doctor Id: {reader["Doctor_Id"]}, Doctor Name: {reader["Name_"]}, Specialization: {reader["Spec_Id"]}, Password: {reader["Password_"]}");
                    }
                }
            }
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

    // DOCTOR STUFF
    public void DoctorMain()
    {
        Console.Clear();
        Console.WriteLine("What do you want to do? Write the number of your option.");
        Console.WriteLine(" ");
        Console.WriteLine("1. Availability.");
        Console.WriteLine("2. Appointments.");
        Console.WriteLine("3. Patient information.");//Done
        Console.WriteLine("4. Add Patient.");//Done
        Console.WriteLine("5. Add Medical record.");//Done
        Console.WriteLine("6. Back to Login."); //Done

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
            Console.WriteLine("Here are all the existing patients.");

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"SELECT * FROM Patient";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"MedicalNumber: {reader["Medical_Number"]}, Name: {reader["F_Name"]} {reader["L_Name"]}");
                    }
                }
            }

            Console.WriteLine("Which patient would you like to look closer at? (ID)");

            int selectedNumber = ReadInt("Patient ID: ");

            // Show basic patient info
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Patient WHERE Medical_Number = @Medical_Number";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Medical_Number", selectedNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine($"MedicalNumber: {reader["Medical_Number"]}, Name: {reader["F_Name"]} {reader["L_Name"]}, Phone Number: {reader["Phone_Number"]}");
                            Console.WriteLine($"Gender: {reader["Gender"]}, Address: {reader["Adress"]}");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Patient not found.");
                            Console.ReadKey();
                            DoctorMain();
                            return;
                        }
                    }
                }
            }

            // Show medical records (diagnosis, description, perscription) for the selected patient
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string recQuery = @"
                SELECT Booking_Date, Booking_Time, Diagnosis, Description, Perscription
                FROM Medical_Records
                WHERE Record_Number = @Record_Number
                ORDER BY Booking_Date DESC, Booking_Time DESC";
                using (var cmd = new NpgsqlCommand(recQuery, conn))
                {
                    cmd.Parameters.AddWithValue("Record_Number", selectedNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No medical records found for this patient.");
                        }
                        else
                        {
                            Console.WriteLine("Medical records:");
                            while (reader.Read())
                            {
                                var bookingDate = reader["Booking_Date"];
                                var bookingTime = reader["Booking_Time"];
                                Console.WriteLine($"Date: {bookingDate}, Time: {bookingTime}");
                                Console.WriteLine($"  Diagnosis   : {reader["Diagnosis"]}");
                                Console.WriteLine($"  Description : {reader["Description"]}");
                                Console.WriteLine($"  Prescription: {reader["Perscription"]}");
                                Console.WriteLine("--------------------------------------------------");
                            }
                        }
                    }
                }
            }
            Console.WriteLine(" ");
            Console.WriteLine("Press any button to return to Doctor menu.");
            Console.ReadKey();
            DoctorMain();
        }
        else if (choice == "4")
        {
            Console.Clear();
            Console.WriteLine("All registered patients: ");
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Patient ORDER BY Medical_Number";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"MedicalNumber: {reader["Medical_Number"]}, Name: {reader["F_Name"]} {reader["L_Name"]}");
                    }
                }
            }
            Console.WriteLine("________________________________________________");
            Console.WriteLine(" ");
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
            medicalNumber = ReadInt("Write the Medical Number for the patient: ");
            Console.WriteLine(" ");
            birthday = ReadDate("Write the birthday for the patient (YYYY-MM-DD): ");
            Console.WriteLine(" ");
            Console.WriteLine("Write the password for the patient.");
            password = Console.ReadLine();
            Console.WriteLine(" ");
            int phoneNum = ReadInt("Write the phonenumber for the patient: ");
            Console.WriteLine(" ");
            date = DateOnly.FromDateTime(DateTime.Now);
            time = TimeOnly.FromDateTime(DateTime.Now);
            Console.WriteLine("Patient has been created.");
            Console.WriteLine("New patient: " + patName_F + " " + patName_L + ", Gender: " + gender + ", Adress: " + adress + ", Medical Number: " + medicalNumber + ", Birthday: " + birthday + ", Phone number: " + phoneNum + ", Added on date: " + date + ", At time: " + time);
            Console.WriteLine();

            using (var conn = GetUserConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Patient (F_Name, L_Name, Gender, Adress, Password_, Registration_Date, Medical_Number, Birth_Date, Phone_Number)
                                 VALUES (@F_Name, @L_Name, @Gender, @Adress, @Password_, @Registration_Date, @Medical_Number, @Birth_Date, @Phone_Number)";

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
            Console.WriteLine("All registered patients: ");
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Patient ORDER BY Medical_Number";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // print a simple row summary (adjust column names as needed)
                        Console.WriteLine($"MedicalNumber: {reader["Medical_Number"]}, Name: {reader["F_Name"]} {reader["L_Name"]}");
                    }
                }
            }
            Console.WriteLine("Press any button to continue.");
            Console.ReadLine();
            DoctorMain();
        }
        else if (choice == "5")
        {
            Console.Clear();
            Console.WriteLine("Add a medical record for your patient.");
            recordNumber = ReadInt("Write the patients medical number: ");
            recordsDate = ReadDate("Write the patients booking date (YYYY-MM-DD): ");
            recordsTime = ReadTime("Write the patients booking time (HH:mm[:ss]): ");
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

                string query = @"INSERT INTO Medical_Records (Record_Number, Booking_Time, Booking_Date, Diagnosis, Description, Perscription)
                                 VALUES (@Record_Number, @Booking_Time, @Booking_Date, @Diagnosis, @Description, @Perscription)";

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

    // PATIENT STUFF
    public void PatientMain()
    {
        Console.Clear();
        Console.WriteLine("What do you want to do? Write the number of your option.");
        Console.WriteLine(" ");
        Console.WriteLine("1. User Information.");//Done
        Console.WriteLine("2. Book an appointment/show appointments.");
        Console.WriteLine("3. Diagnosis and descriptions");//Done
        Console.WriteLine("4. Back to Login.");//Done

        choice = Console.ReadLine();
        if (choice == "4")
        {
            Login();
        }
        else if (choice == "1")
        {
            Console.Clear();
            Console.WriteLine("View your information.");
            Console.WriteLine(" ");
            if (loggedInMedicalNumber == -1)
            {
                Console.WriteLine("No patient logged in. Please log in first.");
                Console.ReadKey();
                Login();
                return;
            }

            using (var conn = GetUserConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Patient WHERE Medical_Number = @Medical_Number";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Medical_Number", loggedInMedicalNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine($"MedicalNumber : {reader["Medical_Number"]}");
                            Console.WriteLine($"Name          : {reader["F_Name"]} {reader["L_Name"]}");
                            Console.WriteLine($"Gender        : {reader["Gender"]}");
                            Console.WriteLine($"Address       : {reader["Adress"]}");
                            Console.WriteLine($"Phone Number  : {reader["Phone_Number"]}");
                            Console.WriteLine($"Birth Date    : {reader["Birth_Date"]}");
                            Console.WriteLine($"Registered on : {reader["Registration_Date"]}");
                            Console.WriteLine($"Password      : {reader["Password_"]}");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Patient not found.");
                        }
                    }
                }
            }
            Console.WriteLine(" ");
            Console.WriteLine("Press any button to return to Patient menu.");
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
            Console.WriteLine("View your diagnosis and descriptions from your medical records.");
            if (loggedInMedicalNumber == -1)
            {
                Console.WriteLine("No patient logged in. Please log in first.");
                Console.ReadKey();
                Login();
                return;
            }
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string recQuery = @"
                SELECT Booking_Date, Booking_Time, Diagnosis, Description, Perscription
                FROM Medical_Records
                WHERE Record_Number = @Record_Number
                ORDER BY Booking_Date DESC, Booking_Time DESC";
                using (var cmd = new NpgsqlCommand(recQuery, conn))
                {
                    cmd.Parameters.AddWithValue("Record_Number", loggedInMedicalNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No medical records found for this patient.");
                        }
                        else
                        {
                            Console.WriteLine("Your medical records:");
                            while (reader.Read())
                            {
                                var bookingDate = reader["Booking_Date"];
                                var bookingTime = reader["Booking_Time"];
                                Console.WriteLine($"Date: {bookingDate}, Time: {bookingTime}");
                                Console.WriteLine($"  Diagnosis   : {reader["Diagnosis"]}");
                                Console.WriteLine($"  Description : {reader["Description"]}");
                                Console.WriteLine($"  Prescription: {reader["Perscription"]}");
                                Console.WriteLine("--------------------------------------------------");
                            }
                        }
                    }
                }
            }
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

    // --- Safe input helpers ---
    private int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (int.TryParse(s, out var v))
                return v;
            Console.WriteLine("Invalid number. Please enter digits only.");
        }
    }

    private DateOnly ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (DateOnly.TryParse(s, out var d))
                return d;
            Console.WriteLine("Invalid date. Use YYYY-MM-DD or a valid date format.");
        }
    }

    private TimeOnly ReadTime(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (TimeOnly.TryParse(s, out var t))
                return t;
            Console.WriteLine("Invalid time. Use HH:mm or HH:mm:ss.");
        }
    }
}   