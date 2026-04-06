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
                if (correct_name) AdminMain();
                break;

            case 2:
                Console.Clear();
                Console.WriteLine("You have chosen to log in as a Doctor. Please Write your ID number (or ID_password).");
                username = Console.ReadLine();
                DoctorCheck();
                if (correct_name) DoctorMain();
                break;

            case 3:
                Console.Clear();
                Console.WriteLine("You have chosen to log in as a Patient. Please Write your Medical Number (or medicalNumber_password).");
                username = Console.ReadLine();
                PatientCheck();
                if (correct_name) PatientMain();
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
                        loggedInDoctorId = docId;
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
                        loggedInMedicalNumber = medNum;
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
        Console.WriteLine("1. Add a specialization.");
        Console.WriteLine("2. Add a Doctor.");
        Console.WriteLine("3. Delete a Doctor.");
        Console.WriteLine("4. Patient Information. (inc. upcoming appointments)");
        Console.WriteLine("5. View existing Doctors (+ login info).");
        Console.WriteLine("6. Back to Login.");

        choice = Console.ReadLine();
        if (choice == "6") { Login(); return; }

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
            using (var conn = GetUserConnection())
            {
                conn.Open();
                string appQuery = @"
            SELECT a.Date_, a.Time_, a.Doctor_Id, d.Name_ AS DoctorName, s.spec_name
            FROM Appointment a
            LEFT JOIN Doctor d ON a.Doctor_Id = d.Doctor_Id
            LEFT JOIN Specialization s ON d.Spec_Id = s.Spec_Id
            WHERE a.Patient_Medical_Number = @Medical_Number
              AND a.Date_ >= @Today
            ORDER BY a.Date_, a.Time_";

                using (var cmd = new NpgsqlCommand(appQuery, conn))
                {
                    cmd.Parameters.AddWithValue("Medical_Number", selectedNumber);
                    cmd.Parameters.AddWithValue("Today", DateOnly.FromDateTime(DateTime.Today));
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No upcoming appointments for this patient.");
                        }
                        else
                        {
                            Console.WriteLine("Upcoming appointments:");
                            DateOnly? currentDate = null;
                            while (reader.Read())
                            {
                                var date = (DateOnly)reader["Date_"];
                                var time = reader["Time_"];
                                var docId = reader["Doctor_Id"];
                                var docName = reader["DoctorName"] is DBNull ? "Unknown" : reader["DoctorName"].ToString();
                                var specName = reader["spec_name"] is DBNull ? "—" : reader["spec_name"].ToString();

                                if (currentDate == null || currentDate != date)
                                {
                                    Console.WriteLine();
                                    Console.WriteLine($"{date:yyyy-MM-dd} ({date.DayOfWeek}):");
                                    currentDate = date;
                                }

                                Console.WriteLine($"  {time}  — Doctor: {docName} (ID {docId}), Specialization: {specName}");
                            }
                            Console.WriteLine();
                        }
                    }
                }
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
        Console.WriteLine("1. Block a weekday / Create an unavailable slot (doctor-only).");
        Console.WriteLine("2. View and manage Appointments.");
        Console.WriteLine("3. Patient information.");
        Console.WriteLine("4. Add Patient.");
        Console.WriteLine("5. Add Medical record. Add after patients booked time.");
        Console.WriteLine("6. Back to Login.");

        choice = Console.ReadLine();
        if (choice == "6")
        {
            Login();
            return;
        }

        if (choice == "1")
        {
            if (loggedInDoctorId == -1)
            {
                Console.WriteLine("No doctor logged in. Please log in first.");
                Console.ReadKey();
                Login();
                return;
            }

            Console.WriteLine("1. Block a weekday for this doctor (mark unavailable).");
            Console.WriteLine("2. Create a doctor-only appointment (blocks specific date/time).");
            var sub = Console.ReadLine();
            if (sub == "1")
            {
                var blockDate = ReadDateInUpcomingWeek("Date to block (YYYY-MM-DD): ");
                var col = GetWeekdayColumn(blockDate);
                SetDoctorWeekdayAvailability(loggedInDoctorId, col, false);
                Console.WriteLine($"Doctor {loggedInDoctorId} set unavailable on {col} (based on {blockDate}).");
                Console.ReadKey();
                DoctorMain();
                return;
            }
            else if (sub == "2")
            {
                var appointmentDate = ReadDateInUpcomingWeek("Appointment date (YYYY-MM-DD): ");
                var appointmentTime = ReadAllowedTime("Appointment time (09:00 / 09:30 / 10:00 / 10:30): ");

                if (!IsDoctorAvailableOnDay(loggedInDoctorId, appointmentDate))
                {
                    Console.WriteLine("Doctor is not marked available on that weekday.");
                    Console.ReadKey();
                    DoctorMain();
                    return;
                }

                if (IsSlotTaken(loggedInDoctorId, appointmentDate, appointmentTime))
                {
                    Console.WriteLine("That slot is already taken. Try another time.");
                    Console.ReadKey();
                    DoctorMain();
                    return;
                }

                if (!BookAppointment(loggedInDoctorId, appointmentDate, appointmentTime, null, "Doctor block"))
                {
                    Console.WriteLine("Booking failed.");
                    Console.ReadKey();
                    DoctorMain();
                    return;
                }

                Console.WriteLine($"Doctor-only appointment saved for {appointmentDate:yyyy-MM-dd} at {appointmentTime}.");
                Console.WriteLine("Note: only this time slot is blocked. Do you also want to mark the entire weekday unavailable? (y/n)");
                var ans = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (ans == "y" || ans == "yes")
                {
                    var col = GetWeekdayColumn(appointmentDate);
                    SetDoctorWeekdayAvailability(loggedInDoctorId, col, false);
                    Console.WriteLine($"The weekday {col} has been set unavailable for doctor {loggedInDoctorId}.");
                }
                else
                {
                    Console.WriteLine("Weekday availability left unchanged.");
                }

                Console.ReadKey();
                DoctorMain();
                return;
            }
            else
            {
                Console.WriteLine("Invalid option");
                Console.ReadKey();
                DoctorMain();
                return;
            }
        }
        else if (choice == "2")
        {
            if (loggedInDoctorId == -1)
            {
                Console.WriteLine("No doctor logged in. Please log in first.");
                Console.ReadKey();
                Login();
                return;
            }

            ShowDoctorAvailabilityAndBookings(loggedInDoctorId);
            Console.WriteLine("Press any key to return to Doctor menu.");
            Console.ReadKey();
            DoctorMain();
            return;
        }
        else if (choice == "3")
        {
            // Show list of patients and allow viewing details
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
                        Console.WriteLine($"MedicalNumber: {reader["Medical_Number"]}, Name: {reader["F_Name"]} {reader["L_Name"]}");
                    }
                }
            }

            Console.WriteLine("Which patient would you like to look closer at? (ID)");
            int selectedNumber = ReadInt("Patient ID: ");

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

            // Show medical records for the selected patient
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
                                Console.WriteLine($"Date: {reader["Booking_Date"]}, Time: {reader["Booking_Time"]}");
                                Console.WriteLine($"  Diagnosis   : {reader["Diagnosis"]}");
                                Console.WriteLine($"  Description : {reader["Description"]}");
                                Console.WriteLine($"  Prescription: {reader["Perscription"]}");
                                Console.WriteLine("--------------------------------------------------");
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Press any button to return to Doctor menu.");
            Console.ReadKey();
            DoctorMain();
            return;
        }
        else if (choice == "4")
        {
            // Add patient
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
                        Console.WriteLine($"MedicalNumber: {reader["Medical_Number"]}, Name: {reader["F_Name"]} {reader["L_Name"]}");
                    }
                }
            }
            Console.WriteLine("Press any button to continue.");
            Console.ReadLine();
            DoctorMain();
            return;
        }
        else if (choice == "5")
        {
            // Add medical record
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
            return;
        }

        Console.Clear();
        Console.WriteLine("Unvalid option. Press any button to try again.");
        Console.ReadKey();
        DoctorMain();
    }

    // PATIENT STUFF
    public void PatientMain()
    {
        Console.Clear();
        Console.WriteLine("What do you want to do? Write the number of your option.");
        Console.WriteLine(" ");
        Console.WriteLine("1. User Information. (Edit information)");
        Console.WriteLine("2. Book an appointment / Show appointments.");
        Console.WriteLine("3. Diagnosis and descriptions");
        Console.WriteLine("4. Back to Login.");

        choice = Console.ReadLine();
        if (choice == "4") { Login(); return; }

        if (choice == "1")
        {
            Console.Clear();
            Console.WriteLine("View your information.");
            if (loggedInMedicalNumber == -1)
            {
                Console.WriteLine("No patient logged in. Please log in first.");
                Console.ReadKey();
                Login();
                return;
            }

            // Display current info
            DisplayPatientInfo(loggedInMedicalNumber);

            // Offer edit options
            Console.WriteLine();
            Console.WriteLine("Do you want to edit your information? (y/n)");
            var editAns = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (editAns == "y" || editAns == "yes")
            {
                EditPatientInformation(loggedInMedicalNumber);
            }
            else
            {
                Console.WriteLine("Press any button to return to Patient menu.");
                Console.ReadKey();
                PatientMain();
                return;
            }
        }

        if (choice == "2")
        {
            Console.Clear();
            Console.WriteLine("1. Book a new appointment");
            Console.WriteLine("2. Show my appointments");
            var sub = Console.ReadLine();
            if (sub == "2")
            {
                ShowAppointmentsForPatient(loggedInMedicalNumber);
                Console.ReadKey();
                PatientMain();
                return;
            }
            else if (sub == "1")
            {
                if (loggedInMedicalNumber == -1)
                {
                    Console.WriteLine("No patient logged in. Please log in first.");
                    Console.ReadKey();
                    Login();
                    return;
                }

                // Allowed base slots
                var allowedSlots = new[]
                {
                    new TimeOnly(9, 0),
                    new TimeOnly(9, 30),
                    new TimeOnly(10, 0),
                    new TimeOnly(10, 30)
                };

                // Choose date (must be Mon-Fri within upcoming week)
                var appointmentDate = ReadDateInUpcomingWeek("Appointment date (YYYY-MM-DD) — must be Mon-Fri within upcoming 7 days: ");
                var weekdayCol = GetWeekdayColumn(appointmentDate);

                Console.WriteLine();
                Console.WriteLine($"Doctors and free times on {appointmentDate.DayOfWeek} ({appointmentDate:yyyy-MM-dd}):");
                var availableDoctorIds = new List<int>();
                using (var conn = GetUserConnection())
                {
                    conn.Open();
                    string query = $@"
                    SELECT d.Doctor_Id, d.Name_, s.spec_name, s.cost_
                    FROM Doctor d
                    LEFT JOIN Specialization s ON d.Spec_Id = s.Spec_Id
                    WHERE COALESCE((SELECT a.""{weekdayCol}"" FROM Availability a WHERE a.Doctor_Id = d.Doctor_Id), true) = true
                    ORDER BY d.Doctor_Id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        bool any = false;
                        while (reader.Read())
                        {
                            any = true;
                            var id = Convert.ToInt32(reader["Doctor_Id"]);
                            var name = reader["Name_"] is DBNull ? "Unknown" : reader["Name_"].ToString();
                            var spec = reader["spec_name"] is DBNull ? "—" : reader["spec_name"].ToString();
                            var cost = reader["cost_"] is DBNull ? "—" : reader["cost_"].ToString();

                            // Get booked times for this doctor on the chosen date
                            var booked = new HashSet<TimeOnly>();
                            reader.Close(); // close reader to run nested query
                            using (var bcmd = new NpgsqlCommand(
                                "SELECT Time_ FROM Appointment WHERE Doctor_Id = @Doctor_Id AND Date_ = @Date_", conn))
                            {
                                bcmd.Parameters.AddWithValue("Doctor_Id", id);
                                bcmd.Parameters.AddWithValue("Date_", appointmentDate);
                                using var breader = bcmd.ExecuteReader();
                                while (breader.Read())
                                {
                                    var tObj = breader["Time_"];
                                    if (tObj is TimeOnly tt) booked.Add(tt);
                                    else if (TimeOnly.TryParse(tObj.ToString(), out var parsed)) booked.Add(parsed);
                                }
                            }

                            // compute available slots
                            var free = allowedSlots.Where(a => !booked.Contains(a)).ToArray();
                            var timesText = free.Length == 0 ? "No free times" : string.Join(", ", free.Select(t => t.ToString("HH:mm")));
                            Console.WriteLine($"ID: {id}, Name: {name}, Spec: {spec}, Cost: {cost}  — Free times: {timesText}");
                            availableDoctorIds.Add(id);

                            cmd.CommandText = $@"
                    SELECT d.Doctor_Id, d.Name_, s.spec_name, s.cost_
                    FROM Doctor d
                    LEFT JOIN Specialization s ON d.Spec_Id = s.Spec_Id
                    WHERE COALESCE((SELECT a.""{weekdayCol}"" FROM Availability a WHERE a.Doctor_Id = d.Doctor_Id), true) = true
                    ORDER BY d.Doctor_Id";
                            using var dummy = cmd.ExecuteReader();
                            dummy.Close();
                            using var reCmd = new NpgsqlCommand(query, conn);
                            using var reReader = reCmd.ExecuteReader();
  
                            break;
                        }

                        if (!any)
                        {
                            Console.WriteLine("No doctors are available on that weekday. Try another date.");
                            Console.ReadKey();
                            PatientMain();
                            return;
                        }
                    }
                }

                // Re-list doctors for selection
                Console.WriteLine();
                Console.WriteLine("Choose a doctor by ID from the list above.");
                int docId = ReadInt("Enter doctor ID to book with: ");

                // Validate doctor is available that day
                if (!IsDoctorAvailableOnDay(docId, appointmentDate))
                {
                    Console.WriteLine("Selected doctor is marked unavailable on that weekday. Choose another date or doctor.");
                    Console.ReadKey();
                    PatientMain();
                    return;
                }

                // Get this doctor's booked times and available slots
                HashSet<TimeOnly> bookedTimes = new();
                using (var conn = GetUserConnection())
                {
                    conn.Open();
                    string bsql = "SELECT Time_ FROM Appointment WHERE Doctor_Id = @Doctor_Id AND Date_ = @Date_";
                    using var bcmd = new NpgsqlCommand(bsql, conn);
                    bcmd.Parameters.AddWithValue("Doctor_Id", docId);
                    bcmd.Parameters.AddWithValue("Date_", appointmentDate);
                    using var breader = bcmd.ExecuteReader();
                    while (breader.Read())
                    {
                        var to = breader["Time_"];
                        if (to is TimeOnly tt) bookedTimes.Add(tt);
                        else if (TimeOnly.TryParse(to.ToString(), out var parsed)) bookedTimes.Add(parsed);
                    }
                }

                var availableSlots = allowedSlots.Where(a => !bookedTimes.Contains(a)).ToArray();
                if (availableSlots.Length == 0)
                {
                    Console.WriteLine("Selected doctor has no free slots on that date. Choose another doctor or date.");
                    Console.ReadKey();
                    PatientMain();
                    return;
                }

                Console.WriteLine("Available times for selected doctor:");
                for (int i = 0; i < availableSlots.Length; i++)
                    Console.WriteLine($"{i + 1}. {availableSlots[i]:HH:mm}");

                // Ask patient to pick one of the displayed available times
                int selIndex = -1;
                while (true)
                {
                    selIndex = ReadInt("Choose time number from the list above: ") - 1;
                    if (selIndex >= 0 && selIndex < availableSlots.Length) break;
                    Console.WriteLine("Invalid selection. Try again.");
                }

                var appointmentTime = availableSlots[selIndex];

                if (IsSlotTaken(docId, appointmentDate, appointmentTime))
                {
                    Console.WriteLine("That slot was just taken. Try again.");
                    Console.ReadKey();
                    PatientMain();
                    return;
                }

                string patientName = GetPatientFullName(loggedInMedicalNumber);
                var bookedResult = BookAppointment(docId, appointmentDate, appointmentTime, loggedInMedicalNumber, patientName);
                if (!bookedResult)
                {
                    Console.WriteLine("Booking failed (permission or DB error).");
                    Console.ReadKey();
                    PatientMain();
                    return;
                }

                Console.WriteLine($"Appointment booked with Doctor {docId} on {appointmentDate:yyyy-MM-dd} at {appointmentTime:HH:mm}.");
                Console.ReadKey();
                PatientMain();
                return;
            }
            else
            {
                Console.WriteLine("Invalid option.");
                Console.ReadKey();
                PatientMain();
                return;
            }
        }

        if (choice == "3")
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
                                Console.WriteLine($"Date: {reader["Booking_Date"]}, Time: {reader["Booking_Time"]}");
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
            return;
        }

        Console.Clear();
        Console.WriteLine("Unvalid option. Press any button to try again.");
        Console.ReadKey();
        PatientMain();
    }

    private bool IsWithinUpcomingWeek(DateOnly date)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var lastDay = today.AddDays(7);
        return date >= today && date <= lastDay;
    }

    private DateOnly ReadDateInUpcomingWeek(string prompt)
    {
        while (true)
        {
            var d = ReadDate(prompt);
            if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
            {
                Console.WriteLine("Bookings are not allowed on weekends. Please choose a weekday (Mon–Fri).");
                continue;
            }
            if (IsWithinUpcomingWeek(d)) return d;
            Console.WriteLine("Date must be within the upcoming week (today through 7 days).");
        }
    }

    private TimeOnly ReadAllowedTime(string prompt)
    {
        var allowed = new[]
        {
        new TimeOnly(9, 0),
        new TimeOnly(9, 30),
        new TimeOnly(10, 0),
        new TimeOnly(10, 30)
    };

        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (TimeOnly.TryParse(s, out var t))
            {
                foreach (var a in allowed)
                    if (a == t) return t;
            }
            Console.WriteLine("Invalid time. Allowed times: 09:00, 09:30, 10:00, 10:30.");
        }
    }

    private string GetWeekdayColumn(DateOnly date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Monday => "Monday",
            DayOfWeek.Tuesday => "Tuesday",
            DayOfWeek.Wednesday => "Wednesday",
            DayOfWeek.Thursday => "Thursday",
            DayOfWeek.Friday => "Friday",
            _ => throw new InvalidOperationException("Weekends are not allowed")
        };
    }

    private bool IsDoctorAvailableOnDay(int doctorId, DateOnly date)
    {
        var col = GetWeekdayColumn(date);
        using var conn = GetUserConnection();
        conn.Open();
        string sql = $@"SELECT ""{col}"" FROM Availability WHERE Doctor_Id = @Doctor_Id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("Doctor_Id", doctorId);
        var res = cmd.ExecuteScalar();
        if (res == null) return true; // default to available if no row
        return res != DBNull.Value && Convert.ToBoolean(res);
    }

    private bool IsSlotTaken(int doctorId, DateOnly date, TimeOnly time)
    {
        using var conn = GetUserConnection();
        conn.Open();
        string q = @"SELECT 1 FROM Appointment WHERE Doctor_Id = @Doctor_Id AND Date_ = @Date_ AND Time_ = @Time_ LIMIT 1";
        using var cmd = new NpgsqlCommand(q, conn);
        cmd.Parameters.AddWithValue("Doctor_Id", doctorId);
        cmd.Parameters.AddWithValue("Date_", date);
        cmd.Parameters.AddWithValue("Time_", time);
        var res = cmd.ExecuteScalar();
        return res != null;
    }

    private bool BookAppointment(int doctorId, DateOnly date, TimeOnly time, int? patientId, string patientName)
    {
        if (patientId.HasValue && loggedInMedicalNumber != -1 && patientId.Value != loggedInMedicalNumber)
        {
            Console.WriteLine("You may only book appointments for the patient you are logged in as.");
            return false;
        }

        using var conn = GetUserConnection();
        conn.Open();
        string q = @"INSERT INTO Appointment (Doctor_Id, Patient_Medical_Number, Patient_Name, Date_, Time_)
                 VALUES (@Doctor_Id, @Patient_Medical_Number, @Patient_Name, @Date_, @Time_)";
        using var cmd = new NpgsqlCommand(q, conn);
        cmd.Parameters.AddWithValue("Doctor_Id", doctorId);
        cmd.Parameters.AddWithValue("Date_", date);
        cmd.Parameters.AddWithValue("Time_", time);
        if (patientId.HasValue) cmd.Parameters.AddWithValue("Patient_Medical_Number", patientId.Value);
        else cmd.Parameters.AddWithValue("Patient_Medical_Number", DBNull.Value);
        cmd.Parameters.AddWithValue("Patient_Name", (object)patientName ?? DBNull.Value);
        cmd.ExecuteNonQuery();
        return true;
    }

    private void SetDoctorWeekdayAvailability(int doctorId, string weekdayColumn, bool isAvailable)
    {
        using var conn = GetUserConnection();
        conn.Open();

        string existsQ = @"SELECT 1 FROM Availability WHERE Doctor_Id = @Doctor_Id LIMIT 1";
        using (var cmd = new NpgsqlCommand(existsQ, conn))
        {
            cmd.Parameters.AddWithValue("Doctor_Id", doctorId);
            var exists = cmd.ExecuteScalar();
            if (exists == null)
            {
                string insert = @"INSERT INTO Availability (Doctor_Id, ""Monday"", ""Tuesday"", ""Wednesday"", ""Thursday"", ""Friday"")
                              VALUES (@Doctor_Id, true, true, true, true, true)";
                using var icmd = new NpgsqlCommand(insert, conn);
                icmd.Parameters.AddWithValue("Doctor_Id", doctorId);
                icmd.ExecuteNonQuery();
            }
        }

        string update = $@"UPDATE Availability SET ""{weekdayColumn}"" = @val WHERE Doctor_Id = @Doctor_Id";
        using var ucmd = new NpgsqlCommand(update, conn);
        ucmd.Parameters.AddWithValue("val", isAvailable);
        ucmd.Parameters.AddWithValue("Doctor_Id", doctorId);
        ucmd.ExecuteNonQuery();
    }

    private void ShowAppointmentsForDoctor(int doctorId)
    {
        using var conn = GetUserConnection();
        conn.Open();
        string q = @"SELECT Date_, Time_, Patient_Medical_Number, Patient_Name
                 FROM Appointment
                 WHERE Doctor_Id = @Doctor_Id
                 ORDER BY Date_, Time_";
        using var cmd = new NpgsqlCommand(q, conn);
        cmd.Parameters.AddWithValue("Doctor_Id", doctorId);
        using var reader = cmd.ExecuteReader();
        Console.WriteLine($"Appointments for Doctor {doctorId}:");
        while (reader.Read())
        {
            Console.WriteLine($"Date: {reader["Date_"]}, Time: {reader["Time_"]}, Patient: {(reader["Patient_Medical_Number"] == DBNull.Value ? "— (doctor-only)" : $"{reader["Patient_Name"]} ({reader["Patient_Medical_Number"]})")}");        }
    }

    private void ShowAppointmentsForPatient(int patientId)
    {
        using var conn = GetUserConnection();
        conn.Open();
        string q = @"SELECT Date_, Time_, Doctor_Id
                 FROM Appointment
                 WHERE Patient_Medical_Number = @Patient_Medical_Number
                 ORDER BY Date_, Time_";
        using var cmd = new NpgsqlCommand(q, conn);
        cmd.Parameters.AddWithValue("Patient_Medical_Number", patientId);
        using var reader = cmd.ExecuteReader();
        Console.WriteLine($"Appointments for Patient {patientId}:");
        while (reader.Read())
        {
            Console.WriteLine($"Date: {reader["Date_"]}, Time: {reader["Time_"]}, Doctor: {reader["Doctor_Id"]}");
        }
    }

    private string GetPatientFullName(int patientId)
    {
        using var conn = GetUserConnection();
        conn.Open();
        string q = @"SELECT F_Name, L_Name FROM Patient WHERE Medical_Number = @Medical_Number";
        using var cmd = new NpgsqlCommand(q, conn);
        cmd.Parameters.AddWithValue("Medical_Number", patientId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read()) return $"{reader["F_Name"]} {reader["L_Name"]}";
        return "Unknown";
    }

    private void ShowDoctorAvailabilityAndBookings(int doctorId)
    {
        using var conn = GetUserConnection();
        conn.Open();

        bool monday = true, tuesday = true, wednesday = true, thursday = true, friday = true;
        string availSql = @"SELECT ""Monday"", ""Tuesday"", ""Wednesday"", ""Thursday"", ""Friday"" FROM Availability WHERE Doctor_Id = @Doctor_Id";
        using (var cmd = new NpgsqlCommand(availSql, conn))
        {
            cmd.Parameters.AddWithValue("Doctor_Id", doctorId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                monday = reader["Monday"] != DBNull.Value && Convert.ToBoolean(reader["Monday"]);
                tuesday = reader["Tuesday"] != DBNull.Value && Convert.ToBoolean(reader["Tuesday"]);
                wednesday = reader["Wednesday"] != DBNull.Value && Convert.ToBoolean(reader["Wednesday"]);
                thursday = reader["Thursday"] != DBNull.Value && Convert.ToBoolean(reader["Thursday"]);
                friday = reader["Friday"] != DBNull.Value && Convert.ToBoolean(reader["Friday"]);
            }
            reader.Close();
        }

        Console.WriteLine();
        Console.WriteLine("Weekly availability (per weekday):");
        Console.WriteLine($"  Monday   : {(monday ? "Available" : "Unavailable")}");
        Console.WriteLine($"  Tuesday  : {(tuesday ? "Available" : "Unavailable")}");
        Console.WriteLine($"  Wednesday: {(wednesday ? "Available" : "Unavailable")}");
        Console.WriteLine($"  Thursday : {(thursday ? "Available" : "Unavailable")}");
        Console.WriteLine($"  Friday   : {(friday ? "Available" : "Unavailable")}");
        Console.WriteLine();

        // 2) List booked dates and times in upcoming week
        var start = DateOnly.FromDateTime(DateTime.Today);
        var end = start.AddDays(7);

        string bookingsSql = @"
        SELECT Date_, Time_, Patient_Medical_Number, Patient_Name
        FROM Appointment
        WHERE Doctor_Id = @Doctor_Id AND Date_ >= @Start AND Date_ <= @End
        ORDER BY Date_, Time_";
        using (var cmd = new NpgsqlCommand(bookingsSql, conn))
        {
            cmd.Parameters.AddWithValue("Doctor_Id", doctorId);
            cmd.Parameters.AddWithValue("Start", start);
            cmd.Parameters.AddWithValue("End", end);

            using var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                Console.WriteLine("No appointments booked for the upcoming week.");
                Console.WriteLine();
                return;
            }

            Console.WriteLine("Upcoming bookings (next 7 days):");
            DateOnly? currentDate = null;
            while (reader.Read())
            {
                var date = (DateOnly)reader["Date_"];
                var time = reader["Time_"];
                var pid = reader["Patient_Medical_Number"];
                var pname = reader["Patient_Name"];

                if (currentDate == null || currentDate != date)
                {
                    Console.WriteLine();
                    Console.WriteLine($"{date:yyyy-MM-dd} ({date.DayOfWeek}):");
                    currentDate = date;
                }

                if (pid == DBNull.Value)
                {
                    Console.WriteLine($"  {time}  — Doctor-only block");
                }
                else
                {
                    Console.WriteLine($"  {time}  — Patient: {pname} ({pid})");
                }
            }
            Console.WriteLine();
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

    // --- Patient edit helpers ---
    private void DisplayPatientInfo(int medNumber)
    {
        using var conn = GetUserConnection();
        conn.Open();
        string query = @"SELECT Medical_Number, F_Name, L_Name, Gender, Adress, Phone_Number, Birth_Date, Registration_Date FROM Patient WHERE Medical_Number = @Medical_Number";
        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("Medical_Number", medNumber);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            Console.WriteLine($"MedicalNumber : {reader["Medical_Number"]}");
            Console.WriteLine($"Name          : {reader["F_Name"]} {reader["L_Name"]}");
            Console.WriteLine($"Gender        : {reader["Gender"]}");
            Console.WriteLine($"Address       : {reader["Adress"]}");
            Console.WriteLine($"Phone Number  : {reader["Phone_Number"]}");
            Console.WriteLine($"Birth Date    : {reader["Birth_Date"]}");
            Console.WriteLine($"Registered on : {reader["Registration_Date"]}");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Patient not found.");
        }
    }

    private void EditPatientInformation(int medNumber)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Edit your information. Current values:");
            DisplayPatientInfo(medNumber);

            Console.WriteLine("Select the field to edit:");
            Console.WriteLine("1. First name");
            Console.WriteLine("2. Last name");
            Console.WriteLine("3. Gender");
            Console.WriteLine("4. Address");
            Console.WriteLine("5. Phone number");
            Console.WriteLine("6. Password");
            Console.WriteLine("7. Back to Patient menu");
            var sel = Console.ReadLine();

            if (sel == "7")
            {
                PatientMain();
                return;
            }

            switch (sel)
            {
                case "1":
                    Console.Write("New first name: ");
                    var newF = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newF))
                        UpdatePatientColumn(medNumber, "F_Name", newF);
                    break;
                case "2":
                    Console.Write("New last name: ");
                    var newL = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newL))
                        UpdatePatientColumn(medNumber, "L_Name", newL);
                    break;
                case "3":
                    Console.Write("New gender: ");
                    var newG = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newG))
                        UpdatePatientColumn(medNumber, "Gender", newG);
                    break;
                case "4":
                    Console.Write("New address: ");
                    var newA = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newA))
                        UpdatePatientColumn(medNumber, "Adress", newA);
                    break;
                case "5":
                    Console.Write("New phone number (digits only): ");
                    var phoneStr = Console.ReadLine();
                    if (int.TryParse(phoneStr, out var phone))
                    {
                        UpdatePatientColumn(medNumber, "Phone_Number", phone);
                    }
                    else
                    {
                        Console.WriteLine("Invalid phone number. Update cancelled.");
                        Console.ReadKey();
                    }
                    break;
                case "6":
                    Console.Write("New password: ");
                    var newPwd = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newPwd))
                        UpdatePatientColumn(medNumber, "Password_", newPwd);
                    break;
                default:
                    Console.WriteLine("Invalid selection. Try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void UpdatePatientColumn(int medNumber, string columnName, object value)
    {
        var allowedMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "F_Name", "f_name" },
            { "L_Name", "l_name" },
            { "Gender", "gender" },
            { "Adress", "adress" },
            { "Phone_Number", "phone_number" },
            { "Password_", "password_" }
        };

        if (!allowedMap.TryGetValue(columnName, out var dbColumn))
        {
            Console.WriteLine("Updating that column is not allowed.");
            Console.ReadKey();
            return;
        }

        using var conn = GetUserConnection();
        conn.Open();

        var sql = $@"UPDATE Patient SET {dbColumn} = @val WHERE Medical_Number = @Medical_Number";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("val", value ?? DBNull.Value);
        cmd.Parameters.AddWithValue("Medical_Number", medNumber);

        var rows = cmd.ExecuteNonQuery();
        if (rows > 0)
        {
            Console.WriteLine("Update successful.");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Update failed (no rows affected).");
        }
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
    }
}