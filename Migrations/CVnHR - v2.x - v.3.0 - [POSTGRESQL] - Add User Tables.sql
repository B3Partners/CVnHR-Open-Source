/*
	This script is optional if the BRMO is installed.
	Replace [SCHEMA] with the correct schema name
	Replace [USER] with the correct user name
*/


--DROP TABLE [SCHEMA].GEBRUIKER_;
--DROP TABLE [SCHEMA].GROEP_;
--DROP TABLE [SCHEMA].GEBRUIKER_GROEPEN;
  
create table [SCHEMA].GEBRUIKER_ (
    gebruikersnaam varchar(255) not null,
    wachtwoord varchar(255),
    primary key (gebruikersnaam)
);

create table [SCHEMA].GROEP_ (
    naam varchar(255) not null,
    beschrijving VARCHAR(MAX),
    primary key (naam)
);

create table [SCHEMA].GEBRUIKER_GROEPEN (
    gebruikersnaam varchar(255) not null,
    groep_ varchar(255) not null,
    primary key (gebruikersnaam, groep_)
);

alter table [SCHEMA].GEBRUIKER_GROEPEN 
	add constraint FK_GEBRUIKER_GROEPEN_GROEP 
	foreign key (groep_) 
	references [SCHEMA].GROEP_;

alter table [SCHEMA].GEBRUIKER_GROEPEN 
    add constraint FK_GEBRUIKER_GROEPEN_GEBRU
    foreign key (gebruikersnaam) 
    references [SCHEMA].GEBRUIKER_;

--grant SELECT, INSERT, UPDATE, DELETE on [SCHEMA].GEBRUIKER_ to [USER];
--grant SELECT, INSERT, UPDATE, DELETE on [SCHEMA].GROEP_ to [USER];
--grant SELECT, INSERT, UPDATE, DELETE on [SCHEMA].GEBRUIKER_GROEPEN to [USER];