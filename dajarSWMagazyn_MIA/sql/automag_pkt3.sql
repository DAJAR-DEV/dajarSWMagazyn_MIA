select distinct w3.skrot,r.nazdop,r.jm,r.kod_tow,r.skt2_0,r.klasa,(select login from dp_swm_lub_towary_hist where oper='OP_BR' and skrot=w3.skrot and autodata=(select max(to_char(autodata, 'YYYY/MM/DD HH24:MI:SS')) from dp_swm_lub_towary_hist where oper='OP_BR' and skrot=w3.skrot)) oper_brak, (select max(to_char(autodata, 'YYYY/MM/DD HH24:MI:SS')) from dp_swm_lub_towary_hist where oper='OP_BR' and skrot=w3.skrot) data_brak from (
select w2.skrot from (
select w1.*
from (
select w.*
,(select count(dt.adres) from dp_swm_lub_towary dt where dt.skrot=w.skrot and dt.strefa in('Z') and dt.is_active='1') ile_z 
,(select count(dt.adres) from dp_swm_lub_towary dt where dt.skrot=w.skrot and dt.strefa in('N','P') and dt.is_active='1') ile_np
from (
select dt.skrot,dt.adres,dt.strefa from dp_swm_lub_towary dt where dt.is_active='1'
) w 
) w1 where w1.ile_z = 0 and w1.ile_np > 0
) w2 where w2.skrot not in(
select distinct (w.skrot) from (
select db.skrot from dp_swm_lub_zog dz,dp_swm_lub_buf db where dz.nr_zamow=db.nr_zamow and dz.schemat=db.schemat and dz.stat_u1 in('DR','RP') 
order by to_char(dz.stda_u1,'YYYY-MM-DD'), decode(dz.stat_u1, 'MG', 1, 'RP', 2, 'DR', 3)) w) 
order by w2.skrot, decode(w2.strefa, 'N', 1, 'P', 2)
) w3, ht_rejna r where w3.skrot = r.nazpot and r.is_deleted='N'