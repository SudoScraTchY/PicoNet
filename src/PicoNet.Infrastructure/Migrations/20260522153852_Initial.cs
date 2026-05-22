using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace PicoNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortenedUrls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NanoId = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    OriginalUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    CustomAlias = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsPermanent = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxClicks = table.Column<int>(type: "integer", nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClickCount = table.Column<long>(type: "bigint", nullable: false),
                    LastAccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Campaign = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AllowedDomains = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true, computedColumnSql: "to_tsvector('english', coalesce(\"OriginalUrl\", '') || ' ' || coalesce(\"CustomAlias\", '') || ' ' || coalesce(\"Tags\", ''))", stored: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortenedUrls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UrlVisits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShortenedUrlId = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Referrer = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VisitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrlVisits_ShortenedUrls_ShortenedUrlId",
                        column: x => x.ShortenedUrlId,
                        principalTable: "ShortenedUrls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_shortened_urls_created_at",
                table: "ShortenedUrls",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_shortened_urls_custom_alias",
                table: "ShortenedUrls",
                column: "CustomAlias",
                unique: true,
                filter: "\"CustomAlias\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_shortened_urls_expiry_time",
                table: "ShortenedUrls",
                column: "ExpiryTime");

            migrationBuilder.CreateIndex(
                name: "IX_shortened_urls_nano_id",
                table: "ShortenedUrls",
                column: "NanoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shortened_urls_search",
                table: "ShortenedUrls",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_shortened_urls_status",
                table: "ShortenedUrls",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_shortened_urls_user_id",
                table: "ShortenedUrls",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_shortened_urls_user_status_deleted",
                table: "ShortenedUrls",
                columns: new[] { "UserId", "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_url_visits_country",
                table: "UrlVisits",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_url_visits_shortened_url_id",
                table: "UrlVisits",
                column: "ShortenedUrlId");

            migrationBuilder.CreateIndex(
                name: "IX_url_visits_url_visited",
                table: "UrlVisits",
                columns: new[] { "ShortenedUrlId", "VisitedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_url_visits_visited_at",
                table: "UrlVisits",
                column: "VisitedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlVisits");

            migrationBuilder.DropTable(
                name: "ShortenedUrls");
        }
    }
}
