USE HomeWorkWeek15;

-- Students --
SELECT * FROM DBO.Students;

 -- task1 --
 SELECT * FROM DBO.Students WHERE DoB> '1990-12-31';

 -- task2 --
 SELECT Firstname, Lastname, YEAR(GETDATE()) - YEAR(DoB) as Age FROM DBO.Students WHERE Country IN ('Georgia', 'Libya'); 

 -- task3 --
 INSERT INTO dbo.Students (Lastname, Firstname, DoB, Email, Quiz1, Quiz2, MiddleTest, FinalTest, Country)
 VALUES ('Kvavilashvili', 'Elene', '2006-06-07', 'ekvav24@freeuni.edu.ge', 10, 10, 55, 25, 'Georgia');

 -- task4 --
 SELECT TOP 5 WITH TIES FirstName, MiddleTest FROM dbo.Students ORDER BY MiddleTest DESC;

 -- task5 --
 DELETE FROM dbo.Students OUTPUT Deleted.* WHERE FinalTest=19; 

 -- task6 --
 UPDATE dbo.Students Set FinalTest=0 WHERE MiddleTest=1; 





 -- Persons --
 SELECT * FROM DBO.Persons;

 -- task1 --
 SELECT * FROM dbo.Persons WHERE PrivateId LIKE '163%';

 -- task2 --
 SELECT * FROM dbo.Persons WHERE Lastname=City;

 -- task3 --
 SELECT * FROM dbo.Persons WHERE  Country IN ('Canada', 'Monaco');
 
 -- task4 --
 SELECT Firstname, Lastname, PrivateId FROM dbo.Persons WHERE Email IS NULL; 

 -- task5 --
 SELECT * FROM dbo.Persons WHERE Country IN ('Spain', 'Turkey') AND Salary BETWEEN 1000 AND 3000;

 -- task6 -- 
 SELECT WorkPlace FROM dbo.Persons WHERE WorkPlace LIKE '%LL[C, P]%' OR WorkPlace LIKE '%PC%';

 -- task7 --
 SELECT Email,
 CASE
 WHEN LEN(Email)-LEN(REPLACE(Email, '.', ''))>2 THEN 'More than 2 dots'
 ELSE 'Less than or equal to 2 dots'
 end as MAILINFO FROM dbo.Persons;

 --or--
 SELECT Email, IIF(LEN(Email)-LEN(REPLACE(Email, '.', ''))>2, 'More than 2 dots', 'Less than or equal to 2 dots')
 as MAILINFO FROM dbo.Persons;

 -- task8 --
 SELECT * FROM dbo.Persons WHERE PINcode LIKE '%51';

 -- task9 --
 SELECT Country, SUM(Salary)/COUNT(Country) as AverageSalary FROM dbo.Persons GROUP BY Country;

