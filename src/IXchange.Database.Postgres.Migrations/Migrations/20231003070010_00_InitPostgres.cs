#nullable disable

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class _00_InitPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Information_Name = table.Column<string>(type: "text", nullable: false),
                    Information_Description = table.Column<string>(type: "text", nullable: false),
                    Information_CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Information_UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyType = table.Column<int>(type: "integer", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dataconverter",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeSnippet = table.Column<string>(type: "text", nullable: false),
                    Displayname = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dataconverter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    BlobName = table.Column<string>(type: "text", nullable: false),
                    PublicLink = table.Column<string>(type: "text", nullable: false),
                    AdditionalData = table.Column<string>(type: "text", nullable: false),
                    Bytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyGlobalConfig",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GlobalConfigType = table.Column<int>(type: "integer", nullable: false),
                    Information_Name = table.Column<string>(type: "text", nullable: false),
                    Information_Description = table.Column<string>(type: "text", nullable: false),
                    Information_CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Information_UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdditionalConfiguration = table.Column<string>(type: "text", nullable: false),
                    ConfigVersion = table.Column<long>(type: "bigint", nullable: false),
                    TblCompanyId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyGlobalConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyGlobalConfig_Company_TblCompanyId",
                        column: x => x.TblCompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gateway",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Information_Name = table.Column<string>(type: "text", nullable: false),
                    Information_Description = table.Column<string>(type: "text", nullable: false),
                    Information_CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Information_UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeviceCommon_FirmwareversionDevice = table.Column<string>(type: "text", nullable: false),
                    DeviceCommon_FirmwareversionService = table.Column<string>(type: "text", nullable: false),
                    DeviceCommon_ConfigversionDevice = table.Column<long>(type: "bigint", nullable: false),
                    DeviceCommon_ConfigversionService = table.Column<long>(type: "bigint", nullable: false),
                    DeviceCommon_State = table.Column<int>(type: "integer", nullable: false),
                    DeviceCommon_LastOnlineTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeviceCommon_LastOfflineTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeviceCommon_Secret = table.Column<string>(type: "text", nullable: false),
                    Position_Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Position_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Position_Altitude = table.Column<double>(type: "double precision", nullable: false),
                    Position_Precision = table.Column<double>(type: "double precision", nullable: false),
                    Position_TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Position_Source = table.Column<int>(type: "integer", nullable: false),
                    AdditionalConfiguration = table.Column<string>(type: "text", nullable: false),
                    AdditionalProperties = table.Column<string>(type: "text", nullable: false),
                    TblCompanyId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gateway", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gateway_Company_TblCompanyId",
                        column: x => x.TblCompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Information_Name = table.Column<string>(type: "text", nullable: false),
                    Information_Description = table.Column<string>(type: "text", nullable: false),
                    Information_CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Information_UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdditionalProperties = table.Column<string>(type: "text", nullable: false),
                    TblCompanyId = table.Column<long>(type: "bigint", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_Company_TblCompanyId",
                        column: x => x.TblCompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementDefinitionTemplate",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Information_Name = table.Column<string>(type: "text", nullable: false),
                    Information_Description = table.Column<string>(type: "text", nullable: false),
                    Information_CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Information_UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValueType = table.Column<int>(type: "integer", nullable: false),
                    TblDataconverterId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementDefinitionTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementDefinitionTemplate_Dataconverter_TblDataconverte~",
                        column: x => x.TblDataconverterId,
                        principalTable: "Dataconverter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    AgbVersion = table.Column<string>(type: "text", nullable: false),
                    LoginName = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    JwtToken = table.Column<string>(type: "text", nullable: false),
                    Locked = table.Column<bool>(type: "boolean", nullable: false),
                    LoginConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DefaultLanguage = table.Column<string>(type: "text", nullable: false),
                    ConfirmationToken = table.Column<string>(type: "text", nullable: false),
                    PushTags = table.Column<string>(type: "text", nullable: true),
                    Setting10MinPush = table.Column<bool>(type: "boolean", nullable: false),
                    TblUserImageId = table.Column<long>(type: "bigint", nullable: true),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_File_TblUserImageId",
                        column: x => x.TblUserImageId,
                        principalTable: "File",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IotDevice",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Information_Name = table.Column<string>(type: "text", nullable: false),
                    Information_Description = table.Column<string>(type: "text", nullable: false),
                    Information_CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Information_UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FallbackPosition_Longitude = table.Column<double>(type: "double precision", nullable: false),
                    FallbackPosition_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    FallbackPosition_Altitude = table.Column<double>(type: "double precision", nullable: false),
                    FallbackPosition_Precision = table.Column<double>(type: "double precision", nullable: false),
                    FallbackPosition_TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FallbackPosition_Source = table.Column<int>(type: "integer", nullable: false),
                    DeviceCommon_FirmwareversionDevice = table.Column<string>(type: "text", nullable: false),
                    DeviceCommon_FirmwareversionService = table.Column<string>(type: "text", nullable: false),
                    DeviceCommon_ConfigversionDevice = table.Column<long>(type: "bigint", nullable: false),
                    DeviceCommon_ConfigversionService = table.Column<long>(type: "bigint", nullable: false),
                    DeviceCommon_State = table.Column<int>(type: "integer", nullable: false),
                    DeviceCommon_LastOnlineTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeviceCommon_LastOfflineTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeviceCommon_Secret = table.Column<string>(type: "text", nullable: false),
                    Upstream = table.Column<int>(type: "integer", nullable: false),
                    Plattform = table.Column<int>(type: "integer", nullable: false),
                    TransmissionType = table.Column<int>(type: "integer", nullable: false),
                    TransmissionInterval = table.Column<int>(type: "integer", nullable: false),
                    MeasurementInterval = table.Column<int>(type: "integer", nullable: false),
                    AdditionalConfiguration = table.Column<string>(type: "text", nullable: false),
                    AdditionalProperties = table.Column<string>(type: "text", nullable: false),
                    SuccessfullyRegisteredInThirdPartySystem = table.Column<bool>(type: "boolean", nullable: false),
                    TblGatewayId = table.Column<long>(type: "bigint", nullable: true),
                    TblDataconverterId = table.Column<long>(type: "bigint", nullable: true),
                    TblCompanyGlobalConfigId = table.Column<long>(type: "bigint", nullable: true),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IotDevice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IotDevice_CompanyGlobalConfig_TblCompanyGlobalConfigId",
                        column: x => x.TblCompanyGlobalConfigId,
                        principalTable: "CompanyGlobalConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IotDevice_Dataconverter_TblDataconverterId",
                        column: x => x.TblDataconverterId,
                        principalTable: "Dataconverter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IotDevice_Gateway_TblGatewayId",
                        column: x => x.TblGatewayId,
                        principalTable: "Gateway",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccessToken",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    GuiltyUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TblUserId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessToken_User_TblUserId",
                        column: x => x.TblUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceHardwareId = table.Column<string>(type: "text", nullable: false),
                    Plattform = table.Column<int>(type: "integer", nullable: false),
                    DeviceIdiom = table.Column<int>(type: "integer", nullable: false),
                    OperatingSystemVersion = table.Column<string>(type: "text", nullable: false),
                    DeviceType = table.Column<string>(type: "text", nullable: false),
                    DeviceName = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: false),
                    DeviceToken = table.Column<string>(type: "text", nullable: false),
                    AppVersion = table.Column<string>(type: "text", nullable: false),
                    LastDateTimeUtcOnline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAppRunning = table.Column<bool>(type: "boolean", nullable: false),
                    ScreenResolution = table.Column<string>(type: "text", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TblUserId = table.Column<long>(type: "bigint", nullable: true),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Device_User_TblUserId",
                        column: x => x.TblUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserRight = table.Column<int>(type: "integer", nullable: false),
                    UserRole = table.Column<int>(type: "integer", nullable: false),
                    AdditionalConfiguration = table.Column<string>(type: "text", nullable: false),
                    AdditionalProperties = table.Column<string>(type: "text", nullable: false),
                    TblUserId = table.Column<long>(type: "bigint", nullable: false),
                    TblCompanyId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_Company_TblCompanyId",
                        column: x => x.TblCompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permission_User_TblUserId",
                        column: x => x.TblUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblIncomeOutputs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ixies = table.Column<int>(type: "integer", nullable: false),
                    IncomeOutput = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TblUserId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblIncomeOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblIncomeOutputs_User_TblUserId",
                        column: x => x.TblUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementDefinition",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Information_Name = table.Column<string>(type: "text", nullable: false),
                    Information_Description = table.Column<string>(type: "text", nullable: false),
                    Information_CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Information_UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdditionalConfiguration = table.Column<string>(type: "text", nullable: false),
                    AdditionalProperties = table.Column<string>(type: "text", nullable: false),
                    MeasurementInterval = table.Column<int>(type: "integer", nullable: false),
                    ValueType = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DownstreamType = table.Column<int>(type: "integer", nullable: false),
                    TblIotDeviceId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementDefinition_IotDevice_TblIotDeviceId",
                        column: x => x.TblIotDeviceId,
                        principalTable: "IotDevice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementDefinitionToProjectAssignment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TblMeasurementDefinitionId = table.Column<long>(type: "bigint", nullable: true),
                    TblProjctId = table.Column<long>(type: "bigint", nullable: true),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementDefinitionToProjectAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementDefinitionToProjectAssignment_MeasurementDefinit~",
                        column: x => x.TblMeasurementDefinitionId,
                        principalTable: "MeasurementDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MeasurementDefinitionToProjectAssignment_Project_TblProjctId",
                        column: x => x.TblProjctId,
                        principalTable: "Project",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MeasurementResult",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location_Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Location_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Location_Altitude = table.Column<double>(type: "double precision", nullable: false),
                    Location_Precision = table.Column<double>(type: "double precision", nullable: false),
                    Location_TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location_Source = table.Column<int>(type: "integer", nullable: false),
                    SpatialPoint = table.Column<Point>(type: "geometry", nullable: false),
                    ValueType = table.Column<int>(type: "integer", nullable: false),
                    Value_Text = table.Column<string>(type: "text", nullable: false),
                    Value_Number = table.Column<double>(type: "double precision", nullable: true),
                    Value_Binary = table.Column<byte[]>(type: "bytea", nullable: true),
                    Value_Bit = table.Column<bool>(type: "boolean", nullable: true),
                    AdditionalConfiguration = table.Column<string>(type: "text", nullable: false),
                    AdditionalProperties = table.Column<string>(type: "text", nullable: false),
                    TblMeasurementDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementResult_MeasurementDefinition_TblMeasurementDefin~",
                        column: x => x.TblMeasurementDefinitionId,
                        principalTable: "MeasurementDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblAbos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovingAverageNotify = table.Column<bool>(type: "boolean", nullable: false),
                    MovingAverageNotifyValue = table.Column<float>(type: "real", nullable: false),
                    ExceedNotify = table.Column<bool>(type: "boolean", nullable: false),
                    ExceedNotifyValue = table.Column<float>(type: "real", nullable: false),
                    UndercutNotify = table.Column<bool>(type: "boolean", nullable: false),
                    UndercutNotifyValue = table.Column<float>(type: "real", nullable: false),
                    FailureForMinutesNotify = table.Column<bool>(type: "boolean", nullable: false),
                    FailureForMinutesNotifyValue = table.Column<float>(type: "real", nullable: false),
                    TblUserId = table.Column<long>(type: "bigint", nullable: false),
                    TblMeasurementDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAbos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblAbos_MeasurementDefinition_TblMeasurementDefinitionId",
                        column: x => x.TblMeasurementDefinitionId,
                        principalTable: "MeasurementDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAbos_User_TblUserId",
                        column: x => x.TblUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TblMeasurementDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    NotificationType = table.Column<int>(type: "integer", nullable: false),
                    TblUserId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblNotifications_MeasurementDefinition_TblMeasurementDefini~",
                        column: x => x.TblMeasurementDefinitionId,
                        principalTable: "MeasurementDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblNotifications_User_TblUserId",
                        column: x => x.TblUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblRatings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TblUserId = table.Column<long>(type: "bigint", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TblMeasurementDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    DataVersion = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblRatings_MeasurementDefinition_TblMeasurementDefinitionId",
                        column: x => x.TblMeasurementDefinitionId,
                        principalTable: "MeasurementDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblRatings_User_TblUserId",
                        column: x => x.TblUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessToken_TblUserId",
                table: "AccessToken",
                column: "TblUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyGlobalConfig_TblCompanyId",
                table: "CompanyGlobalConfig",
                column: "TblCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_TblUserId",
                table: "Device",
                column: "TblUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Gateway_TblCompanyId",
                table: "Gateway",
                column: "TblCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IotDevice_TblCompanyGlobalConfigId",
                table: "IotDevice",
                column: "TblCompanyGlobalConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_IotDevice_TblDataconverterId",
                table: "IotDevice",
                column: "TblDataconverterId");

            migrationBuilder.CreateIndex(
                name: "IX_IotDevice_TblGatewayId",
                table: "IotDevice",
                column: "TblGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinition_TblIotDeviceId",
                table: "MeasurementDefinition",
                column: "TblIotDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinitionTemplate_TblDataconverterId",
                table: "MeasurementDefinitionTemplate",
                column: "TblDataconverterId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinitionToProjectAssignment_TblMeasurementDefi~",
                table: "MeasurementDefinitionToProjectAssignment",
                column: "TblMeasurementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinitionToProjectAssignment_TblProjctId",
                table: "MeasurementDefinitionToProjectAssignment",
                column: "TblProjctId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementResult_TblMeasurementDefinitionId",
                table: "MeasurementResult",
                column: "TblMeasurementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_TblCompanyId",
                table: "Permission",
                column: "TblCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_TblUserId",
                table: "Permission",
                column: "TblUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_TblCompanyId",
                table: "Project",
                column: "TblCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAbos_TblMeasurementDefinitionId",
                table: "TblAbos",
                column: "TblMeasurementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAbos_TblUserId",
                table: "TblAbos",
                column: "TblUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblIncomeOutputs_TblUserId",
                table: "TblIncomeOutputs",
                column: "TblUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblNotifications_TblMeasurementDefinitionId",
                table: "TblNotifications",
                column: "TblMeasurementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblNotifications_TblUserId",
                table: "TblNotifications",
                column: "TblUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblRatings_TblMeasurementDefinitionId",
                table: "TblRatings",
                column: "TblMeasurementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblRatings_TblUserId",
                table: "TblRatings",
                column: "TblUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_TblUserImageId",
                table: "User",
                column: "TblUserImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessToken");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "MeasurementDefinitionTemplate");

            migrationBuilder.DropTable(
                name: "MeasurementDefinitionToProjectAssignment");

            migrationBuilder.DropTable(
                name: "MeasurementResult");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "TblAbos");

            migrationBuilder.DropTable(
                name: "TblIncomeOutputs");

            migrationBuilder.DropTable(
                name: "TblNotifications");

            migrationBuilder.DropTable(
                name: "TblRatings");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "MeasurementDefinition");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "IotDevice");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "CompanyGlobalConfig");

            migrationBuilder.DropTable(
                name: "Dataconverter");

            migrationBuilder.DropTable(
                name: "Gateway");

            migrationBuilder.DropTable(
                name: "Company");
        }
    }
}
