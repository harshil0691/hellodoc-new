using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using hellodoc.DbEntity.DataModels;

namespace hellodoc.DbEntity.DataContext;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminRegion> AdminRegions { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

    public virtual DbSet<BlockRequest> BlockRequests { get; set; }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<CaseTag> CaseTags { get; set; }

    public virtual DbSet<Concierge> Concierges { get; set; }

    public virtual DbSet<EmailLog> EmailLogs { get; set; }

    public virtual DbSet<Encounter> Encounters { get; set; }

    public virtual DbSet<HealthProfessional> HealthProfessionals { get; set; }

    public virtual DbSet<HealthProfessionalType> HealthProfessionalTypes { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<MedicalHistory> MedicalHistories { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<NotificationMessage> NotificationMessages { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PatientDocument> PatientDocuments { get; set; }

    public virtual DbSet<Physician> Physicians { get; set; }

    public virtual DbSet<PhysicianLocation> PhysicianLocations { get; set; }

    public virtual DbSet<PhysicianNotification> PhysicianNotifications { get; set; }

    public virtual DbSet<PhysicianRegion> PhysicianRegions { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestBusiness> RequestBusinesses { get; set; }

    public virtual DbSet<RequestClient> RequestClients { get; set; }

    public virtual DbSet<RequestClosed> RequestCloseds { get; set; }

    public virtual DbSet<RequestConcierge> RequestConcierges { get; set; }

    public virtual DbSet<RequestNote> RequestNotes { get; set; }

    public virtual DbSet<RequestStatusLog> RequestStatusLogs { get; set; }

    public virtual DbSet<RequestType> RequestTypes { get; set; }

    public virtual DbSet<RequestWiseFile> RequestWiseFiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleMenu> RoleMenus { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<ShiftDetail> ShiftDetails { get; set; }

    public virtual DbSet<ShiftDetailRegion> ShiftDetailRegions { get; set; }

    public virtual DbSet<Smslog> Smslogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=123;Server=localhost;Port=5432;Database=hellodoc;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Adminid).HasName("Admin_pkey");

            entity.Property(e => e.Adminid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.Admins)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_admin_aspnetuser");

            entity.HasOne(d => d.Region).WithMany(p => p.Admins).HasConstraintName("fk_admin_region");

            entity.HasOne(d => d.Role).WithMany(p => p.Admins).HasConstraintName("fk_admin_role");
        });

        modelBuilder.Entity<AdminRegion>(entity =>
        {
            entity.HasKey(e => e.Adminregionid).HasName("AdminRegion_pkey");

            entity.Property(e => e.Adminregionid).ValueGeneratedNever();

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_adminregion_admin");

            entity.HasOne(d => d.Region).WithMany(p => p.AdminRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_adminregion_region");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetRoles_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetUsers_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<AspNetUserRole>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("AspNetUserRoles_pkey");

            entity.Property(e => e.Userid).ValueGeneratedNever();

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetUserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aspnetuserrole_role");

            entity.HasOne(d => d.User).WithOne(p => p.AspNetUserRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aspnetuserrole_aspnetuser");
        });

        modelBuilder.Entity<BlockRequest>(entity =>
        {
            entity.HasKey(e => e.Blockrequestid).HasName("BlockRequests_pkey");

            entity.Property(e => e.Blockrequestid).ValueGeneratedNever();

            entity.HasOne(d => d.Request).WithMany(p => p.BlockRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_blockrequest_request");
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Businessid).HasName("Business_pkey");

            entity.Property(e => e.Businessid).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<CaseTag>(entity =>
        {
            entity.HasKey(e => e.Casetagid).HasName("CaseTag_pkey");

            entity.Property(e => e.Casetagid).ValueGeneratedNever();
        });

        modelBuilder.Entity<Concierge>(entity =>
        {
            entity.HasKey(e => e.Conciergeid).HasName("Concierge_pkey");

            entity.Property(e => e.Conciergeid).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(e => e.Emaillogid).HasName("EmailLog_pkey");

            entity.Property(e => e.Emaillogid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Admin).WithMany(p => p.EmailLogs).HasConstraintName("fk_emaillog_admin");

            entity.HasOne(d => d.Physician).WithMany(p => p.EmailLogs).HasConstraintName("fk_emaillog_physician");

            entity.HasOne(d => d.Request).WithMany(p => p.EmailLogs).HasConstraintName("fk_emaillog_request");

            entity.HasOne(d => d.Role).WithMany(p => p.EmailLogs).HasConstraintName("fk_emaillog_asprole");
        });

        modelBuilder.Entity<Encounter>(entity =>
        {
            entity.HasKey(e => e.Encounterid).HasName("Encounter_pkey");

            entity.Property(e => e.Encounterid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Request).WithMany(p => p.Encounters).HasConstraintName("fk_encounter_request");
        });

        modelBuilder.Entity<HealthProfessional>(entity =>
        {
            entity.HasKey(e => e.Vendorid).HasName("HealthProfessionals_pkey");

            entity.Property(e => e.Vendorid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.ProfessionNavigation).WithMany(p => p.HealthProfessionals).HasConstraintName("fk_healthprofession_professiontype");
        });

        modelBuilder.Entity<HealthProfessionalType>(entity =>
        {
            entity.HasKey(e => e.Healthprofessionalid).HasName("HealthProfessionalType_pkey");

            entity.Property(e => e.Healthprofessionalid).ValueGeneratedNever();
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => new { e.LUsername, e.LRole, e.LPassward, e.LEmail }).HasName("login_pkey");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Menuid).HasName("Menu_pkey");

            entity.Property(e => e.Menuid).ValueGeneratedNever();
        });

        modelBuilder.Entity<NotificationMessage>(entity =>
        {
            entity.HasKey(e => e.Notificationid).HasName("pk_notificationid");

            entity.Property(e => e.Notificationid).ValueGeneratedNever();

            entity.HasOne(d => d.Admin).WithMany(p => p.NotificationMessages).HasConstraintName("fk_notification_admin");

            entity.HasOne(d => d.Aspetuser).WithMany(p => p.NotificationMessages).HasConstraintName("fk_notification_aspnetuser");

            entity.HasOne(d => d.Physician).WithMany(p => p.NotificationMessages).HasConstraintName("fk_notification_physician");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OrderDetails_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Request).WithMany(p => p.OrderDetails).HasConstraintName("fk_order_request");

            entity.HasOne(d => d.Vendor).WithMany(p => p.OrderDetails).HasConstraintName("fk_order_healthprofessionals");
        });

        modelBuilder.Entity<PatientDocument>(entity =>
        {
            entity.ToView("patient_documents");
        });

        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(e => e.Physicianid).HasName("Physician_pkey");

            entity.Property(e => e.Physicianid).ValueGeneratedNever();

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.Physicians).HasConstraintName("fk_physician_aspnetuser");

            entity.HasOne(d => d.Region).WithMany(p => p.Physicians).HasConstraintName("fk_physician_region");

            entity.HasOne(d => d.Role).WithMany(p => p.Physicians).HasConstraintName("fk_physician_role");
        });

