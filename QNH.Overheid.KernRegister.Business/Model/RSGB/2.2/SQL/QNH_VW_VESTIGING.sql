-- ORACLE VIEW
CREATE OR REPLACE VIEW QNH_VW_VESTIGING (
       Id,
       VESTIGINGSNUMMER, -- NVARCHAR2(255),
       NAAM, -- NVARCHAR2(255),
       ADRES, -- NVARCHAR2(255),
       STRAAT, -- NVARCHAR2(255),
       HUISNUMMER, -- NVARCHAR2(255),
       HUISNUMMERTOEVOEGING, -- NVARCHAR2(255),
       POSTCODECIJFERS, -- NVARCHAR2(255),
       POSTCODELETTERS, -- NVARCHAR2(255),
       WOONPLAATS, -- NVARCHAR2(255),
       TELEFOON, -- NVARCHAR2(255),
       FAX, -- NVARCHAR2(255),
       EMAIL, -- NVARCHAR2(255),
       GEMEENTE, -- NVARCHAR2(255),
       BAGID,  --NVARCHAR2(255),
       RSIN, -- NVARCHAR2(255),
       EORI, -- NVARCHAR2(255),
       INGEVOEGDOP, -- TIMESTAMP(4),
       GELDIGTOT, -- TIMESTAMP(4),
       ISHOOFDVESTIGING, -- NUMBER(1,0),
       POSTBUSNUMMER, -- NVARCHAR2(255),
       POSTADRES, -- NVARCHAR2(255),
       POSTSTRAAT, -- NVARCHAR2(255),
       POSTHUISNUMMER, -- NVARCHAR2(255),
       POSTHUISNUMMERTOEVOEGING, -- NVARCHAR2(255),
       POSTPOSTCODECIJFERS, -- NVARCHAR2(255),
       POSTPOSTCODELETTERS, -- NVARCHAR2(255),
       POSTWOONPLAATS, -- NVARCHAR2(255),
       POSTGEMEENTE, -- NVARCHAR2(255),
       REGISTRATIEDATUMAANVANG, -- NVARCHAR2(255),
       REGISTRATIEDATUMEINDE, -- NVARCHAR2(255),
       FULLTIMEWERKZAMEPERSONEN, -- NVARCHAR2(255),
       PARTTIMEWERKZAMEPERSONEN, -- NVARCHAR2(255),
       TOTAALWERKZAMEPERSONEN, -- NVARCHAR2(255),
       KvkInschrijving_id -- NUMBER(10,0),
    ) AS
