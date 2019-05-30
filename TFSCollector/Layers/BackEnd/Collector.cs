using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TFSCollector.Model;

namespace TFSCollector.Layers.BackEnd
{
    class Collector
    {


        private const string CONS_DB_FILE = "TFS.db";
        private const string CONS_CONFIG_FILE = "config.json";
        public static string currentDirectory { get; set; }
        public static string workDirectory { get; set; }


        public static IARConfig LoadConfig()
        {

            StreamReader file;
            string configFile;
            IARConfig result;

            try
            {

                if (!Directory.Exists(workDirectory))
                {
                    Directory.CreateDirectory(workDirectory);
                }

                if (File.Exists(Path.Combine(workDirectory, CONS_CONFIG_FILE)))
                {
                    file = new StreamReader(Path.Combine(workDirectory, CONS_CONFIG_FILE));
                    configFile = file.ReadToEnd();
                    file.Close();
                    file.Dispose();
                    file = null;
                    result = JsonConvert.DeserializeObject<ARConfig>(configFile);
                }
                else
                {
                    result = new ARConfig();
                    result.BaseUri = "";
                    result.ProjectName = "";
                    result.UserToken = "";
                    result.LastUpdate = DateTime.Parse("1900-01-01T00:00:00");
                    saveConfig(result);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void saveConfig(IARConfig config)
        {
            StreamWriter file;
            string configFile;
            try
            {
                file = new StreamWriter(Path.Combine(workDirectory, CONS_CONFIG_FILE), false);
                configFile = JsonConvert.SerializeObject(config);
                file.Write(configFile);
                file.Close();
                file.Dispose();
                file = null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> update(string _filter, string _csvFile)
        {

            List<string> result = new List<string>();
            WorkItemLocal workItemLocal;
            DateTime auxDate = DateTime.Parse("1900-01-01");
            bool auxBoolean = false;
            int auxInt = 1;
            List<WorkItemLocal> allItens = new List<WorkItemLocal>();
            List<WorkItemLocal> returnedItens = new List<WorkItemLocal>();
            WorkItemContext db;
            IARConfig Configuration;
            QueryHierarchyItem newBugsQuery = null;
            WorkItemQueryResult queryResult;

            try
            {


                Configuration = LoadConfig();
                db = initializeDataBase();

                if (_filter.Equals(""))
                {
                    _filter = Configuration.DefaultQuery;
                }

                if (Configuration.ProjectName.Equals("")
                      || Configuration.BaseUri.Equals("")
                      || Configuration.UserToken.Equals("")
                      || _filter.Equals(""))
                {
                    throw new Exception("Check configuration: --config");
                }

                if (Configuration.LastUpdate.Year < 1900)
                {
                    Configuration.LastUpdate = DateTime.Parse("1900-01-01");
                }

                VssConnection connection = new VssConnection(new Uri(Configuration.BaseUri), new VssBasicCredential(string.Empty, Configuration.UserToken));
                WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
                List<QueryHierarchyItem> queryHierarchyItems = witClient.GetQueriesAsync(Configuration.ProjectName, depth: 2).Result;
                QueryHierarchyItem myQueriesFolder = queryHierarchyItems.FirstOrDefault(qhi => qhi.Name.Equals("My Queries"));


                if (myQueriesFolder != null)
                {
                    if (myQueriesFolder.Children != null)
                    {
                        newBugsQuery = myQueriesFolder.Children.FirstOrDefault(qhi => qhi.Name.Equals(_filter));
                    }

                    if (newBugsQuery == null)
                    {
                        throw new Exception("Query not found on TFS, please create " + _filter + " query on TFS portal under 'My Queries' folder");
                    }

                    queryResult = witClient.QueryByIdAsync(newBugsQuery.Id).Result;

                    if (queryResult.WorkItems.Any())
                    {
                        int skip = 0;
                        const int batchSize = 100;
                        IEnumerable<WorkItemReference> workItemRefs;
                        do
                        {
                            workItemRefs = queryResult.WorkItems.Skip(skip).Take(batchSize);
                            if (workItemRefs.Any())
                            {
                                List<WorkItem> workItems = witClient.GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id)).Result;
                                foreach (WorkItem workItem in workItems)
                                {

                                    try
                                    {

                                        workItemLocal = new WorkItemLocal();

                                        if (workItem.Id != null) workItemLocal.Id = workItem.Id.Value;
                                        if (workItem.Rev != null) workItemLocal.Rev = workItem.Rev.Value;

                                        workItemLocal.AssignedTo = workItem.Fields.GetValueOrDefault("System.AssignedTo", "").ToString();
                                        workItemLocal.Tags = workItem.Fields.GetValueOrDefault("System.Tags", "").ToString();
                                        workItemLocal.AreaPath = workItem.Fields.GetValueOrDefault("System.AreaPath", "").ToString();
                                        workItemLocal.TeamProject = workItem.Fields.GetValueOrDefault("System.TeamProject", "").ToString();
                                        workItemLocal.IterationPath = workItem.Fields.GetValueOrDefault("System.IterationPath", "").ToString();
                                        workItemLocal.WorkItemType = workItem.Fields.GetValueOrDefault("System.WorkItemType", "").ToString();
                                        workItemLocal.State = workItem.Fields.GetValueOrDefault("System.State", "").ToString();
                                        workItemLocal.Reason = workItem.Fields.GetValueOrDefault("System.Reason", "").ToString();

                                        auxDate = DateTime.Parse("1900-01-01");
                                        DateTime.TryParse(workItem.Fields.GetValueOrDefault("System.CreatedDate", "1900-01-01").ToString(), out auxDate);
                                        workItemLocal.CreatedDate = auxDate;

                                        workItemLocal.CreatedBy = workItem.Fields.GetValueOrDefault("System.CreatedBy", "").ToString();

                                        auxDate = DateTime.Parse("1900-01-01");
                                        DateTime.TryParse(workItem.Fields.GetValueOrDefault("System.ChangedDate", "1900-01-01").ToString(), out auxDate);
                                        workItemLocal.ChangedDate = auxDate;

                                        workItemLocal.ChangedBy = workItem.Fields.GetValueOrDefault("System.ChangedBy", "").ToString();

                                        auxDate = DateTime.Parse("1900-01-01");
                                        DateTime.TryParse(workItem.Fields.GetValueOrDefault("System.ClosedDate", "1900-01-01").ToString(), out auxDate);
                                        workItemLocal.ClosedDate = auxDate;

                                        workItemLocal.ClosedBy = workItem.Fields.GetValueOrDefault("System.ClosedBy", "").ToString();
                                        workItemLocal.ChangedBy = workItem.Fields.GetValueOrDefault("System.ChangedBy", "").ToString();
                                        workItemLocal.Title = workItem.Fields.GetValueOrDefault("System.Title", "").ToString();
                                        workItemLocal.BoardColumn = workItem.Fields.GetValueOrDefault("System.BoardColumn", "").ToString();


                                        auxBoolean = false;
                                        bool.TryParse(workItem.Fields.GetValueOrDefault("System.BoardColumnDone", "").ToString(), out auxBoolean);
                                        workItemLocal.BoardColumnDone = auxBoolean;

                                        workItemLocal.Severity = workItem.Fields.GetValueOrDefault("Microsoft.VSTS.Common.Severity", "").ToString();

                                        auxDate = DateTime.Parse("1900-01-01");
                                        DateTime.TryParse(workItem.Fields.GetValueOrDefault("Microsoft.VSTS.Common.StateChangeDate", "1900-01-01").ToString(), out auxDate);
                                        workItemLocal.StateChangeDate = auxDate;

                                        auxInt = 0;
                                        int.TryParse(workItem.Fields.GetValueOrDefault("Microsoft.VSTS.Common.Priority", "").ToString(), out auxInt);
                                        workItemLocal.Priority = auxInt;

                                        workItemLocal.ValueArea = workItem.Fields.GetValueOrDefault("Microsoft.VSTS.Common.ValueArea", "").ToString();
                                        workItemLocal.SystemInfo = workItem.Fields.GetValueOrDefault("Microsoft.VSTS.TCM.SystemInfo", "").ToString();
                                        workItemLocal.ReproSteps = workItem.Fields.GetValueOrDefault("Microsoft.VSTS.TCM.ReproSteps", "").ToString();

                                        workItemLocal.Url = Configuration.BaseUri + workItemLocal.IterationPath + "/_queries?id=" + workItemLocal.Id;

                                        returnedItens.Add(workItemLocal);

                                        if (db.WorkItens.Find(workItemLocal.Id) != null)
                                        {
                                            db.WorkItens.Update(workItemLocal);
                                        }
                                        else
                                        {
                                            db.WorkItens.Add(workItemLocal);
                                        }

                                        result.Add("Workitem found: " + workItem.Id);

                                    }
                                    catch (Exception e)
                                    {
                                        result.Add("Error: " + e.Message);
                                    }

                                }
                            }
                            skip += batchSize;
                        }
                        while (workItemRefs.Count() == batchSize);

                        db.SaveChanges();

                        if (!_csvFile.Equals("") && returnedItens.Count > 0)
                        {
                            result.Add(saveCSV(returnedItens, _csvFile));
                        }

                    }
                    else
                    {
                        Console.WriteLine("No work items were returned from query.");
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string createQuery(string _name, string _command)
        {
            string result = "";
            QueryHierarchyItem myQueriesFolder;
            QueryHierarchyItem newBugsQuery = null;
            VssConnection connection;
            WorkItemTrackingHttpClient witClient;
            IARConfig configuration;
            List<QueryHierarchyItem> queryHierarchyItems;

            try
            {

                configuration = LoadConfig();

                if (configuration.ProjectName.Equals("")
                    || configuration.BaseUri.Equals("")
                    || configuration.UserToken.Equals(""))
                {
                    throw new Exception("Check configuration: --config");
                }

                connection = new VssConnection(new Uri(configuration.BaseUri), new VssCredentials());
                witClient = connection.GetClient<WorkItemTrackingHttpClient>();
                queryHierarchyItems = witClient.GetQueriesAsync(configuration.ProjectName, depth: 2).Result;                

                myQueriesFolder = queryHierarchyItems.FirstOrDefault(qhi => qhi.Name.Equals("My Queries"));
                if (myQueriesFolder != null)
                {
                    if (myQueriesFolder.Children != null)
                    {
                        newBugsQuery = myQueriesFolder.Children.FirstOrDefault(qhi => qhi.Name.Equals(_name));
                    }

                    if (newBugsQuery == null)
                    {
                        newBugsQuery = new QueryHierarchyItem()
                        {
                            Name = _name,
                            Wiql = _command,
                            IsFolder = false
                        };
                        newBugsQuery = witClient.CreateQueryAsync(newBugsQuery, configuration.ProjectName, myQueriesFolder.Name).Result;
                        result = "Query " + _name + " created";
                    }
                    else
                    {
                        newBugsQuery = new QueryHierarchyItem()
                        {
                            Name = _name,
                            Wiql = _command,
                            IsFolder = false
                        };
                        newBugsQuery = witClient.UpdateQueryAsync(newBugsQuery, configuration.ProjectName, myQueriesFolder.Name).Result;
                        result = "Query " + _name + " updated";
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string saveAllCsv(string _csvFile)
        {
            WorkItemContext db;
            List<WorkItemLocal> workitens;

            try
            {
                db = initializeDataBase();
                workitens = db.WorkItens.ToList();
                return saveCSV(workitens, _csvFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        private static string saveCSV(List<WorkItemLocal> returnedItens, string _csvFile)
        {
            string result = "";
            StreamWriter file;

            try
            {

                file = new StreamWriter(_csvFile, false, System.Text.Encoding.UTF8);
                file.WriteLine("Id;Rev;AssignedTo;AreaPath;TeamProject;IterationPath;WorkItemType;State;Reason;CreatedDate;" +
                               "CreatedBy;ChangedDate;ChangedBy;ClosedDate;ClosedBy;Title;BoardColumn;BoardColumnDone;Severity;" +
                               "StateChangeDate;Priority");
                returnedItens.ForEach(issue => file.WriteLine(issue.ToString()));
                file.Close();
                file.Dispose();
                file = null;
                result = "File saved: " + _csvFile;

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static WorkItemContext initializeDataBase()
        {
            string command = "";
            try
            {
                WorkItemContext db = new WorkItemContext(Path.Combine(workDirectory, CONS_DB_FILE));
                command = "CREATE TABLE IF NOT EXISTS 'workitens' (\n";
                command += "'Id' INTEGER NOT NULL,\n";
                command += "'Rev' INTEGER NOT NULL,\n";
                command += "'Url' TEXT NOT NULL,\n";
                command += "'AssignedTo' TEXT NOT NULL,\n";
                command += "'Tags' TEXT NOT NULL,\n";
                command += "'AreaPath' TEXT NOT NULL,\n";
                command += "'TeamProject' TEXT NOT NULL,\n";
                command += "'IterationPath' TEXT NOT NULL,\n";
                command += "'WorkItemType' TEXT NOT NULL,\n";
                command += "'State' TEXT NOT NULL,\n";
                command += "'Reason' TEXT NOT NULL,\n";
                command += "'CreatedDate' TEXT NOT NULL,\n";
                command += "'CreatedBy' TEXT NOT NULL,\n";
                command += "'ChangedDate' TEXT NOT NULL,\n";
                command += "'ChangedBy' TEXT NOT NULL,\n";
                command += "'ClosedDate' TEXT NOT NULL,\n";
                command += "'ClosedBy' TEXT NOT NULL,\n";
                command += "'Title' TEXT NOT NULL,\n";
                command += "'BoardColumn' TEXT NOT NULL,\n";
                command += "'BoardColumnDone' TEXT NOT NULL,\n";
                command += "'Severity' TEXT NOT NULL,\n";
                command += "'StateChangeDate' TEXT NOT NULL,\n";
                command += "'Priority' INTEGER NOT NULL,\n";
                command += "'ValueArea' TEXT NOT NULL,\n";
                command += "'SystemInfo' TEXT NOT NULL,\n";
                command += "'ReproSteps' TEXT NOT NULL,\n";
                command += "PRIMARY KEY('id')\n";
                command += ");\n";
                db.Database.ExecuteSqlCommand(command);
                db.SaveChanges();
                return db;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
