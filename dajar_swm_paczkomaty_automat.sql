--generowanie zamowien na magazynie 
select w.nr_zamow, case when w.ile_ogrod is null then 0 else w.ile_ogrod end ile_ogrod, case when w.ile_pozycji is null then 0 else w.ile_pozycji end ile_pozycji, w.ile_poduszka, w.stat_u1, w.stat_u2, w.mag, w.akwizytor,w.ile_paczkomat
from (
select zg.nr_zamow, zg.nr_zamow_o, zg.stat_u1, zg.stat_u2,
(select count(index_tow) from ht_zod where ie$0 like zg.nr_zamow||'%' and is_deleted = 'N' and index_tow not in(33032,8836,127365)) ile_pozycji,
(select count(index_tow) from ht_zod where ie$0 like zg.nr_zamow||'%' and is_deleted = 'N' and index_tow not in(33032,8836,127365) and desql_japa_nwa.fsql_japa_rskt2(index_tow) not in('532') and (desql_japa_nwa.fsql_japa_rskt2(index_tow) between '500' and '599')) ile_ogrod,
(select count(index_tow) from ht_zod where ie$0 like zg.nr_zamow||'%' and is_deleted = 'N' and index_tow not in(33032,8836,127365) and desql_japa_nwa.fsql_japa_rskt2(index_tow) in('503', '512', '523', '551', '553', '561', '571', '572', '573')) ile_poduszka,
(select count(o.nr_zamow_dt) from dp_rest_mag_order o where o.nr_zamow_dt=zg.nr_zamow and o.shipping_desc like '%Paczkomaty InPost%') ile_paczkomat,
zg.data_fakt, zg.godzina, (zg.co_zostawi||zg.co_zostaw2) opis, zg.mag, zg.akwizytor
from ht_zog zg
where zg.is_deleted = 'N'
and zg.ie$13 like lpad('43',3)||' '||'%'
and zg.mag in(43) and zg.zrealizowa = ' ' and zg.stat_u1 in('DR','RP') and zg.kod_kontr not in (27223)
order by zg.stat_u1 desc,zg.data_fakt asc, zg.nr_zamow
) w;

select COL_NAME,IND_CLP_EXP, COL_CLP_LEN, IND_NAME, IND_IN_USE
from CLP_TAB_COLS
WHERE TAB_NAME = UPPER('HT_ZOG')
  AND COL_NAME LIKE 'IE%'
ORDER BY COL_NAME;

select * from ht_zog where recno=6784809;

--a_paczkomat_P
select w.nr_zamow,w.schemat,w.nr_zamow_o,w.kontr,w.ile_poz ile_43,w.data_dr,w.data_mg,w.data_pak,w.etykieta_id,w.mag,w.stat_u1 from (
select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o,
(select count(*) from dp_swm_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and status in ('PA') and mag = 43) ile_poz,
(select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_oper do where do.status = dm.nr_zamow and do.typ_oper like '%['||dm.schemat||']%' and do.typ_oper like 'MG%') data_mg,
(select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_oper do where do.status = dm.nr_zamow and do.typ_oper like '%['||dm.schemat||']%' and do.typ_oper like 'PA%') data_pak,
(select min(to_char(db.autodata,'YYYY/MM/DD')) from dp_swm_buf db where db.status in('DR','RP') and db.nr_zamow=dm.nr_zamow) data_dr,
zg.autodata data_mag, 'P' typ_oper,'' etykieta_id, dm.mag,zg.stat_u1,case when zg.schemat='DAJAR' then (select kod_kontr from ht_zog where recno=zg.recno) else (select kod_kontr from ht_zog where recno=zg.recno) end kontr
from dp_swm_mag dm, dp_swm_zog zg where dm.status in ('PA') and zg.nr_zamow not in(select distinct nr_zamow from dp_swm_pak where nr_zamow = dm.nr_zamow and schemat = dm.schemat)
and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.mag = 43
union all
select distinct dp.nr_zamow,dp.schemat,zg.nr_zamow_o,
(select count(*) from dp_swm_mag where nr_zamow = dp.nr_zamow and status in ('PA') and mag=43) ile_poz,
(select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_oper do where do.status = dp.nr_zamow and do.typ_oper like '%['||dp.schemat||']%' and do.typ_oper like 'MG%') data_mg,
(select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_oper do where do.status = dp.nr_zamow and do.typ_oper like '%['||dp.schemat||']%' and do.typ_oper like 'PA%') data_pak,
(select min(to_char(db.autodata,'YYYY/MM/DD')) from dp_swm_buf db where db.status in('DR','RP') and db.nr_zamow=dp.nr_zamow) data_dr,
zg.autodata data_mag, 'P' typ_oper,(select max(dt.shipment_id) from dp_swm_paczka dt where dt.nr_zamow=dp.nr_zamow and dt.schemat=dp.schemat) etykieta_id,'43',zg.stat_u1,case when zg.schemat='DAJAR' then (select kod_kontr from ht_zog where recno=zg.recno) else (select kod_kontr from ht_zog where recno=zg.recno) end kontr
from dp_swm_pak dp, dp_swm_zog zg where dp.login = 'a_paczkomat_P' and dp.status in ('PA')
and zg.nr_zamow = dp.nr_zamow and zg.schemat = dp.schemat
) w left join dp_rest_mag_order o on w.nr_zamow=o.nr_zamow_dt where w.stat_u1 not in('ZW') and o.shipping_desc like '%Paczkomaty InPost%'
order by w.nr_zamow desc;


