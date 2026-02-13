using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ApplySnakeCaseNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginAttempts_Users_UserId",
                table: "LoginAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoginAttempts",
                table: "LoginAttempts");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "LoginAttempts",
                newName: "login_attempts");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "MiddleName",
                table: "users",
                newName: "middle_name");

            migrationBuilder.RenameColumn(
                name: "LockoutEnd",
                table: "users",
                newName: "lockout_end");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "users",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "LastLoginAt",
                table: "users",
                newName: "last_login_at");

            migrationBuilder.RenameColumn(
                name: "IsEmailVerified",
                table: "users",
                newName: "is_email_verified");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "users",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "FailedLoginAttempts",
                table: "users",
                newName: "failed_login_attempts");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "users",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "users",
                newName: "ix_users_email");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "refresh_tokens",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refresh_tokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UserAgent",
                table: "refresh_tokens",
                newName: "user_agent");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "refresh_tokens",
                newName: "revoked_at");

            migrationBuilder.RenameColumn(
                name: "ReplacedByToken",
                table: "refresh_tokens",
                newName: "replaced_by_token");

            migrationBuilder.RenameColumn(
                name: "IsRevoked",
                table: "refresh_tokens",
                newName: "is_revoked");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "refresh_tokens",
                newName: "ip_address");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "refresh_tokens",
                newName: "expiry_date");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "refresh_tokens",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenId",
                table: "refresh_tokens",
                newName: "refresh_token_id");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_Token",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_token");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "login_attempts",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UserAgent",
                table: "login_attempts",
                newName: "user_agent");

            migrationBuilder.RenameColumn(
                name: "IsSuccessful",
                table: "login_attempts",
                newName: "is_successful");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "login_attempts",
                newName: "ip_address");

            migrationBuilder.RenameColumn(
                name: "FailureReason",
                table: "login_attempts",
                newName: "failure_reason");

            migrationBuilder.RenameColumn(
                name: "AttemptedAt",
                table: "login_attempts",
                newName: "attempted_at");

            migrationBuilder.RenameColumn(
                name: "LoginAttemptId",
                table: "login_attempts",
                newName: "login_attempt_id");

            migrationBuilder.RenameIndex(
                name: "IX_LoginAttempts_UserId",
                table: "login_attempts",
                newName: "ix_login_attempts_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens",
                column: "refresh_token_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_login_attempts",
                table: "login_attempts",
                column: "login_attempt_id");

            migrationBuilder.AddForeignKey(
                name: "fk_login_attempts_users_user_id",
                table: "login_attempts",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "refresh_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_login_attempts_users_user_id",
                table: "login_attempts");

            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_login_attempts",
                table: "login_attempts");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "login_attempts",
                newName: "LoginAttempts");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "middle_name",
                table: "Users",
                newName: "MiddleName");

            migrationBuilder.RenameColumn(
                name: "lockout_end",
                table: "Users",
                newName: "LockoutEnd");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "last_login_at",
                table: "Users",
                newName: "LastLoginAt");

            migrationBuilder.RenameColumn(
                name: "is_email_verified",
                table: "Users",
                newName: "IsEmailVerified");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "Users",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "failed_login_attempts",
                table: "Users",
                newName: "FailedLoginAttempts");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_users_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "RefreshTokens",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "RefreshTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "user_agent",
                table: "RefreshTokens",
                newName: "UserAgent");

            migrationBuilder.RenameColumn(
                name: "revoked_at",
                table: "RefreshTokens",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "replaced_by_token",
                table: "RefreshTokens",
                newName: "ReplacedByToken");

            migrationBuilder.RenameColumn(
                name: "is_revoked",
                table: "RefreshTokens",
                newName: "IsRevoked");

            migrationBuilder.RenameColumn(
                name: "ip_address",
                table: "RefreshTokens",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "expiry_date",
                table: "RefreshTokens",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "RefreshTokens",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "refresh_token_id",
                table: "RefreshTokens",
                newName: "RefreshTokenId");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_user_id",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_token",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_Token");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "LoginAttempts",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "user_agent",
                table: "LoginAttempts",
                newName: "UserAgent");

            migrationBuilder.RenameColumn(
                name: "is_successful",
                table: "LoginAttempts",
                newName: "IsSuccessful");

            migrationBuilder.RenameColumn(
                name: "ip_address",
                table: "LoginAttempts",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "failure_reason",
                table: "LoginAttempts",
                newName: "FailureReason");

            migrationBuilder.RenameColumn(
                name: "attempted_at",
                table: "LoginAttempts",
                newName: "AttemptedAt");

            migrationBuilder.RenameColumn(
                name: "login_attempt_id",
                table: "LoginAttempts",
                newName: "LoginAttemptId");

            migrationBuilder.RenameIndex(
                name: "ix_login_attempts_user_id",
                table: "LoginAttempts",
                newName: "IX_LoginAttempts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "RefreshTokenId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoginAttempts",
                table: "LoginAttempts",
                column: "LoginAttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoginAttempts_Users_UserId",
                table: "LoginAttempts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
