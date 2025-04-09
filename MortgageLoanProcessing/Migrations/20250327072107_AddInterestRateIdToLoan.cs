using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MortgageLoanProcessing.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestRateIdToLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InterestRateId",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestRateId",
                table: "Loans");
        }
    }
}
