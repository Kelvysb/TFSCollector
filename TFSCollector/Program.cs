using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TFSCollector.Layers.BackEnd;
using TFSCollector.Model;

namespace TFSCollector
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] input;

            try
            {

                initializeDirs();
                List<string> argsList = args.ToList();
                Dictionary<String, String> inputArgs;
                bool exit = true;
                input = args;

                do
                {

                    argsList = input.ToList();
                    inputArgs = new Dictionary<string, string>();

                    foreach (var arg in argsList)
                    {
                        if (arg.StartsWith("-"))
                        {
                            if (arg.Equals("--update", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (argsList.Count > argsList.IndexOf(arg) + 1 && !argsList[argsList.IndexOf(arg) + 1].StartsWith("-"))
                                {
                                    inputArgs.Add("UPDATE", argsList[argsList.IndexOf(arg) + 1]);
                                }
                                else
                                {
                                    inputArgs.Add("UPDATE", "");
                                }
                             
                            }
                            else if (arg.Equals("--csv", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (argsList.Count > argsList.IndexOf(arg) + 1 && !argsList[argsList.IndexOf(arg) + 1].StartsWith("-"))
                                {
                                    inputArgs.Add("CSV", argsList[argsList.IndexOf(arg) + 1]);
                                }
                                else
                                {
                                    throw new Exception("CSV file must have a name");
                                }
                            }
                            else if (arg.Equals("--query", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (argsList.Count > argsList.IndexOf(arg) + 1 && !argsList[argsList.IndexOf(arg) + 1].StartsWith("-"))
                                {
                                    inputArgs.Add("QUERY", argsList[argsList.IndexOf(arg) + 1]);
                                }
                                else
                                {
                                    throw new Exception("query must have a name");
                                }
                            }
                            else if (arg.Equals("--open", StringComparison.InvariantCultureIgnoreCase))
                            {
                                inputArgs.Add("OPEN", "");
                            }
                            else if (arg.Equals("--config", StringComparison.InvariantCultureIgnoreCase))
                            {
                                inputArgs.Add("CONFIG", "");
                            }
                            else if (arg.Equals("--help", StringComparison.InvariantCultureIgnoreCase) ||
                                     arg.Equals("-h", StringComparison.InvariantCultureIgnoreCase))
                            {
                                inputArgs.Add("HELP", "");
                            }
                            else if (arg.Equals("--version", StringComparison.InvariantCultureIgnoreCase) ||
                                     arg.Equals("-v", StringComparison.InvariantCultureIgnoreCase))
                            {
                                inputArgs.Add("VERSION", "");
                            }
                            else if (arg.Equals("--env", StringComparison.InvariantCultureIgnoreCase) ||
                                     arg.Equals("-env", StringComparison.InvariantCultureIgnoreCase))
                            {
                                inputArgs.Add("ENV", "");
                            }
                            else if (arg.Equals("--no-exit", StringComparison.InvariantCultureIgnoreCase))
                            {
                                inputArgs.Add("NOEXIT", "");
                            }
                            else if (arg.Equals("--exit", StringComparison.InvariantCultureIgnoreCase))
                            {
                                inputArgs.Add("EXIT", "");
                            }
                            else
                            {
                                throw new Exception("Invalid command use -h or --help to a list of valid commands.");
                            }
                        }
                    }


                    if (inputArgs.Count > 0 && !inputArgs.ContainsKey("EXIT"))
                    {

                        if (inputArgs.ContainsKey("NOEXIT"))
                        {
                            exit = false;
                        }
                        else if (inputArgs.ContainsKey("UPDATE"))
                        {
                            update(inputArgs);
                        }
                        else if (inputArgs.ContainsKey("CSV"))
                        {
                            saveCsv(inputArgs);
                        }
                        else if (inputArgs.ContainsKey("QUERY"))
                        {
                            createQuery(inputArgs);
                        }
                        else if (inputArgs.ContainsKey("CONFIG"))
                        {
                            config();
                        }
                        else if (inputArgs.ContainsKey("VERSION"))
                        {
                            Console.WriteLine("TFSCollector version: " + Assembly.GetEntryAssembly().GetName().Version);
                        }
                        else if (inputArgs.ContainsKey("ENV"))
                        {
                            Console.WriteLine("Current working directory: " + Collector.currentDirectory);
                        }
                        else if (inputArgs.ContainsKey("OPEN"))
                        {
                            Open();
                        }
                        else if (inputArgs.ContainsKey("HELP"))
                        {
                            help();
                        }
                        else
                        {
                            Console.WriteLine("Invalid command, use -h or --help to see a list of valid commands.");
                        }

                    }
                    else if (inputArgs.ContainsKey("EXIT"))
                    {
                        exit = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid command, use -h or --help to see a list of valid commands.");
                    }

                    if (!exit)
                    {

                        input = Console.In.ReadLine().Split(" ");
                    }

                } while (!exit);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private static void update(Dictionary<string, string> inputArgs)
        {
            List<string> result;
            string path;

            try
            {

                if (!inputArgs.ContainsKey("CSV"))
                {
                    path = "";
                }
                else
                {
                    if (!Path.IsPathRooted(inputArgs.GetValueOrDefault("CSV")))
                    {
                        path = Path.GetFullPath(Path.Combine(Collector.currentDirectory, inputArgs.GetValueOrDefault("CSV")));
                    }
                    else
                    {
                        path = inputArgs.GetValueOrDefault("CSV");
                    }
                }

                result = Collector.update(inputArgs.GetValueOrDefault("UPDATE"), path);

                foreach (string item in result)
                {
                    Console.WriteLine(item);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void saveCsv(Dictionary<string, string> inputArgs)
        {
            string result;
            string path;
            try
            {
                if (!Path.IsPathRooted(inputArgs.GetValueOrDefault("CSV")))
                {
                    path = Path.GetFullPath(Path.Combine(Collector.currentDirectory, inputArgs.GetValueOrDefault("CSV")));
                }
                else
                {
                    path = inputArgs.GetValueOrDefault("CSV");
                }

                result = Collector.saveAllCsv(path);
                Console.WriteLine(result);       
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void Open()
        {
            try
            {
                var p0 = new Process();
                p0.StartInfo = new ProcessStartInfo(Collector.workDirectory)
                {
                    UseShellExecute = true
                };
                p0.Start();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void config()
        {

            IARConfig configuration;


            try
            {

                configuration = Collector.LoadConfig();
                Console.WriteLine("Current configuration:");
                Console.WriteLine("TFS Url: " + configuration.BaseUri);
                Console.WriteLine("Project Name: " + configuration.ProjectName);
                Console.WriteLine("User token: " + configuration.UserToken);
                Console.WriteLine("Default Query Name: " + configuration.DefaultQuery);

                Console.WriteLine("Update? (y or n)");

                if (Console.ReadLine().Equals("y", StringComparison.InvariantCultureIgnoreCase))
                {

                    Console.WriteLine("Set TFS Url: ");
                    configuration.BaseUri = Console.ReadLine();

                    Console.WriteLine("Set Project Name: ");
                    configuration.ProjectName = Console.ReadLine();

                    Console.WriteLine("Set user token: ");
                    configuration.UserToken = Console.ReadLine();

                    Console.WriteLine("Set Default Query Name: ");
                    configuration.DefaultQuery = Console.ReadLine();

                    Collector.saveConfig(configuration);

                    Console.WriteLine("Configuration updated.");
                    Console.WriteLine("New configuration:");

                    Console.WriteLine("TFS Url: " + configuration.BaseUri);
                    Console.WriteLine("Project Name: " + configuration.ProjectName);
                    Console.WriteLine("User token: " + configuration.UserToken);
                    Console.WriteLine("Default Query Name: " + configuration.DefaultQuery);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void createQuery(Dictionary<string, string> inputArgs)
        {
            string command = "";
            string response = "";
            try
            {
                Console.WriteLine("Query commmand: ");
                command = Console.ReadLine();
                if (!command.Equals(""))
                {
                    response = Collector.createQuery(inputArgs.GetValueOrDefault("QUERY"), command);
                    Console.WriteLine(response);
                }
                else
                {
                    Console.WriteLine("Must inform a query command.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void help()
        {
            Console.WriteLine("Update workitens:");
            Console.WriteLine(" --update [query name] (if query name ommited will execute the default query)");
            Console.WriteLine("     --csv <csvName> (optional generate CSV)");
            Console.WriteLine("");

            Console.WriteLine("Save all in csv:");
            Console.WriteLine(" --csv <csv file Name>");
            Console.WriteLine("");

            Console.WriteLine("Create or update a query in my queries folder:");
            Console.WriteLine(" --query <query Name>");
            Console.WriteLine("");

            Console.WriteLine("Open data base location:");
            Console.WriteLine(" --open");
            Console.WriteLine("");

            Console.WriteLine("Configure connection:");
            Console.WriteLine(" --config");
            Console.WriteLine("");

            Console.WriteLine(" Get current dir:");
            Console.WriteLine("     -env or --env");
            Console.WriteLine("");

            Console.WriteLine(" Program Version:");
            Console.WriteLine("     -v or --version");

            Console.WriteLine(" Help:");
            Console.WriteLine("     -h or --help");
        }

        private static void initializeDirs()
        {
            string auxPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            auxPath = Path.Combine(auxPath, "TFSCollector");
            if (!Directory.Exists(auxPath))
            {
                Directory.CreateDirectory(auxPath);
            }
            Collector.currentDirectory = Environment.CurrentDirectory;
            Collector.workDirectory = auxPath;
        }

    }
}
