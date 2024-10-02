﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Models;

namespace NewsAPI.Data;

public partial class NewsWebDbContext : DbContext
{
    public NewsWebDbContext()
    {
    }

    public NewsWebDbContext(DbContextOptions<NewsWebDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPost> UserPosts { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Category");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3213E83FB771C144");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CommentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CommentText).HasColumnType("text");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Post");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Images__3213E83FD2630EA5");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Caption).HasMaxLength(200);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Post).WithMany(p => p.Images)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ImagePost");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logs__3213E83F2B70C6F2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Details).HasMaxLength(500);
            entity.Property(e => e.EntityId).HasColumnName("EntityID");
            entity.Property(e => e.EntityType).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Posts__3213E83F18446A96");

            entity.HasIndex(e => e.Slug, "UQ__Posts__BC7B5FB6ECF19B45").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Contents).HasColumnType("text");
            entity.Property(e => e.Slug).HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Category");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83FF72FA76F");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4F6058E2F").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534BCAEBBB9").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(250);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPost__3213E83FA6B08632");

            entity.ToTable("UserPost");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.PublishedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Post).WithMany(p => p.UserPosts)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Posts");

            entity.HasOne(d => d.User).WithMany(p => p.UserPosts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Users");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserProf__3213E83F429C11CE");

            entity.ToTable("UserProfile");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("AvatarURL");
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.SocialLinks).HasMaxLength(1000);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
