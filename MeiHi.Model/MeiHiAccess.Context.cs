﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeiHi.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MeiHiEntities : DbContext
    {
        public MeiHiEntities()
            : base("name=MeiHiEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Add> Add { get; set; }
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<AdminPermission> AdminPermission { get; set; }
        public virtual DbSet<AdminRole> AdminRole { get; set; }
        public virtual DbSet<ApplyJoin> ApplyJoin { get; set; }
        public virtual DbSet<Booking> Booking { get; set; }
        public virtual DbSet<HairStyle> HairStyle { get; set; }
        public virtual DbSet<HairStyleType> HairStyleType { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<ProductBrand> ProductBrand { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<RecommandShop> RecommandShop { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RolePermission> RolePermission { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<ServiceType> ServiceType { get; set; }
        public virtual DbSet<Shop> Shop { get; set; }
        public virtual DbSet<ShopBrandImages> ShopBrandImages { get; set; }
        public virtual DbSet<ShopUser> ShopUser { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserComments> UserComments { get; set; }
        public virtual DbSet<UserCommentSharedImg> UserCommentSharedImg { get; set; }
        public virtual DbSet<UserCommentsReply> UserCommentsReply { get; set; }
        public virtual DbSet<UserFavorites> UserFavorites { get; set; }
        public virtual DbSet<UserSuggest> UserSuggest { get; set; }
    }
}