--zakladka package swm_hipo
select w.nr_zamow,w.schemat,w.nr_zamow_o,w.kontr,w.ile_poz ile_43,w.data_dr,w.data_mg,w.data_pak,w.etykieta_id,w.mag,w.stat_u1,o.shipping_desc,o.shipping,o.shipping_amount from (
select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o,
(select count(*) from dp_swm_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and status in ('PA') and mag = 43) ile_poz,
(select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_oper do where do.status = dm.nr_zamow and do.typ_oper like '%['||dm.schemat||']%' and do.typ_oper like 'MG%') data_mg,
(select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_oper do where do.status = dm.nr_zamow and do.typ_oper like '%['||dm.schemat||']%' and do.typ_oper like 'PA%') data_pak,
(select min(to_char(db.autodata,'YYYY/MM/DD')) from dp_swm_buf db where db.status in('DR','RP') and db.nr_zamow=dm.nr_zamow) data_dr,
zg.autodata data_mag, 'P' typ_oper,'' etykieta_id, dm.mag,zg.stat_u1,case when zg.schemat='DAJAR' then (select kod_kontr from ht_zog where recno=zg.recno) else (select kod_kontr from ht_zog where recno=zg.recno) end kontr
from dp_swm_mag dm, dp_swm_zog zg where dm.status in ('PA') and zg.nr_zamow not in(select distinct nr_zamow from dp_swm_pak where nr_zamow = dm.nr_zamow and schemat = dm.schemat)
and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.mag = 43
union all
select distinct dp.nr_zamow,dp.schemat,zg.nr_zamow_o,
(select count(*) from dp_swm_mag where nr_zamow = dp.nr_zamow and status in ('PA') and mag=43) ile_poz,
(select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_oper do where do.status = dp.nr_zamow and do.typ_oper like '%['||dp.schemat||']%' and do.typ_oper like 'MG%') data_mg,
(select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_oper do where do.status = dp.nr_zamow and do.typ_oper like '%['||dp.schemat||']%' and do.typ_oper like 'PA%') data_pak,
(select min(to_char(db.autodata,'YYYY/MM/DD')) from dp_swm_buf db where db.status in('DR','RP') and db.nr_zamow=dp.nr_zamow) data_dr,
zg.autodata data_mag, 'P' typ_oper,(select max(dt.shipment_id) from dp_swm_paczka dt where dt.nr_zamow=dp.nr_zamow and dt.schemat=dp.schemat) etykieta_id,'43',zg.stat_u1,case when zg.schemat='DAJAR' then (select kod_kontr from ht_zog where recno=zg.recno) else (select kod_kontr from ht_zog where recno=zg.recno) end kontr
from dp_swm_pak dp, dp_swm_zog zg where dp.login = 'j_nachyla_m' and dp.status in ('PA')
and zg.nr_zamow = dp.nr_zamow and zg.schemat = dp.schemat
) w left join dp_rest_mag_order o on w.nr_zamow=o.nr_zamow_dt where w.stat_u1 not in('ZW') and o.shipping_desc like '%InPost%'
order by w.nr_zamow desc;

select * from ht_zog where ie$14=desql_graf.df11_2('000465170/21');
select * from dp_swm_zog where nr_zamow='000465170/21';
select * from dp_swm_pak where nr_zamow='000465170/21';
select * from dp_swm_paczka where nr_zamow='000465170/21';
select * from dp_swm_paczka_info where shipment_id in(select shipment_id from dp_swm_paczka where nr_zamow='000465170/21');
select * from dp_swm_paczka_base where shipment_id in(select shipment_id from dp_swm_paczka where nr_zamow='000465170/21');

