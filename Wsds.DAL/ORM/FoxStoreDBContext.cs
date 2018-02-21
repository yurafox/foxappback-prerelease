using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Oracle.ManagedDataAccess.EntityFramework;
using Wsds.DAL.Entities;

namespace Wsds.DAL.ORM
{

    public class CodeConfig : DbConfiguration
    {
        public CodeConfig()
        {
            SetProviderServices("Oracle.ManagedDataAccess.Client",
                EFOracleProviderServices.Instance);
            SetProviderFactory("Oracle.ManagedDataAccess.Client",
                Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);
            //SetDefaultConnectionFactory(new OracleConnectionFactory());
        }
    }

    [DbConfigurationType(typeof(CodeConfig))] 
    public partial class FoxStoreDBContext : DbContext
    {
        public FoxStoreDBContext(string ConnString) : base(ConnString)
        { }

        public virtual DbSet<Action_Offer> ACTION_OFFERS { get; set; }
        public virtual DbSet<Action> ACTIONS { get; set; }
        public virtual DbSet<Client_Order> CLIENT_ORDERS { get; set; }
        public virtual DbSet<Client_Prop_Value> CLIENT_PROP_VALUES { get; set; }
        public virtual DbSet<Client> CLIENTS { get; set; }
        public virtual DbSet<Credit_Entity> CREDIT_ENTITIES { get; set; }
        public virtual DbSet<Credit_Product> CREDIT_PRODUCTS { get; set; }
        public virtual DbSet<Currency> CURRENCIES { get; set; }
        public virtual DbSet<Currency_Rate> CURRENCY_RATES { get; set; }
        public virtual DbSet<Delivery_Option> DELIVERY_OPTIONS { get; set; }
        public virtual DbSet<Entity> ENTITIES { get; set; }
        public virtual DbSet<Enum_Order_Payment_Status> ENUM_ORDER_PAYMENT_STATUS { get; set; }
        public virtual DbSet<Enum_Order_Status> ENUM_ORDER_STATUS { get; set; }
        public virtual DbSet<Enum_Payment_Method> ENUM_PAYMENT_METHODS { get; set; }
        public virtual DbSet<Enum_Prop_Types> ENUM_PROP_TYPES { get; set; }
        public virtual DbSet<Func_Area> FUNC_AREAS { get; set; }
        public virtual DbSet<Locale> LOCALES { get; set; }
        public virtual DbSet<Localization> LOCALIZATIONs { get; set; }
        public virtual DbSet<Manufacturer> MANUFACTURERS { get; set; }
        public virtual DbSet<Offer> OFFERS { get; set; }
        public virtual DbSet<Order_Spec_Product> ORDER_SPEC_PRODUCTS { get; set; }
        public virtual DbSet<Order_Spec_Service> ORDER_SPEC_SERVICES { get; set; }
        public virtual DbSet<Product_Cat> PRODUCT_CATS { get; set; }
        public virtual DbSet<Product_File> PRODUCT_FILES { get; set; }
        public virtual DbSet<Product_Group> PRODUCT_GROUPS { get; set; }
        public virtual DbSet<Product_Prop_Value> PRODUCT_PROP_VALUES { get; set; }
        public virtual DbSet<Product_Template> PRODUCT_TEMPLATE { get; set; }
        public virtual DbSet<Product_Template_Func_Area> PRODUCT_TEMPLATE_FUNC_AREA { get; set; }
        public virtual DbSet<Product_View_History> PRODUCT_VIEW_HISTORY { get; set; }
        public virtual DbSet<Product> PRODUCTS { get; set; }
        public virtual DbSet<Products_In_Group> PRODUCTS_IN_GROUPS { get; set; }
        public virtual DbSet<Prop_Enums_List> PROP_ENUMS_LISTS { get; set; }
        public virtual DbSet<Prop> PROPS { get; set; }
        public virtual DbSet<Quotation> QUOTATIONS { get; set; }
        public virtual DbSet<Service_Group> SERVICE_GROUPS { get; set; }
        public virtual DbSet<Service_Quotation> SERVICE_QUOTATIONS { get; set; }
        public virtual DbSet<Service> SERVICES { get; set; }
        public virtual DbSet<Settlement> SETTLEMENTS { get; set; }
        public virtual DbSet<Supplier> SUPPLIERS { get; set; }
        public virtual DbSet<Variant_Item> VARIANT_ITEMS { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<NavigationPropertyNameForeignKeyDiscoveryConvention>();
            modelBuilder.Entity<Action>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Action>()
                .HasMany(e => e.ACTION_OFFERS)
                .WithRequired(e => e.ACTION)
                .HasForeignKey(e => e.ID_ACTION)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client_Order>()
                .HasMany(e => e.ORDER_SPEC_PRODUCTS)
                .WithRequired(e => e.CLIENT_ORDERS)
                .HasForeignKey(e => e.ID_ORDER)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client_Order>()
                .HasMany(e => e.ORDER_SPEC_SERVICES)
                .WithRequired(e => e.CLIENT_ORDERS)
                .HasForeignKey(e => e.ID_ORDER)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client_Prop_Value>()
                .Property(e => e.PROP_VALUE)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.CLIENT_ORDERS)
                .WithRequired(e => e.CLIENT)
                .HasForeignKey(e => e.ID_CLIENT)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.CLIENT_PROP_VALUES)
                .WithRequired(e => e.CLIENT)
                .HasForeignKey(e => e.ID_CLIENT)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.OFFERS)
                .WithRequired(e => e.CLIENT)
                .HasForeignKey(e => e.ID_CLIENT)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.PRODUCT_VIEW_HISTORY)
                .WithRequired(e => e.CLIENT)
                .HasForeignKey(e => e.ID_CLIENT)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Credit_Entity>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Credit_Entity>()
                .HasMany(e => e.CREDIT_PRODUCTS)
                .WithRequired(e => e.CreditEntity)
                .HasForeignKey(e => e.ID_CREDIT_ENTITY)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Credit_Product>()
                .Property(e => e.PCT_RATE)
                .HasPrecision(18, 5);

            modelBuilder.Entity<Currency>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Currency>()
                .Property(e => e.SHORT_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.ACTION_OFFERS)
                .WithRequired(e => e.CURRENCY)
                .HasForeignKey(e => e.ID_CUR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.CLIENT_ORDERS)
                .WithRequired(e => e.CURRENCY)
                .HasForeignKey(e => e.CUR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.CURRENCY_RATES)
                .WithRequired(e => e.Currency)
                .HasForeignKey(e => e.CUR1)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.CURRENCY_RATES1)
                .WithRequired(e => e.Currency1)
                .HasForeignKey(e => e.CUR2)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.QUOTATIONS)
                .WithRequired(e => e.CURRENCY)
                .HasForeignKey(e => e.ID_CUR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.SERVICE_QUOTATIONS)
                .WithRequired(e => e.CURRENCY)
                .HasForeignKey(e => e.ID_CUR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.SETTLEMENTS)
                .WithRequired(e => e.CURRENCY)
                .HasForeignKey(e => e.CUR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency_Rate>()
                .Property(e => e.RATE)
                .HasPrecision(18, 5);

            modelBuilder.Entity<Delivery_Option>()
                .Property(e => e.COLUMN1)
                .IsUnicode(false);

            modelBuilder.Entity<Entity>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Entity>()
                .Property(e => e.TABLE_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Entity>()
                .HasMany(e => e.LOCALIZATIONS)
                .WithRequired(e => e.Entity)
                .HasForeignKey(e => e.ENTITY_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Enum_Order_Payment_Status>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Enum_Order_Status>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Enum_Order_Status>()
                .HasMany(e => e.CLIENT_ORDERS)
                .WithOptional(e => e.ENUM_ORDER_STATUS)
                .HasForeignKey(e => e.STATUS);

            modelBuilder.Entity<Enum_Payment_Method>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Enum_Payment_Method>()
                .HasMany(e => e.CLIENT_ORDERS)
                .WithRequired(e => e.ENUM_PAYMENT_METHODS)
                .HasForeignKey(e => e.ID_PAYMENT_METHOD)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Enum_Payment_Method>()
                .HasMany(e => e.SUPPLIERS)
                .WithRequired(e => e.EnumPaymentMethod)
                .HasForeignKey(e => e.PAYMENT_METHOD)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Enum_Prop_Types>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Enum_Prop_Types>()
                .HasMany(e => e.CLIENT_PROP_VALUES)
                .WithRequired(e => e.ENUM_PROP_TYPES)
                .HasForeignKey(e => e.ID_PROP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Enum_Prop_Types>()
                .HasMany(e => e.PROPS)
                .WithOptional(e => e.ENUM_PROP_TYPES)
                .HasForeignKey(e => e.PROP_TYPE);

            modelBuilder.Entity<Func_Area>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Func_Area>()
                .HasMany(e => e.PRODUCT_TEMPLATE_FUNC_AREA)
                .WithRequired(e => e.FuncArea)
                .HasForeignKey(e => e.ID_SYSTEM)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Locale>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Locale>()
                .HasMany(e => e.LOCALIZATIONS)
                .WithRequired(e => e.LOCALE)
                .HasForeignKey(e => e.ID_LOCALE)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Localization>()
                .Property(e => e.COLUMN_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Localization>()
                .Property(e => e.LOCALE_VALUE)
                .IsUnicode(false);

            modelBuilder.Entity<Manufacturer>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Manufacturer>()
                .Property(e => e.URL)
                .IsUnicode(false);

            //modelBuilder.Entity<Manufacturer>()
            //    .HasMany(e => e.PRODUCTS)
            //    .WithRequired(e => e.MANUFACTURER)
            //    .HasForeignKey(e => e.ID_MANUFACTURER)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order_Spec_Product>()
                .Property(e => e.QTY)
                .HasPrecision(18, 5);

            modelBuilder.Entity<Order_Spec_Service>()
                .Property(e => e.QTY)
                .HasPrecision(18, 5);

            modelBuilder.Entity<Product_Cat>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Product_Cat>()
                .HasMany(e => e.PRODUCT_GROUPS)
                .WithRequired(e => e.ProductCat)
                .HasForeignKey(e => e.ID_PRODUCT_CAT)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product_Group>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Product_Group>()
                .Property(e => e.PREFIX)
                .IsUnicode(false);

            modelBuilder.Entity<Product_Group>()
                .HasMany(e => e.PRODUCT_TEMPLATE)
                .WithRequired(e => e.ProductGroup)
                .HasForeignKey(e => e.ID_PRODUCT_GROUP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product_Group>()
                .HasMany(e => e.PRODUCTS_IN_GROUPS)
                .WithRequired(e => e.ProductGroup)
                .HasForeignKey(e => e.ID_GROUP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product_Prop_Value>()
                .Property(e => e.PROP_VALUE_STR)
                .IsUnicode(false);

            modelBuilder.Entity<Product_Prop_Value>()
                .Property(e => e.PROP_VALUE_NUMBER)
                .HasPrecision(18, 5);

            modelBuilder.Entity<Product_Prop_Value>()
                .Property(e => e.PROP_VALUE_LONG)
                .IsUnicode(false);

            modelBuilder.Entity<Product_Template>()
                .HasMany(e => e.PRODUCT_TEMPLATE_FUNC_AREA)
                .WithRequired(e => e.PRODUCT_TEMPLATE)
                .HasForeignKey(e => e.ID_PRODUCT_TEMPLATE)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.URL)
                .IsUnicode(false);

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.PRODUCT_FILES)
            //    .WithRequired(e => e.PRODUCT)
            //    .HasForeignKey(e => e.ID_PRODUCT)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.PRODUCT_PROP_VALUES)
                .WithRequired(e => e.PRODUCT)
                .HasForeignKey(e => e.ID_PRODUCT)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.PRODUCT_VIEW_HISTORY)
            //    .WithRequired(e => e.PRODUCT)
            //    .HasForeignKey(e => e.ID_PRODUCT)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.PRODUCTS_IN_GROUPS)
                .WithRequired(e => e.PRODUCT)
                .HasForeignKey(e => e.ID_PRODUCT)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.QUOTATIONS)
            //    .WithRequired(e => e.PRODUCT)
            //    .HasForeignKey(e => e.ID_PRODUCT)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prop_Enums_List>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Prop_Enums_List>()
                .Property(e => e.URL)
                .IsUnicode(false);

            modelBuilder.Entity<Prop_Enums_List>()
                .HasMany(e => e.PRODUCT_PROP_VALUES)
                .WithOptional(e => e.PropEnumsList)
                .HasForeignKey(e => e.PROP_VALUE_ENUM);

            modelBuilder.Entity<Prop>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Prop>()
                .Property(e => e.URL)
                .IsUnicode(false);

            modelBuilder.Entity<Prop>()
                .HasMany(e => e.CLIENT_PROP_VALUES)
                .WithRequired(e => e.PROP)
                .HasForeignKey(e => e.ID_PROP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prop>()
                .HasMany(e => e.PRODUCT_PROP_VALUES)
                .WithRequired(e => e.PROP)
                .HasForeignKey(e => e.ID_PROP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prop>()
                .HasMany(e => e.PRODUCT_TEMPLATE)
                .WithRequired(e => e.PROP)
                .HasForeignKey(e => e.ID_PROP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prop>()
                .HasMany(e => e.PROP_ENUMS_LISTS)
                .WithRequired(e => e.PROP)
                .HasForeignKey(e => e.ID_PROP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prop>()
                .HasMany(e => e.VARIANT_ITEMS)
                .WithRequired(e => e.PROP)
                .HasForeignKey(e => e.PROP1ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prop>()
                .HasMany(e => e.VARIANT_ITEMS1)
                .WithOptional(e => e.PROP1)
                .HasForeignKey(e => e.PROP2ID);

            modelBuilder.Entity<Prop>()
                .HasMany(e => e.VARIANT_ITEMS2)
                .WithOptional(e => e.PROP2)
                .HasForeignKey(e => e.PROP3ID);

            modelBuilder.Entity<Quotation>()
                .Property(e => e.STOCK_QTY)
                .HasPrecision(18, 5);

            //modelBuilder.Entity<Quotation>()
            //    .HasMany(e => e.ACTION_OFFERS)
            //    .WithRequired(e => e.QUOTATION)
            //    .HasForeignKey(e => e.ID_QUOTATION)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Quotation>()
            //    .HasMany(e => e.OFFERS)
            //    .WithRequired(e => e.QUOTATION)
            //    .HasForeignKey(e => e.ID_QUOTATION)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Quotation>()
                .HasMany(e => e.ORDER_SPEC_PRODUCTS)
                .WithRequired(e => e.QUOTATION)
                .HasForeignKey(e => e.ID_QUOTATION)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Service_Group>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Service_Group>()
                .HasOptional(e => e.SERVICE)
                .WithRequired(e => e.ServiceGroup);

            modelBuilder.Entity<Service_Quotation>()
                .HasMany(e => e.ORDER_SPEC_SERVICES)
                .WithRequired(e => e.ServiceQuotation)
                .HasForeignKey(e => e.ID_SERVICE_QUOTATION)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Service>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Service>()
                .HasMany(e => e.SERVICE_QUOTATIONS)
                .WithRequired(e => e.SERVICE)
                .HasForeignKey(e => e.ID_SERVICE)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Settlement>()
                .Property(e => e.ID_USER)
                .HasPrecision(18, 5);

            modelBuilder.Entity<Supplier>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Supplier>()
                .HasMany(e => e.DELIVERY_OPTIONS)
                .WithRequired(e => e.SUPPLIER)
                .HasForeignKey(e => e.ID_SUPPLIER)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Supplier>()
                .HasMany(e => e.QUOTATIONS)
                .WithRequired(e => e.SUPPLIER)
                .HasForeignKey(e => e.ID_SUPPLIER)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Supplier>()
                .HasMany(e => e.SERVICE_QUOTATIONS)
                .WithRequired(e => e.SUPPLIER)
                .HasForeignKey(e => e.ID_SUPPLIER)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Variant_Item>()
            //    .HasMany(e => e.PRODUCTS)
            //    .WithOptional(e => e.VariantItem)
            //    .HasForeignKey(e => e.ID_VARIANT_ITEM);

        }
    
    }
}