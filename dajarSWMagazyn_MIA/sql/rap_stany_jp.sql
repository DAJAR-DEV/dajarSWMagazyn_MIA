select
data,
opis,
('Z mag ' || b.mag1 || ' do mag. ' || b.mag2 || 'przeznaczenie ' || b.mag_przezn) opis_mm,
nr_dok,
kod_kontr,
przychod,
rozchod
from
(
select 
data_trans data,
case 
  when kod_transa=10 then 'MM-WYJSCIE'
else '' end opis,
dok_transa nr_dok,
kod_kontra kod_kontr,
case when ilosc>0 then ilosc else 0 end przychod,
case when ilosc<0 then -ilosc else 0 end rozchod
from 
ht_newro 
where
is_deleted='N' and
ie$0 like '      5595 43%' and
kod_transa in (10)
) A,
(
select 
dok_mm_rea,
dok_mm,
mag1,
mag2,
mag_przezn
from ht_mmtr
) B
where 
a.nr_dok=b.dok_mm
union all
select
data,
opis,
('Z mag ' || b.mag1 || ' MM oryginalna ' || b.dok_mm) opis_mm,
nr_dok,
kod_kontr,
przychod,
rozchod
from
(
select 
data_trans data,
case 
  when kod_transa=11 then 'MM-WEJSCIE'
else '' end opis,
dok_transa nr_dok,
kod_kontra kod_kontr,
case when ilosc>0 then ilosc else 0 end przychod,
case when ilosc<0 then -ilosc else 0 end rozchod
from 
ht_newro 
where
is_deleted='N' and
ie$0 like '      5595 43%' and
kod_transa in (11)
) A,
(
select 
dok_mm_rea,
dok_mm,
mag1,
mag2,
mag_przezn
from ht_mmtr
) B
where 
a.nr_dok=b.dok_mm_rea
union all
select 
data_trans data,
case 
  when kod_transa=5 then 'SPRZEDAZ'
  when kod_transa=12 then 'POBRANIE WEW.'
  when kod_transa=2 then 'DOSTAWA WEW.'
  when kod_transa=4 then 'DOSTAWA WEW.-ZWROT'
  when kod_transa=6 then 'SPRZEDAZ-ZWROT'
  when kod_transa=1 then 'DOSTAWA-PZ'
else '' end opis,
'' opis_mm,
dok_transa nr_dok,
kod_kontra kod_kontr,
case when ilosc>0 then ilosc else 0 end przychod,
case when ilosc<0 then -ilosc else 0 end rozchod
from 
ht_newro 
where
is_deleted='N' and
ie$0 like '      5595 43%' and
kod_transa in (5,12,2,4,6,1)
order by data;
