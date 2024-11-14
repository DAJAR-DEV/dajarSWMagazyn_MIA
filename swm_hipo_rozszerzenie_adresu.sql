select * from dp_swm_adresy;
select * from dp_swm_towary_hist order by autodata desc;
select * from dp_swm_towary_mm order by autodata desc;

--1. utworzenie kopii tabeli adresy 
--create table dp_swm_tt_adresy(adres,hala,rzad,regal,polka,is_active,login,hash,autodata) 
--as select adres,hala,rzad,regal,'01' polka,is_active,login,hash,autodata from dp_swm_adresy;

--2. utworzenie kopii tabeli towary
--create table dp_swm_tt_towary as select * from dp_swm_towary;
--commit;

--3. utworzenie kopii tabeli towary_mm
--create table dp_swm_tt_towary_mm as select * from dp_swm_tt_towary_mm;
--commit;

--4. utworzenie kopii tabeli towary_hist
--create table dp_swm_tt_towary_hist as select * from dp_swm_tt_towary_hist;
--commit;

--update dp_swm_tt_towary set adres=adres||'01';
--commit;

--update dp_swm_tt_adresy set adres=adres||'01';
--commit;

--update dp_swm_tt_towary_mm set adres=adres||'01';
--commit;

--update dp_swm_tt_towary_hist set adres=adres||'01';
--commit;


select * from dp_swm_tt_adresy;
select * from dp_swm_tt_towary;


select * from dp_swm_tt_towary where is_active='1' order by skrot;

select dm.*,mm.data from dp_swm_tt_dok_mm dm,ht_mmtr mm where dm.dok_mm=mm.dok_mm and dm.status in(0);

select dm.*,mm.data from dp_swm_tt_dok_mm dm,ht_mmtr mm where dm.dok_mm=mm.dok_mm and dm.status in(0) 
and mm.data<(sysdate-360) and dm.status='0'
order by dm.dok_mm asc;
--000000006/22

--update dp_swm_tt_dok_mm dm set dm.status='1' where dm.status='0' and (select mm.data from ht_mmtr mm where mm.dok_mm=dm.dok_mm)<(sysdate-90);
--commit;
select * from dp_swm_towary_mm order by autodata desc;
select * from ht_mmtr where dok_mm='000000006/22';


select * from dp_swm_tt_adresy where is_active='1';

select * from dp_swm_towary_hist;
--create table dp_swm_tt_towary_hist as select * from dp_swm_towary_hist;
--commit;