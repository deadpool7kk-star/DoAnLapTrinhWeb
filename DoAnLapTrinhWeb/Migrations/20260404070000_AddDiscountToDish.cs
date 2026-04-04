using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLapTrinhWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountToDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'DiscountPercentage' and Object_ID = Object_ID(N'Dishes'))
                  BEGIN
                      ALTER TABLE Dishes ADD DiscountPercentage int NULL;
                  END"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'DiscountPercentage' and Object_ID = Object_ID(N'Dishes'))
                  BEGIN
                      ALTER TABLE Dishes DROP COLUMN DiscountPercentage;
                  END"
            );
        }
    }
}
