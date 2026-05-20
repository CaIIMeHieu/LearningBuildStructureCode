using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations;
/// <inheritdoc />
public partial class AddCardAndReviewLogTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Cards",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DeckId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Question = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Answer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true),
                RecallDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Interval = table.Column<int>(type: "int", nullable: false),
                EaseFactor = table.Column<double>(type: "float(4)", precision: 4, scale: 2, nullable: false),
                Repetitions = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Cards", x => x.Id);
                table.ForeignKey(
                    name: "FK_Cards_Decks_DeckId",
                    column: x => x.DeckId,
                    principalTable: "Decks",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ReviewLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Quality = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReviewLogs", x => x.Id);
                table.ForeignKey(
                    name: "FK_ReviewLogs_Cards_CardId",
                    column: x => x.CardId,
                    principalTable: "Cards",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Cards_DeckId",
            table: "Cards",
            column: "DeckId");

        migrationBuilder.CreateIndex(
            name: "IX_ReviewLogs_CardId",
            table: "ReviewLogs",
            column: "CardId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ReviewLogs");

        migrationBuilder.DropTable(
            name: "Cards");
    }
}