        modelBuilder.Entity<PhysicianLocation>(entity =>
        {
            entity.HasKey(e => e.Locationid).HasName("PhysicianLocation_pkey");

            entity.Property(e => e.Locationid).ValueGeneratedNever();

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianLocations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physicianlocation_physician");
        });

        modelBuilder.Entity<PhysicianNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PhysicianNotification_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianNotifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physiciannotification_physician");
        });

        modelBuilder.Entity<PhysicianRegion>(entity =>
        {
            entity.HasKey(e => e.Physicianregionid).HasName("PhysicianRegion_pkey");

            entity.Property(e => e.Physicianregionid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physicianregion_physician");

            entity.HasOne(d => d.Region).WithMany(p => p.PhysicianRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physicianregion_region");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Regionid).HasName("Region_pkey");

            entity.Property(e => e.Regionid).ValueGeneratedNever();
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Requestid).HasName("Request_pkey");

            entity.Property(e => e.Requestid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.CasetagNavigation).WithMany(p => p.Requests).HasConstraintName("fk_request_casetag");

            entity.HasOne(d => d.Physician).WithMany(p => p.Requests).HasConstraintName("fk_request_physician");

            entity.HasOne(d => d.User).WithMany(p => p.Requests).HasConstraintName("fk_request_user");
        });

        modelBuilder.Entity<RequestBusiness>(entity =>
        {
            entity.HasKey(e => e.Requestbusinessid).HasName("RequestBusiness_pkey");

            entity.Property(e => e.Requestbusinessid).ValueGeneratedNever();

            entity.HasOne(d => d.Business).WithMany(p => p.RequestBusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_requestbusiness_business");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestBusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_requestbusiness_request");
        });

        modelBuilder.Entity<RequestClient>(entity =>
        {
            entity.HasKey(e => e.Requestclientid).HasName("RequestClient_pkey");

            entity.Property(e => e.Requestclientid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Request).WithMany(p => p.RequestClients).HasConstraintName("fk_requestclient_request");
        });

        modelBuilder.Entity<RequestClosed>(entity =>
        {
            entity.HasKey(e => e.Requestclosedid).HasName("RequestClosed_pkey");

            entity.Property(e => e.Requestclosedid).ValueGeneratedNever();
        });

        modelBuilder.Entity<RequestConcierge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RequestConcierge_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Concierge).WithMany(p => p.RequestConcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_requestconcierge_concierge");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestConcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_requestconcierge_request");
        });

        modelBuilder.Entity<RequestNote>(entity =>
        {
            entity.HasKey(e => e.Requestnotesid).HasName("RequestNotes_pkey");

            entity.Property(e => e.Requestnotesid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Request).WithMany(p => p.RequestNotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_requestnote_request");
        });

        modelBuilder.Entity<RequestStatusLog>(entity =>
        {
            entity.HasKey(e => e.Requeststatuslogid).HasName("RequestStatusLog_pkey");

            entity.Property(e => e.Requeststatuslogid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Admin).WithMany(p => p.RequestStatusLogs).HasConstraintName("fk_rlog_admin");

            entity.HasOne(d => d.Physician).WithMany(p => p.RequestStatusLogs).HasConstraintName("fk_requeststatuslog_physician");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestStatusLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_requeststatuslog_request");
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.HasKey(e => e.Requesttypeid).HasName("RequestType_pkey");

            entity.Property(e => e.Requesttypeid).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<RequestWiseFile>(entity =>
        {
            entity.HasKey(e => e.Requestwisefileid).HasName("RequestWiseFile_pkey");

            entity.Property(e => e.Requestwisefileid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Request).WithMany(p => p.RequestWiseFiles).HasConstraintName("fk_requestwisefile_request");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("Role_pkey");

            entity.Property(e => e.Roleid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.AccounttypeNavigation).WithMany(p => p.Roles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_role_aspnetrole");
        });

        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.HasKey(e => e.Rolemenuid).HasName("RoleMenu_pkey");

            entity.Property(e => e.Rolemenuid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rolemenu_menu");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rolemenu_role");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Shiftid).HasName("Shift_pkey");

            entity.Property(e => e.Shiftid)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.Weekdays).IsFixedLength();

            entity.HasOne(d => d.Physician).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_shift_physician");
        });

        modelBuilder.Entity<ShiftDetail>(entity =>
        {
            entity.HasKey(e => e.Shiftdetailid).HasName("ShiftDetail_pkey");

            entity.Property(e => e.Shiftdetailid)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Region).WithMany(p => p.ShiftDetails).HasConstraintName("fk_shiftdetail_region");

            entity.HasOne(d => d.Shift).WithMany(p => p.ShiftDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_shiftdetail_shift");
        });

        modelBuilder.Entity<ShiftDetailRegion>(entity =>
        {
            entity.HasKey(e => e.Shiftdetailregionid).HasName("ShiftDetailRegion_pkey");

            entity.Property(e => e.Shiftdetailregionid).ValueGeneratedNever();
        });

        modelBuilder.Entity<Smslog>(entity =>
        {
            entity.HasKey(e => e.Smslogid).HasName("SMSLog_pkey");

            entity.Property(e => e.Smslogid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Admin).WithMany(p => p.Smslogs).HasConstraintName("fk_smslog_admin");

            entity.HasOne(d => d.Physician).WithMany(p => p.Smslogs).HasConstraintName("fk_smslog_physician");

            entity.HasOne(d => d.Request).WithMany(p => p.Smslogs).HasConstraintName("fk_smslog_request");

            entity.HasOne(d => d.Role).WithMany(p => p.Smslogs).HasConstraintName("fk_smslog_role");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("User_pkey");

            entity.Property(e => e.Userid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.Users).HasConstraintName("fk_user_aspnetuser");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
