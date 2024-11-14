select distinct w3.skrot,r.nazdop,w3.nr_zamow, w3.kod_odb, w3.naz_odb, w3.ilosc_zam, w3.ilosc, w3.oper from (
select w.*
,(select zg.kod_odb from ht_zog zg where zg.ie$14 = desql_graf.df11_2(w.nr_zamow)) kod_odb
,(select (select (naz||' '||adr||' '||kod_p||' '||miejsc) from ht_konto where kod_odb = zg.kod_odb) odbiorca from ht_zog zg where zg.ie$14 = desql_graf.df11_2(w.nr_zamow)) naz_odb
,(select ilosc from dp_swm_lub_buf_test where nr_zamow = w.nr_zamow and schemat = w.schemat and skrot = w.skrot) ilosc_zam
 from (
select do.nr_zamow,do.schemat,do.skrot,do.ilosc,do.strefa,do.oper from dp_swm_lub_wozek_oper_test do where do.is_active='1' and (do.login is null or do.login='A_KWIATKOWSKI_W')
) w 
) w3, ht_rejna r where w3.skrot = r.nazpot and r.is_deleted='N'