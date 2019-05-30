using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TFSCollector.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkItens",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Rev = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    AssignedTo = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    AreaPath = table.Column<string>(nullable: true),
                    TeamProject = table.Column<string>(nullable: true),
                    IterationPath = table.Column<string>(nullable: true),
                    WorkItemType = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ChangedDate = table.Column<DateTime>(nullable: false),
                    ChangedBy = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    BoardColumn = table.Column<string>(nullable: true),
                    BoardColumnDone = table.Column<bool>(nullable: false),
                    Severity = table.Column<string>(nullable: true),
                    StateChangeDate = table.Column<DateTime>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    ValueArea = table.Column<string>(nullable: true),
                    SystemInfo = table.Column<string>(nullable: true),
                    ReproSteps = table.Column<string>(nullable: true),
                    ClosedDate = table.Column<DateTime>(nullable: false),
                    ClosedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItens", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkItens");
        }
    }
}