SELECT 
	CAST(REPLACE(REPLACE(V.SC_IDENTIF, 'nhr.comVestg.', ''), 'nhr.nietComVestg.', '') AS NUMBER(10,0))  AS Id,
	REPLACE(REPLACE(V.SC_IDENTIF, 'nhr.comVestg.', ''), 'nhr.nietComVestg.', '')	AS VESTIGINGSNUMMER,
	V.VERKORTE_NAAM																                                AS NAAM,
  S.ADRES_BINNENLAND                                                            AS ADRES,
	aoa.GEOR.NAAM_OPENB_RMTE													                            AS STRAAT,
  aoa.HUINUMMER                                                                 AS HUISNUMMER,
  NVL(aoa.HUISLETTER, '') + ' ' + NVL(aoa.HUINUMMERTOEVOEGING, '')              AS HUISNUMMERTOEVOEGING,
  substr(aoa.POSTCODE, 0, 4)                                                    AS POSTCODECIJFERS,
  substr(aoa.POSTCODE, 4, 2)                                                    AS POSTCODELETTERS,
  W.NAAM                                                                        AS WOONPLAATS,
  S.TELEFOONNUMMER                                                              AS TELEFOON,
  S.FAX_NUMMER                                                                  AS FAX,
  S.EMAILADRES                                                                  AS EMAIL,
  GEM.NAAM                                                                      AS GEMEENTE,
  NVL(NA.SC_IDENTIF, '') + ';' + aoa.IDENTIF                                    AS BAGID,
  INNP.RSIN                                                                     AS RSIN,
  CAST(NULL AS NVARCHAR2(255))                                                  AS EORI,
  CAST('01-JAN-0001 01.00.00.000000000 AM' AS TIMESTAMP(4))                     AS INGEVOEGDOP,
  CAST('31-DEC-9999 11.59.59.000000000 PM' AS TIMESTAMP(4))                     AS GELDIGTOT,
  CASE FK_19MAC_KVK_NUMMER WHEN NULL THEN 0 ELSE 1 END                          AS ISHOOFDVESTIGING,
  CASE S.PA_POSTADRESTYPE 
      WHEN 'P' THEN S.PA_POSTBUS__OF_ANTWOORDNUMMER 
      ELSE NULL 
  END                                                                           AS POSTBUSNUMMER,
  CAST(NULL AS NVARCHAR2(255))                                                  AS POSTADRES,
	CAST(NULL AS NVARCHAR2(255))											                            AS POSTSTRAAT,
  CAST(NULL AS NVARCHAR2(255))                                                  AS POSTHUISNUMMER,
  CAST(NULL AS NVARCHAR2(255))                                                  AS POSTHUISNUMMERTOEVOEGING,
  CASE S.PA_POSTADRESTYPE 
      WHEN 'P' THEN substr(S.PA_POSTADRES_POSTCODE, 0 , 4)
      ELSE NULL
  END                                                                           AS POSTPOSTCODECIJFERS,
  CASE S.PA_POSTADRESTYPE 
      WHEN 'P' THEN substr(S.PA_POSTADRES_POSTCODE, 4, 2)
      ELSE NULL
  END                                                                           AS POSTPOSTCODELETTERS,
  POST_W.NAAM                                                                   AS POSTWOONPLAATS,
  POST_GEM.NAAM                                                                 AS POSTGEMEENTE,
  V.DATUM_AANVANG                                                               AS REGISTRATIEDATUMAANVANG,
  V.DATUM_BEEINDIGING                                                           AS REGISTRATIEDATUMEINDE,
  NVL(V.FULLTIME_WERKZAME_MANNEN, 0) + NVL(V.FULLTIME_WERKZAME_VROUWEN,0)       AS FULLTIMEWERKZAMEPERSONEN,
  NVL(V.PARTTIME_WERKZAME_MANNEN, 0) + NVL(V.PARTTIME_WERKZAME_VROUWEN,0)       AS PARTTIMEWERKZAMEPERSONEN,
  NVL(V.FULLTIME_WERKZAME_MANNEN, 0) + NVL(V.FULLTIME_WERKZAME_VROUWEN,0)
  + NVL(V.PARTTIME_WERKZAME_MANNEN, 0) + NVL(V.PARTTIME_WERKZAME_VROUWEN,0)     AS TOTAALWERKZAMEPERSONEN,
  CAST(V.FK_15OND_KVK_NUMMER AS NUMBER(10,0))                                   AS KvkInschrijving_id
FROM
	VESTG V
  JOIN SUBJECT S 
    ON V.SC_IDENTIF = S.IDENTIF
  LEFT OUTER JOIN addresseerb_obj_aand aoa
    ON S.FK_15AOA_IDENTIF = aoa.IDENTIF
    OR S.FK_14AOA_IDENTIF = aoa.IDENTIF
  LEFT OUTER JOIN OPENB_RMTE OPR
    ON aoa.FK_7OPR_IDENTIFCODE = OPR.IDENTIFCODE
  LEFT OUTER JOIN WNPLTS W
    ON aoa.FK_6WPL_IDENTIF = W.IDENTIF
  LEFT OUTER JOIN WNPLTS POST_W
    ON S.FK_PA_4_WPL_IDENTIF = POST_W.IDENTIF
  LEFT OUTER JOIN GEM_OPENB_RMTE GEOR
    ON GEOR.IDENTIFCODE = aoa.FK_7OPR_IDENTIFCODE
  LEFT OUTER JOIN GEMEENTE GEM
    ON W.FK_7GEM_CODE = GEM.CODE
  LEFT OUTER JOIN GEMEENTE POST_GEM
    ON POST_W.FK_7GEM_CODE = POST_GEM.CODE
  LEFT OUTER JOIN NUMMERAAND NA
    ON aoa.IDENTIF = NA.SC_IDENTIF
  LEFT OUTER JOIN VERBLIJFSOBJ_NUMMERAAND VNA
    ON NA.SC_IDENTIF = VNA.FK_NN_RH_NRA_SC_IDENTIF
  LEFT OUTER JOIN VERBLIJFSOBJ VBO
    ON VNA.FK_NN_LH_VBO_SC_IDENTIF = VBO.SC_IDENTIF
  LEFT OUTER JOIN INGESCHR_NIET_NAT_PRS INNP
    ON aoa.IDENTIF = INNP.FK_8AOA_IDENTIF
     

WHERE
    (NA.STATUS IS NULL OR NA.STATUS = 'Naamgeving uitgegeven')
AND (
        VBO.STATUS IS NULL
    OR  VBO.STATUS = 'Verblijfsobject in gebruik (niet ingemeten)'
    OR  VBO.STATUS = 'Verblijfsobject in gebruik'
    );