using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab4.Migrations
{
    public partial class AddNewsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    NewsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SportClubId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.NewsId);
                    table.ForeignKey(
                        name: "FK_News_SportClub_SportClubId",
                        column: x => x.SportClubId,
                        principalTable: "SportClub",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_News_SportClubId",
                table: "News",
                column: "SportClubId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "News");
        }
    }
}
