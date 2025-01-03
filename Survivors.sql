create database HomifyDB

use HomifyDB

create table Users(
	userId int primary key identity (100,1),
	userName nvarchar(50) unique not null,
	firstName nvarchar(50) not null	,
	lastName nvarchar(50) not null,
	passWord nvarchar(40) not null,
	emailContact nvarchar(40) unique not null,
	createdAt date not null default GETDATE(),
)
insert into Users values
('Kevo','Makumbi','Kevin','1234','kevin@gmail.com','12-12-1212')

select * from Users

create table Properties(
	propertyId int primary key identity(50,1),
	userId int not null,
	price Decimal(10,2) not null,
	location nvarchar(255),
	status nvarchar(20) not null,
	foreign key(userId) references Users(userId)
)

alter table Properties 
add plotNumber nvarchar(50)

insert into Properties values
(100,10000,'Salaama','available')

select * from Properties

insert into Properties values
(

create table PropertyImages(
	imageId int identity(1,2),
	propertyId int not null,
	imagePath nvarchar(255) not null,
	foreign key(propertyId) references Properties(propertyId)
)

alter table PropertyImages
add primary key(imageId)

select * from PropertyImages

create table Description(
	description int primary key identity(1,1),
	propertyId int not null,
	landType nvarchar(50),
	size nvarchar(50),
	houseType nvarchar(50),
	bedRooms int,
	parking nvarchar(50),
	bathRooms int,
	YearBuilt date,
	Amentities nvarchar(50),
	foreign key(propertyId) references Properties(propertyId)
)

select * from Description

create table Rentals(
	rentalId int primary key identity(1,1),
	userId int not null,
	clientName nvarchar(50) not null,
	clientEmail nvarchar(50) not null,
	clientContact nvarchar(50) not null,
	clientLocation nvarchar(50) not null,
	rent decimal(10,2) not null,
	billingPeriod nvarchar(50) not null,
	due nvarchar(50),
	foreign key(userId) references Users(userId)
)

select * from Rentals

create database MinistryOfLands

use MinistryOfLands

create table landOwners(
	ownerId int primary key identity(100,1),
	Name nvarchar(50) not null,
	Email nvarchar(50) unique not null,
	Contact nvarchar(50) not null,
	plotNumber nvarchar(50) not null,
	Location nvarchar(50) not null
)