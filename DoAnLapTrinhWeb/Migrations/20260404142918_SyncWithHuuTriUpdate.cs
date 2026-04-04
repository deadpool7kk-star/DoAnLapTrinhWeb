using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLapTrinhWeb.Migrations
{
    /// <inheritdoc />
    public partial class SyncWithHuuTriUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[Dishes]') 
                    AND name = 'ImageUrl'
                )
                BEGIN
                    ALTER TABLE [Dishes] ADD [ImageUrl] nvarchar(255) NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[Dishes]') 
                    AND name = 'ImageUrl'
                )
                BEGIN
                    ALTER TABLE [Dishes] DROP COLUMN [ImageUrl];
                END
            ");
        }
    }
}
