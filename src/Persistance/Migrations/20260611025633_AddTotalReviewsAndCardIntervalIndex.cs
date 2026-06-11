using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations;
    /// <inheritdoc />
public partial class AddTotalReviewsAndCardIntervalIndex : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "TotalReviews",
            table: "UserProfiles",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<Guid>(
            name: "OwnerId",
            table: "ReviewLogs",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex(
            name: "IX_Cards_OwnerId_Interval",
            table: "Cards",
            columns: new[] { "OwnerId", "Interval" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Cards_OwnerId_Interval",
            table: "Cards");

        migrationBuilder.DropColumn(
            name: "TotalReviews",
            table: "UserProfiles");

        migrationBuilder.DropColumn(
            name: "OwnerId",
            table: "ReviewLogs");
    }
}
