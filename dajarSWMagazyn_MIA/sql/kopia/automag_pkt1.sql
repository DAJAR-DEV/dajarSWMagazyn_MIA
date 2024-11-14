select distinct w3.skrot,r.nazdop,r.jm,r.kod_tow,r.skt2_0,r.klasa from (
select w2.skrot from (
select w1.* from (
select w.*
,(select count(dt.adres) from dp_swm_towary dt where dt.skrot=w.skrot and dt.strefa in('Z') and dt.is_active='1') ile_z 
,(select count(dt.adres) from dp_swm_towary dt where dt.skrot=w.skrot and dt.strefa in('N','P') and dt.is_active='1') ile_np
from (
select db.skrot,dz.nr_zamow,dz.schemat,dz.stat_u1,
(select substr(typ_oper,0,2) from dp_swm_oper where status=dz.nr_zamow and typ_oper like '%['||dz.schemat||']%' and (typ_oper like 'DR[%' or typ_oper like 'RP[%')) typ_oper
from dp_swm_zog dz,dp_swm_buf db where dz.nr_zamow=db.nr_zamow and dz.schemat=db.schemat and dz.stat_u1 in('MG') 
order by decode(typ_oper, 'RP', 1, 'DR', 2), to_char(dz.stda_u1,'YYYY-MM-DD'), decode(dz.stat_u1, 'MG', 1, 'RP', 2, 'DR', 3)
) w 
) w1 where w1.ile_z = 0 and w1.ile_np > 0
) w2 left join dp_swm_towary dt on w2.skrot = dt.skrot and dt.strefa in('N','P') and dt.is_active='1' order by decode(dt.strefa, 'N', 1, 'P', 2), w2.skrot
) w3, ht_rejna r where w3.skrot = r.nazpot and r.is_deleted='N'