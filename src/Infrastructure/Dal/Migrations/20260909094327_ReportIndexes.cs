using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomRental.Dal.Migrations
{
    /// <inheritdoc />
    public partial class ReportIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Отчёты фильтруют брони по статусу и дате. Индекс объявлен здесь, а не в
            // конфигурации, потому что EF не строит индексы по свойствам комплексного типа.
            migrationBuilder.Sql(
                "CREATE INDEX ix_bookings_status_date ON bookings (status, date);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX ix_bookings_status_date;");
        }
    }
}
