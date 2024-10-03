using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackendApp.Migrations
{
    /// <inheritdoc />
    public partial class FinalMigrationHopefullyAgainaGAINAgainagainaginnnnn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    UserRole = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostFile",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Path = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostFile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegularUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    UserRole = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegularUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    UsersSentNotificationId = table.Column<long>(type: "bigint", nullable: false),
                    UsersReceivedNotificationId = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connections_RegularUsers_UsersReceivedNotificationId",
                        column: x => x.UsersReceivedNotificationId,
                        principalTable: "RegularUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Connections_RegularUsers_UsersSentNotificationId",
                        column: x => x.UsersSentNotificationId,
                        principalTable: "RegularUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SentId = table.Column<long>(type: "bigint", nullable: false),
                    ReceivedId = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_RegularUsers_ReceivedId",
                        column: x => x.ReceivedId,
                        principalTable: "RegularUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_RegularUsers_SentId",
                        column: x => x.SentId,
                        principalTable: "RegularUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostBase",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    PostedById = table.Column<long>(type: "bigint", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    JobTitle = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string[]>(type: "text[]", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    IsReply = table.Column<bool>(type: "boolean", nullable: true),
                    OriginalPost = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostBase_PostBase_OriginalPost",
                        column: x => x.OriginalPost,
                        principalTable: "PostBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostBase_RegularUsers_PostedById",
                        column: x => x.PostedById,
                        principalTable: "RegularUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegularUserHideableInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    PhoneNumberIsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    LocationIsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentPosition = table.Column<string>(type: "text", nullable: true),
                    CurrentPositionIsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Experience = table.Column<List<string>>(type: "text[]", nullable: false),
                    ExperienceIsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Capabilities = table.Column<List<string>>(type: "text[]", nullable: false),
                    CapabilitiesArePublic = table.Column<bool>(type: "boolean", nullable: false),
                    Education = table.Column<List<string>>(type: "text[]", nullable: false),
                    EducationIsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegularUserHideableInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegularUserHideableInfo_RegularUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RegularUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Read = table.Column<bool>(type: "boolean", nullable: false),
                    NotificationsIds = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NotificIds = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_PostBase_NotificIds",
                        column: x => x.NotificIds,
                        principalTable: "PostBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_RegularUsers_NotificationsIds",
                        column: x => x.NotificationsIds,
                        principalTable: "RegularUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostBasePostFile",
                columns: table => new
                {
                    PostFilesId = table.Column<long>(type: "bigint", nullable: false),
                    PostsUsedInId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostBasePostFile", x => new { x.PostFilesId, x.PostsUsedInId });
                    table.ForeignKey(
                        name: "FK_PostBasePostFile_PostBase_PostsUsedInId",
                        column: x => x.PostsUsedInId,
                        principalTable: "PostBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostBasePostFile_PostFile_PostFilesId",
                        column: x => x.PostFilesId,
                        principalTable: "PostFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostBaseRegularUser",
                columns: table => new
                {
                    InterestedUsersId = table.Column<long>(type: "bigint", nullable: false),
                    LikedPostsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostBaseRegularUser", x => new { x.InterestedUsersId, x.LikedPostsId });
                    table.ForeignKey(
                        name: "FK_PostBaseRegularUser_PostBase_LikedPostsId",
                        column: x => x.LikedPostsId,
                        principalTable: "PostBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostBaseRegularUser_RegularUsers_InterestedUsersId",
                        column: x => x.InterestedUsersId,
                        principalTable: "RegularUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_UsersReceivedNotificationId",
                table: "Connections",
                column: "UsersReceivedNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_UsersSentNotificationId",
                table: "Connections",
                column: "UsersSentNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceivedId",
                table: "Messages",
                column: "ReceivedId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentId",
                table: "Messages",
                column: "SentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificIds",
                table: "Notifications",
                column: "NotificIds");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationsIds",
                table: "Notifications",
                column: "NotificationsIds");

            migrationBuilder.CreateIndex(
                name: "IX_PostBase_OriginalPost",
                table: "PostBase",
                column: "OriginalPost");

            migrationBuilder.CreateIndex(
                name: "IX_PostBase_PostedById",
                table: "PostBase",
                column: "PostedById");

            migrationBuilder.CreateIndex(
                name: "IX_PostBasePostFile_PostsUsedInId",
                table: "PostBasePostFile",
                column: "PostsUsedInId");

            migrationBuilder.CreateIndex(
                name: "IX_PostBaseRegularUser_LikedPostsId",
                table: "PostBaseRegularUser",
                column: "LikedPostsId");

            migrationBuilder.CreateIndex(
                name: "IX_RegularUserHideableInfo_UserId",
                table: "RegularUserHideableInfo",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PostBasePostFile");

            migrationBuilder.DropTable(
                name: "PostBaseRegularUser");

            migrationBuilder.DropTable(
                name: "RegularUserHideableInfo");

            migrationBuilder.DropTable(
                name: "PostFile");

            migrationBuilder.DropTable(
                name: "PostBase");

            migrationBuilder.DropTable(
                name: "RegularUsers");
        }
    }
}
