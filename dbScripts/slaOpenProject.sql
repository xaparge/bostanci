CREATE DEFINER=`root`@`localhost` PROCEDURE `slaOpenProject`()
BEGIN
SELECT projeler.id, gunluk.created_on as change_on, oncelik.name as priority,
gunlukDetay.old_value, gunlukDetay.value, durum.name as value_Name,
projeler.subject, projeler.created_on,projeler.closed_on ,gunlukDetay.prop_key

FROM bitnami_redmine.issues AS projeler 
LEFT OUTER JOIN bitnami_redmine.journals AS gunluk 
ON projeler.id = gunluk.journalized_id 
LEFT OUTER JOIN bitnami_redmine.journal_details as gunlukDetay 
on ((gunluk.id = gunlukDetay.journal_id) and (gunlukDetay.prop_key="status_id" ) ) 
left OUTER join bitnami_redmine.issue_statuses as durum on gunlukDetay.value=durum.id 
left join bitnami_redmine.enumerations as oncelik on oncelik.id= projeler.priority_id

WHERE projeler.tracker_id=1 

and projeler.status_id <> 3
and projeler.status_id <> 5
and projeler.status_id <> 6
and projeler.status_id <> 8
and projeler.status_id <> 9
and projeler.status_id <> 15
and projeler.status_id <> 11

and projeler.id = (
SELECT 
gunluk2.journalized_id as ticket_id
FROM bitnami_redmine.journals AS gunluk2 
LEFT OUTER JOIN bitnami_redmine.issues AS projeler2
ON projeler2.id = gunluk2.journalized_id 
left OUTER JOIN bitnami_redmine.journal_details as gunlukDetay2
on ((gunluk2.id = gunlukDetay2.journal_id) and (gunlukDetay2.prop_key="assigned_to_id" )) 

LEFT OUTER join bitnami_redmine.users as kullanici2 on gunlukDetay2.value= kullanici2.id

LEFT OUTER join bitnami_redmine.groups_users as iksapUser2 on iksapUser2.user_id = kullanici2.id

LEFT OUTER join bitnami_redmine.email_addresses as mail2 on mail2.user_id = iksapUser2.user_id

LEFT OUTER join bitnami_redmine.enumerations as oncelik2 on oncelik2.id= projeler2.priority_id

WHERE 
projeler2.tracker_id=1 
and projeler2.status_id <> 3
and projeler2.status_id <> 5 
and projeler2.status_id <> 6 
and projeler2.status_id <> 8
and projeler2.status_id <> 9
and projeler2.status_id <> 15
and projeler2.status_id <> 11
and iksapUser2.group_id=35
and gunluk2.created_on = (
select max(t1.created_on) FROM bitnami_redmine.journals AS t1 
LEFT OUTER JOIN bitnami_redmine.issues AS t1projeler ON t1projeler.id = t1.journalized_id 
inner JOIN bitnami_redmine.journal_details as t1Detay
on ((t1.id = t1Detay.journal_id) and (t1Detay.prop_key="assigned_to_id" )) 
where t1.journalized_id=gunluk.journalized_id)

ORDER BY gunluk.journalized_id desc)

ORDER BY `projeler`.`id` desc,gunluk.created_on asc;
END