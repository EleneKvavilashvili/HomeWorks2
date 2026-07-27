use TSQL2012

--task1--
select A.contactname, A.city, B.orderdate 
from Sales.Customers A inner join Sales.Orders B on A.custid=B.custid 
where A.city in ('London', 'Madrid')

--task2--
select UPPER(A.productname) as ProductName, A.unitprice as Price, B.categoryname as Category
from Production.Products A left join Production.Categories B on A.categoryid=B.categoryid
where A.unitprice between 20 and 40

--task3--
select A.lastname, A.firstname, B.orderid, B.freight
from HR.Employees A left join Sales.Orders B on A.empid=B.empid
where A.title='Sales Manager' and B.freight>50

--task4--
select B.orderdate, A.contactname, A.city, A.address
from Sales.Customers A inner join Sales.Orders B on A.custid=B.custid
where YEAR(B.orderdate)=2007 and A.country='USA'

--task5--
select distinct A.shipcity 
from Sales.Orders A right join HR.Employees B on A.empid=B.empid 
where B.lastname='Cameron'

--task6--
select B.companyname, A.orderid, A.shipcountry, A.shipcity
from Sales.Orders A left join Sales.Shippers B on A.shipperid=B.shipperid
where A.shipcountry in ('Germany', 'Austria')

--task7--
select distinct * from Production.Products A right join Production.Suppliers B on A.supplierid=B.supplierid
left join Sales.OrderDetails C on A.productid=C.productid 
where B.city='Tokyo' and C.discount>0.0

--task8--
select A.productname, C.categoryname 
from Production.Products A left join Production.Suppliers B on A.supplierid=B.supplierid 
left join Production.Categories C on A.categoryid=C.categoryid
where B.country='Japan' and C.categoryname in ('Seafood', 'Beverages')

--task9--
select B.firstname, B.lastname, C.companyname
from Sales.Orders A inner join HR.Employees B on A.empid=B.empid
inner join Sales.Shippers C on A.shipperid=C.shipperid
where YEAR(A.orderdate)='2007' and
((B.firstname = 'Sara' AND B.lastname = 'Davis') OR (B.firstname = 'Maria' AND B.lastname = 'Cameron'))

--task10--
select A.productname, C.categoryname
from Production.Products A right join Production.Suppliers B on A.supplierid=B.supplierid
left join Production.Categories C on A.categoryid=C.categoryid
where B.country='USA' and C.categoryname not in ('Seafood', 'Beverages')

--task11--
select A.orderid, B.lastname, B.firstname, B.city, C.contactname
from Sales.Orders A left join HR.Employees B on A.empid=B.empid
left join Sales.Customers C on A.custid=C.custid
where C.city=B.city

--task12-
select distinct A.contactname
from Sales.Customers A left join Sales.Orders B on A.custid=B.custid
left join  Sales.OrderDetails C on B.orderid=C.orderid
left join Production.Products D on C.productid=D.productid
left join Production.Categories E on D.categoryid=E.categoryid
where E.categoryname in ('Beverages', 'Dairy Products')





use Hardware

--task1--
select Name, Price from dbo.Products 
where ManufacturerId=(select ManufacturerId from dbo.Manufacturers where Name='Hewlett-Packard')

--task2--
select Name, Price from dbo.Products
where ManufacturerId not in (select ManufacturerId from dbo.Manufacturers where Name='Fujitsu')

--task3--
select Name, Price from dbo.Products
where ManufacturerId in (select ManufacturerId from dbo.Manufacturers where Name in ('Sony', 'Fujitsu', 'IBM', 'Intel'))

--task4--
select Name from dbo.Manufacturers
where ManufacturerId in (select ManufacturerId from dbo.Products where Price>200)

--task5--
select Name, Price from dbo.Products
where ManufacturerId not in (select ManufacturerId from dbo.Manufacturers where Name in ('Genius', 'Dell'))

--task6--
select COUNT(*) as Count from dbo.Manufacturers
where ManufacturerId in (select ManufacturerId from dbo.Products where Name like '%drive%')

--task7--
select COUNT(*) as Count from dbo.Products
where ManufacturerId = (select ManufacturerId from dbo.Manufacturers where Name='Intel')
and Price>(select SUM(Price)/COUNT(Name) from dbo.Products 
where ManufacturerId=(select ManufacturerId from dbo.Manufacturers where Name='Intel'))





use WorldEvents

select * from dbo.Category
select * from dbo.Continent
select * from dbo.Country
select * from dbo.Event

--task1--
select COUNT(*) as Count from dbo.Event
where CountryID in (select CountryID from dbo.Country 
where ContinentID=(select ContinentID from dbo.Continent where ContinentName='Europe'))

--task2--
select MIN(EventDate) from dbo.Event
where CountryID in (select CountryID from dbo.Country
where ContinentID=(select ContinentID from dbo.Continent where ContinentName='Africa'))

--task3--
select COUNT(*) as Count from dbo.Country
where ContinentID in (select ContinentID from dbo.Continent where ContinentName in ('South America', 'North America'))

--task4--
select COUNT(*) from dbo.Event
where MONTH(EventDate)=1 and DAY(EventDate)=1
and CategoryId=(select CategoryID from dbo.Category where CategoryName='Economy')

--task5--
select MAX(EventDate) from dbo.Event
where CountryID in (select CountryID from dbo.Country
where ContinentID=(select ContinentID from dbo.Continent where ContinentName='Europe'))
and CategoryID=(select CategoryID from dbo.Category where CategoryName='Sports')
