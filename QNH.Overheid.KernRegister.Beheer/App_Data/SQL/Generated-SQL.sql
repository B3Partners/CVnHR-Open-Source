
    drop table if exists KERNREGRELBEH.DEPONERINGSSTUK cascade;

    drop table if exists KERNREGRELBEH.FUNCTIEVERVULLING cascade;

    drop table if exists KERNREGRELBEH.HANDELSNAAM cascade;

    drop table if exists KERNREGRELBEH.KVKINSCHRIJVING cascade;

    drop table if exists KERNREGRELBEH.SBIACTIVITEIT cascade;

    drop table if exists KERNREGRELBEH.SBICODE cascade;

    drop table if exists KERNREGRELBEH.VESTIGING cascade;

    drop table if exists KERNREGRELBEH.VESTIGINGSBIACTIVITEIT cascade;

    drop table if exists RSGB.QNH_VW_DEPONERINGSSTUK cascade;

    drop table if exists RSGB.QNH_VW_FUNCTIEVERVULLING cascade;

    drop table if exists RSGB.QNH_VW_KVKINSCHRIJVING cascade;

    drop table if exists RSGB.QNH_VW_SBIACTIVITEIT cascade;

    drop table if exists RSGB.QNH_VW_SBICODE cascade;

    drop table if exists RSGB.QNH_VW_VESTIGING cascade;

    drop table if exists RSGB.QNH_VW_VESTIGINGSBIACTIVITEIT cascade;

    create table KERNREGRELBEH.DEPONERINGSSTUK (
        Id  serial,
       DEPOTID varchar(255),
       DATUMDEPONERING timestamp,
       TYPE varchar(255),
       STATUS varchar(255),
       GAATOVER varchar(255),
       KvkInschrijving_id int4,
       primary key (Id)
    );

    create table KERNREGRELBEH.FUNCTIEVERVULLING (
        Id  serial,
       FUNCTIE varchar(255),
       FUNCTIETITEL varchar(255),
       VOLLEDIGENAAM varchar(255),
       SCHORSING varchar(255),
       LANGSTZITTENDE varchar(255),
       BEVOEGDHEID varchar(255),
       HANDELINGSBEKWAAM varchar(255),
       KvkInschrijving_id int4,
       primary key (Id)
    );

    create table KERNREGRELBEH.HANDELSNAAM (
        Id  serial,
       HANDELSNAAM varchar(255),
       KvkInschrijving_id int4,
       primary key (Id)
    );

    create table KERNREGRELBEH.KVKINSCHRIJVING (
        Id  serial,
       NAAM varchar(255),
       KVKNUMMER varchar(255),
       PEILMOMENT varchar(255),
       INGEVOEGDOP timestamp,
       GELDIGTOT timestamp,
       OPGEVRAAGDOP timestamp,
       REGISTRATIEDATUMAANVANG varchar(255),
       REGISTRATIEDATUMEINDE varchar(255),
       DATUMOPRICHTING varchar(255),
       DATUMUITSCHRIJVING varchar(255),
       PERSOONRECHTSVORM varchar(255),
       UITGEBREIDERECHTSVORM varchar(255),
       BIJZONDERERECHTSTOESTAND varchar(255),
       REDENINSOLVENTIE varchar(255),
       BEPERKINGINRECHTSHANDELING varchar(255),
       EIGENAARHEEFTGEDEPONEERD varchar(255),
       VOLLEDIGENAAMEIGENAAR varchar(255),
       FULLTIMEWERKZAMEPERSONEN varchar(255),
       PARTTIMEWERKZAMEPERSONEN varchar(255),
       TOTAALWERKZAMEPERSONEN varchar(255),
       GEPLAATSTKAPITAAL varchar(255),
       GESTORTKAPITAAL varchar(255),
       RECHTERLIJKEUITSPRAAK varchar(255),
       BERICHTENBOXNAAM varchar(255),
       primary key (Id)
    );

    create table KERNREGRELBEH.SBIACTIVITEIT (
        Id  serial,
       ISHOOFDSBIACTIVITEIT boolean,
       SbiCode_id varchar(255),
       KvKInschrijving_id int4,
       primary key (Id)
    );

    create table KERNREGRELBEH.SBICODE (
        Code varchar(255) not null,
       OMSCHRIJVING varchar(255),
       primary key (Code)
    );

    create table KERNREGRELBEH.VESTIGING (
        Id  serial,
       VESTIGINGSNUMMER varchar(255),
       NAAM varchar(255),
       ADRES varchar(255),
       STRAAT varchar(255),
       HUISNUMMER varchar(255),
       HUISNUMMERTOEVOEGING varchar(255),
       POSTCODECIJFERS varchar(255),
       POSTCODELETTERS varchar(255),
       WOONPLAATS varchar(255),
       TELEFOON varchar(255),
       FAX varchar(255),
       EMAIL varchar(255),
       GEMEENTE varchar(255),
       BAGID varchar(255),
       RSIN varchar(255),
       EORI varchar(255),
       INGEVOEGDOP timestamp,
       GELDIGTOT timestamp,
       ISHOOFDVESTIGING boolean,
       POSTBUSNUMMER varchar(255),
       POSTADRES varchar(255),
       POSTSTRAAT varchar(255),
       POSTHUISNUMMER varchar(255),
       POSTHUISNUMMERTOEVOEGING varchar(255),
       POSTPOSTCODECIJFERS varchar(255),
       POSTPOSTCODELETTERS varchar(255),
       POSTWOONPLAATS varchar(255),
       POSTGEMEENTE varchar(255),
       REGISTRATIEDATUMAANVANG varchar(255),
       REGISTRATIEDATUMEINDE varchar(255),
       FULLTIMEWERKZAMEPERSONEN varchar(255),
       PARTTIMEWERKZAMEPERSONEN varchar(255),
       TOTAALWERKZAMEPERSONEN varchar(255),
       KvkInschrijving_id int4,
       primary key (Id)
    );

    create table KERNREGRELBEH.VESTIGINGSBIACTIVITEIT (
        Id  serial,
       ISHOOFDSBIACTIVITEIT boolean,
       SbiCode_id varchar(255),
       Vestiging_id int4,
       primary key (Id)
    );

    create table RSGB.QNH_VW_DEPONERINGSSTUK (
        Id  serial,
       DEPOTID varchar(255),
       DATUMDEPONERING timestamp,
       TYPE varchar(255),
       STATUS varchar(255),
       GAATOVER varchar(255),
       KvkInschrijving_id int4,
       primary key (Id)
    );
