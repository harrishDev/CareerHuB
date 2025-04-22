using CareerHub.entity;
using CareerHub.dao;
using CareerHub.exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerHub.Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IDatabaseManager dbManager = new DatabaseManager();
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n==== CareerHub Job Board ====");
                Console.WriteLine("1. Initialize Database");
                Console.WriteLine("2. Register Company");
                Console.WriteLine("3. Register Applicant");
                Console.WriteLine("4. Post Job");
                Console.WriteLine("5. Apply for Job");
                Console.WriteLine("6. View All Jobs");
                Console.WriteLine("7. View Applications for a Job");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");

                try
                {
                    int choice = int.Parse(Console.ReadLine());

                    switch (choice)
                    {
                        case 1:
                            dbManager.InitializeDatabase();
                            Console.WriteLine("Database Initialized.");
                            break;

                        case 2:
                            Console.Write("Company Name: ");
                            string compName = Console.ReadLine();
                            Console.Write("Location: ");
                            string compLocation = Console.ReadLine();
                            Company company = new Company { CompanyName = compName, Location = compLocation };
                            dbManager.InsertCompany(company);
                            Console.WriteLine("Company registered.");
                            break;

                        
                        case 3:
                            Console.Write("First Name: ");
                            string fname = Console.ReadLine();

                            Console.Write("Last Name: ");
                            string lname = Console.ReadLine();

                            Console.Write("Email: ");
                            string email = Console.ReadLine();

                            if (!email.Contains("@") || !email.Contains("."))
                                throw new InvalidEmailFormatException("Invalid email format!");

                            Console.Write("Phone: ");
                            string phone = Console.ReadLine();

                            Console.Write("Resume File Path (.pdf or .docx, <1MB): ");
                            string resumePath = Console.ReadLine();

                            if (!File.Exists(resumePath))
                                throw new FileUploadException("File not found!");

                            FileInfo fileInfo = new FileInfo(resumePath);
                            if (fileInfo.Length > 1024 * 1024)  // 1MB limit
                                throw new FileUploadException("File size exceeds 1MB.");

                            if (!(resumePath.EndsWith(".pdf") || resumePath.EndsWith(".docx")))
                                throw new FileUploadException("Only .pdf or .docx files are allowed.");

                            // Save file to local folder (e.g., ./Resumes/)
                            string resumesDir = Path.Combine(Directory.GetCurrentDirectory(), "Resumes");
                            if (!Directory.Exists(resumesDir))
                                Directory.CreateDirectory(resumesDir);

                            string destFileName = Path.Combine(resumesDir, $"{Guid.NewGuid()}_{Path.GetFileName(resumePath)}");
                            File.Copy(resumePath, destFileName, true);  // true = overwrite if exists

                            // Save relative path to DB
                            Applicant applicant = new Applicant
                            {
                                FirstName = fname,
                                LastName = lname,
                                Email = email,
                                Phone = phone,
                                Resume = destFileName
                            };

                            dbManager.InsertApplicant(applicant);
                            Console.WriteLine("Applicant registered and resume uploaded successfully.");
                            break;


                        case 4:
                            Console.Write("Company ID: ");
                            int cid = int.Parse(Console.ReadLine());
                            Console.Write("Job Title: ");
                            string title = Console.ReadLine();
                            Console.Write("Job Description: ");
                            string desc = Console.ReadLine();
                            Console.Write("Location: ");
                            string loc = Console.ReadLine();
                            Console.Write("Salary: ");
                            decimal salary = decimal.Parse(Console.ReadLine());

                            if (salary < 0)
                                throw new NegativeSalaryException("Salary cannot be negative!");

                            Console.Write("Job Type: (Full-time/ Part-time/ Contract) ");
                            string type = Console.ReadLine();

                            JobListing job = new JobListing
                            {
                                CompanyID = cid,
                                JobTitle = title,
                                JobDescription = desc,
                                JobLocation = loc,
                                Salary = salary,
                                JobType = type,
                                PostedDate = DateTime.Now
                            };

                            dbManager.InsertJobListing(job);
                            Console.WriteLine("Job posted.");
                            break;

                        case 5:
                            Console.Write("Applicant ID: ");
                            int appID = int.Parse(Console.ReadLine());
                            Console.Write("Job ID: ");
                            int jobID = int.Parse(Console.ReadLine());
                            Console.Write("Cover Letter: ");
                            string cover = Console.ReadLine();
                            Console.Write("Application Date (yyyy-MM-dd): ");
                            DateTime appDate = DateTime.Parse(Console.ReadLine());
                            DateTime deadline = DateTime.Now.AddMonths(1); // for example

                            if (appDate > deadline)
                                throw new ApplicationDeadlineException("Application deadline has passed!");

                            JobApplication application = new JobApplication
                            {
                                ApplicantID = appID,
                                JobID = jobID,
                                CoverLetter = cover,
                                ApplicationDate = appDate
                            };

                            dbManager.InsertJobApplication(application);
                            Console.WriteLine("Job application submitted.");
                            break;

                        case 6:
                            var jobs = dbManager.GetJobListings();
                            Console.WriteLine("\n--- Job Listings ---");
                            foreach (var j in jobs)
                            {
                                Console.WriteLine($"{j.JobID}: {j.JobTitle} at {j.JobLocation} | Salary: {j.Salary}");
                            }
                            break;

                        case 7:
                            Console.Write("Enter Job ID: ");
                            int jid = int.Parse(Console.ReadLine());
                            var apps = dbManager.GetApplicationsForJob(jid);
                            Console.WriteLine("\n--- Applications ---");
                            foreach (var a in apps)
                            {
                                Console.WriteLine($"AppID: {a.ApplicationID}, ApplicantID: {a.ApplicantID}, Date: {a.ApplicationDate}");
                            }
                            break;

                        case 0:
                            exit = true;
                            Console.WriteLine("Goodbye!");
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (InvalidEmailFormatException ex)
                {
                    Console.WriteLine("Email Error: " + ex.Message);
                }
                catch (NegativeSalaryException ex)
                {
                    Console.WriteLine("Salary Error: " + ex.Message);
                }
                catch (FileUploadException ex)
                {
                    Console.WriteLine("File Error: " + ex.Message);
                }
                catch (ApplicationDeadlineException ex)
                {
                    Console.WriteLine("Deadline Error: " + ex.Message);
                }
                catch (DatabaseConnectionException ex)
                {
                    Console.WriteLine("DB Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("General Error: " + ex.Message);
                }
            }
        }
    }
}