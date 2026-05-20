using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations;
    /// <inheritdoc />
public partial class AddCardDeckIndex : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Cards_DeckId",
            table: "Cards");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Cards",
            type: "nvarchar(4000)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldMaxLength: 10000,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Cards_DeckId_RecallDate",
            table: "Cards",
            columns: new[] { "DeckId", "RecallDate" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Cards_DeckId_RecallDate",
            table: "Cards");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Cards",
            type: "nvarchar(max)",
            maxLength: 10000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(4000)",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Cards_DeckId",
            table: "Cards",
            column: "DeckId");
    }
}
