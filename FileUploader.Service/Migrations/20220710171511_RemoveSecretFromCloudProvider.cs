using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileUploader.Service.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSecretFromCloudProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageAccountConnectionString",
                table: "AzureCloudProviders");

            migrationBuilder.DropColumn(
                name: "AccessKeyId",
                table: "AwsCloudProviders");

            migrationBuilder.DropColumn(
                name: "SecretAccessKey",
                table: "AwsCloudProviders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageAccountConnectionString",
                table: "AzureCloudProviders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccessKeyId",
                table: "AwsCloudProviders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecretAccessKey",
                table: "AwsCloudProviders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
