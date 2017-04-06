/****** Script for statistics of the scan scooter for today  ******/



create table #ControlTypeLanguage (
	CTLN_Type int, CTLN_Description varchar(255	) );

Insert into #ControlTypeLanguage VALUES ( 0, 'Parking Monitor' ), (1, 'Uurrooster'), (2, 'SMS'), (3, 'Abonnement'), 
			(5, 'Tweede RB'), (7, 'Bevroren Nummerplaat'), (20, 'Sensibilisering'), (100, 'YellowBrick'), (110, 'PayByPhone'),
			(120, 'Parkeon'), (130, 'Mobigo'), (140, 'Presto'), (150, 'MobileFor'), (160, 'ParkMobile'), (170, 'Extenso'),
			(180, 'CEVI'), (190, 'Hectronic'), (200, 'Parkare'), (999, 'TOTAAL');


create table #sctoday ( SCTD_Type int, SCTD_LPHS_ID uniqueidentifier );

insert into #sctoday
  SELECT (CASE WHEN PMQU.HTQU_ID IS NULL THEN LHDT.LHDT_TypeControle ELSE 0 END),LPHS.LPHS_ID FROM
  (SELECT DISTINCT LPHS_ValidationGroup_ID FROM LicenseplateHistory2
  WHERE LPHS_MOBI_ID = '25F56799-008C-4896-9ADD-685D121DDB7A')  A
  CROSS APPLY
        (
        SELECT  TOP 1 *
        FROM    LicenseplateHistory2 AS B
        WHERE   B.LPHS_ValidationGroup_ID = A.LPHS_ValidationGroup_ID AND B.LPHS_MOBI_ID = '25F56799-008C-4896-9ADD-685D121DDB7A'
		ORDER BY LPHS_CreatedOn DESC
        ) LPHS
 LEFT JOIN [PARKINGMONITORQueue_LOCT_F850F71B-CFB8-469A-A092-88D3E207CC28] AS PMQU ON LPHS.LPHS_ValidationGroup_ID = HTQU_ID
 LEFT JOIN LicenseplateHistoryDetail AS LHDT ON LHDT_LPHS_ID = LPHS.LPHS_ID AND LHDT_ContinueTicket = 0

 --select * FROM #sctoday ORDER BY SCTD_Type

 create table #scresults ( SCRS_Type int, SCRS_Count int )

insert into #scresults
Select SCTD_TYPE,COUNT(*) 
from #sctoday
GROUP BY SCTD_TYPE

insert into #scresults
values ( 999, (SELECT COUNT(*) FROM #sctoday) )


declare @sql2 varchar(max) = 
'Select (SELECT CTLN_DEscription FROM #ControlTypeLanguage WHERE CTLN_Type = SCRS_Type) AS ''Type Scan'',SCRS_Count AS ''Aantal ' + CONVERT(varchar,@begin,103) +
''' from #scresults
  order by SCRS_Type'
EXEC(@SQL2)


drop table #scresults
drop table #sctoday
drop table #ControlTypeLanguage

create table #StepLanguage (
	STLN_Step int, STLN_Description varchar(255	) );

Insert into #StepLanguage VALUES ( 0, 'Ongewerkte hit' ), (1, 'Geannuleerde hit'), (2, 'Gevalideerde hit'), (4, 'Duplicate hit'), (9, 'TOTAAL');

create table #pmtoday ( PMTD_Step int, PMTD_HTQU_ID uniqueidentifier );

insert into #pmtoday
select HTQU_STEP, HTQU_ID FROM [ParkingMonitorQueue_LOCT_F850F71B-CFB8-469A-A092-88D3E207CC28] 
WHERE HTQU_CReatedOn >= @begin

UPDATE #pmtoday 
SET PMTD_Step = 2
WHERE PMTD_Step = 1 AND EXISTS (SELECT 1 FROM [MOBILEQueue_LOCT_F850F71B-CFB8-469A-A092-88D3E207CC28] 
								WHERE HTQU_ID = PMTD_HTQU_ID)


create table #pmresults ( PMRS_Step int, PMRS_Count int )

insert into #pmresults
Select PMTD_STEP,COUNT(*) 
from #pmtoday
GROUP BY PMTD_STEP

insert into #pmresults
values ( 9, (SELECT COUNT(*) FROM #pmtoday) )


declare @sql3 varchar(max) = 
'Select (SELECT STLN_DEscription FROM #StepLanguage WHERE STLN_Step = PMRS_STEP) AS ''Type Hit'',PMRS_Count AS ''Aantal ' + CONVERT(varchar,@begin,103) +
''' from #pmresults
  order by PMRS_STEP'
EXEC(@SQL3)

drop table #StepLanguage
drop table #pmtoday
drop table #pmresults


declare @sql varchar(max) = 
'SELECT  (CASE WHEN HTQU_AssignedtoStepUSER_ID IS NULL THEN ''Onverwerkt'' ELSE (SELECT USER_NAME FROM [User] WHERE USER_ID = HTQU_AssignedtoStepUSER_ID) END) AS ''Parkeerwachter'',
		COUNT(*) AS ''Aantal ' + CONVERT(varchar,getDate(),103) + ''',
		SUM(CASE WHEN LPHS_SLFT_ID IS NULL THEN 0 ELSE 1 END) AS ''Retributie'',
		SUM(CASE WHEN LPHS_ValidType_Id IS NULL THEN 0 ELSE 1 END) AS ''Geldig'',
		SUM(CASE WHEN LPHS_IsUnchecked = 1 THEN 1 ELSE 0 END) AS ''Weggereden''
  FROM [dbo].[MOBILEQueue_LOCT_F850F71B-CFB8-469A-A092-88D3E207CC28]
  CROSS APPLY
        (
        SELECT  TOP 1 *
        FROM    LicenseplateHistory2
        WHERE   LPHS_ValidationGroup_ID = HTQU_ID
		ORDER BY LPHS_CreatedOn DESC
        ) LPHS
  WHERE [HTQU_CreatedOn] >= ''' + CONVERT(varchar,@begin,120)	 + '''
  GROUP BY HTQU_AssignedtoStepUSER_ID
  ORDER BY HTQU_AssignedtoStepUSER_ID'
  EXEC(@SQL)
