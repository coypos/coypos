create table Categories
(
    ID               int identity
        constraint PK__Categori__3214EC27CBE2DFDB
            primary key,
    Name             nvarchar(255) not null,
    ParentCategoryID int
        constraint FK__Categorie__Paren__123EB7A3
            references Categories,
    Image            nvarchar(max),
    IsVisible        bit
        constraint DF_Categories_IsVisible default 0,
    UpdateDate       datetime,
    CreateDate       datetime
        constraint DF__Categorie__Creat__17036CC0 default getdate()
)
go

create trigger trg_UpdateDate_Categories
    on dbo.Categories
    after insert, update
    as
    begin
        update t
        set t.UpdateDate = GETDATE()
        FROM Categories t
        inner join inserted i on t.ID = i.ID
    end
go

disable trigger trg_UpdateDate_Categories on Categories
go

create table Categories_dictionary
(
    ID          int identity
        primary key,
    Category_ID int           not null
        constraint Categories_dictionary_ID_fk
            references Categories,
    Name_ENG    nvarchar(255) not null,
    Name_PL     nvarchar(255) not null,
    Name_UA     nvarchar(255) not null,
    Name_DE     nvarchar(255) not null,
    UpdateDate  datetime,
    CreateDate  datetime
        constraint DF__Categorie_DICT__Creat default getdate()
)
go

create table Employees
(
    ID      int identity
        constraint Employees_pk
            primary key,
    Name    varchar(256) not null,
    CardID  varchar(256) not null,
    PIN     varchar(256) not null,
    Enabled bit          not null,
    Admin   bit          not null
)
go

create table Images
(
    id    int identity,
    image varchar(max) not null
)
go

create table Languages
(
    Name        varchar(256) not null,
    CountryCode varchar(2)   not null,
    Enabled     bit          not null,
    Image       varchar(256),
    ID          int identity
        constraint ID
            primary key
)
go

create table PaymentMethods
(
    ID       int identity
        constraint PaymentMethods_pk
            primary key,
    Name     varchar(256)  not null,
    Image    varchar(256),
    AuthData varchar(256),
    Enabled  bit default 1 not null
)
go

create table Products
(
    Enabled       bit
        constraint DF__Products__Status__571DF1D5 default 1 not null,
    Name          nvarchar(255)                             not null,
    Barcode       nvarchar(20)                              not null,
    Price         decimal(10, 2)                            not null,
    isLoose       bit
        constraint DF__Products__isLoos__59063A47 default 0 not null,
    Description   varchar(512),
    CategoryID    int
        constraint Products_Categories_ID_fk
            references Categories,
    Image         nvarchar(max),
    Weight        int,
    ID            int identity
        constraint PK__Products__3214EC27AAEE907B
            primary key,
    UpdateDate    datetime,
    CreateDate    datetime
        constraint DF__Products__Create__1F98B2C1 default getdate(),
    AgeRestricted bit default 0                             not null
)
go

create trigger trg_UpdateDate_Products
    on dbo.Products
    after insert, update
    as
    begin
        update t
        set t.UpdateDate = GETDATE()
        FROM Products t
        inner join inserted i on t.ID = i.ID
    end
go

disable trigger trg_UpdateDate_Products on Products
go

create table Products_dictionary
(
    ID         int identity
        primary key,
    Product_ID int           not null
        constraint Products_dictionary_ID_fk
            references Products,
    Name_ENG   nvarchar(255) not null,
    Name_PL    nvarchar(255) not null,
    Name_UA    nvarchar(255) not null,
    Name_DE    nvarchar(255) not null,
    UpdateDate datetime,
    CreateDate datetime default getdate()
)
go

create table Promotions
(
    ID                 int identity
        constraint Promotions_pk
            primary key,
    IDs                varchar(4098) not null,
    DiscountPercentage int           not null,
    StartDate          date          not null,
    EndDate            date          not null,
    UpdateDate         datetime,
    CreateDate         datetime      not null
)
go

create trigger trg_UpdateDate_Promotions
    on Promotions
    after insert, update
    as
    begin
        update t
        set t.UpdateDate = GETDATE()
        FROM Promotions t
        inner join inserted i on t.ID = i.ID
    end
go

disable trigger trg_UpdateDate_Promotions on Promotions
go

create table Settings
(
    ID     int identity,
    _key   varchar(255) not null,
    _value varchar(max)
)
go

create table Users
(
    ID                  int identity
        constraint Users_pk
            primary key,
    Name                varchar(255)       not null,
    Role                varchar(50),
    PhoneNumber         varchar(20),
    CardNumber          varchar(16),
    UpdateDate          datetime,
    Points              int      default 0 not null,
    Email               varchar(100)       not null,
    Password            varchar(128)       not null,
    Salt                varchar(128)       not null,
    LoginToken          varchar(256),
    LoginTokenValidDate datetime,
    CreateDate          datetime default getdate()
)
go

create table LoyaltyCards
(
    ID         int identity
        constraint LoyaltyCards_pk
            primary key,
    UserID     int         not null
        references Users,
    CardNumber varchar(20) not null,
    Points     int         not null,
    CreateDate as getdate(),
    UpdateDate datetime
)
go

create table Receipts
(
    UserID          int
        constraint Receipts_Users_ID_fk
            references Users,
    Action          varchar(50),
    UpdateDate      datetime,
    TransactionId   varchar(256),
    CreateDate      datetime default getdate() not null,
    ID              int identity
        constraint Receipts_pk
            primary key,
    PaymentMethodID int
        constraint Receipts_PaymentMethods_ID_fk
            references PaymentMethods
)
go

create table Transactions
(
    ID            int identity,
    ProductID     int                              not null
        constraint Transactions_Product_ID_fk
            references Products,
    Quantity      decimal(10, 2),
    TotalPrice    decimal(10, 2),
    ReceiptID     int
        constraint Transactions_Receipts_ID_fk
            references Receipts,
    UpdateDate    datetime,
    OriginalPrice decimal(10, 2) default 0         not null,
    CreateDate    datetime       default getdate() not null
)
go

create trigger trg_UpdateDate_Users
    on Users
    after insert, update
    as
    begin
        update t
        set t.UpdateDate = GETDATE()
        FROM Users t
        inner join inserted i on t.ID = i.ID
    end
go

disable trigger trg_UpdateDate_Users on Users
go

create table sysdiagrams
(
    name         sysname not null,
    principal_id int     not null,
    diagram_id   int identity
        primary key,
    version      int,
    definition   varbinary(max),
    constraint UK_principal_name
        unique (principal_id, name)
)
go

exec sp_addextendedproperty 'microsoft_database_tools_support', 1, 'SCHEMA', 'dbo', 'TABLE', 'sysdiagrams'
go

IF NOT EXISTS (SELECT 1 FROM master.dbo.Employees)
BEGIN
INSERT INTO master.dbo.Employees (Name, CardID, PIN, Enabled, Admin)
VALUES (N'admin', N'1234', N'1234', 1, 1);
END

go