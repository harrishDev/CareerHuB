using CareerHub.entity;
using CareerHub.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace CareerHub.dao
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager()
        {
            _connectionString = DBConnUtil.GetConnectionString();
        }

        public void InitializeDatabase()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var commands = new List<string>
                {
                    @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Companies' AND xtype='U')
                      CREATE TABLE Companies (
                          CompanyID INT IDENTITY(1,1) PRIMARY KEY,
                          CompanyName NVARCHAR(100),
                          Location NVARCHAR(100)
                      )",
                    @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Applicants' AND xtype='U')
                      CREATE TABLE Applicants (
                          ApplicantID INT IDENTITY(1,1) PRIMARY KEY,
                          FirstName NVARCHAR(100),
                          LastName NVARCHAR(100),
                          Email NVARCHAR(100),
                          Phone NVARCHAR(20),
                          Resume NVARCHAR(200)
                      )",
                    @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Jobs' AND xtype='U')
                      CREATE TABLE Jobs (
                          JobID INT IDENTITY(1,1) PRIMARY KEY,
                          CompanyID INT FOREIGN KEY REFERENCES Companies(CompanyID),
                          JobTitle NVARCHAR(100),
                          JobDescription NVARCHAR(MAX),
                          JobLocation NVARCHAR(100),
                          Salary DECIMAL(18,2),
                          JobType NVARCHAR(50),
                          PostedDate DATETIME
                      )",
                    @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Applications' AND xtype='U')
                      CREATE TABLE Applications (
                          ApplicationID INT IDENTITY(1,1) PRIMARY KEY,
                          JobID INT FOREIGN KEY REFERENCES Jobs(JobID),
                          ApplicantID INT FOREIGN KEY REFERENCES Applicants(ApplicantID),
                          ApplicationDate DATETIME,
                          CoverLetter NVARCHAR(MAX)
                      )"
                };

                foreach (var sql in commands)
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertCompany(Company company)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Companies (Company_Name, Location) VALUES (@Company_Name, @Location)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Company_Name", company.CompanyName);
                cmd.Parameters.AddWithValue("@Location", company.Location);
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertApplicant(Applicant applicant)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Applicants (First_Name, Last_Name, Email, Phone, Resume) VALUES (@First_Name, @Last_Name, @Email, @Phone, @Resume)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@First_Name", applicant.FirstName);
                cmd.Parameters.AddWithValue("@Last_Name", applicant.LastName);
                cmd.Parameters.AddWithValue("@Email", applicant.Email);
                cmd.Parameters.AddWithValue("@Phone", applicant.Phone);
                cmd.Parameters.AddWithValue("@Resume", applicant.Resume);
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertJobListing(JobListing job)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Jobs (Company_ID, Job_Title, Job_Description, Job_Location, Salary, Job_Type, Posted_Date)
                                 VALUES (@Company_ID, @Job_Title, @Job_Description, @Job_Location, @Salary, @Job_Type, @Posted_Date)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Company_ID", job.CompanyID);
                cmd.Parameters.AddWithValue("@Job_Title", job.JobTitle);
                cmd.Parameters.AddWithValue("@Job_Description", job.JobDescription);
                cmd.Parameters.AddWithValue("@Job_Location", job.JobLocation);
                cmd.Parameters.AddWithValue("@Salary", job.Salary);
                cmd.Parameters.AddWithValue("@Job_Type", job.JobType);
                cmd.Parameters.AddWithValue("@Posted_Date", job.PostedDate);
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertJobApplication(JobApplication application)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Applications (Job_ID, Applicant_ID, Application_Date, Cover_Letter)
                                 VALUES (@Job_ID, @Applicant_ID, @Application_Date, @Cover_Letter)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Job_ID", application.JobID);
                cmd.Parameters.AddWithValue("@Applicant_ID", application.ApplicantID);
                cmd.Parameters.AddWithValue("@Application_Date", application.ApplicationDate);
                cmd.Parameters.AddWithValue("@Cover_Letter", application.CoverLetter);
                cmd.ExecuteNonQuery();
            }
        }

        public List<JobListing> GetJobListings()
        {
            var jobs = new List<JobListing>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Jobs";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    jobs.Add(new JobListing
                    {
                        JobID = (int)reader["Job_ID"],
                        CompanyID = (int)reader["Company_ID"],
                        JobTitle = reader["Job_Title"].ToString(),
                        JobDescription = reader["Job_Description"].ToString(),
                        JobLocation = reader["Job_Location"].ToString(),
                        Salary = (decimal)reader["Salary"],
                        JobType = reader["Job_Type"].ToString(),
                        PostedDate = (DateTime)reader["Posted_Date"]
                    });
                }
            }

            return jobs;
        }

        public List<Company> GetCompanies()
        {
            var companies = new List<Company>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Companies";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    companies.Add(new Company
                    {
                        CompanyID = (int)reader["Company_ID"],
                        CompanyName = reader["Company_Name"].ToString(),
                        Location = reader["Location"].ToString()
                    });
                }
            }

            return companies;
        }

        public List<Applicant> GetApplicants()
        {
            var applicants = new List<Applicant>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Applicants";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    applicants.Add(new Applicant
                    {
                        ApplicantID = (int)reader["Applicant_ID"],
                        FirstName = reader["First_Name"].ToString(),
                        LastName = reader["Last_Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Resume = reader["Resume"].ToString()
                    });
                }
            }

            return applicants;
        }

        public List<JobApplication> GetApplicationsForJob(int jobID)
        {
            var applications = new List<JobApplication>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Applications WHERE Job_ID = @Job_ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Job_ID", jobID);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    applications.Add(new JobApplication
                    {
                        ApplicationID = (int)reader["Application_ID"],
                        JobID = (int)reader["Job_ID"],
                        ApplicantID = (int)reader["Applicant_ID"],
                        ApplicationDate = (DateTime)reader["Application_Date"],
                        CoverLetter = reader["Cover_Letter"].ToString()
                    });
                }
            }

            return applications;
        }
    }
}