desc dp_swm_paczka_info;

select * from dp_swm_paczka_info where shipment_id='SH0000004/22';
select pi.paczka_id,pi.firma_id,case when pi.pl_type='1' then 'paczka' when pi.pl_type='2' then 'paleta' when pi.pl_type='3' then 'koperta' when pi.pl_type='4' then 'paczkomatA' when pi.pl_type='5' then 'paczkomatB' when pi.pl_type='6' then 'paczkomatC' when pi.pl_type='7' then 'paczka_poczta' when pi.pl_type='8' then 'paczka_inpost' when pi.pl_type='9' then 'paczka_zagranica' when pi.pl_type='10' then 'paczka_cieszyn' end typ,
case when pi.pl_non_std=0 then 'standard' else 'niestandard' end rodzaj,pi.pl_weight waga, pi.pl_width szer,pi.pl_height wys,pi.pl_length dlu,pi.pl_quantity ile_opak
from dp_swm_paczka_info pi
where pi.shipment_id in('SH0000004/22');

select pi.paczka_id,pi.firma_id,case when trim(pi.pl_type)='1' then 'paczka' when trim(pi.pl_type)='2' then 'paleta' when trim(pi.pl_type)='3' then 'koperta' when trim(pi.pl_type)='4' then 'paczkomatA' when trim(pi.pl_type)='5' then 'paczkomatB' when trim(pi.pl_type)='6' then 'paczkomatC' when trim(pi.pl_type)='7' then 'paczka_poczta' when trim(pi.pl_type)='8' then 'paczka_inpost' when trim(pi.pl_type)='9' then 'paczka_poczta' when trim(pi.pl_type)='10' then 'paczka_cieszyn' when trim(pi.pl_type)='HP' then 'półpaleta [HP]' when trim(pi.pl_type)='EP' then 'bezzwrotna paleta [EP]' when trim(pi.pl_type)='CC' then 'colli [CC]' when trim(pi.pl_type)='FP' then 'europaleta [FP]' when trim(pi.pl_type)='NP' then 'paleta 1.0x2.0 [NP]' when trim(pi.pl_type)='VP' then '1/4 paleta [VP]' when trim(pi.pl_type)='PC' then 'chep [PC]' when trim(pi.pl_type)='DR' then 'przekracza europalete [DR]' end typ,
case when trim(pi.pl_non_std)='0' then 'standard' when trim(pi.pl_non_std)='20' then 'cargo ekspedycja [20]' when trim(pi.pl_non_std)='21' then 'cargo zamówienia [21]' when trim(pi.pl_non_std)='A' then 'rozmiar 8 x 38 x 64 cm [A]' when trim(pi.pl_non_std)='B' then 'rozmiar 19 x 38 x 64 cm [B]' when trim(pi.pl_non_std)='C' then 'rozmiar 41 x 38 x 64 cm [C]' when trim(pi.pl_non_std)='D' then 'rozmiar 50 x 50 x 80 cm [D]' when trim(pi.pl_non_std)='RECZNIE' then 'rozmiar 1 x 1 x 1 cm [RECZNIE]' else 'niestandard' end rodzaj, 
pi.pl_weight waga, pi.pl_width szer,pi.pl_height wys,pi.pl_length dlu,pi.pl_quantity ile_opak, pi.pl_euro_ret nstd from
dp_swm_paczka_info pi
where pi.shipment_id like 'SH0000004/22' and pi.schemat='DOMINUS' order by pi.paczka_id asc;



select * from dp_swm_paczka_info where firma_id='INPOST_ALLEGRO';
select * from dp_swm_paczka where shipment_id in(select shipment_id from dp_swm_paczka_info where firma_id='INPOST_ALLEGRO') order by st_shipment_date desc;

select * from dp_swm_paczka_base where shipment_id in('520000016243300064291850','675220686287860139392193');

select * from dp_swm_paczka where shipment_id='675220637286300134939669';
select * from dp_swm_paczka_base  where shipment_id='675220637286300134939669';
select * from dp_swm_paczka_ss where shipment_id='675220637286300134939669';

select increment_id from dp_rest_mag_order where nr_zamow_dt='000423033/22';
select shipping_desc from dp_rest_mag_order where nr_zamow_dt='000423033/22';
select shipping_amount from dp_rest_mag_order where nr_zamow_dt='000423033/22';

select * from dp_rest_mag_order where shipping_desc like '%Paczkomaty InPost%' order by entity_id desc;
select firma_id,firma_nazwa from dp_swm_firma where firma_id='INPOST' order by firma_nazwa asc;