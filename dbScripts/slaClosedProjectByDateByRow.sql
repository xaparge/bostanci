CREATE DEFINER=`root`@`localhost` PROCEDURE `slaClosedProjectByDateByRow`(
IN `monthvalue` INT, 
IN `yearvalue` INT, 
IN `pageStart` INT, 
IN `pageEnd` INT )
BEGIN


SELECT projeler.id, gunluk.created_on as change_on, oncelik.name as priority,
gunlukDetay.old_value, gunlukDetay.value, durum.name as value_Name,
projeler.subject, projeler.created_on,projeler.closed_on ,gunlukDetay.prop_key
 
 FROM bitnami_redmine.issues AS projeler  
 LEFT OUTER JOIN bitnami_redmine.journals AS gunluk
	ON projeler.id = gunluk.journalized_id 
    INNER JOIN  
    ( SELECT projeler2.id 
 FROM bitnami_redmine.journals  AS gunluk2
 LEFT OUTER JOIN  bitnami_redmine.issues AS projeler2
	ON projeler2.id = gunluk2.journalized_id 

    
    
 LEFT OUTER JOIN bitnami_redmine.journal_details as gunlukDetay2
	on ((gunluk2.id = gunlukDetay2.journal_id) and (gunlukDetay2.prop_key="status_id" ) ) 
 left OUTER join bitnami_redmine.issue_statuses as durum2 on gunlukDetay2.value=durum2.id 
 left join bitnami_redmine.enumerations as oncelik2 on oncelik2.id= projeler2.priority_id
 
 WHERE  projeler2.tracker_id=1 
 and projeler2.status_id = 5
and YEAR(projeler2.closed_on)=yearvalue
and MONTH(projeler2.closed_on)=monthvalue
	
    GROUP BY projeler2.id
	ORDER BY projeler2.id desc, gunluk2.created_on asc
    LIMIT pageStart,pageEnd)as selectedrows
  ON projeler.id = selectedrows.id
        
    
    
    
 LEFT OUTER JOIN bitnami_redmine.journal_details as gunlukDetay 
	on ((gunluk.id = gunlukDetay.journal_id) and (gunlukDetay.prop_key="status_id" ) ) 
 left OUTER join bitnami_redmine.issue_statuses as durum on gunlukDetay.value=durum.id 
 left join bitnami_redmine.enumerations as oncelik on oncelik.id= projeler.priority_id
 
 WHERE  projeler.tracker_id=1 
 and projeler.status_id = 5
and YEAR(projeler.closed_on)=yearvalue
and MONTH(projeler.closed_on)=monthvalue


	ORDER BY projeler.id desc, gunluk.created_on asc;
    
END