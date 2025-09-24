using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace webBanSach.Models;

public partial class WebBanSachContext : DbContext
{
    public WebBanSachContext()
    {
    }

    public WebBanSachContext(DbContextOptions<WebBanSachContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CT_DonHang> CT_DonHangs { get; set; }
    public virtual DbSet<DanhGia> DanhGias { get; set; }
    public virtual DbSet<DonHang> DonHangs { get; set; }
    public virtual DbSet<GioHang> GioHangs { get; set; }
    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }
    public virtual DbSet<LienHe> LienHes { get; set; }
    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
    public virtual DbSet<NhaXuatBan> NhaXuatBans { get; set; }
    public virtual DbSet<Sach> Saches { get; set; }
    public virtual DbSet<TacGia> TacGias { get; set; }
    public virtual DbSet<TheLoai> TheLoais { get; set; }
    public virtual DbSet<Sach_TheLoai> Sach_TheLoais { get; set; }
    public virtual DbSet<Sach_TacGia> Sach_TacGias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning Để bảo mật connection string, bạn nên đưa nó ra file cấu hình (appsettings.json).
        => optionsBuilder.UseSqlServer("Server=DESKTOP-GQIR9Q2;Database=webBanSach;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CT_DonHang>(entity =>
        {
            entity.HasKey(e => new { e.MaDH, e.MaSach }).HasName("PK__CT_DonHa__EC06D123723AB0E9");

            entity.ToTable("CT_DonHang", tb => tb.HasTrigger("trg_UpdateTongTien"));

            entity.Property(e => e.ThanhTien).HasComputedColumnSql("([SoLuong]*[DonGia])", true);

            entity.HasOne(d => d.MaDHNavigation).WithMany(p => p.CT_DonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_DonHang__MaDH__7C4F7684");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.CT_DonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_DonHan__MaSac__7D439ABD");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasKey(e => e.MaDG).HasName("PK__DanhGia__27258660DE0DA3BE");

            entity.Property(e => e.NgayDG).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNDNavigation).WithMany(p => p.DanhGias)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaND__02FC7413");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.DanhGias)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaSach__02084FDA");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDH).HasName("PK__DonHang__27258661E4103BED");

            entity.Property(e => e.NgayDat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasDefaultValue(0m);
            entity.Property(e => e.TrangThai).HasDefaultValue("Chờ xử lý");

            entity.HasOne(d => d.MaKMNavigation).WithMany(p => p.DonHangs)
                .HasConstraintName("FK__DonHang__MaKM__778AC167");

            entity.HasOne(d => d.MaNDNavigation).WithMany(p => p.DonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaND__76969D2E");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => new { e.MaND, e.MaSach }).HasName("PK__GioHang__EC06806634AF55D0");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNDNavigation).WithMany(p => p.GioHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaND__68487DD7");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.GioHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaSach__693CA210");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKM).HasName("PK__KhuyenMa__2725CF15CFA52012");

            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<LienHe>(entity =>
        {
            entity.HasKey(e => e.MaLH).HasName("PK__LienHe__2725C77FF6B5218F");

            entity.Property(e => e.NgayGui).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue("Chưa xử lý");

            entity.HasOne(d => d.MaNDNavigation).WithMany(p => p.LienHes)
                .HasConstraintName("FK__LienHe__MaND__08B54D69");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaND).HasName("PK__NguoiDun__2725D7244F14902E");

            entity.Property(e => e.LoaiNguoiDung).HasDefaultValue("User");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue("Hoạt động");
        });

        modelBuilder.Entity<NhaXuatBan>(entity =>
        {
            entity.HasKey(e => e.MaNXB).HasName("PK__NhaXuatB__3A19482CAB7CC228");
        });

        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasKey(e => e.MaSach).HasName("PK__Sach__B235742D34612BBA");

            entity.Property(e => e.SoLuong).HasDefaultValue(0);

            entity.HasOne(d => d.MaNXBNavigation).WithMany(p => p.Saches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sach__MaNXB__5441852A");
        });

        modelBuilder.Entity<TacGia>(entity =>
        {
            entity.HasKey(e => e.MaTG).HasName("PK__TacGia__2725007450B5226A");
        });

        modelBuilder.Entity<TheLoai>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__TheLoai__730A57593F5344DA");
        });

        modelBuilder.Entity<Sach_TheLoai>(entity =>
        {
            entity.HasKey(e => new { e.MaSach, e.MaLoai }).HasName("PK__Sach_TheLoai");

            entity.ToTable("Sach_TheLoai");

            entity.HasOne(d => d.MaSachNavigation)
                .WithMany(p => p.Sach_TheLoais)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.MaLoaiNavigation)
                .WithMany(p => p.Sach_TheLoais)
                .HasForeignKey(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Sach_TacGia>(entity =>
        {
            entity.HasKey(e => new { e.MaSach, e.MaTG }).HasName("PK__Sach_TacGia");

            entity.ToTable("Sach_TacGia");

            entity.HasOne(d => d.MaSachNavigation)
                .WithMany(p => p.Sach_TacGias)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.MaTGNavigation)
                .WithMany(p => p.Sach_TacGias)
                .HasForeignKey(d => d.MaTG)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
