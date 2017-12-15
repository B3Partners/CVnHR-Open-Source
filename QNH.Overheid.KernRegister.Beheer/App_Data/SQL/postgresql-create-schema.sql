create table deponeringsstuk (
	id bigserial not null,
	depotid varchar(255),
	datumdeponering timestamp(4),
	type varchar(255),
	status varchar(255),
	gaatover varchar(255),
	kvkinschrijving_id bigint,
	primary key (id)
);

create table functievervulling (
	id bigserial not null,
	functie varchar(255),
	functietitel varchar(255),
	volledigenaam varchar(255),
	schorsing varchar(255),
	langstzittende varchar(255),
	bevoegdheid varchar(255),
	handelingsbekwaam varchar(255),
	kvkinschrijving_id bigint,
	primary key (id)
);

create table handelsnaam (
	id bigserial not null,
	handelsnaam varchar(255),
	kvkinschrijving_id bigint,
	primary key (id)
);

create table kvkinschrijving (
	id bigserial not null,
	naam varchar(255),
	kvknummer varchar(255),
	peilmoment varchar(255),
	ingevoegdop timestamp(4),
	geldigtot timestamp(4),
	opgevraagdop timestamp(4),
	registratiedatumaanvang varchar(255),
	registratiedatumeinde varchar(255),
	datumoprichting varchar(255),
	datumuitschrijving varchar(255),
	persoonrechtsvorm varchar(255),
	uitgebreiderechtsvorm varchar(255),
	bijzondererechtstoestand varchar(255),
	redeninsolventie varchar(255),
	beperkinginrechtshandeling varchar(255),
	eigenaarheeftgedeponeerd varchar(255),
	volledigenaameigenaar varchar(255),
	fulltimewerkzamepersonen varchar(255),
	parttimewerkzamepersonen varchar(255),
	totaalwerkzamepersonen varchar(255),
	geplaatstkapitaal varchar(255),
	gestortkapitaal varchar(255),
	rechterlijkeuitspraak varchar(255),
	berichtenboxnaam varchar(255),
	primary key (id)
);

create table sbiactiviteit (
	id bigserial not null,
	-- NUMBER(1,0), gemapped naar boolean
	ishoofdsbiactiviteit boolean,
	sbicode_id varchar(255),
	kvkinschrijving_id bigint,
	primary key (id)
);

create table sbicode (
	code varchar(255) not null,
	omschrijving varchar(255),
	primary key (code)
);

create table vestiging (
	id bigserial not null,
	vestigingsnummer varchar(255),
	naam varchar(255),
	adres varchar(255),
	straat varchar(255),
	huisnummer varchar(255),
	huisnummertoevoeging varchar(255),
	postcodecijfers varchar(255),
	postcodeletters varchar(255),
	woonplaats varchar(255),
	telefoon varchar(255),
	fax varchar(255),
	email varchar(255),
	gemeente varchar(255),
	bagid varchar(255),
	rsin varchar(255),
	eori varchar(255),
	ingevoegdop timestamp(4),
	geldigtot timestamp(4),
	-- NUMBER(1,0), gemapped naar boolean
	ishoofdvestiging boolean,
	postbusnummer varchar(255),
	postadres varchar(255),
	poststraat varchar(255),
	posthuisnummer varchar(255),
	posthuisnummertoevoeging varchar(255),
	postpostcodecijfers varchar(255),
	postpostcodeletters varchar(255),
	postwoonplaats varchar(255),
	postgemeente varchar(255),
	registratiedatumaanvang varchar(255),
	registratiedatumeinde varchar(255),
	fulltimewerkzamepersonen varchar(255),
	parttimewerkzamepersonen varchar(255),
	totaalwerkzamepersonen varchar(255),
	kvkinschrijving_id bigint,
	primary key (id)
);

create table vestigingsbiactiviteit (
	id bigserial not null,
	-- NUMBER(1,0), gemapped naar boolean
	ishoofdsbiactiviteit boolean,
	sbicode_id varchar(255),
	vestiging_id bigint,
	primary key (id)
);

alter table deponeringsstuk 
	add constraint fke7111dd91de582d3 
	foreign key (kvkinschrijving_id) 
	references kvkinschrijving;

alter table functievervulling 
	add constraint fkd6a1fefa1de582d3 
	foreign key (kvkinschrijving_id) 
	references kvkinschrijving;

alter table handelsnaam 
	add constraint fkc66ee6e81de582d3 
	foreign key (kvkinschrijving_id) 
	references kvkinschrijving;

create index ix_kvkinschrijving_naam on kvkinschrijving (naam);

create index ix_kvkinschrijving_kvknummer on kvkinschrijving (kvknummer);

alter table sbiactiviteit 
	add constraint fk771ac42a540ed1a5 
	foreign key (sbicode_id) 
	references sbicode;

alter table sbiactiviteit 
	add constraint fk771ac42a1de582d3 
	foreign key (kvkinschrijving_id) 
	references kvkinschrijving;

create index ix_vestiging_vestigingsnummer on vestiging (vestigingsnummer);

create index ix_vestiging_naam on vestiging (naam);

create index ix_vestiging_bagid on vestiging (bagid);

create index ix_vestiging_rsin on vestiging (rsin);

alter table vestiging 
	add constraint fkb0a96c581de582d3 
	foreign key (kvkinschrijving_id) 
	references kvkinschrijving;

alter table vestigingsbiactiviteit 
	add constraint fk237fb584540ed1a5 
	foreign key (sbicode_id) 
	references sbicode;

alter table vestigingsbiactiviteit 
	add constraint fk237fb58456465feb 
	foreign key (vestiging_id) 
	references vestiging;
