using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace TFSCollector.Model
{


    class WorkItemContext : DbContext
    {
        private string connectionPath = "";

        public WorkItemContext(string _connectionPath)
        {
            connectionPath = _connectionPath;
        }
        public DbSet<WorkItemLocal> WorkItens { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + connectionPath);
        }
    }

    class WorkItemLocal
    {

        #region constructors
        public WorkItemLocal()
        {
            Id = 0;
            Rev = 0;
            Url = "";
            AssignedTo = "";
            Tags = "";
            AreaPath = "";
            TeamProject = "";
            IterationPath = "";
            WorkItemType = "";
            State = "";
            Reason = "";
            CreatedDate = DateTime.Parse("1900-01-01");
            CreatedBy = "";
            ChangedDate = DateTime.Parse("1900-01-01");
            ChangedBy = "";
            ClosedDate = DateTime.Parse("1900-01-01");
            ClosedBy = "";
            Title = "";
            BoardColumn = "";
            BoardColumnDone = false;
            Severity = "";
            StateChangeDate = DateTime.Parse("1900-01-01");
            Priority = 0;
            ValueArea = "";
            SystemInfo = "";
            ReproSteps = "";
        }
        #endregion

        #region methods

        #endregion

        #region properties
        public long Id { get; set; }
        public int Rev { get; set; }
        public string Url { get; set; }
        public string AssignedTo { get; set; }
        public string Tags { get; set; }
        public string AreaPath { get; set; }
        public string TeamProject { get; set; }
        public string IterationPath { get; set; }
        public string WorkItemType { get; set; }
        public string State { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ChangedDate { get; set; }
        public string ChangedBy { get; set; }
        public string Title { get; set; }
        public string BoardColumn { get; set; }
        public bool BoardColumnDone { get; set; }
        public string Severity { get; set; }
        public DateTime StateChangeDate { get; set; }
        public int Priority { get; set; }
        public string ValueArea { get; set; }
        public string SystemInfo { get; set; }
        public string ReproSteps { get; set; }
        public DateTime ClosedDate { get; set; }
        public string ClosedBy { get; set; }

        public override string ToString()
        {
            try
            {
                return Id + ";" +
                       Rev + ";" +
                       AssignedTo.Replace(";", "") + ";" +
                       AreaPath.Replace(";", "") + ";" +
                       TeamProject.Replace(";", "") + ";" +
                       IterationPath.Replace(";", "") + ";" +
                       WorkItemType.Replace(";", "") + ";" +
                       State.Replace(";", "") + ";" +
                       Reason.Replace(";", "") + ";" +
                       CreatedDate.ToString() + ";" +
                       CreatedBy.Replace(";", "") + ";" +
                       ChangedDate.ToString() + ";" +
                       ChangedBy.Replace(";", "") + ";" +
                       ClosedDate.ToString() + ";" +
                       ClosedBy.Replace(";", "") + ";" +
                       Title.Replace(";", "") + ";" +
                       BoardColumn.Replace(";", "") + ";" +
                       BoardColumnDone + ";" +
                       Severity.Replace(";", "") + ";" +
                       StateChangeDate.ToString() + ";" +
                       Priority;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion
    }
}
