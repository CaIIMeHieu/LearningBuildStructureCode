using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations;
    /// <inheritdoc />
public partial class AddUserProfileAndBadges : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserProfiles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TimeZoneId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "UTC"),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                StreakCurrent = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                StreakLongest = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                LastReviewedDate = table.Column<DateOnly>(type: "date", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserProfiles", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserProfiles_AspNetUsers_Id",
                    column: x => x.Id,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserBadges",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                BadgeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                EarnedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserBadges", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserBadges_UserProfiles_UserId",
                    column: x => x.UserId,
                    principalTable: "UserProfiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserBadges_UserId",
            table: "UserBadges",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_UserBadges_UserId_BadgeType",
            table: "UserBadges",
            columns: new[] { "UserId", "BadgeType" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserBadges");

        migrationBuilder.DropTable(
            name: "UserProfiles");
    }
}
