select w2.data_trans,w2.opis,w2.opis_mm,w2.nr_dok,w2.kod_kontr,w2.przychod,w2.rozchod from (
select to_char(w1.data_trans,'YYYY-MM-DD') data_trans, 
case 
  when w1.kod_transa=5 then 'SPRZEDAZ'
  when w1.kod_transa=12 then 'POBRANIE WEW.'
  when w1.kod_transa=2 then 'DOSTAWA WEW.'
  when w1.kod_transa=4 then 'DOSTAWA WEW.-ZWROT'
  when w1.kod_transa=6 then 'SPRZEDAZ-ZWROT'
  when w1.kod_transa=1 then 'DOSTAWA-PZ'
else '' end opis,
case when mm.dok_mm <> ' ' then ('Z mag ' || mm.mag1 || ' MM oryginalna ' || mm.dok_mm)  else '' end opis_mm,
w1.nr_dok,w1.kod_kontr,w1.przychod,w1.rozchod from (
select w.*
from (
select index_tow,data_trans,kod_transa,'' opis_mm, dok_transa nr_dok, kod_kontra kod_kontr,
case when ilosc>0 then ilosc else 0 end przychod,
case when ilosc<0 then -ilosc else 0 end rozchod
from ht_newro where is_deleted='N' and
ie$0 like lpad('111434', 10, ' ')||lpad('20', 3, ' ')||'%' and
kod_transa in (5,12,2,4,6,1) order by data_trans) w
where to_char(w.data_trans,'YYYY-MM-DD') between '2017-01' and '2017-12') w1 left join ht_mmtr mm on w1.nr_dok = mm.dok_mm_rea order by w1.data_trans
) w2